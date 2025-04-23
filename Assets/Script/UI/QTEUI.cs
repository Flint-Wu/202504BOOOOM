
using UnityEngine;
using UnityEngine.UI;
using DiasGames.Components;
using DG.Tweening;
using DiasGames.Climbing;
using System.Collections;
namespace DiasGames.Abilities
{
    public class QTEUI : AbstractAbility
    {
        public static QTEUI Instance { get; private set; }
        // Start is called before the first frame update
        [Header("QTE条的基础条")]
        [SerializeField] private Image BaseBar;
        [Header("QTE条的正确条")]
        [SerializeField] private Image CorretBar;
        public Image Playerpoint;
        [SerializeField] private PlayerPhysicalStrength characterStrength;
        
        //QTE的正确率为当前体力值与最大体力值的比值+10%,其取值为0.1-0.8之间
        [Header("QTE的正确率为当前体力值与最大体力值的比值+10%,其取值为0.1-0.8之间")]
        [Range(0.1f, 0.8f)] public float BaseQTEAccuracy = 0.1f;
        public float QTEBaseBarWidth;
        public float[] QTECorretBarWidthRange = new float[2];
        public AbilityScheduler scheduler;

        [Header("QTE条的移动速度 (几秒跑完整个进度条)")]
        public float PlayerPointPeiod = 3f;
        [Header("QTE条出现的初始位置限制")]
        public float StartPercentage = 0.2f;
        public bool isPlayerJudge = false;
        public float _clicktime = 0f;
        public bool isClicking = false;
        public bool isQTEfail = false;
        //光标迟滞停止的时间
        [Header("光标迟滞停止的时间,模拟结冰的效果")]
        public float decayTime = 0.1f;
        
        [Header("玩家的水量状态")]
        public PlayerWaterState playerWaterState;
        [Header("玩家QTE失败水量流失量")]
        public int lossWater = 10;
        private float newBaseBarWidthPercentage = 1f;
    // 在类的顶部，和其他实例变量一起定义
        private bool isBarVisible = true;
        void Awake()
        {
            if (scheduler != null)
            {
                // 手动设置 action 引用
                SetActionReference(ref scheduler.characterActions);
            }
            else
            {
                Debug.LogError("找不到 AbilityScheduler 组件，无法获取输入动作！");
            }
            Instance = this;
            QTEBaseBarWidth = BaseBar.rectTransform.sizeDelta.x;

            StartCoroutine(DisableBar());
        }
        public override bool ReadyToRun()
        {
            // Implement logic to determine if the ability is ready to run
            return true; // Default implementation, modify as needed
        }
        
        public override void OnStartAbility()
        {
            // Implement logic for when the ability starts
        }

        public override void UpdateAbility()
        {
            // Implement logic for updating the ability
        }
        // Update is called once per frame
        void Update()
        {
            
            // if (Input.GetKeyDown(KeyCode.F))
            // {
            //     Debug.Log("test F key");
            // }
            if (isClicking)
            {
                WaitForJudge();
                _clicktime += Time.deltaTime;
                //SetQTEAccuracy();

            }
        }

