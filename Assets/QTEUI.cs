
using UnityEngine;
using UnityEngine.UI;
using DiasGames.Components;
using DG.Tweening;
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
        [Range(0.1f, 0.8f)] public float QTEAccuracy = 0.1f;
        public float QTEBaseBarWidth;
        public float[] QTECorretBarWidthRange = new float[2];
        public AbilityScheduler scheduler;

        [Header("QTE条的移动速度 (几秒跑完整个进度条)")]
        public float PlayerPointPeiod = 3f;
        public bool isPlayerJudge = false;
        public float _clicktime = 0f;
        public bool isClick = false;

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
            SetQTEAccuracy();
            // if (Input.GetKeyDown(KeyCode.F))
            // {
            //     Debug.Log("test F key");
            // }
            if (isClick)
            {
                _clicktime += Time.deltaTime;
                WaitForJudge();
            }
        }

        void SetQTEAccuracy()
        {
            QTEAccuracy = characterStrength.currentPhysicalStrength / characterStrength.maxPhysicalStrength + 0.1f;
            QTEAccuracy = Mathf.Clamp(QTEAccuracy, 0.1f, 0.8f);
            QTECorretBarWidthRange[0] = QTEBaseBarWidth * (1 - QTEAccuracy) / 2;
            QTECorretBarWidthRange[1] = QTEBaseBarWidth * (1 + QTEAccuracy) / 2;
            CorretBar.rectTransform.sizeDelta = new Vector2(QTEAccuracy*QTEBaseBarWidth, CorretBar.rectTransform.sizeDelta.y);
        }
        public void StartClick()
        {
            isClick = true;
            isPlayerJudge = false;
            _clicktime =0f;
        }
        void WaitForJudge()
        {
            PlayerJudge();
            if (isPlayerJudge)
            {
                //如果玩家已经判断过了,就不再执行了
                return;
            }
            //指针的x坐标从QTEBaseBarWidth/2到-QTEBaseBarWidth/2之间移动
            float Speed = QTEBaseBarWidth / PlayerPointPeiod;
            Playerpoint.rectTransform.anchoredPosition = new Vector2(QTEBaseBarWidth - Speed * _clicktime, Playerpoint.rectTransform.anchoredPosition.y);

        }
        void PlayerJudge()
        {
            //如果玩家按下E键,新输入系统的Interact
            //Debug.Log(_action);
            //如果Playerpoint.rectTransform.anchoredPosition.x小于0,
            if (Playerpoint.rectTransform.anchoredPosition.x < 0)
            {
                TriggerFail();
            }
            if (_action.interact)
            {
                //如果指针在正确范围内
                if (Playerpoint.rectTransform.anchoredPosition.x > QTECorretBarWidthRange[0] && Playerpoint.rectTransform.anchoredPosition.x < QTECorretBarWidthRange[1])
                {
                    TriggerSucess();
                    //执行QTE成功，corretbar闪烁为绿色（Dotween）
                }
                else
                {
                    TriggerFail();    
                    //执行QTE失败的逻辑
                }
            }
        }
        
        void TriggerSucess()
        {
            //执行QTE成功的逻辑
            Debug.Log("QTE成功");
            CorretBar.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
            isPlayerJudge = true;  
            isClick = false;       

        }
        void TriggerFail()
        {
            //执行QTE失败的逻辑
            Debug.Log("QTE失败");
            CorretBar.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
            isPlayerJudge = true;    
            isClick = false;   
            PlayerPhysicalStrength.Instance.FailedOnQTE();  
        }

    }
}
    


