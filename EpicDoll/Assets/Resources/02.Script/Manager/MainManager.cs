using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {
    public GameObject Cam;      // 메인 카메라

    public float speed;         // 카메라 전진 연출 속도
    public float stopZ;         // 카메라 연출 정지 위치

    public bool isStop;         // 정지 체크
    public bool isCtCheck;      // 페이드 효과 체크
    public bool isUIUseESC;     // UI ESC 체크

    public bool sfxMute;        // 효과음 Mute 체크
    public bool bgmMute;        // 배경음 Mute 체크
    
    // 화면 연출 용 컬러
    public UISprite BlackSprite;
    public UISprite BlackSprite2;
    public UISprite whiteSprite;

    // 팀 로고
    public UISprite EpicDollsLogo;

    // 불투명 박스
    public UISprite AlphaBlackBoard;

    // 메인 메뉴 UI
    public UISprite StartButton;
    public UISprite OptionButton;
    public UISprite StaffButton;
    public UISprite EndButton;

    // 메인 메뉴 UI BoxCollider
    public BoxCollider StartBox;
    public BoxCollider OptionBox;
    public BoxCollider StaffBox;
    public BoxCollider EndBox;

    // 해당 메뉴 창
    public GameObject OptionWindow;
    public UISprite StaffWindow;
    
    // 창 종료 이미지
    public GameObject XButton;

    // 사운드 메뉴 UI
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

    // 데이터 저장 슬롯 UI
    public GameObject PlayerDataSlot1;
    public GameObject PlayerDataSlot2;
    public GameObject PlayerDataSlot3;
    public UISprite PlayerDataSlot1Sprite;
    public UISprite PlayerDataSlot2Sprite;
    public UISprite PlayerDataSlot3Sprite;

    //각 슬롯 항목들
    public UILabel Slot1Num;
    public UILabel Slot2Num;
    public UILabel Slot3Num;

    public UILabel Play1Label;
    public UILabel Play2Label;
    public UILabel Play3Label;

    public UILabel PlayTime1;
    public UILabel PlayTime2;
    public UILabel PlayTime3;

    SoundOption soundOp;
    System.Collections.Generic.List<GameInfo> gameInfo = new System.Collections.Generic.List<GameInfo>();
    void Awake()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera");
        
        BlackSprite.SetScreenRect(-10, -70, Screen.width * 4, Screen.height * 4);
        BlackSprite2.SetScreenRect(-10, -70, Screen.width * 4, Screen.height * 4);
        whiteSprite.SetScreenRect(-10, -70, Screen.width * 4, Screen.height * 4);

        EpicDollsLogo.enabled = false;

        AlphaBlackBoard.enabled = false;

        StartButton.enabled = false;
        OptionButton.enabled = false;
        StaffButton.enabled = false;
        EndButton.enabled = false;

        StartBox.enabled = false;
        OptionBox.enabled = false;
        StaffBox.enabled = false;
        EndBox.enabled = false;

        OptionWindow.SetActive(false);
        StaffWindow.enabled = false;

        XButton.SetActive(false);

        PlayerDataSlot1.SetActive(false);
        PlayerDataSlot2.SetActive(false);
        PlayerDataSlot3.SetActive(false);
    }

    void Start()
    {
        StartCoroutine("MainFadeIn");

        if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.GAMEOPTIONXML))
        {
            soundOp = XMLParsing.Instance.XmlLoadOption();
        }
        else
        {
            soundOp.bgmValue = 1;
            soundOp.effValue = 1;
            soundOp.bgmMute = false;
            soundOp.effMute = false;
        }
    }
    
    void FixedUpdate()
    {
        // 카메라가 도착지에 도착시 isStop이 True
        if (Cam.transform.position.z > stopZ) { isStop = true; }

        // 도착지에 도착하지 않았을 경우
        if (!isStop) 
        {
            // 카메라를 도착지까지 일정한 속도로 이동
            Cam.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            
            // Enter를 누를 시 카메라 연출 최종 위치로 이동
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Cam.transform.position = Vector3.forward * stopZ;
            }
        }
        else
        {
            if(!isCtCheck)
            {
                // 메인 UI가 보여짐
                StartCoroutine("UIFadeIn");
                isCtCheck = true;
            }
            BlackSprite.enabled = false;
            whiteSprite.enabled = false;
        }

        // isUIUseESC가 True인 상태에서 ESC를 누르면
        if (Input.GetKeyDown(KeyCode.Escape) && isUIUseESC)
        {
            EpicDollsLogo.enabled = true;

            StartBox.enabled = true;
            OptionBox.enabled = true;
            StaffBox.enabled = true;
            EndBox.enabled = true;

            StartButton.enabled = true;
            OptionButton.enabled = true;
            StaffButton.enabled = true;
            EndButton.enabled = true;

            OptionWindow.SetActive(false);
            StaffWindow.enabled = false;
            XButton.SetActive(false);

            PlayerDataSlot1.SetActive(false);
            PlayerDataSlot2.SetActive(false);
            PlayerDataSlot3.SetActive(false);
        }
    }

    IEnumerator MainFadeIn() // 첫 시작 페이드 효과
    {
        for (float i = 1f; i >= 0; i -= 0.01f)
        {
            BlackSprite.alpha = i;
            yield return 0;
        }
        for (float i = 1f; i >= 0; i -= 0.01f)
        {
            whiteSprite.alpha = i;
            yield return 0;
        }
        yield return 0;
    }

    IEnumerator MainFadeOut() // 게임 데이터 슬롯 선택 이후 연출 페이드 효과
    {
        //데이터 슬롯창 닫음
        PlayerDataSlot1.SetActive(false);
        PlayerDataSlot2.SetActive(false);
        PlayerDataSlot3.SetActive(false);

        yield return new WaitForSeconds(2.5f);
        for (float i = 0f; i <= 1; i += 0.01f)
        {
            BlackSprite2.alpha = i;
            yield return 0;
        }
        BlackSprite2.GetComponent<UISprite>().alpha = 1;
        SceneManager.LoadScene(02, LoadSceneMode.Single);
    }

    IEnumerator UIFadeIn() // UI fade 효과
    {
        AlphaBlackBoard.enabled = true;
        EpicDollsLogo.enabled = true;
        StartButton.enabled = true;
        OptionButton.enabled = true;
        StaffButton.enabled = true;
        EndButton.enabled = true;

        for (float i = 0f; i <= 1; i += 0.01f)
        {
            AlphaBlackBoard.alpha = i * 0.5f;
            EpicDollsLogo.alpha = i;
            StartButton.alpha = i;
            OptionButton.alpha = i;
            StaffButton.alpha = i;
            EndButton.alpha = i;
            yield return 0;
        }

        StartBox.enabled = true;
        OptionBox.enabled = true;
        StaffBox.enabled = true;
        EndBox.enabled = true;

        isUIUseESC = true; // 첫 시작시 ESC 눌러서 메인 UI 바로 뜨는 버그 안생기게 하기
        yield return 0;
    }

    IEnumerator UIFadeOut() // UI가 사라지는 연출
    {
        XButton.SetActive(false);
        for (float i = 1f; i > 0; i -= 0.01f) // 알파 값을 1에서 0으로
        {
            PlayerDataSlot1Sprite.alpha = i;
            PlayerDataSlot2Sprite.alpha = i;
            PlayerDataSlot3Sprite.alpha = i;
            AlphaBlackBoard.alpha = i * 0.5f; 
            yield return 0;
        }
        
        PlayerDataSlot1.SetActive(false); // 화면 출력 OFF
        PlayerDataSlot2.SetActive(false);
        PlayerDataSlot3.SetActive(false);
        
        yield return 0;
    }

    IEnumerator StartCameraAct() // 데이터 슬롯 선택시 실행
    {
        // 가속도
        float accel = 0.05f; 
        
        // 카메라의 위치가 일정한 위치에 도달할 때까지 이동
        while (Cam.transform.position.z < 19.35f) 
        {
            Cam.transform.Translate(Vector3.forward * speed * accel * Time.deltaTime);
            accel += 0.07f;
            yield return new WaitForSeconds(0.001f);
        }
        yield return 0;
    }

    public void OnOpeningUI()
    {
        if (UIButton.current.name.Equals("GameButton")) 
        {

            if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.PLAYINFOXML))
            {
                gameInfo = XMLParsing.Instance.XmlLoadGameDatas();

                //저장된 진행 정보를 화면에 출력
                float minute0 = gameInfo[0].playTimes / 60;
                float second0 = gameInfo[0].playTimes % 60;
                Slot1Num.text = "1"; //gameInfo[0].IDNumber.ToString();
                Play1Label.text = string.Format("{0:0.}", ((100 / 21) * gameInfo[0].tutoCount)) + "  %";
                PlayTime1.text = string.Format("{0:0.}", minute0) + "  분  " + string.Format("{0:0.}", second0) + "  초";

                float minute1 = gameInfo[1].playTimes / 60;
                float second1 = gameInfo[1].playTimes % 60;
                Slot2Num.text = "2"; //gameInfo[1].IDNumber.ToString();
                Play2Label.text = string.Format("{0:0.}", ((100 / 21) * gameInfo[1].tutoCount)) + "  %";
                PlayTime2.text = string.Format("{0:0.}", minute1) + "  분  " + string.Format("{0:0.}", second1) + "  초";

                float minute2 = gameInfo[2].playTimes / 60;
                float second2 = gameInfo[2].playTimes % 60;
                Slot3Num.text = "3";//gameInfo[2].IDNumber.ToString();
                Play3Label.text = string.Format("{0:0.}", ((100 / 21) * gameInfo[2].tutoCount)) + "  %";
                PlayTime3.text = string.Format("{0:0.}", minute2) + "  분  " + string.Format("{0:0.}", second2) + "  초";
            }
            else
            {
                
            }
            

            EpicDollsLogo.enabled = false;

            StartBox.enabled = false;
            OptionBox.enabled = false;
            StaffBox.enabled = false;
            EndBox.enabled = false;

            StartButton.enabled = false;
            OptionButton.enabled = false;
            StaffButton.enabled = false;
            EndButton.enabled = false;

            XButton.SetActive(true);

            PlayerDataSlot1.SetActive(true);
            PlayerDataSlot2.SetActive(true);
            PlayerDataSlot3.SetActive(true);
        }
        if (UIButton.current.name.Equals("OptionButton")) // 사운드 옵션 버튼
        {
            EpicDollsLogo.enabled = false;

            XButton.SetActive(true);
            OptionWindow.SetActive(true);

            StartBox.enabled = false;
            OptionBox.enabled = false;
            StaffBox.enabled = false;
            EndBox.enabled = false;

            StartButton.enabled = false;
            OptionButton.enabled = false;
            StaffButton.enabled = false;
            EndButton.enabled = false;

            SFXSoundSlider.value = soundOp.effValue;
            BackgroundSoundSlider.value = soundOp.bgmValue;
            sfxMute = soundOp.effMute;
            bgmMute = soundOp.bgmMute;

            if (soundOp.effMute)
            {
                SFXSoundButton.normalSprite = "main_sound mute"; 
                SFXSoundButtonSprite.spriteName = "main_sound mute";
                SFXBoxButton.normalSprite = "main_box check";
                SFXBoxButtonSprite.spriteName = "main_box check";
            }
            else
            {
                SFXSoundButton.normalSprite = "main_sound";
                SFXSoundButtonSprite.spriteName = "main_sound";
                SFXBoxButton.normalSprite = "main_box";
                SFXBoxButtonSprite.spriteName = "main_box";
            }
            if (soundOp.bgmMute)
            {
                BackgroundBoxButton.normalSprite = "main_box check";
                BackgroundBoxButtonSprite.spriteName = "main_box check";
                BackgroundSoundButton.normalSprite = "main_sound mute";
                BackgroundSoundButtonSprite.spriteName = "main_sound mute";
            }
            else
            {
                BackgroundBoxButton.normalSprite = "main_box";
                BackgroundBoxButtonSprite.spriteName = "main_box";
                BackgroundSoundButton.normalSprite = "main_sound";
                BackgroundSoundButtonSprite.spriteName = "main_sound";
            }
            Debug.Log("게임 옵션 로드 끝");
        }
        if (UIButton.current.name.Equals("StaffButton")) // 스태프 버튼
        {
            EpicDollsLogo.enabled = false;

            XButton.SetActive(true);
            StaffWindow.enabled = true;

            StartBox.enabled = false;
            OptionBox.enabled = false;
            StaffBox.enabled = false;

            EndBox.enabled = false;
            StartButton.enabled = false;
            OptionButton.enabled = false;
            StaffButton.enabled = false;
            EndButton.enabled = false;
        }
        if (UIButton.current.name.Equals("EndButton")) // 종료 버튼
        {
            Application.Quit();
        }
        if (UIButton.current.name.Equals("XButton")) // 창 종료 버튼
        {
            EpicDollsLogo.enabled = true;

            StaffWindow.enabled = false;
            XButton.SetActive(false);

            StartBox.enabled = true;
            OptionBox.enabled = true;
            StaffBox.enabled = true;
            EndBox.enabled = true;

            StartButton.enabled = true;
            OptionButton.enabled = true;
            StaffButton.enabled = true;
            EndButton.enabled = true;

            if (OptionWindow.activeInHierarchy.Equals(true))
            {
                XMLParsing.Instance.XmlSaveOption(soundOp.effValue, soundOp.bgmValue, soundOp.effMute, soundOp.bgmMute);
                OptionWindow.SetActive(false);
            }
        }

    }
    public void OnSoundSliderUI() // 사운드 슬라이더 관련
    {
        if (UISlider.current.name.Equals("SFXSlider"))
        {
           soundOp.effValue = UISlider.current.value;
        }
        if (UISlider.current.name.Equals("BackgroundSlider"))
        {
           soundOp.bgmValue = UISlider.current.value;
        }
    }

    public void OnSoundButtonUI() // 사운드 버튼 관련
    {
        if (UIButton.current.name.Equals("SFXSoundButton")) // 누른 버튼이 "SFXSoundButton" 일 경우
        {
            if (UIButton.current.normalSprite.Equals("main_sound")) // 버튼의 이미지가 "main_sound"일 경우
            {
                soundOp.effMute = true;
                SFXSoundButton.normalSprite = "main_sound mute"; // UI Button NormalSprite 변경
                SFXSoundButtonSprite.spriteName = "main_sound mute";  // UI Button Background 변경
                SFXBoxButton.normalSprite = "main_box check";
                SFXBoxButtonSprite.spriteName = "main_box check";
            }
            else
            {
                soundOp.effMute = false;
                SFXSoundButton.normalSprite = "main_sound";
                SFXSoundButtonSprite.spriteName = "main_sound";
                SFXBoxButton.normalSprite = "main_box";
                SFXBoxButtonSprite.spriteName = "main_box";
            }
        }
        if (UIButton.current.name.Equals("SFXBoxButton"))  // 효과음 박스 버튼
        {
            if (UIButton.current.normalSprite.Equals("main_box"))
            {
                soundOp.effMute = true;
                SFXSoundButton.normalSprite = "main_sound mute";
                SFXSoundButtonSprite.spriteName = "main_sound mute";
                SFXBoxButton.normalSprite = "main_box check";
                SFXBoxButtonSprite.spriteName = "main_box check";
            }
            else
            {
                soundOp.effMute = false;
                SFXSoundButton.normalSprite = "main_sound";
                SFXSoundButtonSprite.spriteName = "main_sound";
                SFXBoxButton.normalSprite = "main_box";
                SFXBoxButtonSprite.spriteName = "main_box";
            }
        }

        if (UIButton.current.name.Equals("BackgroundSoundButton")) // 배경음 사운드 버튼
        {
            if (UIButton.current.normalSprite.Equals("main_sound"))
            {
                BackgroundBoxButton.normalSprite = "main_box check";
                BackgroundBoxButtonSprite.spriteName = "main_box check";
                BackgroundSoundButton.normalSprite = "main_sound mute";
                BackgroundSoundButtonSprite.spriteName = "main_sound mute";
                soundOp.bgmMute = true;
            }
            else
            {
                BackgroundBoxButton.normalSprite = "main_box";
                BackgroundBoxButtonSprite.spriteName = "main_box";
                BackgroundSoundButton.normalSprite = "main_sound";
                BackgroundSoundButtonSprite.spriteName = "main_sound";
                soundOp.bgmMute = false;
            }
        }
        if (UIButton.current.name.Equals("BackgroundBoxButton")) // 배경음 박스 버튼
        {
            if (UIButton.current.normalSprite.Equals("main_box"))
            {
                BackgroundBoxButton.normalSprite = "main_box check";
                BackgroundBoxButtonSprite.spriteName = "main_box check";
                BackgroundSoundButton.normalSprite = "main_sound mute";
                BackgroundSoundButtonSprite.spriteName = "main_sound mute";
                soundOp.bgmMute = true;
            }
            else
            {
                BackgroundBoxButton.normalSprite = "main_box";
                BackgroundBoxButtonSprite.spriteName = "main_box";
                BackgroundSoundButton.normalSprite = "main_sound";
                BackgroundSoundButtonSprite.spriteName = "main_sound";
                soundOp.bgmMute = false;
            }
        }
    }
    
    public void OnDataSlot1() // 데이터 슬롯 1
    {
        SlotDataNumberSave.Instance.slotNum = 0;
        StartCoroutine(UIFadeOut()); //UI가 흐려지며 사라지는 연출
        StartCoroutine(StartCameraAct()); //카메라가 문앞으로 이동하는 연출
        StartCoroutine(MainFadeOut());
    }
    public void OnDataSlot2() // 데이터 슬롯 2
    {
        SlotDataNumberSave.Instance.slotNum = 1;
        StartCoroutine(UIFadeOut()); //UI가 흐려지며 사라지는 연출
        StartCoroutine(StartCameraAct()); //카메라가 문앞으로 이동하는 연출
        StartCoroutine(MainFadeOut());
    }
    public void OnDataSlot3() // 데이터 슬롯 3
    {
        SlotDataNumberSave.Instance.slotNum = 2;
        StartCoroutine(UIFadeOut()); //UI가 흐려지며 사라지는 연출
        StartCoroutine(StartCameraAct()); //카메라가 문앞으로 이동하는 연출
        StartCoroutine(MainFadeOut());
    }
   
}