using System.Collections.Generic;
using UnityEngine;
using DiasGames.Components;
using DiasGames.Climbing;
using DiasGames.Debugging;
using UnityEngine.Rendering;

namespace DiasGames.Abilities
{

    public class ClimbAbility : AbstractAbility
    {    
        [SerializeField] private LayerMask climbMask;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private List<string> ignoreTags = new List<string>();
        [Space]
        [SerializeField] private Transform grabReference;
        [SerializeField] private float globalRadiusDetection = 0.5f;
        [SerializeField] private Vector2 offsetOnLedge;
        [Header("Capsule Cast Parameters")]
        [SerializeField] private float capsuleCastDistance = 0.75f;
        [SerializeField] private float capsuleHeight = 1f;
        [SerializeField] private float capsuleRadius = 0.15f;
        [SerializeField] private int capsuleCastIterations = 10;
        [Header("Sphere Cast Parameters (For Top Detection)")]
        [SerializeField] private float sphereCastMaxHeight = 1f;
        [SerializeField] private float sphereCastDistance = 2f;
        [SerializeField] private float sphereCastRadius = 0.1f;
        [Header("Shimmy Casting")]
        [SerializeField] private int sideCastIterations = 10;
        [SerializeField] private float sideCastRange = 0.5f;
        [SerializeField] private float sideCastRadius = 0.1f;
        [SerializeField] private float sideCastHeight = 0.5f;
        [Header("Foot Casting")]
        [SerializeField] private LayerMask footMask;
        [SerializeField] private float footCastRadius = 0.3f;
        [SerializeField] private float footCapsuleHeight = 1f;
        [SerializeField] private float footCastDistance = 1f;
        [Header("Matching Position Parameters")]
        [SerializeField] private float startClimbMatchTime = 0.15f;
        [SerializeField] private AnimationCurve defaultMatchingCurve;
        [Space]
        // state machine
        [SerializeField] public ClimbStateContext _context;

        [Header("Debug")]
        [SerializeField] private Color debugColor;
      
      
      

        // public getters
        public float CapsuleCastHeight { get { return capsuleHeight; } }
        public float CapsuleCastRadius { get { return capsuleRadius; } }
        public float SphereCastHeight { get { return sphereCastMaxHeight; } }
        public float SphereCastRadius { get { return sphereCastRadius; } }
        public LayerMask ClimbMask { get { return climbMask; } }
        public Collider CurrentCollider { get { return _currentCollider; } }
        public List<string> IgnoreTags { get { return ignoreTags; } }

        // components
        private IMover _mover;
        private ICapsule _capsule;
        private ClimbIK _climbIK;
        private CastDebug _debug;

        // private internal climbing controller
        [Header("玩家当前攀附的物体")]
        public Collider _currentCollider;
        private RaycastHit _currentHorizontalHit;
        private RaycastHit _currentTopHit;
        private RaycastHit _wallHit;
        private float _hangWeight = 0;
        private float _hangvel;
        private Transform _targetClimbCharPos; // effector transform to help set character position on movable ledges
        private Transform _climbTargetHit; // effector transform to help set character position on movable ledges (for top and horizontal hits)

        private Vector3 _lastHitPoint;
        private float _timeWithoutLedge = 0;

        // internal vars
        private Camera _mainCamera;
        private Vector2 _localCoordMove;
        private bool _updateTransform = true;

        // waiting animation to finish
        private bool _waitingAnimation;
        private string _animationStateToWait;
        private bool _matchTarget;
        private Vector3 _matchTargetPosition;
        private Quaternion _matchTargetRotation;
        private float _targetNormalizedTime;

        // tween parameters
        private bool _isDoingTween = false;
        private float _currentTweenWeight = 0;
        private float _tweenDuration = 0;
        private float _tweenStartTime;
        private float _tweenStep;
        private Vector3 _tweenStartPosition;
        private Quaternion _tweenStartRotation;
        private Vector3 _tweenBezierPoint;
        private Transform _tweenTarget;
        private AnimationCurve _targetCurve;

        // side shimmy control
        private float _leftDistanceToShimmy;
        private float _rightDistanceToShimmy;
        private float _shimmyMinRatio = 0.5f;
        private bool _stopRightShimmy;
        private bool _stopLeftShimmy;

        // avoid climb the same ledge when droping
        private Collider _ledgeBlocked;
        private float _timeBlockStarted;

        #region State Machine Methods

        public void Idle() => _context.CurrentClimbState.Idle(_context);
        public void ClimbUp() => _context.CurrentClimbState.ClimbUp(_context);
        public void Jump() => _context.CurrentClimbState.Jump(_context);
        public void Drop() => _context.CurrentClimbState.Drop(_context);
        public void CornerOut(CornerSide side)
        {
            _context.CornerOut.cornerSide = side;
            _context.CurrentClimbState.CornerOut(_context);
        }

        #endregion

        private void Awake()
        {
            _mover = GetComponent<IMover>();
            _capsule = GetComponent<ICapsule>();
            _climbIK = GetComponent<ClimbIK>();
            _debug = GetComponent<CastDebug>();

            _mainCamera = Camera.main;

            CreateTransforms();
        }

        private void CreateTransforms()
        {
            if (_targetClimbCharPos == null)
                _targetClimbCharPos = new GameObject("Climb Char Target").transform;

            if (_tweenTarget == null)
                _tweenTarget = new GameObject("Climb Tween Target").transform;

            if(_climbTargetHit == null)
                _climbTargetHit = new GameObject("Climb Hit").transform;
        }