       void EnableBar()
        {
            // 停止所有协程（包括正在运行的DisableBar协程）
            StopAllCoroutines();
            
            // 杀死所有相关DOTween动画以防止冲突
            BaseBar.DOKill(true); // true参数表示完成当前动画
            CorretBar.DOKill(true);
            Playerpoint.DOKill(true);
            
            // 确保对象处于活动状态
            BaseBar.gameObject.SetActive(true);
            CorretBar.gameObject.SetActive(true);
            Playerpoint.gameObject.SetActive(true);
            
            // 重置颜色（如果之前可能改变了颜色）
            BaseBar.color = new Color(BaseBar.color.r, BaseBar.color.g, BaseBar.color.b, 0);
            CorretBar.color = new Color(CorretBar.color.r, CorretBar.color.g, CorretBar.color.b, 0);
            Playerpoint.color = new Color(Playerpoint.color.r, Playerpoint.color.g, Playerpoint.color.b, 0);
            
            // 应用淡入效果
            BaseBar.DOFade(1, 0.2f).SetEase(Ease.OutSine).SetUpdate(true);
            CorretBar.DOFade(1, 0.2f).SetEase(Ease.OutSine).SetUpdate(true);
            Playerpoint.DOFade(1, 0.2f).SetEase(Ease.OutSine).SetUpdate(true);
            
            // 记录状态，表示UI现在是可见的
            isBarVisible = true;
        }
        IEnumerator DisableBar()
        {
            // 如果UI已经被隐藏，直接退出
            if (!isBarVisible) yield break;
            
            // 等待延迟
            yield return new WaitForSeconds(0.5f);
            
            // 如果在等待期间UI被重新激活，退出协程
            if (!isBarVisible) yield break;
            
            // 应用淡出效果
            BaseBar.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => BaseBar.gameObject.SetActive(false));
            CorretBar.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => CorretBar.gameObject.SetActive(false));
            Playerpoint.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => 
            {
                Playerpoint.gameObject.SetActive(false);
                isBarVisible = false; // 更新状态
                ResetClicking(); // 重置点击状态
            });
        }
        void SetQTEAccuracy()
        {
            //BaseBar的宽度根据体力值的变化而变化，判断条的宽度为BaseBar的宽度*QTEAccuracy
            //float newBaseBarWidthPercentage = characterStrength.currentPhysicalStrength / characterStrength.maxPhysicalStrength+0.1f;
            BaseBar.rectTransform.sizeDelta = new Vector2(QTEBaseBarWidth*newBaseBarWidthPercentage, BaseBar.rectTransform.sizeDelta.y);


            float QTEAccuracy = characterStrength.currentPhysicalStrength / characterStrength.maxPhysicalStrength*BaseQTEAccuracy;
            QTEAccuracy = Mathf.Clamp(QTEAccuracy, 0.1f, BaseQTEAccuracy);

            QTECorretBarWidthRange[0] = Random.Range(QTEBaseBarWidth*StartPercentage, QTEBaseBarWidth * (1 - QTEAccuracy));
            QTECorretBarWidthRange[1] = QTECorretBarWidthRange[0] + QTEBaseBarWidth * QTEAccuracy;
            CorretBar.rectTransform.anchoredPosition = new Vector2(QTECorretBarWidthRange[0], CorretBar.rectTransform.anchoredPosition.y);
            CorretBar.rectTransform.sizeDelta = new Vector2(QTEAccuracy*BaseBar.rectTransform.sizeDelta.x, CorretBar.rectTransform.sizeDelta.y);
        }
        public void StartClick()
        {
            // if(_clicktime>0.1f&&_clicktime<PlayerPointPeiod)
            // {
            //     //如果玩家还没有判断过（没触发QTE就继续跳跃），就直接触发失败
            //     TriggerFail();
            //     return;
            // }
            isClicking = true;
            isPlayerJudge = false;
            EnableBar();
            SetQTEAccuracy();

        }
        void WaitForJudge()
        {
            
            if (isPlayerJudge)
            {
                //如果玩家已经判断过了,就不再执行了
                return;
            }
            //指针的x坐标从QTEBaseBarWidth/2到-QTEBaseBarWidth/2之间移动
            float Speed = QTEBaseBarWidth / PlayerPointPeiod;
            Playerpoint.rectTransform.anchoredPosition = new Vector2(Speed * _clicktime, Playerpoint.rectTransform.anchoredPosition.y);
            PlayerJudge();
        }
        void PlayerJudge()
        {
            //如果玩家按下E键,新输入系统的Interact
            //Debug.Log(_action);
            //如果Playerpoint.rectTransform.anchoredPosition.x小于0,
            //float newBaseBarWidthPercentage = characterStrength.currentPhysicalStrength / characterStrength.maxPhysicalStrength+0.1f;
            if (Playerpoint.rectTransform.anchoredPosition.x > QTEBaseBarWidth*newBaseBarWidthPercentage)
            {
                isPlayerJudge = true;
                TriggerFail();
                StartCoroutine(DisableBar());
                return;
            }


            if (_action.jump)
            {
                //Dotween实现playerpoint迟滞停止的效果
                isPlayerJudge = true; 
                float Speed = QTEBaseBarWidth / PlayerPointPeiod;
                //指针变为蓝模拟结冰的效果
                if(decayTime!=0)
                {
                    Playerpoint.DOColor(Color.blue, decayTime).SetLoops(2, LoopType.Yoyo);
                }

                Playerpoint.rectTransform.DOLocalMoveX(Playerpoint.rectTransform.anchoredPosition.x+Speed * decayTime, decayTime).SetEase(Ease.OutSine)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() => 
                    {
                        // 在动画完成后获取最终位置进行判断
                        float currentX = Playerpoint.rectTransform.anchoredPosition.x;
                        
                        if (currentX > QTECorretBarWidthRange[0] && currentX < QTECorretBarWidthRange[1])
                        {
                            TriggerSucess();
                            // 可以在这里添加绿色闪烁效果（如DOTween颜色动画）
                        }
                        else
                        {
                            TriggerFail();
                            // 可以在这里添加红色闪烁效果或其他失败反馈
                        }
                        StartCoroutine(DisableBar());
                    });
            }
        }
        
        public void TriggerSucess()
        {
            //执行QTE成功的逻辑
            Debug.Log("QTE成功");
            BaseBar.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
            isQTEfail = false;
        }
        void TriggerFail()
        {
            //执行QTE失败的逻辑
            Debug.Log("QTE失败");
            BaseBar.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
 
            
            // PlayerPhysicalStrength.Instance.FailedOnQTE();
            ClimbAbility climbAbility = GameObject.FindGameObjectWithTag("Player").GetComponent<ClimbAbility>();
            climbAbility.ForceDrop();
            
            playerWaterState.ChangeWater();
            if(playerWaterState.IsInCritical)
            {
                this.transform.root.GetComponentInChildren<Health>().Damage(200);
                //通过Health组件来判断死亡
            }
            Debug.Log(playerWaterState.CurrentWater);
        }

        void ResetClicking()
        {
            isClicking = false;
            _clicktime =0f;  
        }

    }
}
    


