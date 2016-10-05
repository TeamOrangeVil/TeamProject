using UnityEngine;
using System.Collections;
using System;

struct EventSoundName
{
    //이벤트
    public const string givenClothes = "Clothes";
    public const string spiderRun = "SpiderRun";
    public const string spiderAtk = "SpiderAtk";
    public const string spiderIn = "SpiderIn";
    public const string spiderHome = "SpiderHome";

    //시스템
    public const string gameover = "GameOver";
    public const string rewind = "Rewind";
    public const string tutoInfo = "Info";
    public const string checkPoint = "CheckPoint";
    public const string fadeInOut = "FadeInOut";
    public const string ropeFork = "RopeFork";
    public const string hpLost = "HpLost";
    public const string showSetting = "ShowSetting";
    public const string dameged = "Dameged";

};

//오브젝트 사운드 이펙트를 위한 매니져 입니다.
[RequireComponent(typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager gInstance = null;

    public AudioSource effectAudio;
    public AudioSource bgmAudio;
    public AudioSource playerAudio;
    public AudioSource menDeskAudio;

    public AudioClip[] bgm;
    public AudioClip[] soundEffects;

    public UISlider sliderSound;// 이펙트 소리 크기 조절
    public UISlider sliderBgm;//  배경음 소리 크기 조절

    SoundOption soundOption; //사운드 옵션 받아옴

    public int bgmNum;

    public static SoundEffectManager Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }
    void Awake()
    {
        gInstance = this;
        effectAudio = GetComponent<AudioSource>();
    }
    public void SoundOptionSet()
    {
        if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.GAMEOPTIONXML))
        {
            soundOption = XMLParsing.Instance.XmlLoadOption();
            EffectSoundControl(soundOption.effValue);
            BGMSoundControl(soundOption.bgmValue);
            effectAudio.mute = soundOption.effMute;
            playerAudio.mute = soundOption.effMute;
            menDeskAudio.mute = soundOption.effMute;
            bgmAudio.mute = soundOption.bgmMute;
        }
    }
    public void EffectSoundControl(float soundValue)
    {
        effectAudio.volume = soundValue;
        playerAudio.volume = soundValue;
        menDeskAudio.volume = soundValue;
    }
    public void BGMSoundControl(float soundValue)
    {
        bgmAudio.volume = soundValue;
    }
    public void EffectSoundStop()
    {
        effectAudio.Stop();
    }
    public void EffectMuteCheck()
    {
        if (!effectAudio.mute)
        {
            effectAudio.mute = true;
            playerAudio.mute = true;
            menDeskAudio.mute = true;
        }
        else
        {
            effectAudio.mute = false;
            playerAudio.mute = false;
            menDeskAudio.mute = false;
        }
    }
    public void bgmMuteCheck()
    {
        if (!bgmAudio.mute)
        {
            bgmAudio.mute = true;
        }
        else
        {
            bgmAudio.mute = false;
        }
    }
    public void SoundEnd()
    {
        effectAudio.loop = false;
        effectAudio.Stop();
    }
    public void GetAudio(AudioSource audio)
    {
        menDeskAudio = audio;
    }
    public void BGMStart(int bgmNumber)
    {
        if (bgmAudio.isPlaying) { bgmAudio.Stop(); }
        bgmAudio.clip = bgm[bgmNumber];
        bgmNum = bgmNumber;
        bgmAudio.Play();
    }
    public void SoundDelay(string soundName, float waitTime)
    {
        if (effectAudio.loop) { effectAudio.loop = false; }
        switch (soundName)
        {
            //옵젝 ---------------------------------------------------------------
            case Objname.hpPlus://채력템
                effectAudio.PlayOneShot(soundEffects[0]);
                break;
            case Objname.checkPointPlus://쳌포템
                effectAudio.PlayOneShot(soundEffects[6]);
                break;
            case Objname.key01://열쇠
                effectAudio.PlayOneShot(soundEffects[1]);
                break;
            case Objname.compass://컴퍼스
                effectAudio.PlayOneShot(soundEffects[2]);
                break;
            case Objname.men://서랍장 맨 구출
                effectAudio.PlayOneShot(soundEffects[3]);
                break;
            case Objname.toolBox://공구통
                effectAudio.PlayOneShot(soundEffects[7]);
                break;
            case Objname.safeSawdust://안전 톱밥
                effectAudio.PlayOneShot(soundEffects[8]);
                break;
            //함정 ---------------------------------------------------------------
            case TrapNames.mobleFloor://발판 추락
                effectAudio.PlayOneShot(soundEffects[4]);
                break;
            case TrapNames.brokenHouse://박살나는 집
                effectAudio.PlayOneShot(soundEffects[5]);
                break;
            //기타 상황별 --------------------------------------------------------
            case EventSoundName.gameover://게임 오버(추락사)
                effectAudio.PlayOneShot(soundEffects[9]);
                break;
            case EventSoundName.checkPoint://쳌포 발동
                effectAudio.PlayOneShot(soundEffects[10]);
                break;
            case EventSoundName.rewind://되감기
                effectAudio.clip = soundEffects[11];
                effectAudio.loop = true;
                effectAudio.Play();
                break;
            case EventSoundName.fadeInOut://페이드 인아웃
                effectAudio.PlayOneShot(soundEffects[12]);
                break;
            case EventSoundName.dameged://함정 충돌
                effectAudio.PlayOneShot(soundEffects[13]);
                break;
            case EventSoundName.tutoInfo://튣토 메시지
                effectAudio.PlayOneShot(soundEffects[14]);
                break;
            case EventSoundName.spiderRun://거미 지나감
                effectAudio.PlayOneShot(soundEffects[15]);
                break;
            case EventSoundName.givenClothes://옷 받음
                effectAudio.PlayOneShot(soundEffects[16]);
                break;
            case EventSoundName.spiderAtk://거미 공격
                effectAudio.PlayOneShot(soundEffects[17]);
                break;
            case EventSoundName.spiderHome://거미 본거지
                effectAudio.PlayOneShot(soundEffects[18]);
                break;
            case EventSoundName.spiderIn://거미 입 장
                effectAudio.PlayOneShot(soundEffects[19]);
                break;
        }
    }
}