        public override bool ReadyToRun()
        {
            if (_mover.IsGrounded()) return false;

            return HasLedge();
        }
        public override void OnStartAbility()
        {
            UpdateContextVars();
            //停止角色的所有移动并禁用重力，这样角色才能悬挂在边缘处而不会下落。
            _mover.StopMovement();
            _mover.DisableGravity();
            //启动IK（反向运动学）系统，并更新其参考点。IK系统用于调整角色的手和脚，让它们准确地对齐到攀爬表面。
            _climbIK.RunIK();
            _climbIK.UpdateIKReferences(climbMask, footMask,_currentHorizontalHit);
            //检查是否有墙面支撑脚部。如果有墙面，_hangWeight为0（表示有支撑）；如果没有墙面，则为1（表示悬挂状态）。
            // 这个值会传递给动画器，影响角色的悬挂姿势。
            _hangWeight = HasWall() ? 0 : 1;
            _animator.SetFloat("HangWeight", _hangWeight);
            //重置角色的相关状态
            _waitingAnimation = false;
            _matchTarget = false;
            _updateTransform = true;
            //设置ClimbStateContext为idle状态
            _context.SetState(_context.Idle);
            //启动补间动画，将角色平滑移动到计算得出的攀爬位置。参数包括：
            DoTween(GetCharacterPositionOnLedge(), GetCharacterRotationOnLedge(), startClimbMatchTime, _currentCollider);
            //设置动画状态为“Climb.Start Climb”，表示角色正在开始攀爬动作。
            SetAnimationState("Climb.Start Climb");
            PlayerPhysicalStrength.Instance.ReducePhysicalStrength(PlayerPhysicalStrength.Instance.JumpStrength);
            QTEUI.Instance.StartClick();

            _timeWithoutLedge = 0;
        }

        public override void OnStopAbility()
        {
            //停止IK系统，禁用重力，并停止角色的根运动（Root Motion）。恢复脚本控制的重力。
            _climbIK.StopIK();
            _mover.StopRootMotion();
            _mover.EnableGravity();
            //如果动画状态有掉落或跳跃，则设置角色的速度为向下的3倍，模拟掉落或跳跃的效果。
            if (!string.IsNullOrEmpty(_animationStateToWait))
            {
                if (_animationStateToWait.Contains("Drop"))
                {
                    _mover.SetVelocity(Vector3.down * 3f);
                    //Debug.Log("Drop");
                }

                if (_animationStateToWait.Contains("Jump"))
                {
                }
            }
            //启用角色的碰撞体，允许角色与其他物体发生碰撞。
            _capsule.EnableCollision();
        }

        public override void UpdateAbility()
        {
            //注：UpdateAbility方法在每一帧都会被调用，用于更新角色的状态和动画。

            // 更新IK系统的参考点，确保手脚正确放置
            // 更新角色脚部与墙面的接触状态（影响悬挂动画）
            // 更新补间动画进度（用于平滑过渡角色位置）
            _climbIK.UpdateIKReferences(climbMask, footMask,_currentHorizontalHit);

            UpdateFootWall();
            UpdateTween();

            // 检查是否正等待特定动画完成（如攀爬上沿、跳跃或下落）
            // 监控动画进度，当达到指定进度(_targetNormalizedTime)时，停止攀爬能力
            // 根据需要使用MatchTarget功能精确定位角色（例如在攀爬上沿时，确保脚正确踩在平台上）
            // 禁用碰撞体，防止在动画过程中与环境碰撞
            if (_waitingAnimation)
            {
                if (_animator.IsInTransition(0)) return;

                var state = _animator.GetCurrentAnimatorStateInfo(0);
                float normalizedTime = Mathf.Repeat(state.normalizedTime, 1);
                if (state.IsName(_animationStateToWait))
                {
                    if (_matchTarget && !_animator.isMatchingTarget)
                    {
                        _capsule.DisableCollision();
                        _animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.RightFoot,
                            new MatchTargetWeightMask(Vector3.one, 0f), 0.4f, 0.9f);

                        _matchTarget = false;
                    }

                    if (normalizedTime >= _targetNormalizedTime)
                    {
                        StopAbility();
                        return;
                    }
                }

                return;
            }
            //3. 补间动画和位置调整
            // 如果正在执行补间动画，不执行后续代码
            // 检查攀爬目标位置是否有轻微变化（比如攀爬物体移动），并相应调整角色位置
            if (_isDoingTween)
                return;

            if (Vector3.Distance(_lastHitPoint,_targetClimbCharPos.position) < 0.25f && _updateTransform)
            {
                _mover.SetPosition(transform.position + (_targetClimbCharPos.position - _lastHitPoint));
            }
            //4. 处理输入和攀爬状态
            // 处理玩家的输入命令（如跳跃、上爬或下落）
            // 检查当前是否仍有可攀爬的平台
            // 设置角色位置，使其正确贴合攀爬表面
            // 检查左右两侧是否可继续攀爬或需要转角
            // 应用根运动，允许动画控制角色移动
            // 调用状态机的Idle方法，可能触发状态转换
            // 设置动画参数，控制左右移动的动画播放
            // 尝试检测内角（角落处的攀爬转弯）
            // 更新最后碰撞点位置和无攀爬表面计时器
            ProccessInput();

