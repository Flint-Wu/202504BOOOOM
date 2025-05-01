using UnityEngine;

namespace DiasGames.Components
{
    public class CharacterAudioPlayer : MonoBehaviour
    {
        public static CharacterAudioPlayer Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private AudioSource effectsSource;
        [SerializeField] private AudioSource 旁白音效;
        [SerializeField] private AudioClip[] StartAudioClip;
        [SerializeField] private AudioClip[] FallenAudioClip;
        [SerializeField] private AudioClip[] SlipAudioClip;
        [SerializeField] private AudioClip[] UseTreeAudioClip;
        [SerializeField] private AudioClip[] WarningUseNailAudioClip;
        [SerializeField] private AudioClip[] WaterIsDepletedAudioClip;
        [SerializeField] private AudioClip[] UseFruitAudioClip;
        [SerializeField] private AudioClip[] UseNailAudioClip;
        [SerializeField] private AudioClip[] GetAwayNailAudioClip;
        [SerializeField] private AudioClip[] NotPourWaterAudioClip;
        [SerializeField] private AudioClip[] PourWaterAudioClip;
        [SerializeField] private AudioClip[] GetHillTopAudioClip;
        [SerializeField] private AudioClip[] UseSeedAudioClip;
        [SerializeField] private AudioClip[] RandomAudioClip;

        void Start()
        {
            if(旁白音效 == null)
            {
                旁白音效 = GameObject.Find("旁白音效").GetComponent<AudioSource>();
            }
            for (int i = 0; i < StartAudioClip.Length; i++)
            {
                if (StartAudioClip[i] != null)
                {
                    Play旁白音效(StartAudioClip[i]);
                }
            }
        }

        void Update()
        {
            //随机播放音效的频率为1minute一次
            if (RandomAudioClip.Length > 0 && Time.time % 60 < 1)
            {
                PlayRandomAudioClip();
            }
        }
        public void PlayFallenAudioClip()
        {
            if (FallenAudioClip.Length == 0) return;

            AudioClip clip = FallenAudioClip[Random.Range(0, FallenAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlaySlipAudioClip()
        {
            if (SlipAudioClip.Length == 0) return;
            //百分之50概率不播放
            if (Random.Range(0, 100) < 50) return;

            int index = Random.Range(0, SlipAudioClip.Length);
            AudioClip clip = SlipAudioClip[index];

            Play旁白音效(clip);
            //之后不在播放这个音效
            SlipAudioClip[index] = null;
        }

        public void PlayUseTreeAudioClip()
        {
            if (UseTreeAudioClip.Length == 0) return;

            AudioClip clip = UseTreeAudioClip[Random.Range(0, UseTreeAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayWarningUseNailAudioClip()
        {
            if (WarningUseNailAudioClip.Length == 0) return;


            AudioClip clip = WarningUseNailAudioClip[Random.Range(0, WarningUseNailAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayWaterIsDepletedAudioClip()
        {
            if (WaterIsDepletedAudioClip.Length == 0) return;

            AudioClip clip = WaterIsDepletedAudioClip[Random.Range(0, WaterIsDepletedAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayUseFruitAudioClip()
        {
            if (UseFruitAudioClip.Length == 0) return;

            AudioClip clip = UseFruitAudioClip[Random.Range(0, UseFruitAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayGetAwayAudioClip()
        {
            if (GetAwayNailAudioClip.Length == 0) return;

            AudioClip clip = GetAwayNailAudioClip[Random.Range(0, GetAwayNailAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayNotPourWaterAudioClip()
        {
            if (NotPourWaterAudioClip.Length == 0) return;

            AudioClip clip = NotPourWaterAudioClip[Random.Range(0, NotPourWaterAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayPourWaterAudioClip()
        {
            if (PourWaterAudioClip.Length == 0) return;

            AudioClip clip = PourWaterAudioClip[Random.Range(0, PourWaterAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayUseNailAudioClip()
        {
            if (UseNailAudioClip.Length == 0) return;

            AudioClip clip = UseNailAudioClip[Random.Range(0, UseNailAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayClimbToTheHillTopAudioClip()
        {
            if (GetHillTopAudioClip.Length == 0) return;

            AudioClip clip = GetHillTopAudioClip[Random.Range(0, GetHillTopAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayUseSeedAudioClip()
        {
            if (UseSeedAudioClip.Length == 0) return;

            AudioClip clip = UseSeedAudioClip[Random.Range(0, UseSeedAudioClip.Length)];

            Play旁白音效(clip);
        }

        public void PlayRandomAudioClip()
        {
            if (RandomAudioClip.Length == 0) return;

            AudioClip clip = RandomAudioClip[Random.Range(0, RandomAudioClip.Length)];

            Play旁白音效(clip);
        }
        public void PlayVoice(AudioClip clip)
        {
            if (voiceSource == null) return;

            voiceSource.clip = clip;
            voiceSource.Play();
        }
        public void PlayVoice(AudioClip[] clips)
        {
            if (voiceSource == null || clips.Length == 0) return;

            AudioClip clip = clips[Random.Range(0, clips.Length)];

            voiceSource.clip = clip;
            voiceSource.Play();
        }
        public void Play旁白音效(AudioClip clip)
        {
            if (旁白音效 == null) return;
            //如果已经在播放旁白音效，return
            if (旁白音效.isPlaying) return;

            旁白音效.clip = clip;
            旁白音效.Play();
        }

        public void PlayEffect(AudioClip clip)
        {
            if (effectsSource == null) return;

            effectsSource.clip = clip;
            effectsSource.Play();
        }

        public void PlayEffect(AudioClip[] clips)
        {
            if (effectsSource == null || clips.Length == 0) return;

            AudioClip clip = clips[Random.Range(0, clips.Length)];

            effectsSource.clip = clip;
            effectsSource.Play();
        }
    }
}