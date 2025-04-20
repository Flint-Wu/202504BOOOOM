using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using UnityEngine;

namespace DiasGames.Climbing
{
    [System.Serializable]
    public class ClimbDropState : ClimbStateBase
    {
        [SerializeField] private string dropToFall = "Climb.Braced Drop";
        [SerializeField] private string dropHop = "Climb.Hop Drop";
        [SerializeField] private float dropDuration = 0.3f;
        [Header("Casting")]
        [SerializeField] private float maxHeightBelow = 1.5f;
        [SerializeField] private float maxCastingDistance = 1.5f;
        [SerializeField] private float castRadius = 0.75f;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private float _startTime;
        //private BuildingSystem _buildingSystem;

        private RaycastHit _targetHorizontalHit;
        private RaycastHit _targetVerticalHit;
        private PlayerWaterState _playerWaterState;
        private bool isUseNail = false;

        public override void EnterState(ClimbStateContext context)
        {
            _playerWaterState = context.climb.GetComponent<PlayerWaterState>();
            //_buildingSystem = context.climb.GetComponent<BuildingSystem>();
            // 判断是跳跃下降还是自由下落
            if(context.animator.GetFloat("HangWeight") < 0.6f && FoundLedgeToDrop(context) && 
            !_playerWaterState.IsInCritical)
            {
                context.animator.CrossFadeInFixedTime(dropHop, 0.1f);
                context.climb.DoTween(_targetPosition, _targetRotation, dropDuration, _targetVerticalHit.collider);
                SetLefttHandIK(context);
                context.climb.StartCoroutine(ResetIK(context));
            }
            else
            {
                context.animator.CrossFadeInFixedTime(dropToFall, 0.1f);
                context.climb.FinishAfterAnimation(dropToFall);
                context.climb.BlockCurrentLedge();
            }

            _startTime = Time.time;
        }

        public override void ExitState(ClimbStateContext context)
        {
            _startTime = 0;
            isUseNail = false;
        }
        public override void Update(ClimbStateContext context)
        {

            // 如果已经使用了钉子，不继续检查
            // if(isUseNail) return;
            // // 检查是否在下落时间窗口内（0.6 * dropDuration）
            // if( Time.time - _startTime < dropDuration * 0.8f&&
            //     Time.time - _startTime > dropDuration*0.3f)
            // {
            //     // 尝试放置建筑物（钉子）
            //     BuildingSystem buildingSystem = context.climb.GetComponent<BuildingSystem>();
            //     if (buildingSystem != null)
            //     {
            //         Transform nailTransform = buildingSystem.PlaceBuildingPrefab(true);
            //         if(nailTransform != null)
            //         {
            //             // 设置急刹车状态的目标并切换状态
            //             context.Brake.SetTarget(nailTransform);
            //             context.SetState(context.Brake);
            //             return;
            //         }
            //     }
                
            // }

        }
        private void SetLefttHandIK(ClimbStateContext context)
        {
            context.ik.SetLeftHandJumpEffector(_targetVerticalHit.point);
        }

        private IEnumerator ResetIK(ClimbStateContext context)
        {
            yield return new WaitForSeconds(dropDuration * 0.6f);
            context.ik.SetLeftHandIKTarget(ClimbIK.TargetHandIK.OnLedge);
            context.ik.SetRightHandIKTarget(ClimbIK.TargetHandIK.OnLedge);
        }

        public override void Idle(ClimbStateContext context)
        {
            //// 等待下降动作完成
            //此方法控制状态转换，在下降动作完成后将角色状态切换回攀爬空闲状态。
            if (Time.time - _startTime < dropDuration) return;

            context.SetState(context.Idle);
        }

        private bool FoundLedgeToDrop(ClimbStateContext context)
        {
            // 此方法使用复杂的物理投射来找到下方可攀爬的点，具体步骤：
            // 在角色前方下方创建胶囊体
            // 向前投射胶囊体寻找可能的垂直表面
            // 从水平碰撞点向下投射球体寻找顶部表面
            // 应用多种条件过滤，确保找到的点是合适的攀爬位置
            // 计算每个点的"下降因素"，优先选择最符合向下方向的点

            Vector3 capsuleTop = context.grabReference.position + Vector3.down * castRadius - context.transform.forward * maxCastingDistance;
            Vector3 capsuleBot = context.grabReference.position + Vector3.down * (maxHeightBelow + castRadius) - context.transform.forward * maxCastingDistance;
            List<ClimbablePoint> _availablePoints = new List<ClimbablePoint>();

            foreach (var forwardHit in Physics.CapsuleCastAll(capsuleBot, capsuleTop, castRadius, context.transform.forward, 
                maxCastingDistance * 2, context.climb.ClimbMask, QueryTriggerInteraction.Collide))
            {
                // valid hit?
                if (forwardHit.distance == 0) continue;

                // calculate top start position
                Vector3 startTop = forwardHit.point;
                startTop.y = context.grabReference.position.y;

                foreach(var topHit in Physics.SphereCastAll(startTop, 0.2f, Vector3.down, maxHeightBelow,
                    context.climb.ClimbMask, QueryTriggerInteraction.Collide))
                {
                    // is a valid hit?
                    if (topHit.distance == 0) continue;

                    // is it the same collider?
                    if (topHit.collider != forwardHit.collider) continue;

                    RaycastHit hit = forwardHit;
                    RaycastHit top = topHit;

                    // try get ledge component
                    if(top.collider.TryGetComponent(out Ledge ledge))
                    {
                        var closest = ledge.GetClosestPoint(top.point);
                        if (closest)
                        {
                            top.point = closest.position;
                            hit.normal = closest.forward;
                        }
                    }

                    // check if point is free to climb
                    if (!context.climb.PositionFreeToClimb(hit, topHit)) continue;

                    // check point direction
                    if (Vector3.Dot(-hit.normal, context.transform.forward) < 0.7f) continue;

                    // calculate target position to get direction
                    Vector3 targetPosition = context.climb.GetCharacterPositionOnLedge(hit, top);
                    Vector3 direction = (targetPosition - context.transform.position).normalized;

                    // create new climb point to add to list
                    ClimbablePoint newPoint = new ClimbablePoint();
                    newPoint.horizontalHit = hit;
                    newPoint.verticalHit = top;
                    newPoint.factor = Vector3.Dot(Vector3.down, direction);

                    // add point to the list
                    _availablePoints.Add(newPoint);
                }
            }


            // found any point?
            if(_availablePoints.Count > 0)
            {
                _availablePoints.Sort((x, y) => y.factor.CompareTo(x.factor));
                var point = _availablePoints[0];

                _targetPosition = context.climb.GetCharacterPositionOnLedge(point.horizontalHit, point.verticalHit);
                _targetRotation = context.climb.GetCharacterRotationOnLedge(point.horizontalHit);

                _targetHorizontalHit = point.horizontalHit;
                _targetVerticalHit = point.verticalHit;
                return true;
            }

            return false;
        }

        //来自ClimbAbility.cs
    }
}