            if (HasCurrentLedge())
            {
                SetCharacterPosition();
                CheckLedgeSide();

                ProccesMovement();

                _mover.ApplyRootMotion(Vector3.one);

                _context.CurrentClimbState.Idle(_context);
                //5.横向移动逻辑
                // 管理左右两侧可攀爬距离的限制
                // 当接近边缘时停止对应方向的水平移动
                // 根据输入和限制条件设置动画的水平参数
                if (_context.CurrentClimbState == _context.Idle)
                {
                    if (_rightDistanceToShimmy < sideCastRange * _shimmyMinRatio)
                        _stopRightShimmy = true;
                    if (_rightDistanceToShimmy >= sideCastRange * 0.95f)
                        _stopRightShimmy = false;

                    if (_leftDistanceToShimmy < sideCastRange * _shimmyMinRatio)
                        _stopLeftShimmy = true;
                    if (_leftDistanceToShimmy >= sideCastRange * 0.95f)
                        _stopLeftShimmy = false;

                    if ((_localCoordMove.x > 0 && _stopRightShimmy) ||
                    (_localCoordMove.x < 0 && _stopLeftShimmy))
                        _animator.SetFloat("Horizontal", 0);
                    else
                        _animator.SetFloat("Horizontal", _localCoordMove.x, 0.1f, Time.deltaTime);
                }
                else
                    _animator.SetFloat("Horizontal", 0);

                _animator.SetFloat("Vertical", _localCoordMove.y, 0.1f, Time.deltaTime);

                _context.CurrentClimbState.CornerIn(_context);
                _lastHitPoint = _targetClimbCharPos.position;
                _timeWithoutLedge = 0;
            }
            //6. 处理无攀爬表面的情况
            // 如果不再有可攀爬的表面，增加"无攀爬表面"的计时
            // 重置攀爬目标变换的父物体
            // 如果超过0.1秒没有找到攀爬表面，标记当前攀爬物为已阻止（避免立即再次攀爬）并停止攀爬能力
            else
            {
                if (_updateTransform)
                    _timeWithoutLedge += Time.deltaTime;

                _climbTargetHit.parent = null;

                if (_timeWithoutLedge > 0.1f)
                {
                    BlockCurrentLedge();
                    StopAbility();
                }
            }
            //7. 更新上下文变量：更新ClimbStateContext中的变量，以便在状态机中使用
            UpdateContextVars();
        }

        public void SetVelocity(Vector3 velocity, bool gravity = false)
        {
            //禁用跟运动（Root Motion），以便手动控制角色的速度
            _capsule.EnableCollision();
            _mover.StopRootMotion();
            _mover.SetVelocity(velocity);
            if (gravity) _mover.EnableGravity();
        }

        private void UpdateFootWall()
        {
            //该方法负责处理角色脚部与墙面的接触状态。
            //它会根据角色的状态和环境来决定脚部是否需要悬挂在墙面上。0和1代表了悬挂的程度。
            float targetWeight = HasWall() ? 0 : 1;
            _hangWeight = Mathf.SmoothDamp(_hangWeight, targetWeight, ref _hangvel, 0.12f);
            _animator.SetFloat("HangWeight", _hangWeight);
        }

