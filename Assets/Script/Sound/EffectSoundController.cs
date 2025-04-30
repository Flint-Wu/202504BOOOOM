using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSoundController : MonoBehaviour
{
    public static EffectSoundController Instance { get; private set; }
    public AudioSource EffectAudioSource;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    [SerializeField] private AudioClip[] ClimbClip;
    [SerializeField] private AudioClip[] DropClip;
    [SerializeField] private AudioClip[] GameOverClip;
    [SerializeField] private AudioClip[] UsePinClip;
    [SerializeField] private AudioClip[] RecoverWaterClip;
    [SerializeField] private AudioClip[] ResetPhysicalClip;
    [SerializeField] private AudioClip[] WarningClip;
    [SerializeField] private AudioClip[] UIConfirmClip;
    [SerializeField] private AudioClip[] UIFailClip;
    [SerializeField] private AudioClip[] UISuccessClip;
    [SerializeField] private AudioClip[] SpecialClip;
    [SerializeField] private AudioClip[] PourTreeClip;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayAudioClip(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    void PlaySoundOnEffect(AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        //播放音效：会顶替
        
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        EffectAudioSource.clip = clip;
        EffectAudioSource.Play();
    }
    public void PlayClimbAudioClip()
    {
        PlaySoundOnEffect(ClimbClip);
    }
    public void PlayDropAudioClip()
    {
        PlayAudioClip(DropClip);
    }
    public void PlayGameOverAudioClip()
    {
        PlayAudioClip(GameOverClip);
    }
    public void PlayUsePinAudioClip()
    {
        PlayAudioClip(UsePinClip);
    }
    public void PlayRecoverWaterAudioClip()
    {
        PlayAudioClip(RecoverWaterClip);
    }
    public void PlayWarningAudioClip()
    {
        PlayAudioClip(WarningClip);
    }
    public void PlayUIConfirmAudioClip()
    {
        PlayAudioClip(UIConfirmClip);
    }
    public void PlayUIFailAudioClip()
    {
        PlayAudioClip(UIFailClip);
    }
    public void PlayUISuccessAudioClip()
    {
        PlayAudioClip(UISuccessClip);
    }
    public void PlaySpecailAudioClip()
    {
        PlayAudioClip(SpecialClip);
    }
    public void PlayResetPhysicalAudioClip()
    {
        PlayAudioClip(ResetPhysicalClip);
    }

    public void PlayPourTreeAudioClip(int index)
    {
        if (index < 0 || index >= ResetPhysicalClip.Length) return;
        AudioClip clip = PourTreeClip[index];
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}
