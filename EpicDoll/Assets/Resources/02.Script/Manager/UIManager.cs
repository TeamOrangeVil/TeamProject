using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager gInstance = null;
    List<EventScript> eventScripts;
    List<InfoScript> InfoScripts;
    SoundOption soundOp;

    // UI 이미지 관련
    public TypewriterEffect WriteEffect;
    public UILabel TextLabel;
    public UISprite HelperSprite;
    public UISprite SaveDollSprite;
    public UISprite DancerSprite;
    public UISprite KnightSprite;
    public UISprite PlayerSprite;
    public UISprite Theme01PlayerSprite;
    public UISprite TextLogSprite;
    public UILabel SkipLabel;
    public UILabel CutToonSkipLabel;

    public UISprite HelpTextLogSprite;
    public UILabel HelpTextLabel;
    public TypewriterEffect HelpWriteEffect;

    public UISprite ThemeSlot1;
    public UISprite ThemeSlot2;
    public UISprite ThemeSlot3;
    public UISprite ThemeSlot4;
    public UISprite[] ThemeSlots;
    public Vector3[] ThemeSlotPos;

    public GameObject target;

    public UISprite FKey;
    public UISprite DKey;
    public UISprite WKey;
    public UISprite RKey;
    public UISprite SpaceKey;
    public UISprite SpaceKey2;
    public UISprite RightKey;
    public UISprite LeftKey;
    public UISprite SaveItem;
    public UISprite TrapUI;
    public UISprite HPItemUI;
    public UISprite Cloud;
    public UISprite Nail;
    public UISprite SawDust;
    public UISprite Compass;
    public UISprite DeskKey;
    public UISprite LockDesk;


    public UISprite HPBar;
    public UISprite CheckBar;

    public UISprite FadeBoard;
    public UISprite HintMaskBoard;
    public UISprite HintBlackBoard;
    public UISprite HintBoard;
    public UILabel HintSkipLabel;
    public UIPanel HintPanel1;
    public UIPanel HintPanel2;
    public UISprite HintArrow;

    public float WaitCount = 0.0f; // 코루틴 대기 시간
    public int StateUICount = 0;
    public int TextIndex = 0; // 대사 순서

    Hashtable HintArrowHT = new Hashtable();
    Hashtable PlayerHash = new Hashtable(); //ITWEEN MoveTo 함수 용
    Hashtable NPCHash = new Hashtable();    //ITWEEN MoveTo 함수 용

    public UISprite HintAlarmR;
    public UISprite HintAlarmSpace;
    public GameObject targetPlayer;     // 타겟이 되는 플레이어 캐릭터
    public Camera cameraUI;             // 바라볼 카메라
    public Vector2 screenPos;           // 월드 좌표를 스크린 좌표로 받을 벡터

    // 설정 창 관련
    public GameObject InGameUIWindow;
    public GameObject SoundOptionWindow;
    public GameObject MyInfoWindow;
    public GameObject HelpWindow;
    public GameObject StaffWindow;
    public UISprite InGameUIWindowSprite;
    public UIPanel SettingPanel;

    public UIButton SFXSoundButton;
    public UISprite SFXSoundButtonSprite;
    public UIButton SFXBoxButton;
    public UISprite SFXBoxButtonSprite;
    public UISlider SFXSoundSlider;
    public UIButton BackgroundSoundButton;
    public UISprite BackgroundSoundButtonSprite;
    public UIButton BackgroundBoxButton;
    public UISprite BackgroundBoxButtonSprite;
    public UISlider BackgroundSoundSlider;

    public UILabel InfoProcess;
    public UILabel InfoPlayTime;
    public UILabel InfoWalkDistance;
    public UILabel InfoHitCount;
    public UILabel InfoDieCount;

    public GameObject XButton;

    // 캐릭터 정보창 관련
    public Transform player;
    public Vector3 startPositionPlayer;
    public float playerDistanceX;
    public float PlayerWalk;

    public bool bgmMute;
    public bool sfxMute;

    public GameObject EndingCredit;
    public float CreditSpeed = 0.001f;

    public void OnInGameUI() // 메인 매뉴 UI
    {
        if (UIButton.current.name.Equals("StartButton"))
        {
            Time.timeScale = 1;
            SettingPanel.enabled = false; // 설정 패널 비활성화
            InGameUIWindow.SetActive(false); // 첫 설정 창 비활성화
        }
        if (UIButton.current.name.Equals("MyInfoButton"))
        {
            XButton.SetActive(true);
            InGameUIWindow.SetActive(false);
            MyInfoWindow.SetActive(true);
            InfoProcess.text = string.Format( "{0:0.00}",((100 / 21) * TutorialManager.Instance.ProgressCount)) + " %";

            // 플레이 시간
            float minute = GameManager.Instance.playTime / 60;
            float second = GameManager.Instance.playTime % 60;
            InfoPlayTime.text = string.Format("{0:00.}",minute)  + " 분 " + string.Format("{0:00.}", second) + "초";
            // 움직인 거리
            InfoWalkDistance.text = string.Format("{0:0.00}", PlayerWalk)  + " mm";
            // 맞은 횟수
            InfoHitCount.text = GameManager.Instance.hitTimes.ToString() + " 회";
            // 죽은 횟수
            InfoDieCount.text = GameManager.Instance.deadTimes.ToString() + " 회";
        }
        if (UIButton.current.name.Equals("OptionButton"))
        {
            XButton.SetActive(true);
            InGameUIWindow.SetActive(false);
            SoundOptionWindow.SetActive(true);

            if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.GAMEOPTIONXML))
            {
                soundOp = XMLParsing.Instance.XmlLoadOption();
                sfxMute = soundOp.effMute;
                bgmMute = soundOp.bgmMute;
                SFXSoundSlider.value = soundOp.effValue;
                BackgroundSoundSlider.value = soundOp.bgmValue;
                if (soundOp.effMute)
                {
                    SFXSoundButton.normalSprite = "sound mute";
                    SFXSoundButtonSprite.spriteName = "sound mute";
                    SFXBoxButton.normalSprite = "box check";
                    SFXBoxButtonSprite.spriteName = "box check";
                }
                else
                {
                    SFXSoundButton.normalSprite = "sound";
                    SFXSoundButtonSprite.spriteName = "sound";
                    SFXBoxButton.normalSprite = "box";
                    SFXBoxButtonSprite.spriteName = "box";
                }
                if (soundOp.bgmMute)
                {
                    BackgroundBoxButton.normalSprite = "box check";
                    BackgroundBoxButtonSprite.spriteName = "box check";
                    BackgroundSoundButton.normalSprite = "sound mute";
                    BackgroundSoundButtonSprite.spriteName = "sound mute";
                }
                else
                {
                    BackgroundBoxButton.normalSprite = "box";
                    BackgroundBoxButtonSprite.spriteName = "box";
                    BackgroundSoundButton.normalSprite = "sound";
                    BackgroundSoundButtonSprite.spriteName = "sound";
                }
            }
        }
        if (UIButton.current.name.Equals("HelpButton"))
        {
            XButton.SetActive(true);
            InGameUIWindow.SetActive(false);
            HelpWindow.SetActive(true);
        }
        if (UIButton.current.name.Equals("ExitButton"))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(01, LoadSceneMode.Single);
        }
        if(UIButton.current.name.Equals("XButton"))
        {
            if(!InGameUIWindow.activeInHierarchy)
            {
                XButton.SetActive(false);
                InGameUIWindow.SetActive(true);
                MyInfoWindow.SetActive(false);
                if (SoundOptionWindow.activeInHierarchy.Equals(true))
                {
                    XMLParsing.Instance.XmlSaveOption(soundOp.effValue, soundOp.bgmValue, soundOp.effMute, soundOp.bgmMute);
                    SoundOptionWindow.SetActive(false);
                }
                HelpWindow.SetActive(false);
            }
        }
    }

    public void OnSoundSliderUI() // 사운드 슬라이더 관련
    {
        if (UISlider.current.name.Equals("SFXSlider"))
        {
            soundOp.effValue = UISlider.current.value;
            SoundEffectManager.Instance.EffectSoundControl(UISlider.current.value);
        }
        if (UISlider.current.name.Equals("BackgroundSlider"))
        {
            soundOp.bgmValue = UISlider.current.value;
            SoundEffectManager.Instance.BGMSoundControl(UISlider.current.value);
        }
    }

    public void OnSoundButtonUI() // 사운드 버튼 관련
    {
        if (UIButton.current.name.Equals("SFXSoundButton")) // 효과음 사운드 버튼
        {
            if (UIButton.current.normalSprite.Equals("sound"))
            {
                SFXSoundButton.normalSprite = "sound mute";
                SFXSoundButtonSprite.spriteName = "sound mute";
                SFXBoxButton.normalSprite = "box check";
                SFXBoxButtonSprite.spriteName = "box check";
                SoundEffectManager.Instance.EffectMuteCheck();
                soundOp.effMute = true;
            }
            else
            {
                SFXSoundButton.normalSprite = "sound";
                SFXSoundButtonSprite.spriteName = "sound";
                SFXBoxButton.normalSprite = "box";
                SFXBoxButtonSprite.spriteName = "box";
                SoundEffectManager.Instance.EffectMuteCheck();
                soundOp.effMute = false;
            }
        }
        if (UIButton.current.name.Equals("SFXBoxButton"))  // 효과음 박스 버튼
        {
            if (UIButton.current.normalSprite.Equals("box"))
            {
                SFXSoundButton.normalSprite = "sound mute";
                SFXSoundButtonSprite.spriteName = "sound mute";
                SFXBoxButton.normalSprite = "box check";
                SFXBoxButtonSprite.spriteName = "box check";
                SoundEffectManager.Instance.EffectMuteCheck();
                soundOp.effMute = true;
            }
            else
            {
                SFXSoundButton.normalSprite = "sound";
                SFXSoundButtonSprite.spriteName = "sound";
                SFXBoxButton.normalSprite = "box";
                SFXBoxButtonSprite.spriteName = "box";
                SoundEffectManager.Instance.EffectMuteCheck();
                soundOp.effMute = false;
            }
        }

        if (UIButton.current.name.Equals("BackgroundSoundButton")) // 배경음 사운드 버튼
        {
            if (UIButton.current.normalSprite.Equals("sound"))
            {
                BackgroundBoxButton.normalSprite = "box check";
                BackgroundBoxButtonSprite.spriteName = "box check";
                BackgroundSoundButton.normalSprite = "sound mute";
                BackgroundSoundButtonSprite.spriteName = "sound mute";
                SoundEffectManager.Instance.bgmMuteCheck();
                soundOp.bgmMute = true;
            }
            else
            {
                BackgroundBoxButton.normalSprite = "box";
                BackgroundBoxButtonSprite.spriteName = "box";
                BackgroundSoundButton.normalSprite = "sound";
                BackgroundSoundButtonSprite.spriteName = "sound";
                SoundEffectManager.Instance.bgmMuteCheck();
                soundOp.bgmMute = false;
            }
        }
        if (UIButton.current.name.Equals("BackgroundBoxButton")) // 배경음 박스 버튼
        {
            if (UIButton.current.normalSprite.Equals("box"))
            {
                BackgroundBoxButton.normalSprite = "box check";
                BackgroundBoxButtonSprite.spriteName = "box check";
                BackgroundSoundButton.normalSprite = "sound mute";
                BackgroundSoundButtonSprite.spriteName = "sound mute";
                SoundEffectManager.Instance.bgmMuteCheck();
                soundOp.bgmMute = true;
            }
            else
            {
                BackgroundBoxButton.normalSprite = "box";
                BackgroundBoxButtonSprite.spriteName = "box";
                BackgroundSoundButton.normalSprite = "sound";
                BackgroundSoundButtonSprite.spriteName = "sound";
                SoundEffectManager.Instance.bgmMuteCheck();
                soundOp.bgmMute = false;
            }
        }
    }

    /*
     * if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingPanel.enabled.Equals(false)) // 설정 패널이 활성화 되지 않았을 경우
            {
                Time.timeScale = 0;
                SettingPanel.enabled = true; // 설정 패널 활성화
                InGameUIWindow.SetActive(true); // 첫 설정 창 활성화
            }
            else // 설정 패널이 활성화 된 경우
            {
                if (InGameUIWindow.activeInHierarchy.Equals(true)) // 첫 설정 창이 활성화 된 경우
                {
                    Time.timeScale = 1;
                    SettingPanel.enabled = false; // 설정 패널 비활성화
                    InGameUIWindow.SetActive(false); // 첫 설정 창 비활성화
                }
                else if (SoundOptionWindow.activeInHierarchy.Equals(true)) // 사운드 설정 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    SoundOptionWindow.SetActive(false);
                }
                else if (MyInfoWindow.activeInHierarchy.Equals(true)) // 내 정보 설정 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    MyInfoWindow.SetActive(false);
                }
                else if (HelpWindow.activeInHierarchy.Equals(true)) // 도움말 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    HelpWindow.SetActive(false);
                }
                else if (StaffWindow.activeInHierarchy.Equals(true)) // 제작진 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    StaffWindow.SetActive(false);
                }
            }
    */

    public static UIManager Instance
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

        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Start()
    {
        // iTween.MoveTo(HintArrow.gameObject, iTween.Hash("x", -80, "time", 0.9f, "islocal", true, "looptype", "loop", "easeType", "linear"));
        //HintArrowHT.Add("position", new Vector3(-110,-241,0));
        HintArrowHT.Add("x", -80);
        HintArrowHT.Add("time", 0.9f);
        HintArrowHT.Add("islocal", true);
        HintArrowHT.Add("looptype", "loop");
        HintArrowHT.Add("easeType", "linear");

        //PlayerHash.Add("position", Vector2.right * 50.0f); // 적용될 오브젝트의 위치
        PlayerHash.Add("islocal", true); // 로컬 좌표계로 이동
        PlayerHash.Add("x", 1f);
        PlayerHash.Add("time", 1.0f);
        //PlayerHash.Add("delay", 0.1f);
        //PlayerHash.Add("speed", 1.0f); // 움직임 속도
        PlayerHash.Add("easetype", iTween.EaseType.easeOutExpo);
        //PlayerHash.Add("onstarttarget", this.gameObject);
        //PlayerHash.Add("oncompletetarget", this.gameObject);
        //PlayerHash.Add("onstart", "OnItweenStart");
        //PlayerHash.Add("onupdate", "OnItweenUpdate");
        //PlayerHash.Add("oncomplete", "OnItweenEnd");
        //PlayerHash.Add("looktarget", Vector2.zero);

        eventScripts = new List<EventScript>();
        eventScripts = XMLParsing.Instance.EventScriptsLoad(1);
        InfoScripts = new List<InfoScript>();
        InfoScripts = XMLParsing.Instance.InfoScriptsLoad(1);

        TextLabel = GameObject.Find("TextLabel").GetComponent<UILabel>();
        WriteEffect = GameObject.Find("TextLabel").GetComponent<TypewriterEffect>();
        HelperSprite = GameObject.Find("HelperSprite").GetComponent<UISprite>();
        SaveDollSprite = GameObject.Find("SaveDollSprite").GetComponent<UISprite>();
        DancerSprite = GameObject.Find("DancerSprite").GetComponent<UISprite>();
        KnightSprite = GameObject.Find("KnightSprite").GetComponent<UISprite>();
        PlayerSprite = GameObject.Find("PlayerSprite").GetComponent<UISprite>();
        SkipLabel = GameObject.Find("SkipLabel").GetComponent<UILabel>();
        CutToonSkipLabel = GameObject.Find("CutToonSkipLabel").GetComponent<UILabel>();

        Theme01PlayerSprite = GameObject.Find("Theme01PlayerSprite").GetComponent<UISprite>();
        TextLogSprite = GameObject.Find("TextLogSprite").GetComponent<UISprite>();
        ThemeSlot1 = GameObject.Find("ThemeSlot01").GetComponent<UISprite>();
        ThemeSlot2 = GameObject.Find("ThemeSlot02").GetComponent<UISprite>();
        ThemeSlot3 = GameObject.Find("ThemeSlot03").GetComponent<UISprite>();
        ThemeSlot4 = GameObject.Find("ThemeSlot04").GetComponent<UISprite>();

        target = GameObject.FindGameObjectWithTag("PLAYER");
        ThemeSlots = new UISprite[] { ThemeSlot1, ThemeSlot2, ThemeSlot3, ThemeSlot4 };
        ThemeSlotPos = new Vector3[4];

        FKey = GameObject.Find("FKey").GetComponent<UISprite>();
        DKey = GameObject.Find("DKey").GetComponent<UISprite>();
        WKey = GameObject.Find("WKey").GetComponent<UISprite>();
        RKey = GameObject.Find("RKey").GetComponent<UISprite>();
        SpaceKey = GameObject.Find("SpaceKey").GetComponent<UISprite>();
        SpaceKey2 = GameObject.Find("SpaceKey2").GetComponent<UISprite>();
        RightKey = GameObject.Find("RightKey").GetComponent<UISprite>();
        LeftKey = GameObject.Find("LeftKey").GetComponent<UISprite>();
        SaveItem = GameObject.Find("SaveItem").GetComponent<UISprite>();
        HPItemUI = GameObject.Find("HPItem").GetComponent<UISprite>();
        TrapUI = GameObject.Find("Trap").GetComponent<UISprite>();
        Cloud = GameObject.Find("Cloud").GetComponent<UISprite>();
        Nail = GameObject.Find("NailImage").GetComponent<UISprite>();
        SawDust = GameObject.Find("SawDust").GetComponent<UISprite>();
        Compass = GameObject.Find("Compass").GetComponent<UISprite>();
        DeskKey = GameObject.Find("DeskKey").GetComponent<UISprite>();
        LockDesk = GameObject.Find("LockDesk").GetComponent<UISprite>();

        FadeBoard = GameObject.Find("FadeBoard").GetComponent<UISprite>();
        HintMaskBoard = GameObject.Find("HintMaskBoard").GetComponent<UISprite>();
        HintBlackBoard = GameObject.Find("HintBlackBoard").GetComponent<UISprite>();
        HintBoard = GameObject.Find("HintBoard").GetComponent<UISprite>();
        HintSkipLabel = GameObject.Find("HintSkipLabel").GetComponent<UILabel>();
        HintPanel1 = GameObject.Find("HintPanel01").GetComponent<UIPanel>();
        HintPanel2 = GameObject.Find("HintPanel02").GetComponent<UIPanel>();
        HintArrow = GameObject.Find("HintArrow").GetComponent<UISprite>();

        HintAlarmR = GameObject.Find("HintAlarmR").GetComponent<UISprite>();
        HintAlarmSpace = GameObject.Find("HintAlarmSpace").GetComponent<UISprite>();

        InGameUIWindowSprite = GameObject.Find("InGameUIWindowSprite").GetComponent<UISprite>();
        SettingPanel = GameObject.Find("SettingPanel").GetComponent<UIPanel>();

        InfoProcess = GameObject.Find("Process").GetComponent<UILabel>();
        InfoPlayTime = GameObject.Find("PlayTime").GetComponent<UILabel>();
        InfoWalkDistance = GameObject.Find("WalkDistance").GetComponent<UILabel>();
        InfoHitCount = GameObject.Find("HitCount").GetComponent<UILabel>();
        InfoDieCount = GameObject.Find("DieCount").GetComponent<UILabel>();
        

        //InGameUIWindow = GameObject.Find("InGameUIWindow").GetComponent<GameObject>();

        TextLabel.enabled = false;
        HelperSprite.enabled = false;
        SaveDollSprite.enabled = false;
        DancerSprite.enabled = false;
        KnightSprite.enabled = false;
        PlayerSprite.enabled = false;
        Theme01PlayerSprite.enabled = false;
        TextLogSprite.enabled = false;
        SkipLabel.enabled = false;
        CutToonSkipLabel.enabled = false;

        HelpTextLogSprite.enabled = false;
        HelpTextLabel.enabled = false;

        ThemeSlot1.enabled = false;
        ThemeSlot2.enabled = false;
        ThemeSlot3.enabled = false;
        ThemeSlot4.enabled = false;

        FKey.enabled = false;
        DKey.enabled = false;
        WKey.enabled = false;
        RKey.enabled = false;
        SpaceKey.enabled = false;
        SpaceKey2.enabled = false;
        RightKey.enabled = false;
        LeftKey.enabled = false;
        SaveItem.enabled = false;
        HPItemUI.enabled = false;
        TrapUI.enabled = false;
        Cloud.enabled = false;
        Nail.enabled = false;
        SawDust.enabled = false;
        Compass.enabled = false;
        DeskKey.enabled = false;
        LockDesk.enabled = false;

        HPBar.enabled = false;
        CheckBar.enabled = false;

        FadeBoard.enabled = false;
        HintMaskBoard.enabled = false;
        HintBlackBoard.enabled = false;
        HintBoard.enabled = false;
        HintSkipLabel.enabled = false;
        HintArrow.enabled = false;

        FadeBoard.SetScreenRect(0, 0, Screen.width, Screen.height);

        HintAlarmR.enabled = false;
        HintAlarmSpace.enabled = false;

        //InGameUIWindowSprite.SetScreenRect(0,0, Screen.width, Screen.height);
        //InGameUIWindowSprite.SetRect(0, 0, Screen.width, Screen.height);
        InGameUIWindowSprite.SetScreenRect(0, 0, Screen.width*3, Screen.height*3);
        //InGameUIWindowSprite.SetScreenRect(-Screen.width, -Screen.height, (int)(Screen.width*1.5f), (int)(Screen.height*1.5f));
        InGameUIWindow.SetActive(false);
        SoundOptionWindow.SetActive(false);
        MyInfoWindow.SetActive(false);
        HelpWindow.SetActive(false);
        StaffWindow.SetActive(false);
        SettingPanel.enabled = false;

        XButton.SetActive(false);

        //InfoProcess.enabled = false;
        //InfoPlayTime.enabled = false;
        //InfoWalkDistance.enabled = false;
        //InfoHitCount.enabled = false;
        //InfoDieCount.enabled = false;

        startPositionPlayer = player.position;

        if (!System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.GAMEOPTIONXML))
        {
            soundOp.bgmValue = 1;
            soundOp.effValue = 1;
            soundOp.bgmMute = false;
            soundOp.effMute = false;
        }
    }

    void FixedUpdate()
    {
        playerDistanceX = (startPositionPlayer.x - player.position.x);
        playerDistanceX = playerDistanceX < 0 ? -playerDistanceX : playerDistanceX; 
        PlayerWalk += playerDistanceX;
        startPositionPlayer = player.position;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingPanel.enabled.Equals(false)) // 설정 패널이 활성화 되지 않았을 경우
            {
                Time.timeScale = 0;
                SettingPanel.enabled = true; // 설정 패널 활성화
                InGameUIWindow.SetActive(true); // 첫 설정 창 활성화
                XButton.SetActive(false);
            }
            else // 설정 패널이 활성화 된 경우
            {
                if (InGameUIWindow.activeInHierarchy.Equals(true)) // 첫 설정 창이 활성화 된 경우
                {
                    Time.timeScale = 1;
                    SettingPanel.enabled = false; // 설정 패널 비활성화
                    InGameUIWindow.SetActive(false); // 첫 설정 창 비활성화
                    XButton.SetActive(false);
                }
                else if (SoundOptionWindow.activeInHierarchy.Equals(true)) // 사운드 설정 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    SoundOptionWindow.SetActive(false);
                    XButton.SetActive(false);
                }
                else if (MyInfoWindow.activeInHierarchy.Equals(true)) // 내 정보 설정 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    MyInfoWindow.SetActive(false);
                    XButton.SetActive(false);
                }
                else if (HelpWindow.activeInHierarchy.Equals(true)) // 도움말 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    HelpWindow.SetActive(false);
                    XButton.SetActive(false);
                }
                else if (StaffWindow.activeInHierarchy.Equals(true)) // 제작진 창이 활성화 된 경우
                {
                    InGameUIWindow.SetActive(true);
                    StaffWindow.SetActive(false);
                    XButton.SetActive(false);
                }
            }
            //bool isSA;
            //Time.timeScale = Time.timeScale == 1.0f ? 0.0f : 1.0f;
            //isSA = SettingPanel.isActiveAndEnabled == true ? false : true;
            //SettingPanel.enabled = isSA;
            //esc누르면 
        }
        //HintBoard.SetScreenRect(0, Screen.height/3, Screen.width, (int)HintBoard.localSize.y);
        //HintBoard.SetRect(0, 0, (int)HintBoard.localSize.x, (int)HintBoard.localSize.y);
        //Vector3 test = HintBoard.localCenter;
        HintBoard.width = Screen.width * 2;
        //HintBoard.transform.localPosition = test;
        //PlayerSprite.SetScreenRect(Screen.width/2, 0, (int)PlayerSprite.localSize.x, (int)PlayerSprite.localSize.y);
        //HintBoard.localSize = new Vector3(Screen.width, HintBoard.transform.localScale.y, HintBoard.transform.localScale.z);

        screenPos = Camera.main.WorldToScreenPoint(targetPlayer.transform.position); // Awake Start에 설치해도 되나 신이 달라서
        HintAlarmR.transform.position = cameraUI.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y + 10.0f,1));
        HintAlarmSpace.transform.position = cameraUI.ScreenToWorldPoint(new Vector3(screenPos.x - 5.0f, screenPos.y,1));

    }
    IEnumerator PlayerDie()
    {
        float i = 0;
        while (i < 0.99f)
        {
            float color = FadeBoard.GetComponent<UISprite>().alpha;
            color = i;
            FadeBoard.GetComponent<UISprite>().alpha = color;
            i += 0.1f;
            yield return 0;
        }
        FadeBoard.GetComponent<UISprite>().alpha = 1;
        while (i > 0.01f)
        {
            float color = FadeBoard.GetComponent<UISprite>().alpha;
            color = i;
            FadeBoard.GetComponent<UISprite>().alpha = color;
            i -= 0.02f;
            yield return new WaitForSeconds(0.05f);
        }

        yield return 0;
    }
    IEnumerator FadeOut() // 급 어두워지는 효과
    {
        SoundEffectManager.Instance.SoundDelay("FadeInOut", 0);
        FadeBoard.transform.position = CharacterController2D.Instance.transform.position - Vector3.forward;
        FadeBoard.transform.localScale = new Vector3(Screen.width, Screen.height * 0.5f, -1);
        FadeBoard.enabled = true;

         for (float i = 0f; i <= 1; i += 0.05f)
         {
             float color = FadeBoard.GetComponent<UISprite>().alpha;
             color = i;
             FadeBoard.GetComponent<UISprite>().alpha = color;
             yield return 0;
         }
        FadeBoard.GetComponent<UISprite>().alpha = 1;
    }

    IEnumerator FadeIn() // 점점 밝아지는 효과
    {
        for (float i = 1f; i >= 0; i -= 0.01f)
        {
            float color = FadeBoard.GetComponent<UISprite>().alpha;
            color = i;
            FadeBoard.GetComponent<UISprite>().alpha = color;
            yield return 0;
        }
        FadeBoard.enabled = false;
    }

    public void TalkStart(int TextIndex) // 대화 시작
    {
        TextLogSprite.enabled = true; //대화 배경 ON
        TextLabel.enabled = true; // 대사 ON
        SkipLabel.enabled = true;

        TextLabel.text = eventScripts[TextIndex].script; // 해당 스크립트
        WriteEffect.ResetToBeginning(); // 라이팅 출력

        if (eventScripts[TextIndex].target.Equals("Player")) // 스크립트 대사의 타겟이 플레이어일때
        {
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema01)) //플레이어의 테마가 1일 경우
            {
                //iTween.MoveTo(PlayerSprite.gameObject, PlayerHash);
                iTween.MoveTo(PlayerSprite.gameObject, iTween.Hash("x", 320, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(PlayerSprite.gameObject, iTween.Hash("x", 1.1f, "y", 1.1f, "time", 0.2f));
                PlayerSprite.enabled = true;
                PlayerSprite.color = new Vector4(1, 1, 1, 1);
            }
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema02)) // 플레이어의 테마가 2일 경우
            {
                iTween.MoveTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 320, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 1.1f, "y", 1.1f, "time", 0.2f));
                Theme01PlayerSprite.enabled = true;
                Theme01PlayerSprite.color = new Vector4(1, 1, 1, 1);
            }
            if (eventScripts[TextIndex].other.Equals("Helper")) // 상대방이 안내원 이라면
            {
                iTween.MoveTo(HelperSprite.gameObject, iTween.Hash("x", -211, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(HelperSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                HelperSprite.enabled = true;
                HelperSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1); // 듣는 역할은 어두운 이미지
            }
            if (eventScripts[TextIndex].other.Equals("SaveDoll")) // 상대방이 서랍장에 갇힌 인형 이라면
            {
                Debug.Log("상대방이 서랍장에 갇힌 인형 이라면");////////////////////////////////////////////////////////////
                iTween.MoveTo(SaveDollSprite.gameObject, iTween.Hash("x", -211, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(SaveDollSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                SaveDollSprite.enabled = true;
                SaveDollSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
            }
            if (eventScripts[TextIndex].other.Equals("Dancer")) // 상대방이 댄서라면
            {
                iTween.MoveTo(DancerSprite.gameObject, iTween.Hash("x", -211, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(DancerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                DancerSprite.enabled = true;
                DancerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
            }
            if (eventScripts[TextIndex].other.Equals("Knight")) // 상대방이 기사라면
            {
                iTween.MoveTo(KnightSprite.gameObject, iTween.Hash("x", -211, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(KnightSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                KnightSprite.enabled = true;
                KnightSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
            }

        }
        if (eventScripts[TextIndex].target.Equals("Helper")) // 스크립트 대사의 타겟이 안내원 이라면
        {
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema01))
            {
                //iTween.MoveTo(HelperSprite.gameObject, PlayerHash);
                // iTween.MoveTo(HelperSprite.gameObject, iTween.Hash("x", -1, "time", 0.2f, "islocal", true));
                iTween.MoveTo(PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                PlayerSprite.enabled = true;
            }
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema02))
            {
                iTween.MoveTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                Theme01PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                Theme01PlayerSprite.enabled = true;
            }
            iTween.MoveTo(HelperSprite.gameObject, iTween.Hash("x", -291, "time", 0.9f, "islocal", true));
            iTween.ScaleTo(HelperSprite.gameObject, iTween.Hash("x", 1.1f, "y", 1.1f, "time", 0.2f));
            HelperSprite.enabled = true;
            HelperSprite.color = new Vector4(1, 1, 1, 1);
        }
        if (eventScripts[TextIndex].target.Equals("SaveDoll")) // 스크립트 대사의 타겟이 안내원 이라면
        {
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema01))
            {
                iTween.MoveTo(PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                PlayerSprite.enabled = true;
            }
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema02))
            {
                iTween.MoveTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                Theme01PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                Theme01PlayerSprite.enabled = true;
            }
            Debug.Log("스크립트 대사의 타겟이 안내원 이라면");//////////////////////////////////////////////////////
            iTween.MoveTo(HelperSprite.gameObject, iTween.Hash("x", -291, "time", 0.9f, "islocal", true));
            iTween.ScaleTo(HelperSprite.gameObject, iTween.Hash("x", 1.1f, "y", 1.1f, "time", 0.2f));
            SaveDollSprite.enabled = true;
            SaveDollSprite.color = new Vector4(1, 1, 1, 1);
        }
        if (eventScripts[TextIndex].target.Equals("Dancer")) // 스크립트 대사의 타겟이 댄서라면
        {
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema01))
            {
                iTween.MoveTo(PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                PlayerSprite.enabled = true;
            }
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema02))
            {
                iTween.MoveTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                Theme01PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                Theme01PlayerSprite.enabled = true;
            }
            iTween.MoveTo(DancerSprite.gameObject, iTween.Hash("x", -291, "time", 0.9f, "islocal", true));
            iTween.ScaleTo(DancerSprite.gameObject, iTween.Hash("x", 1.1f, "y", 1.1f, "time", 0.2f));
            DancerSprite.enabled = true;
            DancerSprite.color = new Vector4(1, 1, 1, 1);
        }
        if (eventScripts[TextIndex].target.Equals("Knight")) // 스크립트 대사의 타겟이 기사라면
        {
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema01))
            {
                iTween.MoveTo(PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                PlayerSprite.enabled = true;
            }
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema02))
            {
                iTween.MoveTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 214, "time", 0.9f, "islocal", true));
                iTween.ScaleTo(Theme01PlayerSprite.gameObject, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.2f));
                Theme01PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                Theme01PlayerSprite.enabled = true;
            }
            iTween.MoveTo(KnightSprite.gameObject, iTween.Hash("x", -291, "time", 0.9f, "islocal", true));
            iTween.ScaleTo(KnightSprite.gameObject, iTween.Hash("x", 1.1f, "y", 1.1f, "time", 0.2f));
            KnightSprite.enabled = true;
            KnightSprite.color = new Vector4(1, 1, 1, 1);
        }
    }

    public float WaitCheck(int TextIndex) // 대사 출력 시간
    {
        WaitCount = 0;
        int i = eventScripts[TextIndex].script.Length;
        if (i > 15) // 텍스트의 길이가 15 이상이라면
        {
            WaitCount = i * 0.07f;
            return WaitCount;
        }
        else // 그 외에
        {
            WaitCount = i * 0.13f;
            return WaitCount;
        }
    }

    public void UIOff() // 대화 UI OFF
    {
        TextLabel.enabled = false;
        HelperSprite.enabled = false;
        SaveDollSprite.enabled = false;
        DancerSprite.enabled = false;
        KnightSprite.enabled = false;
        PlayerSprite.enabled = false;
        Theme01PlayerSprite.enabled = false;
        TextLogSprite.enabled = false;
        SkipLabel.enabled = false;
        Compass.enabled = false;
    }

    public void HelpPrint(int TextIndex) // 안내 대사 ON
    {
        SoundEffectManager.Instance.SoundDelay("Info", 0);
        HelpTextLogSprite.enabled = true;
        HelpTextLabel.enabled = true;
        HelpTextLabel.text = InfoScripts[TextIndex].script; // 해당 스크립트
    }

    public void HintPrint(int TextIndex, int choose) // 마스크 형태를 가지고 있는 힌트 도움말
    {
        switch (choose)
        {
            case 0:
                HintBlackBoard.enabled = true; // 전체가 검은
                HintBoard.enabled = true; // 도움말이 달릴
                HelpTextLabel.enabled = true;
                HelpTextLabel.text = InfoScripts[TextIndex].script; // 해당 스크립트
                break;
            case 1:
                HintMaskBoard.enabled = true; // 마스크가 적용된
                HintBoard.enabled = true;
                HintArrow.enabled = true;
                HelpTextLabel.enabled = true;
                HelpTextLabel.text = InfoScripts[TextIndex].script; // 해당 스크립트
                break;
        }
    }
    /*
    1. (-110,-241) 2 (-101,-294) 3 1번과 같음 4 (6, 283)
    */
    public void HelpUIOFF() // 모든 안내 UI OFF
    {
        HelpTextLogSprite.enabled = false;
        HelpTextLabel.enabled = false;
        HintMaskBoard.enabled = false;
        FKey.enabled = false;
        DKey.enabled = false;
        WKey.enabled = false;
        RKey.enabled = false;
        SpaceKey.enabled = false;
        SpaceKey2.enabled = false;
        RightKey.enabled = false;
        LeftKey.enabled = false;
        SaveItem.enabled = false;
        Cloud.enabled = false;
        Nail.enabled = false;
        SawDust.enabled = false;
        HPItemUI.enabled = false;
        HintBlackBoard.enabled = false;
        HintMaskBoard.enabled = false;
        HintBoard.enabled = false;
        HintSkipLabel.enabled = false;
        HintArrow.enabled = false;
        Compass.enabled = false;
        DeskKey.enabled = false;
        LockDesk.enabled = false;
    }

    public void HelpInfoIcon(int HelpCount) // 안내 표시
    {
        switch (HelpCount)
        {
            case 1: // 움직임, 점프
                RightKey.enabled = true;
                LeftKey.enabled = true;
                SpaceKey.enabled = true;
                break;
            case 2: // 오르기
                SpaceKey2.enabled = true;
                //SpaceKey.transform.localPosition = new Vector3(-133, -383, 0);
                break;
            case 3: // 세이브 포인트 키
                //SpaceKey.enabled = true;
                //SpaceKey.transform.localPosition += Vector3.right * 235.0f;
                FKey.enabled = true;
                break;
            case 4: // 돌아가기 키
                WKey.enabled = true;
                break;
            case 5: // 세이브 포인트 아이템
                //iTween.Stop(HintArrow.gameObject);
                HintArrow.transform.localPosition = new Vector3(-110, -241, 0);
                iTween.MoveTo(HintArrow.gameObject, iTween.Hash("x", -80, "time", 0.9f, "islocal", true, "looptype", "loop", "easeType", "linear"));
                SaveItem.enabled = true;
                break;
            case 6: // 못 아이템
                HintArrow.transform.localPosition = new Vector3(-101, -241, 0);
                iTween.MoveTo(HintArrow.gameObject, iTween.Hash("x", -71, "time", 0.9f, "islocal", true, "looptype", "loop", "easeType", "linear"));
                Nail.enabled = true;
                SawDust.enabled = true;
                break;
            case 7: // 체력 아이템
                HintArrow.transform.localPosition = new Vector3(-110, -241, 0);
                iTween.MoveTo(HintArrow.gameObject, iTween.Hash("x", -80, "time", 0.9f, "islocal", true, "looptype", "loop", "easeType", "linear"));
                HPItemUI.enabled = true;
                break;
            case 8: // 구름, R 키, 로프액션 설명
                HintArrow.transform.localPosition = new Vector3(55, 224, 0);
                iTween.MoveTo(HintArrow.gameObject, iTween.Hash("x", 85, "time", 0.9f, "islocal", true, "looptype", "loop", "easeType", "linear"));
                Cloud.enabled = true;
                RKey.enabled = true;
                break;
            case 9:
                Compass.enabled = true;
                break;
            case 10:
                DeskKey.enabled = true;
                break;
            case 11:
                HelpTextLabel.transform.position = Vector3.zero;
                LockDesk.enabled = true;
                break;
        }
    }

    public IEnumerator EndCredit() // 엔딩 크레딧 함수
    {
        //엔딩 크레딧이 일정 높이 올라갈 때까지
        while (EndingCredit.transform.position.y < 1500.0f)
        {
            EndingCredit.transform.Translate((Vector2.up * CreditSpeed * Time.deltaTime));
            //EndingCredit.transform.localPosition = Vector3.up * Time.timeScale * CreditSpeed;
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(01, LoadSceneMode.Single); // 메인화면으로 이동
            }
            yield return 0;
        }

        SceneManager.LoadScene(01, LoadSceneMode.Single); // 메인화면으로 이동
        yield return 0;
    }
    /*
    public void StateUI()
    {
        switch(StateUICount)
        {
            case 0:
                HPBar.enabled = false;
                CheckBar.enabled = false;
                break;
            case 1:
                HPBar.enabled = true;
                CheckBar.enabled = true;
                break;
        }
    }
    */
}