        /// <summary>
        /// 判断玩家周围是否存在可攀爬的边缘
        /// </summary>
        private bool HasLedge()
        {

            // 1. 初始球形检测
            // 首先在角色的抓取参考点周围执行球形重叠检测
            // 使用globalRadiusDetection参数定义检测半径
            // 只检测在climbMask层上的物体（这些被标记为可攀爬的表面）
            // 如果没有检测到任何可能的攀爬表面，立即返回false
            Collider[] colls = Physics.OverlapSphere(grabReference.position, globalRadiusDetection, climbMask, QueryTriggerInteraction.Collide);

            if (colls.Length == 0) return false;

            // 2. 设置胶囊体投射参数
            // 计算胶囊体投射的起点和终点
            // 胶囊体大致模拟了角色的手部可触及范围
            // 将360度均分为capsuleCastIterations个方向，为后续的全方位检测做准备
            Vector3 capsuleBotPoint = grabReference.position + Vector3.down * (capsuleHeight*0.5f - capsuleRadius);
            Vector3 capsuleTopPoint = grabReference.position + Vector3.up * (capsuleHeight*0.5f - capsuleRadius);
            float angleStep = 360.0f / capsuleCastIterations;

            // create two lists: one for horizontal hits and other for top hits
            // they must have the same index, to match final result
            List<RaycastHit> horizontalHits = new List<RaycastHit>();
            List<RaycastHit> topHits = new List<RaycastHit>();

            // 3. 全方位胶囊体投射
            // 系统会向周围360度各个方向投射胶囊体
            // 使用三角函数计算每个投射方向
            // 每个方向都从角色略微后方开始投射，增加检测灵活性
            for(int i=0; i < capsuleCastIterations; i++)
            {
                // get current angle direction in radians
                float currentAngleRad = i*angleStep * Mathf.Deg2Rad;

                // calculate direction to cast
                Vector3 direction = new Vector3(Mathf.Cos(currentAngleRad), 0, Mathf.Sin(currentAngleRad)).normalized;

                // perform capsule cast all. It will allow to check all available ledges
                // also set start point a little back to allow more flexible ledge climbing
                RaycastHit[] hitsArray = Physics.CapsuleCastAll(capsuleBotPoint - direction * capsuleCastDistance, capsuleTopPoint - direction * capsuleCastDistance,
                    capsuleRadius, direction, capsuleCastDistance * 2, climbMask, QueryTriggerInteraction.Collide);

                // 4. 筛选水平碰撞点
                // 对找到的每个水平碰撞点进行一系列检验
                // 忽略最近被阻断的边缘（比如刚从那个边缘跳下来）
                // 忽略带有特定标签的物体（通过ignoreTags列表）
                // 确保碰撞点是有效的（距离不为0）
                // 检查角色朝向与表面法线的角度，确保角色基本面向攀爬表面
                foreach(RaycastHit horHit in hitsArray)
                {
                    // check if this ledge is blocked
                    if (_ledgeBlocked != null && _ledgeBlocked == horHit.collider && Time.time - _timeBlockStarted < 1f)
                        continue;

                    // check if this ledge is to be ignored
                    if (ignoreTags.Contains(horHit.collider.tag)) continue;

                    // is it a valid hit?
                    if (horHit.distance != 0)
                    {  // now, perform a top cast, to check if it's a valid ledge

                        // check angle
                        if (Vector3.Dot(transform.forward, -horHit.normal) < -0.1f) continue;

                        // 5. 顶部表面检测
                        // 对每个有效的水平碰撞点，从其上方投射球体向下检测
                        // 只考虑与水平碰撞点相同的碰撞体
                        // 确保顶部表面朝上（与上方向量夹角小于60度）
                        // 将符合条件的顶部碰撞点添加到候选列表
                        Vector3 startSphere = horHit.point;
                        startSphere.y = grabReference.position.y + sphereCastMaxHeight;

                        // perform sphere cast all
                        var topHitsArray = Physics.SphereCastAll(startSphere, sphereCastRadius, Vector3.down, 
                            sphereCastDistance, climbMask, QueryTriggerInteraction.Collide);

                        // create a temporary list to choose the best hit after cast
                        List<RaycastHit> possibleTopHits = new List<RaycastHit>();
                        foreach(var topHit in topHitsArray)
                        {
                            // is it a valid hit?
                            if (topHit.distance == 0) continue;

                            // is it the same collider?
                            if (topHit.collider != horHit.collider) continue;

                            // has possible normal?
                            if (Vector3.Dot(Vector3.up, topHit.normal) < 0.5f) continue;

                            // add this hit in possible hits
                            possibleTopHits.Add(topHit);
                        }
                        // 6. 选择最佳顶部点
                        // 从所有可能的顶部点中，选择高度最接近角色抓取点的那个
                        // 这确保攀爬点位于角色可以舒适抓取的位置
                        // found any possible hit?
                        if (possibleTopHits.Count == 0) continue;

                        // now select the closest hit 
                        RaycastHit closestHit = possibleTopHits[0];
                        float currentDistance = Mathf.Abs(closestHit.point.y - grabReference.position.y);
                        foreach(var closestCandidate in possibleTopHits)
                        {
                            if (Mathf.Abs(closestHit.point.y - grabReference.position.y) < currentDistance)
                                closestHit = closestCandidate;
                        }

                        RaycastHit hor = horHit;
                        RaycastHit top = closestHit;
                        // 7. Ledge组件特殊处理
                        // 检查碰撞体是否有自定义的Ledge组件
                        // 如果有，尝试获取最接近的预定义攀爬点
                        // 在某些情况下（角色方向与预定义点方向差异较大），使用预定义点的位置和朝向
                        if(top.collider.TryGetComponent(out Ledge ledge))
                        {
                            Transform closest = ledge.GetClosestPoint(top.point);
                            if(closest != null)
                            {
                                if (Vector3.Dot(closest.forward, transform.forward) < 0.2f)
                                {
                                    hor.normal = closest.forward;
                                    top.point = closest.position;
                                }
                            }
                        }
                        // 8. 检查可攀爬性
                        // 调用PositionFreeToClimb方法检查该点是否被其他物体阻挡
                        // 如果没有阻挡，将水平和顶部碰撞点添加到候选列表
                        // check if point is free to climb
                        if (!PositionFreeToClimb(hor, top)) continue;

                        // finally add both hits to possible selection
                        horizontalHits.Add(hor);
                        topHits.Add(top);
                    }
                }
            }

            // found any valid climbing?
            // 9. 选择最佳攀爬方向
            // 如果没有找到任何有效的攀爬点组合，返回false
            // 遍历所有候选攀爬点，选择与角色朝向最匹配的那个
            // 使用点积计算匹配度，点积越大表示方向越接近
            if (horizontalHits.Count == 0) return false;

            int index = 0;
            float bestDot = -1;
            for (int i = 0; i < horizontalHits.Count && i < topHits.Count; i++)
            {
                // caluclate dot to check wich ledge has the best match
                float dot = Vector3.Dot(transform.forward, -horizontalHits[i].normal);

                // if dot is greater than currento best dot, update best dot
                if(dot > bestDot)
                {
                    bestDot = dot;
                    index = i;
                }
            }

            // set controller vars
            // 10. 更新攀爬系统状态
            // 保存选中的最佳攀爬点信息
            // 更新攀爬系统的内部状态
            // 调用UpdateClimbHit()更新目标变换
            // 记录最后碰撞点的位置，用于后续位置调整
            // 返回true，表示找到了可攀爬的表面
            // set current collider in use
            _currentCollider = topHits[index].collider;

            // set current raycast hits to access for positioning methods
            _currentHorizontalHit = horizontalHits[index];
            _currentTopHit = topHits[index];

            UpdateClimbHit();
            _lastHitPoint = _targetClimbCharPos.position;

            return true;
        }

        private void CheckLedgeSide()
        {
            // cast left
            CastShimmy(ref _leftDistanceToShimmy, -1);

            // cast right
            CastShimmy(ref _rightDistanceToShimmy, 1);

        }

