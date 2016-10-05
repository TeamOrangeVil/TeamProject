using UnityEngine;
using System.Collections;
using Spine.Unity;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager gInstance = null;

    public enum TutorialStep { }

    public int tutorialCnt = 100; // 튜토리얼 순서
    public float WalkTime = 0.01f; // NPC 걷기
    public int textCnt = 0; // 대사 순서

    public bool isTutorial = false; // 튜토리얼 작동하는지
    public bool tutoDataSet = false; //튜토리얼 데이터가 로드 완료되었는지 확인
    public bool isDist = false; // 캐릭터와 안내인형 거리 확인 제한
    public bool isSkip = false; // 대사 스킵
    public bool isSpider = false; // 빠르게 지나가는 거미 확인
    public int skinNum = 0;//옷
    int bgmNum = 1;//배경음 선택

    public HelperController HelperScript; // 안내인형 스크립트
    public FollowCamera CameraScript; // 카메라 

    public Transform PlayerTr; // 플레이어 위치
    public Transform HelperDollTr; // 안내 인형 위치

    public Vector3 TutorialDist; // 안내 인형과 캐릭터 거리

    public GameObject CutToon1; // 컷신1
    public GameObject CutToon2; // 컷신2

    public SkeletonAnimation Spider; // 플레이어를 공격하는 거미
    public SkeletonAnimation ShadowSpider; // 플레이어를 지나치는 거미

    public GameObject BeforeObj; // 댄서 인형과 만나기 전 배치되어 있는 오브젝트
    public GameObject AfterObj; // 댄서 인형을 만난 후 배치되어 있는 오브젝트

    public SkeletonAnimation DeskBox; // 갇힌 인형이 들어있는 서랍장
    public SkeletonAnimation SaveDoll; // 서랍장에 갇힌 인형
    public SkeletonAnimation Knight; // 축음기 바늘을 들고 있는 기사
    public SkeletonAnimation Dancer; // 댄서 인형 애니

    public GameObject Key;
    public GameObject Compas;
    public GameObject SaveFloor; // 서랍장에 갇힌 인형의 발판

    public GameObject TempDancer; // 댄서 인형
    public FieldObject Gramophone; // 축음기

    public GameObject BossSpider;

    public float gameProgress = 0;
    public float ProgressCount = 0;
    public float maxProgress = 21;

    public static TutorialManager Instance
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

        HelperScript = GameObject.FindGameObjectWithTag("HELPER").GetComponent<HelperController>();
        CameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();

        PlayerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        HelperDollTr = GameObject.FindGameObjectWithTag("HELPER").GetComponent<Transform>();

        Spider = GameObject.Find("Spider").GetComponent<SkeletonAnimation>();
        ShadowSpider = GameObject.Find("ShadowSpider").GetComponent<SkeletonAnimation>();
        DeskBox = GameObject.Find("Men_Temp").GetComponent<SkeletonAnimation>();
        SaveDoll = GameObject.Find("SaveDoll").GetComponent<SkeletonAnimation>();
        Knight = GameObject.Find("Knight").GetComponent<SkeletonAnimation>();

        Key = GameObject.Find("Key");
        Compas = GameObject.Find("Compass");
        Dancer = GameObject.Find("Dancer").GetComponent<SkeletonAnimation>();
        //TempDancer = GameObject.Find("Dancer").GetComponent<GameObject>();
        Gramophone = GameObject.Find("gramophone").GetComponent<FieldObject>();

        AfterObj.SetActive(false);
        //SoundEffectManager.Instance.BGMStart(1);
        BossSpider.SetActive(false);
        //isTutorial = true;//이전 게임 데이터를 받기 전까지 튜토리얼 대기
    }

    void Update()
    {
        if (!isTutorial && tutoDataSet)
        {
            TutorialCheck();
        }
        TutorialDist = HelperDollTr.transform.position - PlayerTr.transform.position; // NPC와 캐릭터 거리

        if (Input.GetKeyDown(KeyCode.Return))
        {
            isSkip = true;
        }
        if (Input.GetKeyDown(KeyCode.Q)) // QA 분들용 특정 구간 이동
        {
            CharacterController2D.Instance.Player.transform.position = new Vector3(190, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.F1)) // 체력 회복
        {
            GameManager.Instance.HpPlusGet();
        }
        if (Input.GetKeyDown(KeyCode.F2)) // 체크 포인트 푀복
        {
            GameManager.Instance.CheckPlusGet();
        }
        if (Input.GetKeyDown(KeyCode.Space)) // 대화 생략
        {
            UIManager.Instance.WriteEffect.Finish();
        }
    }

    void TutorialCheck() // 튜토리얼 체크
    {
        if (tutorialCnt.Equals(100))
        {
            StartCoroutine(Tutorial100());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(0))
        {
            if (CutToon1.activeInHierarchy.Equals(true) && isSkip)
            {
                CutToon1.SetActive(false);
                SoundEffectManager.Instance.BGMStart(2);
                UIManager.Instance.CutToonSkipLabel.enabled = false;
                StopAllCoroutines();
                isSkip = false;
                tutorialCnt = 1;
            }
            if (CutToon2.activeInHierarchy.Equals(true) && isSkip)
            {
                CutToon2.SetActive(false);
                SoundEffectManager.Instance.BGMStart(5);
                UIManager.Instance.CutToonSkipLabel.enabled = false;
                StopAllCoroutines();
                isSkip = false;
                tutorialCnt = 23;
            }
        }
        if (tutorialCnt.Equals(1))
        {
            StartCoroutine(Tutorial1());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(2) && TutorialDist.x < 5.0f)
        {
            StartCoroutine(Tutorial2());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(3))
        {
            if (!isSpider && CharacterController2D.Instance.transform.position.x > 26.23f) // 캐릭터의 위치가 책 위에 있고 isSpider가 false면
            {
                StartCoroutine(ShadowSpiderAct());
                SoundEffectManager.Instance.SoundDelay("SpiderRun", 0);
                isSpider = true;
            }
            if (TutorialDist.x < 2.5f)
            {
                StartCoroutine(Tutorial3());
                tutorialCnt = 0;
            }

        }
        if (tutorialCnt.Equals(4) && TutorialDist.x < 2.5f)
        {
            StartCoroutine(Tutorial4());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(5))
        {
            if (!CharacterController2D.Instance.isInfo)
            {
                UIManager.Instance.HelpPrint(3);
                UIManager.Instance.HelpTextLabel.transform.localPosition = Vector3.zero;
                UIManager.Instance.FKey.transform.localPosition = new Vector3(-171.4f, -93f, 0);
                UIManager.Instance.FKey.enabled = true;
                CharacterController2D.Instance.isAct = true;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.HelpUIOFF();
                UIManager.Instance.HelpTextLabel.transform.localPosition = new Vector3(0, -290, 0);
                CharacterController2D.Instance.isAct = true;
                CharacterController2D.Instance.isInfo = false;
                StartCoroutine(Tutorial5());
                tutorialCnt = 0;
            }

        }
        if (tutorialCnt.Equals(6) && CharacterController2D.Instance.transform.position.x > 63.3f)
        {
            StartCoroutine(Tutorial6());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(7))
        {
            if (!CharacterController2D.Instance.isInfo)
            {
                UIManager.Instance.HelpPrint(4);
                UIManager.Instance.HelpTextLabel.transform.localPosition = Vector3.zero;
                UIManager.Instance.WKey.transform.localPosition = new Vector3(-245f, -93f, 0);
                UIManager.Instance.WKey.enabled = true;
                CharacterController2D.Instance.isAct = true;
                if (Input.GetKeyDown(KeyCode.W))
                {
                    GameManager.Instance.RewindStart();
                    UIManager.Instance.HelpUIOFF();
                    CharacterController2D.Instance.isInfo = false;
                    StartCoroutine(Tutorial7());
                    tutorialCnt = 0;
                }
            }
            if (CharacterController2D.Instance.isInfo && Input.GetKeyDown(KeyCode.W))
            {
                GameManager.Instance.RewindStart();
                UIManager.Instance.HelpUIOFF();
                CharacterController2D.Instance.isInfo = false;
                UIManager.Instance.HelpTextLabel.transform.localPosition = new Vector3(0, -290, 0);
                StartCoroutine(Tutorial7());
                tutorialCnt = 0;
            }
        }
        if (tutorialCnt.Equals(8) && CharacterController2D.Instance.transform.position.x > 80.31f) // 체크 포인트 아이템
        {
            StartCoroutine(Tutorial8());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(9) && CharacterController2D.Instance.transform.position.x > 90.45f) // 위험한 오브젝트 경고
        {
            StartCoroutine(Tutorial9());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(10) && CharacterController2D.Instance.transform.position.x > 103.55f) // 체력 아이템
        {
            StartCoroutine(Tutorial10());
            tutorialCnt = 0;
        }



        if (tutorialCnt.Equals(11) && CharacterController2D.Instance.transform.position.x > 129f) // 로프 액션
        {
            StartCoroutine(Tutorial11());
            tutorialCnt = 0;
        }
        /*if (tutorialCnt.Equals(12) && CharacterController2D.Instance.isAct) // 로프액션으로 캐릭터 움직임 제한 되면
        {
            Debug.Log("튜토12 작동");
            StartCoroutine(Tutorial12());
            tutorialCnt = 0;
        }*/
        if (tutorialCnt.Equals(13) && CharacterController2D.Instance.transform.position.x > 238.35f)
        {
            StartCoroutine(Tutorial13());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(14))
        {
            if (GameManager.Instance.mansWall && !GameManager.Instance.meetMan)
            {
                StartCoroutine(FindMan());
                tutorialCnt = 0;
            }

            if (GameManager.Instance.getKey) // 열쇠를 먹으면 실행
            {
                StartCoroutine(Tutorial14());
                tutorialCnt = 0;
            }

        }
        if (tutorialCnt.Equals(15)) // 흔들리는 서랍장 밟으면 실행
        {
            if (GameManager.Instance.mansWall)
            {
                StartCoroutine(FindMan());
                tutorialCnt = 0;
            }
            if (GameManager.Instance.meetMan)
            {
                StartCoroutine(Tutorial15());
                tutorialCnt = 0;
            }
        }
        if (tutorialCnt.Equals(16) && GameManager.Instance.meetDancer) // 댄서 만나면 실행 기사한테 아이템 가져오라는 미션
        {
            StartCoroutine(Tutorial16());
            tutorialCnt = 0;
        }

        if (tutorialCnt.Equals(17)) // 카운트가 17인 상태에서 
        {
            if (!GameManager.Instance.getCompas && GameManager.Instance.meetDancer) //기사 퀘스트까지 안깨고(false조건) 댄서에게 가면  // false or True // 플레이어 거리
            {
                StartCoroutine(FailTutorialDancer()); // 빨리 구해와!
                tutorialCnt = 0;
            }
            if (!GameManager.Instance.getCompas && GameManager.Instance.meetKnight) // 캠퍼스 무시하고 기사 만나러 가면 // false or True // 플레이어 거리
            {
                StartCoroutine(FailTutorialKnight()); // 빨리 구해와!
                tutorialCnt = 0;
            }
            if (GameManager.Instance.getCompas) // 캠퍼스 먹으면 작동
            {
                StartCoroutine(Tutorial17());
                tutorialCnt = 0;
            }
        }

        if (tutorialCnt.Equals(18)) // 카운트가 18인 상태에서 
        {
            if (GameManager.Instance.getCompas && GameManager.Instance.meetDancer) // 캠퍼스 얻고 댄서 만나러 가면 // false or True // 플레이어 거리
            {
                StartCoroutine(FailTutorialDancer()); // 빨리 구해와!
                tutorialCnt = 0;
            }
            if (GameManager.Instance.getCompas && GameManager.Instance.meetKnight) // 캠퍼스를 얻고 기사를 만나면
            {
                StartCoroutine(Tutorial18());
                tutorialCnt = 0;
            }
        }

        if (tutorialCnt.Equals(19) && GameManager.Instance.meetDancer) // 댄서에게 줄 아이템 있으면 조건 식
        {
            StartCoroutine(Tutorial19());
            tutorialCnt = 0;
        }

        if (tutorialCnt.Equals(20) && CharacterController2D.Instance.transform.position.x > 414f) // 죽어가는 서랍장 인형을 만나는 조건 식 포함
        {
            StartCoroutine(Tutorial20());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(21) && CharacterController2D.Instance.transform.position.x > 543.5f) // 안내인형과 거미를 발견할 적당한 거리 추가
        {
            StartCoroutine(Tutorial21());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(22) && CharacterController2D.Instance.transform.position.x > 547f) // 안내 인형을 구해주는 컷 신
        {
            StartCoroutine(Tutorial22());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(23) ) //
        {
            StartCoroutine(Tutorial23());
            tutorialCnt = 0;
        }
    }
    public void TutoDataSave()
    {
        Debug.Log("튜토 데이터 세입");
        GameManager.Instance.gameInfo.playerTr = CharacterController2D.Instance.tr.position;
        GameManager.Instance.gameInfo.tutoCount = tutorialCnt;
        GameManager.Instance.gameInfo.InfoDollTr = HelperDollTr.position;
        GameManager.Instance.gameInfo.spiderTr = Spider.transform.position;
        GameManager.Instance.gameInfo.shadowSpiderTr = ShadowSpider.transform.position;
        GameManager.Instance.gameInfo.spiderAct = Spider.enabled;
        GameManager.Instance.gameInfo.shadowSpiderAct = ShadowSpider.GetComponent<MeshRenderer>().enabled;
        GameManager.Instance.gameInfo.dancerTr = TempDancer.transform.position;
        GameManager.Instance.gameInfo.gramophoneAct = Gramophone.enabled;
        GameManager.Instance.gameInfo.skinNum = skinNum;
        bgmNum = SoundEffectManager.Instance.bgmNum;
        GameManager.Instance.gameInfo.bgmNum = bgmNum;
    }
    public void TutoDataLoad(int tutoCount, int skinnum, int bgmnum, Vector3 infoDollTr, Vector3 spiderTr, Vector3 shadowSpiderTr,
        bool spiderAct, bool shadowSpiderAct, Vector3 dancerTr, bool gramophoneAct)//튜토리얼 데이터를 게임 매니져에서 받아오는 함수
    {
        if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.PLAYINFOXML))
        {
            //받아온 정보들을 바탕으로 튜토리얼 맞춤
            tutorialCnt = tutoCount;
            skinNum = skinnum;
            AniSpriteChange.Instance.SpriteChange(skinNum);
            SoundEffectManager.Instance.BGMStart(bgmnum);
            HelperDollTr.position = infoDollTr;
            Spider.transform.position = spiderTr;
            ShadowSpider.transform.position = shadowSpiderTr;
            Spider.enabled = spiderAct;
            ShadowSpider.GetComponent<MeshRenderer>().enabled = shadowSpiderAct;
            if (GameManager.Instance.mansWall)
            {
                DeskBox.state.SetAnimation(0, "OPEN", false);
            }
            if (GameManager.Instance.returnToWay)
            {
                BeforeObj.SetActive(false);
                AfterObj.SetActive(true);
            }
            TempDancer.transform.position = dancerTr;
            if (gramophoneAct) { Gramophone.EffectOn(); }
            if (GameManager.Instance.getKey) { Key.SetActive(false); }
            if (GameManager.Instance.getCompas) { Compas.SetActive(false); }
            
        }
        else
        {
            isTutorial = false;
        }
    }
    IEnumerator Tutorial100()
    {
        UIManager.Instance.UIOff();
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HPBar.enabled = false;
        UIManager.Instance.CheckBar.enabled = false;
        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        UIManager.Instance.CutToonSkipLabel.enabled = true;
        yield return new WaitForSeconds(10.0f);

        CutToon1.SetActive(false); // 컷 신 Off
        SoundEffectManager.Instance.BGMStart(2);
        UIManager.Instance.CutToonSkipLabel.enabled = false;
        tutorialCnt = 1;
        yield return 0;
    }

    /// <summary> 
    /// 튜토리얼 1
    /// 플레이어 캐릭터가 단상에서 내려온다
    /// 안내 인형이 캐릭터를 향해 걸어온다
    /// 플레이어 캐릭터와 안내 인형이 대화를 한다
    /// 안내 인형이 걸어가서 책 위로 올라간다
    /// 플레이어 캐릭터를 바라보며 말한다
    /// </summary>
    /// <returns></returns>
    IEnumerator Tutorial1()
    {
        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        CameraScript.CameraState = FollowCamera.State.PLAYER;

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        yield return new WaitForSeconds(1.0f);
        CharacterController2D.Instance.Player.skeleton.FlipX = true; // 플레이어는 오른쪽을 바라본다.

        yield return new WaitForSeconds(1.0f);
        HelperScript.Helper.skeleton.FlipX = false; // 헬퍼는 왼쪽을 바라본다.

        CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
        HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
        while (HelperDollTr.transform.position.x > 4.0f)// 헬퍼와 플레이어는 서로를 향해 걸어간다.
        {
                HelperDollTr.transform.Translate(Vector2.left * 0.08f * Time.timeScale);
                CharacterController2D.Instance.transform.Translate((Vector2.right * 0.08f * Time.timeScale));
            yield return 0;
        }

        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있는다.
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
        yield return new WaitForSeconds(0.5f);

        isSkip = false;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(25))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();

        HelperScript.Helper.Skeleton.FlipX = true;
        CameraScript.CameraState = FollowCamera.State.HELPER; // 카메라가 헬퍼를 비춘다.

        HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);

        while (HelperDollTr.transform.position.x < 17.0f) // 헬퍼는 책 한권 위로 올라간다.
        {
            if (HelperDollTr.transform.position.x > 8f && 9.5f > HelperDollTr.transform.position.x)
            {
                HelperScript.SetAnimation("GUIDEJUMP", false, 1.0f);
                HelperScript.isJump = true;
                HelperScript.isFloor = true;
            }
            if (HelperDollTr.transform.position.x > 9.5f && 11.5f > HelperDollTr.transform.position.x)
            {
                HelperScript.isJump = false;
                HelperScript.isFloor = false;
            }
            if (HelperDollTr.transform.position.x > 11.5f)
            {
                HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
            }
            HelperDollTr.transform.Translate(Vector2.right * 0.08f * Time.timeScale);
            yield return 0;
        }

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
        HelperScript.Helper.skeleton.FlipX = false; // 헬퍼는 왼쪽을 바라본다.

        isSkip = false;
        textCnt = 25;

        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(26))
            {
                break;
            }
        }

        UIManager.Instance.UIOff(); // UI OFF
        CameraScript.CameraState = FollowCamera.State.PLAYER; // 카메라가 플레이어를 비춘다.
        yield return new WaitForSeconds(0.1f);
        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        UIManager.Instance.HintPrint(1, 0); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(1); // 해당하는 아이콘 이미지

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;

        tutorialCnt = 2;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
        yield return 0;
    }

    /// <summary>
    /// 튜토리얼2
    /// 안내인형은 한권의 책에서 내려와 책이 쌓여져 있는 곳으로 올라간다
    /// 안내인형은 플레이어 캐릭터를 바라보며 말한다
    /// </summary>
    /// <returns></returns>
    IEnumerator Tutorial2()
    {
        UIManager.Instance.HelpUIOFF();

        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있음
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.0f);
        CameraScript.CameraState = FollowCamera.State.HELPER;
        HelperScript.Helper.skeleton.FlipX = true;

        HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
        while (HelperDollTr.transform.position.x < 25.3f) // 헬퍼는 높은책 한권 위로 올라간다.
        {
            if (HelperDollTr.transform.position.x > 17f && 23.6f > HelperDollTr.transform.position.x)
            {
                HelperDollTr.transform.Translate(Vector2.right * 0.08f);
            }
            if (HelperDollTr.transform.position.x > 23.5f && 25f > HelperDollTr.transform.position.x)
            {
                HelperScript.SetAnimation("GUIDEJUMP", false, 1.0f);
                HelperScript.isJump = true;
                HelperScript.isFloor = true;
                HelperDollTr.transform.Translate(Vector2.right * 0.08f * Time.timeScale);
            }
            yield return 0;
        }

        HelperScript.SetAnimation("GUIDEHANG", true, 1.0f);
        yield return new WaitForSeconds(1.0f);
        HelperScript.StartCoroutine("CLIMBING");
        yield return new WaitForSeconds(2.0f);
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);

        while (HelperDollTr.transform.position.x < 31.5f) // 헬퍼는 책 위에서 일정거리를 걸어간다.
        {
            HelperDollTr.transform.Translate(Vector2.right * 0.08f * Time.timeScale);
            yield return 0;
        }

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
        HelperScript.Helper.skeleton.FlipX = false; // 헬퍼가 왼쪽으로 돌아본다.

        yield return new WaitForSeconds(0.1f);

        isSkip = false;
        textCnt = 26;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount - 0.5f);
            textCnt += 1;
            if (textCnt.Equals(27))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        CameraScript.CameraState = FollowCamera.State.PLAYER;
        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        UIManager.Instance.HintPrint(2, 0); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(2); // 해당하는 아이콘 이미지

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        tutorialCnt = 3;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
        yield return 0;


    }

    // 책 더미를 밟은 상황
    IEnumerator Tutorial3()
    {
        CharacterController2D.Instance.isAct = true;
        UIManager.Instance.HelpUIOFF();
        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f);
        yield return new WaitForSeconds(1.0f);
        CameraScript.CameraState = FollowCamera.State.HELPER;
        HelperScript.Helper.skeleton.FlipX = true; // 헬퍼가 오른쪽을 바라봄

        HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
        while (HelperDollTr.transform.position.x < 56.5f) // 헬퍼는 책 위에서 일정거리를 걸어간다.
        {
            if (HelperDollTr.transform.position.x < 38.5f)
            {

                HelperDollTr.transform.Translate(Vector2.right * 0.1f * Time.timeScale);
            }
            if (HelperDollTr.transform.position.x > 38.5f && HelperDollTr.transform.position.x < 45.5f)
            {
                HelperScript.SetAnimation("GUIDECRAWL", true, 1.0f);
                HelperDollTr.transform.Translate(Vector2.right * 0.05f * Time.timeScale);
            }
            if (HelperDollTr.transform.position.x > 45.5f && HelperDollTr.transform.position.x < 56.5f)
            {
                HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
                HelperDollTr.transform.Translate(Vector2.right * 0.1f * Time.timeScale);
            }
            yield return 0;
        }

        HelperScript.Helper.skeleton.FlipX = false;
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
        yield return new WaitForSeconds(1.0f);

        isSkip = false;
        textCnt = 27;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(28))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        CameraScript.CameraState = FollowCamera.State.PLAYER;
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 4;
        ProgressCount += 1;
        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
        yield return 0;
    }

    // 쓰러질 것 같은 더미 옆에서 있는 상태
    IEnumerator Tutorial4()
    {
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있음
        yield return new WaitForSeconds(1.0f);

        isSkip = false;
        textCnt = 28;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(30))
            {
                break;
            }
        }

        HelperController.Instance.Helper.skeleton.SetToSetupPose();
        UIManager.Instance.UIOff();
        yield return new WaitForSeconds(0.1f);
        HelperScript.SetAnimation("GUIDETHEME", false, 1.0f);
        SoundEffectManager.Instance.SoundDelay("Clothes", 0);
        float aniTime = HelperScript.Helper.state.GetCurrent(0).EndTime; // 테마 전달 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);
        HelperScript.SetAnimation("GUIDEGET", false, 1.0f);
        aniTime = HelperScript.Helper.state.GetCurrent(0).EndTime;
        yield return new WaitForSeconds(aniTime + 1.0f);
        HelperScript.Helper.skeleton.SetAttachment("guide/clothe", null);


        isSkip = false;
        textCnt = 30;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(31))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        HelperController.Instance.Helper.skeleton.SetToSetupPose();
        yield return new WaitForSeconds(0.3f);
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        AniSpriteChange.Instance.SpriteChange(1);
        skinNum = 1;
        CharacterController2D.Instance.isTutorialNeedle = true; // 이제부터 바늘을 들고다님
        yield return new WaitForSeconds(2.0f);

        isSkip = false;
        textCnt = 31;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(43))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        UIManager.Instance.HPBar.enabled = true;
        UIManager.Instance.CheckBar.enabled = true;

        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        UIManager.Instance.HintPrint(3, 0); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(3); // 해당하는 아이콘 이미지

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        yield return new WaitForSeconds(1.5f);

        CharacterController2D.Instance.isSkill = true;
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        ProgressCount += 1;
        tutorialCnt = 5;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
    }

    // 처음 체크포인트 행동한 상태
    IEnumerator Tutorial5()
    {
        CharacterController2D.Instance.isSkill = false;
        CharacterController2D.Instance.SetAnimation("save 3", false, 1.0f);
        GameManager.Instance.CheckStart(CharacterController2D.Instance.tr.position);
        float aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime;
        yield return new WaitForSeconds(aniTime + 1.0f);
        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f);
        UIManager.Instance.HelpUIOFF();
        isSkip = false;
        textCnt = 43;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(49))
            {
                break;
            }
        }
        UIManager.Instance.UIOff();
        CameraScript.CameraState = FollowCamera.State.SPIDER;
        SoundEffectManager.Instance.SoundDelay("SpiderAtk", 0);
        while (Spider.transform.position.x > 60.0f)
        {
            Spider.state.SetAnimation(0, "WALK", true);
            Spider.transform.Translate((Vector2.left * 0.18f * Time.timeScale));
            yield return 0;
        }
        CameraScript.CameraState = FollowCamera.State.SPIDERFOOT;

        Spider.state.SetAnimation(0, "ATTACK", false);
        //yield return new WaitForSeconds(0.8f);
        yield return new WaitForSeconds(0.3f);
        //UIManager.Instance.HPBar.fillAmount -= 1f;

        //CharacterController2D.Instance.SetAnimation("SYNCOPE", false, 1.0f);
        //CharacterController2D.Instance.rb.AddForce((Vector2.left * 300f) + (Vector2.up * 300f));
        //yield return new WaitForSeconds(2.0f);

        UIManager.Instance.UIOff();
        UIManager.Instance.HelpUIOFF();

        UIManager.Instance.StartCoroutine("FadeOut");//페이드 아웃

        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.FadeBoard.GetComponent<UISprite>().alpha = 1;
        CharacterController2D.Instance.SetAnimation("SYNCOPE", false, 1.0f);
        CharacterController2D.Instance.rb.AddForce((Vector2.left * 300f) + (Vector2.up * 300f));

        CameraScript.CameraState = FollowCamera.State.PLAYER;

        Spider.transform.position = new Vector3(556.66f, transform.position.y - 1.0f, 0);
        HelperScript.Helper.gameObject.SetActive(false);

        UIManager.Instance.HPBar.fillAmount += 1f;

        yield return new WaitForSeconds(1.5f);

        UIManager.Instance.StartCoroutine("FadeIn");//페이드 인

        yield return new WaitForSeconds(2.5f);

        CharacterController2D.Instance.SetAnimation("WAKEUP", false, 1.0f);
        aniTime = DeskBox.state.GetCurrent(0).EndTime; // 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);

        CharacterController2D.Instance.Player.skeleton.FlipX = false;
        yield return new WaitForSeconds(1.0f);
        CharacterController2D.Instance.Player.skeleton.FlipX = true;
        yield return new WaitForSeconds(1.0f);

        isSkip = false;
        textCnt = 49;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(50))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();

        tutorialCnt = 6;
        CharacterController2D.Instance.isAct = false;
        ProgressCount += 1;
        yield return 0;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
    }

    IEnumerator Tutorial6() // 비행기 떨어지는 테스트
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.0f);
        UIManager.Instance.HintPrint(4, 0); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(4); // 해당하는 아이콘 이미지

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        CharacterController2D.Instance.isSkill = true;
        tutorialCnt = 7;
        yield return 0;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
    }

    IEnumerator Tutorial7()
    {
        Debug.Log("7");
        //CharacterController2D.Instance.isAct = false;
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HelpUIOFF();
        tutorialCnt = 8;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator Tutorial8() //세이브 포인트 아이템 설명
    {
        Debug.Log("8");
        UIManager.Instance.HintPanel2.clipOffset = new Vector2(128.6f, -214.4f);
        UIManager.Instance.HelpTextLabel.transform.localPosition = new Vector3(0, -289f, 0);
        iTween.Stop(UIManager.Instance.HintArrow.gameObject);
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        UIManager.Instance.HintPanel2.clipOffset = new Vector2(133.8f, -239.1f);
        UIManager.Instance.HintPrint(5, 1); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(5); // 해당하는 아이콘 이미지

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 9;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator Tutorial9() // 위험한 오브젝트 설명
    {
        Debug.Log("9");
        iTween.Stop(UIManager.Instance.HintArrow.gameObject);
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        UIManager.Instance.HintPanel2.clipOffset = new Vector2(128.6f, -262.4f);
        UIManager.Instance.HintPrint(6, 1); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(6); // 해당하는 아이콘 이미지

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.RestartPointSet();
        tutorialCnt = 10;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator Tutorial10() // 체력 아이템 설명
    {
        Debug.Log("10");
        iTween.Stop(UIManager.Instance.HintArrow.gameObject);
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        UIManager.Instance.HintPanel2.clipOffset = new Vector2(128.6f, -186.3f);
        UIManager.Instance.HintPrint(7, 1); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(7); // 해당하는 아이콘 이미지

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 11;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator Tutorial11() // 로프액션
    {
        iTween.Stop(UIManager.Instance.HintArrow.gameObject);
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        UIManager.Instance.HintPanel2.clipOffset = new Vector2(335.6f, 273);
        UIManager.Instance.HintPrint(8, 1); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(8); // 해당하는 아이콘 이미지

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 13;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    /* IEnumerator Tutorial12()
     {
         UIManager.Instance.HelpTextLogSprite.transform.localPosition = new Vector3(0, -125.35f, 0);
         UIManager.Instance.Cloud.transform.localPosition = new Vector3(-284, -125.35f, 0);
         UIManager.Instance.RKey.transform.localPosition = new Vector3(195, -125.35f, 0);
         UIManager.Instance.HelpUIOFF();
         yield return new WaitForSeconds(1.5f);
         tutorialCnt = 13;
         yield return 0;
     }*/

    IEnumerator Tutorial13()
    {
        Debug.Log("13");
        /*UIManager.Instance.HelpTextLogSprite.transform.localPosition = new Vector3(0, -125.35f, 0);
        UIManager.Instance.Cloud.transform.localPosition = new Vector3(-284, -125.35f, 0);
        UIManager.Instance.RKey.transform.localPosition = new Vector3(195, -125.35f, 0);*/
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        textCnt = 50;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(53))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 14;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator Tutorial14() // 열쇠를 먹으면 작동
    {
        Debug.Log("14");
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        textCnt = 53;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(54))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 15;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator Tutorial15() 
    {
        Debug.Log("15");
        float aniTime;
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        UIManager.Instance.HintAlarmSpace.enabled = false;

        yield return new WaitForSeconds(1.0f);
        //철컥하는 사운드 작동
        DeskBox.state.SetAnimation(0, "SHAKE", false);//흔들리는 서랍장 애니메이션 중지하도록 트랩쪽에서 작동
        yield return new WaitForSeconds(1.5f);

        if (CharacterController2D.Instance.isHang)
        {
            CharacterController2D.Instance.StartCoroutine("CLIMBING");
            aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime;
            yield return new WaitForSeconds(aniTime);
        }

        while (PlayerTr.transform.position.x > 237f)
        {
            CharacterController2D.Instance.isAct = true;
            CharacterController2D.Instance.Player.skeleton.FlipX = false;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.left * 0.08f * Time.timeScale));
            yield return 0;
        }

        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.Player.skeleton.FlipX = true; // 플레이어는 오른쪽을 바라본다.

        yield return new WaitForSeconds(0.5f);
        DeskBox.state.SetAnimation(0, "OPEN", false);
        CharacterController2D.Instance.Player.skeleton.FlipX = true; // 플레이어는 오른쪽을 바라본다.
        aniTime = DeskBox.state.GetCurrent(0).EndTime; // 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);

        SaveDoll.timeScale = 1.0f;
        SaveDoll.state.SetAnimation(0, "SURPRISE", false);

        while (SaveDoll.transform.position.y < 6.5f) // 바닥이 올라온다.
        {
            SaveFloor.transform.Translate((Vector2.up * 0.08f * Time.timeScale));
            yield return 0;
        }

        yield return new WaitForSeconds(1.0f);
        SaveDoll.state.SetAnimation(0, "STAY", false);

        isSkip = false;
        textCnt = 54;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(65))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();

        SaveDoll.skeleton.FlipX = true;
        SaveDoll.state.SetAnimation(0, "WALK", true);

        while (SaveDoll.transform.position.x < 256f) //구원받은 인형은 오른쪽을 바라보고 빠른 속도로 도망간다.
        {
            SaveDoll.transform.Translate((Vector2.right * 0.08f * Time.timeScale));
            yield return 0;
        }

        SaveDoll.transform.position = Vector3.zero;
        SaveDoll.enabled = false;

        isSkip = false;
        textCnt = 65;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(66))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();

        yield return new WaitForSeconds(0.5f);
        CharacterController2D.Instance.isAct = false;
        GameManager.Instance.RestartPointSet();
        tutorialCnt = 16;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator Tutorial16() // 댄서를 만나면 작동
    {
        Debug.Log("16");
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        textCnt = 66;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(77))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();

        // 댄서가 발로 차는 애니메이션
        Dancer.state.SetAnimation(0, "attack", false);

        CharacterController2D.Instance.SetAnimation("SYNCOPE", false, 1.0f);
        CharacterController2D.Instance.rb.AddForce((Vector2.left * 300f) + (Vector2.up * 300f));
        yield return new WaitForSeconds(1.5f);

        UIManager.Instance.StartCoroutine("FadeOut");

        BeforeObj.SetActive(false);
        AfterObj.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Dancer.enabled = false;
        UIManager.Instance.StartCoroutine("FadeIn");

        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.meetDancer = false;

        CharacterController2D.Instance.SetAnimation("WAKEUP", false, 1.0f);

        UIManager.Instance.HintPrint(9, 0); // 비마스크 형태의 안내 배경
        UIManager.Instance.HelpInfoIcon(9); // 해당하는 아이콘 이미지
        UIManager.Instance.HelpTextLabel.transform.localPosition = new Vector3(0, -271, 0);

        yield return new WaitForSeconds(1.5f);
        CharacterController2D.Instance.isInfo = true; // 안내말 스킵을 제공하는 bool true
        UIManager.Instance.HintSkipLabel.enabled = true;
        GameManager.Instance.RestartPointSet();
        tutorialCnt = 17;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator FailTutorialDancer() // 댄서를 기사 퀘스트 못깨고 만나면 작동
    {
        UIManager.Instance.HelpUIOFF();// 도움말 제거
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        yield return new WaitForSeconds(0.1f);

        while (PlayerTr.transform.position.x > 357.0f)// 헬퍼와 플레이어는 서로를 향해 걸어간다.
        {
            CharacterController2D.Instance.Player.skeleton.FlipX = false;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.left * 0.08f * Time.timeScale));
            yield return 0;
        }

        isSkip = false;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.Player.skeleton.FlipX = true;

        isSkip = false;
        textCnt = 77;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(78))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        GameManager.Instance.meetDancer = false;
        CharacterController2D.Instance.isAct = false;

        if (!GameManager.Instance.getCompas)
        {
            UIManager.Instance.HelpTextLabel.transform.localPosition = Vector3.zero;
            UIManager.Instance.Compass.transform.localPosition += Vector3.up * 309f;
            UIManager.Instance.HelpInfoIcon(9);
            UIManager.Instance.HelpPrint(9); // 도움말 창에 컴퍼스를 구해와요
            tutorialCnt = 17;
        }
        else
        {
            tutorialCnt = 18;
        }

        yield return new WaitForSeconds(3.5f);
        UIManager.Instance.HelpUIOFF();// 도움말 제거

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator FailTutorialKnight() // 교환할 물건없이 기사와의 만남
    {
        // 통안에 들어가면 바로 대화 시작
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        while (PlayerTr.transform.position.x < 95.0f)
        {
            CharacterController2D.Instance.Player.skeleton.FlipX = true;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.right * 0.08f * Time.timeScale));
            yield return 0;
        }
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.Player.skeleton.FlipX = false;

        Knight.state.SetAnimation(0, "fight ready", true);
        yield return new WaitForSeconds(1.0f);
        Knight.state.SetAnimation(0, "TALK ANGER", true);

        isSkip = false;
        textCnt = 79;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(81))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        //\기사한테 줄 컴퍼스 없으면 저리 가버렸 이 바늘축 대신할 거 안가져오면 안줄꺼임 하면서 체력 닮ㅇㅁㅇㄹ

        //yield return new WaitForSeconds(3.5f);
        if (!GameManager.Instance.getCompas)
        {
            UIManager.Instance.HelpInfoIcon(9);
            UIManager.Instance.HelpPrint(9); // 도움말 창에 컴퍼스를 구해와요
            tutorialCnt = 17;
        }
        else
        {
            tutorialCnt = 18;
        }
        UIManager.Instance.Compass.transform.localPosition = new Vector3(UIManager.Instance.Compass.transform.localPosition.x, 0, 0);
        UIManager.Instance.HelpTextLabel.transform.localPosition = Vector3.zero;
        GameManager.Instance.meetKnight = false;
        CharacterController2D.Instance.isAct = false;

        yield return new WaitForSeconds(3.5f);
        UIManager.Instance.HelpUIOFF();// 도움말 제거

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
    }

    IEnumerator Tutorial17() // 캠퍼스를 먹으면 작동
    {
        GameManager.Instance.getCompas = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        textCnt = 78;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(79))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        GameManager.Instance.RestartPointSet();
        CharacterController2D.Instance.isAct = false;
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 18;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator Tutorial18() // 기사한테 줄 캠퍼스 있는 상태에서 통안에 들어가면 바로 대화 시작 
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;

        while (PlayerTr.transform.position.x < 97f)
        {
            CharacterController2D.Instance.Player.skeleton.FlipX = true;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.right * 0.08f * Time.timeScale));
            yield return 0;
        }
        /*
        for (float i = 1f; i >= 0; i -= WalkTime)// 기사와 거리를 둔다
        {
            CharacterController2D.Instance.Player.skeleton.FlipX = true;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.right * WalkTime * 2.0f));
            yield return 0;
        }*/
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.Player.skeleton.FlipX = false;
        yield return new WaitForSeconds(1.5f);

        Knight.state.SetAnimation(0, "talk", true);

        isSkip = false;
        textCnt = 81;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(91))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        Knight.state.SetAnimation(0, "stay", true);

        // 캐릭터 통 밖으로 나감
        // 통 입구 콜리더로 봉쇄 님 못들어감

        // 축음기로 가라는 도움말 창 뜸

        GameManager.Instance.RestartPointSet();
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 19;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator Tutorial19() // 댄서에게 줄 아이템 있을 경우
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        textCnt = 91;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(94))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        GameManager.Instance.meetMan = false; // 서랍장에 들어있던 인형의 값을 다시 false

        //TempDancer.SetActive(false);
        Dancer.transform.position = new Vector3(0, 0, 0);
        Gramophone.EffectOn();

        SoundEffectManager.Instance.BGMStart(3);
        GameManager.Instance.RestartPointSet();
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 20;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator Tutorial20() //댄서 미션을 끝내고 거미줄 쳐진 죽어가는 서랍장 인형을 처음 만났을 때
    {
        SoundEffectManager.Instance.SoundDelay("SpiderHome", 0);
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        float aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime; // 테마 전달 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);
        CharacterController2D.Instance.SetAnimation("SURPRISE", false, 1.0f);
        aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime; // 테마 전달 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);

        isSkip = false;
        textCnt = 95;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(112))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        UIManager.Instance.HelpTextLabel.transform.localPosition = Vector3.zero;
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpInfoIcon(12);
        UIManager.Instance.HelpPrint(12); // 괴물과 안내 인형을 찾으러 앞으로 가세요
        tutorialCnt = 21;
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.HelpUIOFF();
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator Tutorial21() //안내 인형과 거미를 발견
    {
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        isSkip = false;
        textCnt = 112;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(114))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpInfoIcon(13);
        UIManager.Instance.HelpPrint(13); // 비행기 나무 모빌 위로 기어올라가세요
        tutorialCnt = 22;
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.HelpUIOFF();
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator Tutorial22() //안내 인형을 구해주는 컷 신
    {
        SoundEffectManager.Instance.BGMStart(4);
        isSkip = false;
        UIManager.Instance.UIOff();
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HPBar.enabled = false;
        UIManager.Instance.CheckBar.enabled = false;
        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        CutToon2.SetActive(true); // 컷 신 ON
        Spider.enabled = false;
        UIManager.Instance.CutToonSkipLabel.enabled = true;
        yield return new WaitForSeconds(10.0f);
        CutToon2.SetActive(false); // 컷 신 Off
        SoundEffectManager.Instance.BGMStart(5);
        UIManager.Instance.CutToonSkipLabel.enabled = false;

        CharacterController2D.Instance.isAct = false;

        tutorialCnt = 23;
        ProgressCount += 1;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator Tutorial23() //추격전
    {
        Spider.enabled = false;
        UIManager.Instance.HPBar.enabled = true;
        UIManager.Instance.CheckBar.enabled = true;
        CharacterController2D.Instance.isAct = false;
        CameraScript.CameraState = FollowCamera.State.SPIDERTRACE;
        CharacterController2D.Instance.Player.transform.position = new Vector3(607, 0, 0);
        BossSpider.transform.position = new Vector3(581.96f, -0.3f, 2);
        BossSpider.SetActive(true);

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }

    IEnumerator FindMan() // 열쇠로 서랍장에 있는 인형을 구해줘야 할때
    {
        CharacterController2D.Instance.isAct = true;
        while (PlayerTr.transform.position.x > 241.0f)
        {
            CharacterController2D.Instance.Player.skeleton.FlipX = false;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.left * 0.08f * Time.timeScale));
            yield return 0;
        }

        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.Player.skeleton.FlipX = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        isSkip = false;
        textCnt = 116;
        while (!isSkip)
        {
            UIManager.Instance.TalkStart(textCnt);
            UIManager.Instance.WaitCheck(textCnt);
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
            textCnt += 1;
            if (textCnt.Equals(117))
            {
                break;
            }
        }

        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        GameManager.Instance.mansWall = false;

        if (GameManager.Instance.getKey) // 열쇠를 먹었으면
        {
            UIManager.Instance.HelpUIOFF();
            UIManager.Instance.HelpInfoIcon(11);
            UIManager.Instance.HelpPrint(16);
            tutorialCnt = 15;
        }

        if (!GameManager.Instance.getKey) // 열쇠를 먹지 않았으면
        {
            UIManager.Instance.HelpUIOFF();

            UIManager.Instance.HelpTextLabel.transform.localPosition = Vector3.zero;
            UIManager.Instance.HelpInfoIcon(10);
            UIManager.Instance.HelpPrint(15);
            tutorialCnt = 14;
        }
        UIManager.Instance.HelpTextLabel.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpTextLabel.transform.localPosition += Vector3.down * -267;
        //CharacterController2D.Instance.isAct = false;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장

        yield return 0;
    }
    IEnumerator ShadowSpiderAct()
    {
        ShadowSpider.transform.position = new Vector3(10.0f, ShadowSpider.transform.position.y, ShadowSpider.transform.position.z);
        ShadowSpider.skeleton.FlipX = true;
        while (ShadowSpider.transform.position.x < 100)
        {
            ShadowSpider.transform.Translate(Vector2.right * 0.4f * Time.timeScale);
            yield return 0;
        }
        ShadowSpider.GetComponent<MeshRenderer>().enabled = false;

        TutoDataSave(); // 게임 데이터 세이브 기록
        GameManager.Instance.GameInfoDataSave(SlotDataNumberSave.Instance.slotNum); // 게임 데이터 세이브 저장
        yield return 0;
    }
}