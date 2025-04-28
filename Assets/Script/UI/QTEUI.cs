
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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
        [SerializeField] private Image[] OtherImage;
        [SerializeField] private Image CorretBar;
        [Header("正确QTE跳出的精灵")]
        [SerializeField] private Image CorrectSprite;
        [Header("错误QTE跳出的精灵")]
        [SerializeField] private Image WrongSprite;
        [Header("QTE条的指针")]
        public Image Playerpoint;
        [SerializeField] private PlayerPhysicalStrength characterStrength;
        
        //QTE的正确率为当前体力值与最大体力值的比值+10%,其取值为0.1-0.8之间
        [Header("QTE的正确率为当前体力值与最大体力值的比值+10%,其取值为0.1-0.8之间")]
        [Range(0.1f, 0.8f)] public float BaseQTEAccuracy = 0.1f;
        public float QTEBaseBarHeight;
        public float[] QTECorretBarHeightRange = new float[2];
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
        [Header("是否是玩家跳跃和QTE出现的Gaptime（gaptime禁用跳跃操作以防bug）")]
        public bool isGapTime = false;
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
            QTEBaseBarHeight = BaseBar.rectTransform.sizeDelta.y;

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

        Image[] uiElements = { BaseBar, CorretBar, Playerpoint, OtherImage[0], OtherImage[1]};
            
            foreach (Image uiElement in uiElements)
            {
                // 杀死动画
                uiElement.DOKill(true);
                
                // 确保激活
                uiElement.gameObject.SetActive(true);
                
                // 重置并应用淡入效果
                uiElement.color = new Color(uiElement.color.r, uiElement.color.g, uiElement.color.b, 0);
                uiElement.DOFade(1, 0.2f).SetEase(Ease.OutSine).SetUpdate(true);
            }
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
            OtherImage[0].DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => OtherImage[0].gameObject.SetActive(false));
            OtherImage[1].DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => OtherImage[1].gameObject.SetActive(false));
            BaseBar.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => BaseBar.gameObject.SetActive(false));
            CorretBar.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => CorretBar.gameObject.SetActive(false));
            //如果CorrectSprite active,就隐藏
            if (CorrectSprite.gameObject.activeSelf)
            {
                CorrectSprite.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => CorrectSprite.gameObject.SetActive(false));
            }
            //如果WrongSprite active,就隐藏
            if (WrongSprite.gameObject.activeSelf)
            {
                WrongSprite.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() => WrongSprite.gameObject.SetActive(false));
            }
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
            BaseBar.rectTransform.sizeDelta = new Vector2(BaseBar.rectTransform.sizeDelta.x,QTEBaseBarHeight*newBaseBarWidthPercentage);


            float QTEAccuracy = characterStrength.currentPhysicalStrength / characterStrength.maxPhysicalStrength*BaseQTEAccuracy;
            QTEAccuracy = Mathf.Clamp(QTEAccuracy, 0.1f, BaseQTEAccuracy);

            QTECorretBarHeightRange[0] = Random.Range(QTEBaseBarHeight*StartPercentage, QTEBaseBarHeight * (1 - QTEAccuracy));
            QTECorretBarHeightRange[1] = QTECorretBarHeightRange[0] + QTEBaseBarHeight * QTEAccuracy;
            CorretBar.rectTransform.anchoredPosition = new Vector2(CorretBar.rectTransform.anchoredPosition.x,QTECorretBarHeightRange[0]);
            CorretBar.rectTransform.sizeDelta = new Vector2(CorretBar.rectTransform.sizeDelta.x,QTEAccuracy*BaseBar.rectTransform.sizeDelta.y);
        }
        public void StartClick()
        {
            // 防止重复触发
            if (isClicking) return;
            
            // 使用协程代替Invoke
            StartCoroutine(DelayedClickingStart());
        }

        private IEnumerator DelayedClickingStart()
        {
            // 使用WaitForSeconds而非Invoke
            yield return new WaitForSeconds(0.6f);
            
            isClicking = true;
            isPlayerJudge = false;
            EnableBar();
            SetQTEAccuracy();
            isGapTime = false;
        }

        void WaitForJudge()
        {
            
            if (isPlayerJudge)
            {
                //如果玩家已经判断过了,就不再执行了
                return;
            }
            //指针的x坐标从QTEBaseBarWidth/2到-QTEBaseBarWidth/2之间移动
            float Speed = QTEBaseBarHeight / PlayerPointPeiod;
            Playerpoint.rectTransform.anchoredPosition = new Vector2(Playerpoint.rectTransform.anchoredPosition.x,Speed * _clicktime);
            PlayerJudge();
        }
        void PlayerJudge()
        {
            //如果玩家按下E键,新输入系统的Interact
            //Debug.Log(_action);
            //如果Playerpoint.rectTransform.anchoredPosition.x小于0,
            //float newBaseBarWidthPercentage = characterStrength.currentPhysicalStrength / characterStrength.maxPhysicalStrength+0.1f;
            if (Playerpoint.rectTransform.anchoredPosition.y > QTEBaseBarHeight*newBaseBarWidthPercentage)
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
                float Speed = QTEBaseBarHeight / PlayerPointPeiod;
                //指针变为蓝模拟结冰的效果
                if(decayTime!=0)
                {
                    Playerpoint.DOColor(Color.blue, decayTime).SetLoops(2, LoopType.Yoyo);
                }

                Playerpoint.rectTransform.DOLocalMoveY(Playerpoint.rectTransform.anchoredPosition.y+Speed * decayTime, decayTime).SetEase(Ease.OutSine)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() => 
                    {
                        // 在动画完成后获取最终位置进行判断
                        float currenty = Playerpoint.rectTransform.anchoredPosition.y;
                        
                        if (currenty > QTECorretBarHeightRange[0] && currenty < QTECorretBarHeightRange[1])
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
            CorrectSprite.gameObject.SetActive(true);
            CorrectSprite.DOKill(true);
            CorrectSprite.color = new Color(CorrectSprite.color.r, CorrectSprite.color.g, CorrectSprite.color.b, 1);
            CorrectSprite.transform.DOShakeScale(0.5f, 0.2f, 10, 0, false).SetEase(Ease.OutSine);
            isQTEfail = false;
        }
        void TriggerFail()
        {
            //执行QTE失败的逻辑
            Debug.Log("QTE失败");
            BaseBar.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
            WrongSprite.gameObject.SetActive(true);
            WrongSprite.DOKill(true);
            WrongSprite.color = new Color(WrongSprite.color.r, WrongSprite.color.g, WrongSprite.color.b, 1);
            WrongSprite.rectTransform.DOShakeAnchorPos(0.5f, 10f).SetEase(Ease.OutSine);
            // PlayerPhysicalStrength.Instance.FailedOnQTE();
            ClimbAbility climbAbility = GameObject.FindGameObjectWithTag("Player").GetComponent<ClimbAbility>();
            //climbAbility.StopAbility();
            climbAbility.ForceDrop();
            
            playerWaterState.ChangeWater();
            // if(playerWaterState.IsInCritical)
            // {
            //     this.transform.root.GetComponentInChildren<Health>().Damage(200);
            //     //通过Health组件来判断死亡
            // }
            Debug.Log(playerWaterState.CurrentWater);
        }

        void ResetClicking()
        {
            isClicking = false;
            _clicktime =0f;  
        }

    }
}
    