        /// <summary>
        /// 此函数在侧向投射多个球体。
        /// 它设置还剩多少米可以进行侧向攀爬（shimmy）。
        /// </summary>
        /// <param name="shimmyDistance">侧向可攀爬的距离引用</param>
        /// <param name="direction">方向（1为右，-1为左）</param>
        private void CastShimmy(ref float shimmyDistance, int direction)
        {
            // 横移距离检测机制
            // 这段代码首先计算了用于检测的步长，将检测范围sideCastRange均分成sideCastIterations个部分。随后，代码假设角色可以移动最大距离，并通过迭代检测来逐步降低这个估计值。

            // 每次迭代中，代码创建一个沿指定方向（左/右）偏移的胶囊体，从远到近进行检测。这种"由远及近"的检测策略非常巧妙，一旦发现无法攀爬的地方，就立即更新最大可移动距离shimmyDistance。

            // 胶囊体投射检测
            // 对于每个检测点，代码创建了一个垂直方向的胶囊体：

            // 胶囊体顶部：中心点上方
            // 胶囊体底部：中心点下方
            // 胶囊体半径：由sideCastRadius定义
            // 通过Physics.CapsuleCastAll向前投射这些胶囊体，代码可以检测到角色前方是否存在可攀爬表面。这种方法比单纯的射线检测更可靠，因为它考虑了角色手臂的体积。

            // 有效命中筛选
            // 并非所有碰撞都被视为有效。代码通过以下条件筛选有效命中：

            // 距离必须大于0（排除已经接触的表面）
            // 表面法线与当前攀爬表面法线的点积必须大于0.7（确保表面角度相近）
            // 碰撞体必须与当前攀爬的碰撞体相同（限制角色不会跨越不同物体）
            // 转角触发机制
            // 代码最后部分是触发转角动作的逻辑。当满足以下所有条件时，角色将执行转角动作：

            // 可移动距离小于最小比例（检测到边缘或转角）
            // 玩家有足够的水平输入（大于0.2）
            // 输入方向与检测方向一致
            // calculate steps to cast spheres
            float step = sideCastRange / sideCastIterations;

            // set current max distance to the maximum
            shimmyDistance = sideCastRange;

            // do iterations
            for (int i = 0; i < sideCastIterations; i++)
            {
                // set start position to cast
                Vector3 center = grabReference.position + transform.right * direction * (sideCastRange - step * i);
                Vector3 capsuleTop = center + Vector3.up * (sideCastHeight / 2f - sideCastRadius);
                Vector3 capsuleBot = center + Vector3.down * (sideCastHeight / 2f - sideCastRadius);

                // debug start sphere position
                DrawCapsule(capsuleBot, capsuleTop, sideCastRadius, debugColor);

                // create a list of hits that is available to shimmy
                List<RaycastHit> hits = new List<RaycastHit>();

                // do sphere cast and loop through all
                foreach (var hit in Physics.CapsuleCastAll(capsuleTop, capsuleBot, sideCastRadius, transform.forward,
                    capsuleCastDistance, climbMask, QueryTriggerInteraction.Collide))
                {
                    // is a valid hit?
                    if (hit.distance == 0) continue;

                    // check angle
                    if (Vector3.Dot(_currentHorizontalHit.normal, hit.normal) < 0.7f) continue;

                    // if hit is the same of current collider
                    // TODO: allow climb different collider
                    if (hit.collider == _currentCollider)
                    {
                        // add this hit to the list
                        hits.Add(hit);

                        // debug final hit pos
                        DrawSphere(hit.point, sideCastRadius, debugColor);
                        Debug.DrawLine(center, hit.point, debugColor);
                    }
                }

                // if nothing was found, update max distance available
                if (hits.Count == 0)
                    shimmyDistance = sideCastRange - step * i;
            }

            if (shimmyDistance > sideCastRange * _shimmyMinRatio) return;

            if (Mathf.Abs(_localCoordMove.x) < 0.2f) return;

            if (_localCoordMove.x < 0 && direction == 1) return;
            if (_localCoordMove.x > 0 && direction == -1) return;

            CornerOut(direction == 1 ? CornerSide.Right : CornerSide.Left);
        }

        /// <summary>
        /// this function is called inside update ability. 
        /// It assumes character has already found a ledge
        /// 此函数在UpdateAbility中调用。
        /// 它假设角色已经找到了一个边缘
        /// </summary>
        /// <returns></returns>
        private bool HasCurrentLedge()
        {
            // start sphere position for horizontal cast
            // 1. 初始胶囊体设置
            // 创建一个以角色抓取参考点为中心的胶囊体
            // 通过调试函数可视化这个胶囊体
            Vector3 capsuleBot = grabReference.position + Vector3.down * (sideCastHeight / 2f - sideCastRadius);
            Vector3 capsuleTop = grabReference.position + Vector3.up * (sideCastHeight / 2f - sideCastRadius);

            // debug initial sphere cast
            DrawCapsule(capsuleBot, capsuleTop, capsuleRadius, Color.red);

            // list of climbable points
            List<ClimbablePoint> climbables = new List<ClimbablePoint>();
            // 2. 前向胶囊体投射
            // 向角色前方投射胶囊体，检测可能的攀爬表面
            // 只考虑在climbMask层上的碰撞体
            // do sphere cast on forward direction and loop through all hits
            foreach (var hit in Physics.CapsuleCastAll(capsuleTop, capsuleBot, capsuleRadius, transform.forward,
                capsuleCastDistance, climbMask, QueryTriggerInteraction.Collide))
            {
                // is it a valid hit?
                if (hit.distance == 0) continue;

                // only keep checking if this hit is the same of current collider or
                // if current collider is null
                // TODO: improve to allow climb other colliders//
                //3. 当前碰撞体匹配验证
                // 验证检测到的碰撞体是否与当前正在攀爬的碰撞体相同
                // 如果匹配，在碰撞点绘制调试球体
                if (hit.collider == _currentCollider)
                {
                    // debug horizontal cast found
                    DrawSphere(hit.point, capsuleRadius, Color.red);

                    // set top start cast position
                    //4. 顶部射线检测
                    // 创建一系列从角色上方向下射出的射线，形成一个密集的检测网格
                    // 这种方法允许更精确地定位顶部边缘，而不仅仅依赖于单个点
                    Vector3 initial = grabReference.position + Vector3.up;
                    int lineIterations = 20;
                    // loop raycast for top
                    for (int i = 0; i < lineIterations; i++)
                    {
                        Vector3 topStart = initial + transform.forward * i * (1f / lineIterations);

                        foreach (var top in Physics.RaycastAll(topStart, Vector3.down, 3f, climbMask, QueryTriggerInteraction.Collide))
                        {
                            // is top hit valid?
                            //5. 顶部碰撞点验证
                            // 验证顶部碰撞是否有效
                            // 确保顶部碰撞与水平碰撞属于同一物体
                            // 检查攀爬位置是否被障碍物阻挡
                            // 确保顶部表面足够水平（与上方向量的夹角小于60度）
                            if (top.distance == 0) continue;

                            // check if top hit is the same as horizontal hit

                            if (top.collider == hit.collider)
                            {
                                // check if point is free to climb
                                if (!PositionFreeToClimb(hit, top))
                                    continue;

                                if (Vector3.Dot(top.normal, Vector3.up) < 0.5f)
                                    continue;

                                // update current climb parameters
                                //6. 更新攀爬参数
                                _currentCollider = top.collider;
                                _currentTopHit = top;
                                _currentHorizontalHit = hit;

                                if(_currentCollider.tag == "Nail")
                                {
                                    PlayerPhysicalStrength.Instance.startRecovering();
                                }
                                else
                                {
                                    PlayerPhysicalStrength.Instance.stopRecovering();
                                }

                                if(ignoreTags.Contains(_currentCollider.tag))
                                    return false;
                                

                                // correct top point
                                //6. 更新攀爬参数
                                // 更新系统中的攀爬参数（当前碰撞体、水平碰撞点和顶部碰撞点）
                                // 检查碰撞体的标签是否在忽略列表中
                                // 修正顶部点的Y坐标，确保它与水平碰撞点在同一高度
                                // 调用UpdateClimbHit更新攀爬目标变换
                                Vector3 point = _currentHorizontalHit.point;
                                point.y = top.point.y;
                                _currentTopHit.point = point;

                                UpdateClimbHit();

                                // debug final hit found
                                DrawSphere(top.point, sphereCastRadius, Color.red);
                                Debug.DrawLine(topStart, top.point, Color.red);

                                // debug ray
                                Debug.DrawLine(topStart, top.point, Color.red);

                                return true;
                            }
                        }

                        // debug ray
                        Debug.DrawLine(topStart, topStart + Vector3.down * sphereCastMaxHeight, Color.red);
                    }
                }


                // closest precision cast if loose current collider
                //7. 丢失当前碰撞体的恢复机制
                // 当失去对当前攀爬表面的追踪时，系统会尝试重新找到可攀爬的表面
                // 创建ClimbablePoint结构体存储潜在的攀爬点信息
                // 对顶部点进行同样的验证
                if (_currentCollider == null)
                {
                    // set top start cast position
                    Vector3 initial = hit.point;
                    initial.y = grabReference.position.y + SphereCastHeight;

                    // loop through all hits
                    foreach (var top in Physics.SphereCastAll(initial, sphereCastRadius,
                        Vector3.down, 3f, climbMask, QueryTriggerInteraction.Collide))
                    {
                        // is top hit valid?
                        if (top.distance == 0) continue;

                        // check if top hit is the same as horizontal hit
                        if (top.collider == hit.collider)
                        {
                            // check if point is free to climb
                            if (!PositionFreeToClimb(hit, top))
                                continue;

                            if (Vector3.Dot(top.normal, Vector3.up) < 0.5f)
                                continue;

                            // create climbable point
                            ClimbablePoint climbable = new ClimbablePoint();
                            climbable.horizontalHit = hit;
                            climbable.verticalHit = top;

                            // correct top point
                            Vector3 point = hit.point;
                            point.y = top.point.y;
                            climbable.verticalHit.point = point;

                            // try get ledge component
                            if (top.collider.TryGetComponent(out Ledge ledge))
                            {
                                var closest = ledge.GetClosestPoint(climbable.verticalHit.point);
                                if (closest)
                                {
                                    climbable.verticalHit.point = closest.position;
                                    climbable.horizontalHit.normal = closest.forward;
                                }
                            }

                            // calculate factor to get closest point
                            climbable.factor = Mathf.Abs(_currentTopHit.point.y - grabReference.position.y);

                            // add to list
                            climbables.Add(climbable);
                        }
                    }

                    //8. 选择最佳攀爬点
                    //如果找到多个可能的攀爬点，根据"因子"排序
                    // 选择最合适的点更新攀爬参数
                    // 调用UpdateClimbHit更新攀爬目标变换
                    if (climbables.Count > 0)
                    {
                        // sort by closest distance
                        climbables.Sort((x, y) => y.factor.CompareTo(x.factor));
                        var climb = climbables[0];

                        // update current climb parameters
                        _currentCollider = climb.verticalHit.collider;
                        _currentTopHit = climb.verticalHit;
                        _currentHorizontalHit = climb.horizontalHit;

                        UpdateClimbHit();

                        return true;
                    }
                }
            }

            ResetClimbVars();
            return false;
        }
        /// <summary>
        /// 功能: 更新攀爬目标变换，确保角色正确跟随可能移动的攀爬表面
        /// </summary>
        private void UpdateClimbHit()
        {
            //当角色横向移动超过一定阈值 (_localCoordMove.x > 0.4f)
            // 当攀爬目标变换与当前碰撞体不同步时
            // 当能力刚开始运行或尚未运行时
            if (Mathf.Abs(_localCoordMove.x) > 0.4f || _climbTargetHit.parent != _currentCollider.transform 
                || !IsAbilityRunning || Time.time - StartTime < 0.1f)
            {
                _climbTargetHit.parent = _currentCollider.transform;
                _climbTargetHit.position = _currentTopHit.point;
                _climbTargetHit.forward = _currentHorizontalHit.normal;
            }

            _targetClimbCharPos.parent = _currentCollider.transform;
            _targetClimbCharPos.position = GetCharacterPositionOnLedge();
        }
        /// <summary>
        /// 检测角色脚部是否有墙面支撑
        /// </summary>
        /// <returns></returns>
        private bool HasWall()
        {
            //计算胶囊体的起始位置(基于攀爬目标位置或当前位置)
            // 确定检测方向(沿着攀爬表面法线的反方向或角色前方)
            // 使用 Physics.CapsuleCast 检测脚部前方是否有墙体
            // 通过调试绘制函数可视化胶囊体及其碰撞情况
            // 如果检测到墙体，返回 true，表示角色脚有支撑；否则返回 false，表示角色处于悬挂状态。这个结果会影响角色的悬挂动画权重。
            Vector3 targetPos = _currentHorizontalHit.collider != null && !_isDoingTween ? GetCharacterPositionOnLedge() : transform.position;
            Vector3 direction = _currentHorizontalHit.collider != null && !_isDoingTween ? -_currentHorizontalHit.normal : transform.forward;

            Vector3 capsuleBot = targetPos + Vector3.up * footCastRadius;
            Vector3 capsuleTop = targetPos + Vector3.up * (footCapsuleHeight - footCastRadius);

            DrawCapsule(capsuleTop, capsuleBot, footCastRadius, Color.cyan);

            if (Physics.CapsuleCast(capsuleBot, capsuleTop, footCastRadius, direction, out _wallHit, footCastDistance,
                footMask, QueryTriggerInteraction.Collide))
            {
                DrawCapsule(capsuleTop + direction * _wallHit.distance, capsuleBot + direction * _wallHit.distance, footCastRadius, Color.blue);
                return true;
            }

            return false;
        }

        private void UpdateContextVars()
        {
            _context.climb = this;
            _context.ik = _climbIK;
            _context.animator = _animator;
            _context.grabReference = grabReference;
            _context.transform = transform;
            _context.currentCollider = _currentCollider;
            _context.horizontalHit = _currentHorizontalHit;
            _context.topHit = _currentTopHit;
            _context.input = _localCoordMove;
        }

        /// <summary>
        /// 设置攀爬能力在特定动画完成后结束。
        /// </summary>
        public void FinishAfterAnimation(string animationState, Vector3 targetMatchPosition, Quaternion targetMatchRotation, float targetNormalizedTime = 0.9f)
        {
            _animationStateToWait = animationState;
            _waitingAnimation = true;

            _capsule.DisableCollision();

            _matchTarget = true;
            _matchTargetPosition = targetMatchPosition;
            _matchTargetRotation = targetMatchRotation;
            _targetNormalizedTime = targetNormalizedTime;
        }
        public void FinishAfterAnimation(string animationState, float targetNormalizedTime = 0.9f)
        {
            FinishAfterAnimation(animationState, Vector3.zero, Quaternion.identity, targetNormalizedTime);
            _matchTarget = false;
        }
        /// <summary>
        /// 将摄像机空间的输入转换为角色本地空间的移动方向
        /// </summary>
        private void ProccesMovement()
        {
            Vector3 CamForward = Vector3.Scale(_mainCamera.transform.forward, new Vector3(1, 0, 1));
            Vector3 cameraRelativeMove = _action.move.x * _mainCamera.transform.right + _action.move.y * CamForward;
            cameraRelativeMove.Normalize();

            _localCoordMove.x = Vector3.Dot(cameraRelativeMove, transform.right);
            _localCoordMove.y = Vector3.Dot(cameraRelativeMove, transform.forward);
        }
        /// <summary>
        /// 处理玩家攀爬状态下的输入命令
        /// </summary>
        private void ProccessInput()
        {
            // 当按下跳跃键时:
            // 如果没有水平移动或有明显的向上输入，触发攀爬上爬 (ClimbUp())
            // 如果有任何方向的输入，触发跳跃 (Jump())
            // 当按下下降键时，触发下落 (Drop())
            if (_action.jump)
            {
                if(Mathf.Approximately(_localCoordMove.x, 0) || _localCoordMove.y > 0.5f)
                    ClimbUp();


                if (_localCoordMove != Vector2.zero)
                    Jump();

            }

            if (_action.drop)
                Drop();
                //Debug.Log("Drop");
        }

        /// <summary>
        /// This function disable logic that set character position on ledge
        /// </summary>
        public void DisableTransformUpdate()
        {
            _updateTransform = false;
        }

        /// <summary>
        /// Allow logic to set character position on ledge
        /// </summary>
        public void EnableTransformUpdate()
        {
            _updateTransform = true; 
            
            _hangWeight = 0;
            _animator.SetFloat("HangWeight", _hangWeight);
        }

        public void DoTween(Vector3 targetPosition, Quaternion targetRotation, float duration, Collider targetLedge)
        {
            DoTween(targetPosition, targetRotation, duration, defaultMatchingCurve, targetLedge);
        }

        public void DoTween(Vector3 targetPosition, Quaternion targetRotation, float duration, AnimationCurve curve, Collider targetLedge)
        {
            // set target
            _tweenTarget.parent = targetLedge != null ? targetLedge.transform : null;

            // set base parameters for tween
            _isDoingTween = true;
            _currentTweenWeight = 0;
            _tweenDuration = duration;

            // set position paramters
            _tweenStartPosition = transform.position;
            _tweenTarget.position = targetPosition;

            // set rotation parameters
            _tweenStartRotation = transform.rotation;
            _tweenTarget.rotation = targetRotation;

            // set time control parameters
            _tweenStartTime = Time.time;
            _tweenStep = 1 / duration;

            // set curve
            _targetCurve = curve;

            // calculate bezier point
            Quaternion midRot = Quaternion.Lerp(_tweenStartRotation, _tweenTarget.rotation, 0.5f);
            Vector3 forward = midRot * Vector3.forward;
            _tweenBezierPoint = Vector3.Lerp(_tweenStartPosition, _tweenTarget.position, 0.5f) - forward;

            // stops root motion
            _mover.StopRootMotion();
        }

        private void UpdateTween()
        {
            if (!_isDoingTween) return;

            if (Time.time - _tweenStartTime > _tweenDuration + 0.1f || Mathf.Approximately(_currentTweenWeight, 1f))
            {
                if(_tweenTarget.position != Vector3.zero)
                    _mover.SetPosition(_tweenTarget.position);

                transform.rotation = _tweenTarget.rotation;

                _targetClimbCharPos.parent = _tweenTarget.parent;
                _targetClimbCharPos.position = _tweenTarget.position;
                _targetClimbCharPos.rotation = _tweenTarget.rotation;
                _lastHitPoint = _tweenTarget.position;

                _isDoingTween = false;
                return;
            }

            _currentTweenWeight = Mathf.MoveTowards(_currentTweenWeight, 1, _tweenStep * Time.deltaTime);

            float weight = _targetCurve.Evaluate(_currentTweenWeight);

            if (_tweenTarget.position != Vector3.zero)
            {
                if (Quaternion.Dot(_tweenStartRotation, _tweenTarget.rotation) > 0.85f)
                    _mover.SetPosition(Vector3.Lerp(_tweenStartPosition, _tweenTarget.position, weight));
                else
                    _mover.SetPosition(BezierLerp(_tweenStartPosition, _tweenTarget.position, _tweenBezierPoint, weight));
            }

            transform.rotation = Quaternion.Lerp(_tweenStartRotation, _tweenTarget.rotation, weight);
        }

        public Vector3 BezierLerp(Vector3 start, Vector3 end, Vector3 bezier, float t)
        {
            Vector3 point = Mathf.Pow(1 - t, 2) * start;
            point += 2 * (1 - t) * t * bezier;
            point += t * t * end;

            return point;
        }

        // blocks current time to be climbed during 1 second
        public void BlockCurrentLedge()
        {
            _ledgeBlocked = _currentCollider;
            _timeBlockStarted = Time.time;
        }

        public bool PositionFreeToClimb(RaycastHit horHit, RaycastHit topHit)
        {
            Vector3 targetCharacterPosition = GetCharacterPositionOnLedge(horHit, topHit);

            Vector3 bot = targetCharacterPosition + Vector3.up * _capsule.GetCapsuleRadius();
            Vector3 top = targetCharacterPosition + Vector3.up * (_capsule.GetCapsuleHeight() - _capsule.GetCapsuleRadius());

            if (Physics.OverlapCapsule(bot, top, _capsule.GetCapsuleRadius(), obstacleMask, QueryTriggerInteraction.Ignore).Length > 0)
                return false;

            return true;
        }

        private void SetCharacterPosition()
        {
            if (_isDoingTween || !_updateTransform) return;

            _mover.SetPosition(GetCharacterPositionOnLedge());
            transform.rotation = GetCharacterRotationOnLedge();
        }

        private Vector3 GetCharacterPositionOnLedge()
        {
            Vector3 normal = _climbTargetHit.forward;
            normal.y = 0;
            normal.Normalize();

            return _climbTargetHit.position + Vector3.up * offsetOnLedge.y + normal * offsetOnLedge.x;
        }

        public Vector3 GetCharacterPositionOnLedge(RaycastHit horHit, RaycastHit topHit)
        {
            Vector3 normal = horHit.normal;
            normal.y = 0;
            normal.Normalize();

           return topHit.point + Vector3.up * offsetOnLedge.y + normal * offsetOnLedge.x;
        }

        private Quaternion GetCharacterRotationOnLedge()
        {
            Vector3 normal = _climbTargetHit.forward;
            normal.y = 0;
            normal.Normalize();

            return Quaternion.LookRotation(-normal);
        }

        public Quaternion GetCharacterRotationOnLedge(RaycastHit horHit)
        {
            Vector3 normal = horHit.normal;
            normal.y = 0;
            normal.Normalize();

            return Quaternion.LookRotation(-normal);
        }

        public void ResetClimbVars()
        {
            _currentHorizontalHit = new RaycastHit();
            _currentTopHit = new RaycastHit();
            _currentCollider = null;
        }
        // 添加到ClimbAbility类中
        public void ForceDrop()
        {
            if (IsAbilityRunning && _context != null && _context.CurrentClimbState != null)
            {
                _context.CurrentClimbState.Drop(_context);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            if(grabReference != null && !IsAbilityRunning)
                Gizmos.DrawWireSphere(grabReference.position, globalRadiusDetection);
        }

        public void DrawSphere(Vector3 center, float radius, Color color, float duration = 0)
        {
            if (_debug)
                _debug.DrawSphere(center, radius, color, duration);
        }

        public void DrawCapsule(Vector3 p1, Vector3 p2, float radius, Color color, float duration = 0)
        {
            if (_debug)
                _debug.DrawCapsule(p1, p2, radius, color, duration);
        }

        public void DrawLabel(string text, Vector3 position, Color color, float duration = 0)
        {
            if (_debug)
                _debug.DrawLabel(text, position, color, duration);
        }
    }

}