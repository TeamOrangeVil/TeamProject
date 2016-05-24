using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{

    public int tutorialCnt = 100; // 튜토리얼 순서
    public float WalkTime = 0.01f; // NPC 걷기

    public bool isTutorial = false; // 튜토리얼 작동하는지
    public bool isDist = false; // 캐릭터와 Helper 거리 확인 제한
    public bool isSkip = false;

    public Vector3 PlayerScreenPos;
    public Vector3 HelperScreenPos;
    public Camera cameraUI;

    public HelperController HelperScript; // 헬퍼 스크립트
    public FollowCamera CameraScript;

    public Transform PlayerTr;
    public Transform HelperDollTr;

    public Vector3 TutorialDist; // NPC와 캐릭터 거리

    public GameObject CutToon1; // 컷신
    public GameObject CutToon2;

    public SkeletonAnimation Spider;
    public GameObject BlackBoard;

    public GameObject BeforeObj;
    public GameObject AfterObj;

    public SkeletonAnimation DeskBox;

    void Awake()
    {
        //BeforeObj = GameObject.Find("BeforeObject").GetComponent<GameObject>();
        //AfterObj = GameObject.Find("AfterObject").GetComponent<GameObject>();

        HelperScript = GameObject.FindGameObjectWithTag("HELPER").GetComponent<HelperController>();
        CameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();

        PlayerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        HelperDollTr = GameObject.FindGameObjectWithTag("HELPER").GetComponent<Transform>();
        Spider = GameObject.Find("Spider").GetComponent<SkeletonAnimation>();
        DeskBox = GameObject.Find("Men_Temp").GetComponent<SkeletonAnimation>();
        AfterObj.SetActive(false);
    }
    void Start()
    {

    }
    public void TutoStart(bool isStart)
    {
        isTutorial = isStart;
    }
    public void RestartGame()
    {
        CutToon1.SetActive(false); // 컷 신 바로 Off
        if (tutorialCnt > 5)
            Spider.transform.position = new Vector3(415.5f, transform.position.y, 0);
    }
    void Update()
    {
        if (!isTutorial)
        {
            TutorialCheck();
        }
        TutorialDist = HelperDollTr.transform.position - PlayerTr.transform.position; // NPC와 캐릭터 거리

        if (Input.GetKeyDown(KeyCode.Return))
        {
            isSkip = true;
        }
        if (Input.GetKeyDown(KeyCode.Q)) // QA 분들용 어려운 맵 무시
        {
            CharacterController2D.Instance.Player.transform.position = new Vector3(190, 1, 0);
        }
    }

    void TutorialCheck()
    {
        if (tutorialCnt.Equals(0))
        {
            if (CutToon1.activeInHierarchy.Equals(true) && isSkip)
            {
                CutToon1.SetActive(false);
                UIManager.Instance.CutToonSkipLabel.enabled = false;
                StopAllCoroutines();
                isSkip = false;
                tutorialCnt = 1;
            }
            if (CutToon2.activeInHierarchy.Equals(true) && isSkip)
            {
                CutToon2.SetActive(false);
                UIManager.Instance.CutToonSkipLabel.enabled = false;
                StopAllCoroutines();
                isSkip = false;
                tutorialCnt = 23;
            }
                /*if (!GameManager.Instance.getCompas && GameManager.Instance.meetDancer) //기사 퀘스트까지 안깨고(false조건) 댄서에게 가면  // false or True // 플레이어 거리
                {
                    Debug.Log("댄서가 화냄");
                    StartCoroutine(FailTutorialDancer()); // 빨리 구해와!
                    tutorialCnt = 0;
                }
                if (!GameManager.Instance.getCompas && GameManager.Instance.meetKnight) // 캠퍼스 무시하고 기사 만나러 가면 // false or True // 플레이어 거리
                {
                    Debug.Log("기사가 화냄");
                    StartCoroutine(FailTutorialKnight()); // 빨리 구해와!
                    tutorialCnt = 0;
                }
                if (GameManager.Instance.getCompas) // 캠퍼스 먹으면 작동
                {
                    Debug.Log("캠퍼스 먹음");
                    StartCoroutine(Tutorial17());
                    tutorialCnt = 0;
                }*/
            }
        if (tutorialCnt.Equals(100))
        {
            StartCoroutine(Tutorial100());
            tutorialCnt = 0;
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
        if (tutorialCnt.Equals(3) && TutorialDist.x < 2.5f)
        {
            StartCoroutine(Tutorial3());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(4) && TutorialDist.x < 2.5f)
        {
            StartCoroutine(Tutorial4());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(5) && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Tutorial5());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(6) && CharacterController2D.Instance.transform.position.x > 63.3f)
        {
            StartCoroutine(Tutorial6());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(7) && Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(Tutorial7());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(8) && CharacterController2D.Instance.transform.position.x > 75.6f) // 체크 포인트 아이템
        {
            StartCoroutine(Tutorial8());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(9) && CharacterController2D.Instance.transform.position.x > 88.0f) // 위험한 오브젝트 경고
        {
            StartCoroutine(Tutorial9());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(10) && CharacterController2D.Instance.transform.position.x > 102.5f) // 체력 아이템
        {
            StartCoroutine(Tutorial10());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(11) && CharacterController2D.Instance.transform.position.x > 126.0f) // 로프 액션
        {
            StartCoroutine(Tutorial11());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(12) && CharacterController2D.Instance.isAct) // 로프액션으로 캐릭터 움직임 제한 되면
        {
            StartCoroutine(Tutorial12());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(13) && CharacterController2D.Instance.transform.position.x > 238.35f)
        {
            StartCoroutine(Tutorial13());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(14) && GameManager.Instance.getKey) // 열쇠를 먹으면 실행
        {
            StartCoroutine(Tutorial14());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(15) && GameManager.Instance.meetMan) // 흔들리는 서랍장 밟으면 실행
        {
            StartCoroutine(Tutorial15());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(16) && GameManager.Instance.meetDancer) // 댄서 만나면 실행 기사한테 아이템 가져오라는 미션
        {
            StartCoroutine(Tutorial16());
            tutorialCnt = 0;
        }

        if (tutorialCnt.Equals(17)) // 카운트가 17인 상태에서 
        {
            Debug.Log("카운트가 17인 상태에서");
            if (!GameManager.Instance.getCompas && GameManager.Instance.meetDancer) //기사 퀘스트까지 안깨고(false조건) 댄서에게 가면  // false or True // 플레이어 거리
            {
                Debug.Log("댄서가 화냄");
                StartCoroutine(FailTutorialDancer()); // 빨리 구해와!
                tutorialCnt = 0;
            }
            if (!GameManager.Instance.getCompas && GameManager.Instance.meetKnight) // 캠퍼스 무시하고 기사 만나러 가면 // false or True // 플레이어 거리
            {
                Debug.Log("기사가 화냄");
                StartCoroutine(FailTutorialKnight()); // 빨리 구해와!
                tutorialCnt = 0;
            }
            if (GameManager.Instance.getCompas) // 캠퍼스 먹으면 작동
            {
                Debug.Log("캠퍼스 먹음");
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

        if (tutorialCnt.Equals(20) ) // 죽어가는 서랍장 인형을 만나는 조건 식 포함
        {
            StartCoroutine(Tutorial20());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(21)) // 안내인형과 거미를 발견할 적당한 거리 추가
        {
            StartCoroutine(Tutorial20());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(22)) // 안내 인형을 구해주는 컷 신
        {
            StartCoroutine(Tutorial22());
            tutorialCnt = 0;
        }
        if (tutorialCnt.Equals(23)) // 컷 신 끝나면 바로 특별한 조건 없음
        {
            StartCoroutine(Tutorial23());
            tutorialCnt = 0;
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
        UIManager.Instance.CutToonSkipLabel.enabled = false;
        tutorialCnt = 1;
        yield return 0;
    }
    IEnumerator Tutorial1()
    {

        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        CameraScript.CameraState = FollowCamera.State.PLAYER;


        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        for (float i = 1f; i >= 0; i -= WalkTime)// 단상에서 일어난다.
        {
            CharacterController2D.Instance.SetAnimation("DROP", true, 1.0f); // 단상에서 일어나는 애니메이션
            yield return 0;
        }
        CharacterController2D.Instance.Player.skeleton.flipX = true; // 플레이어는 오른쪽을 바라본다.

        for (float i = 1f; i >= 0; i -= WalkTime)// 단상에서 걸어간다.
        {
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.right * WalkTime * 3.5f));
            yield return 0;
        }

        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있는다.
        yield return new WaitForSeconds(0.5f);
        HelperScript.Helper.skeleton.flipX = false; // 헬퍼는 왼쪽을 바라본다.
        yield return new WaitForSeconds(0.5f);

        for (float i = 1f; i >= 0; i -= WalkTime) //헬퍼가 캐릭터 쪽으로 걸어온다.
        {

            HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
            HelperScript.transform.Translate((Vector2.left * WalkTime));
            yield return 0;
        }

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        isSkip = false;
        for (int i = 0; i < 25; i++)//긴 대화를 나눈다. 25
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();

        HelperScript.Helper.Skeleton.flipX = true;
        CameraScript.CameraState = FollowCamera.State.HELPER; // 카메라가 헬퍼를 비춘다.

        for (float i = 1f; i >= 0; i -= WalkTime - 0.005f)// 헬퍼가 책 위로 올라간다.
        {

            HelperScript.transform.Translate((Vector2.right * WalkTime * 7.5f));
            if (i < 0.65f && i > 0.60f)
            {
                HelperScript.SetAnimation("GUIDEJUMP", false, 1.0f);
                HelperScript.isJump = true;
            }
            else
            {
                HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
                HelperScript.isJump = false;
            }
            yield return 0;
        }

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
        HelperScript.Helper.skeleton.flipX = false; // 헬퍼는 왼쪽을 바라본다.

        UIManager.Instance.TextPrintStart(25);
        UIManager.Instance.WaitCheck(25);
        yield return new WaitForSeconds(UIManager.Instance.WaitCount + 1.0f);

        UIManager.Instance.UIOff();
        CameraScript.CameraState = FollowCamera.State.PLAYER; // 카메라가 플레이어를 비춘다.
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.HelpTextPrintStart(1);
        UIManager.Instance.HelpCount = 1;
        UIManager.Instance.HelpInfo();

        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 2;
        yield return 0;

    }
    // 책 한권을 밟은 상태
    IEnumerator Tutorial2()
    {
        UIManager.Instance.HelpUIOFF();

        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있음
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.0f);
        CameraScript.CameraState = FollowCamera.State.HELPER;
        HelperScript.Helper.skeleton.flipX = true;

        for (float i = 1f; i >= 0; i -= WalkTime)// 헬퍼가 책 위로 올라간다.
        {
            if (!HelperScript.isHang)
            {
                HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
            }
            else
            {
                HelperScript.SetAnimation("GUIDEHANG", true, 1.0f);
            }
            if (!HelperScript.isHangLimit)
            {
                HelperScript.transform.Translate((Vector2.right * WalkTime * 7.0f));
            }

            if (i < 0.45f && i > 0.449f)
            {
                HelperScript.SetAnimation("GUIDEJUMP", false, 1.0f);
                HelperScript.isJump = true;
            }
            else
            {
                HelperScript.isJump = false;
            }
            yield return 0;
        }
        HelperScript.SetAnimation("GUIDEHANG", true, 1.0f);
        yield return new WaitForSeconds(1.0f);
        HelperScript.StartCoroutine("CLIMBING");
        yield return new WaitForSeconds(2.0f);
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);


        for (float i = 1f; i >= 0; i -= WalkTime + 0.005f) // 올라간 책 위에서 옆으로 걸어간다.
        {
            HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);

            HelperScript.transform.Translate((Vector2.right * WalkTime * 6.0f));
            yield return 0;
        }
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
        HelperScript.Helper.skeleton.flipX = false; // 헬퍼가 왼쪽으로 돌아본다.

        yield return new WaitForSeconds(0.1f);

        UIManager.Instance.TextPrintStart(26);
        UIManager.Instance.WaitCheck(26);
        yield return new WaitForSeconds(UIManager.Instance.WaitCount - 0.5f);

        UIManager.Instance.UIOff();
        CameraScript.CameraState = FollowCamera.State.PLAYER;
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 3;
        yield return 0;

        UIManager.Instance.HelpTextPrintStart(2);
        UIManager.Instance.HelpCount = 2;
        UIManager.Instance.HelpInfo();
    }
    // 책 더미를 밟은 상황
    IEnumerator Tutorial3()
    {
        CharacterController2D.Instance.isAct = true;
        UIManager.Instance.HelpUIOFF();
        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f);
        yield return new WaitForSeconds(1.0f);
        CameraScript.CameraState = FollowCamera.State.HELPER;
        HelperScript.Helper.skeleton.flipX = true; // 헬퍼가 오른쪽을 바라봄

        for (float i = 1f; i >= 0; i -= 0.003f) // 기어가기 
        {
            if (HelperScript.isCrawl)
            {
                HelperScript.SetAnimation("GUIDECRAWL", true, 1.0f);
            }
            if (!HelperScript.isCrawl)
            {
                HelperScript.SetAnimation("GUIDEWALK", true, 1.0f);
            }

            HelperScript.transform.Translate((Vector2.right * WalkTime * 7.5f));
            yield return 0;
        }

        HelperScript.Helper.skeleton.flipX = false;
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.TextPrintStart(27);
        UIManager.Instance.WaitCheck(27);
        yield return new WaitForSeconds(UIManager.Instance.WaitCount);

        UIManager.Instance.UIOff();
        CameraScript.CameraState = FollowCamera.State.PLAYER;
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 4;
        yield return 0;
    }
    // 쓰러질 것 같은 더미 옆에서 있는 상태
    IEnumerator Tutorial4()
    {
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있음
        yield return new WaitForSeconds(1.0f);

        isSkip = false;
        for (int i = 28; i < 30; i++)//테마와 관련된 이야기를 한다. 길이는 몇인가
        {
            HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (isSkip)
            {
                isSkip = false;
                break;
            }

            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        yield return new WaitForSeconds(0.1f);
        HelperScript.SetAnimation("GUIDETHEME", false, 1.0f);
        float aniTime = HelperScript.Helper.state.GetCurrent(0).EndTime; // 테마 전달 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);
        HelperScript.SetAnimation("GUIDEGET", false, 1.0f);
        aniTime = HelperScript.Helper.state.GetCurrent(0).EndTime;
        yield return new WaitForSeconds(aniTime + 1.0f);
        HelperScript.Helper.skeleton.SetAttachment("guide/clothe", null);


        UIManager.Instance.TextPrintStart(30);
        UIManager.Instance.WaitCheck(30);
        yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        UIManager.Instance.UIOff();
        HelperController.Instance.Helper.skeleton.SetToSetupPose();
        yield return new WaitForSeconds(0.3f);
        HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);

        //옷 주는 이펙트 효과++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //AniSpriteChange.Instance.currentName = AniSpriteChange.Instance.Thema02; //캐릭터 복장 테마 복장으로 변신
        AniSpriteChange.Instance.SpriteChange(1);
        CharacterController2D.Instance.isTutorialNeedle = true; // 이제부터 바늘을 들고다님
        yield return new WaitForSeconds(2.0f);

        isSkip = false;
        for (int i = 31; i < 43; i++)//긴 대화를 나눈다.
        {
            HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (i.Equals(42))
            {
                CharacterController2D.Instance.Needle.SetActive(true);
            }
            if (isSkip)
            {
                CharacterController2D.Instance.Needle.SetActive(true);
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }

        CharacterController2D.Instance.isSkill = true;
        UIManager.Instance.UIOff();
        UIManager.Instance.HelpTextPrintStart(3);
        UIManager.Instance.HelpCount = 3;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HPBar.enabled = true;
        UIManager.Instance.CheckBar.enabled = true;

        tutorialCnt = 5;

    }
    // 처음 체크포인트 행동한 상태
    IEnumerator Tutorial5()
    {
        CharacterController2D.Instance.isSkill = false;
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("CHECKPOINT", false, 1.0f);
        float aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime;
        yield return new WaitForSeconds(aniTime + 1.0f);
        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f);
        UIManager.Instance.HelpUIOFF();
        isSkip = false;
        for (int i = 43; i < 49; i++)//긴 대화를 나눈다.
        {
            HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (i.Equals(44))
            {
                CameraScript.StartCoroutine("CameraShake");
            }
            if (i.Equals(47))
            {
                CharacterController2D.Instance.SetAnimation("SURPRISE", false, 1.0f);
                UIManager.Instance.HelpUIOFF();
            }
            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        CameraScript.CameraState = FollowCamera.State.SPIDER;
        for (float i = 1f; i >= 0; i -= WalkTime - 0.005f)// 거미가 유저를 향해 온다
        {
            Spider.state.SetAnimation(0, "WALK", true);
            Spider.transform.Translate((Vector2.right * WalkTime * -15.0f));
            yield return 0;
        }
        Spider.state.SetAnimation(0, "ATTACK", false);
        yield return new WaitForSeconds(0.8f);
        UIManager.Instance.HPBar.fillAmount -= 1f;

        /*for (float i = 1f; i >= 0; i -= WalkTime)// 유저가 쓰러진다
        {
            CharacterController2D.Instance.transform.Translate((Vector2.right * WalkTime * -0.5f));
            CharacterController2D.Instance.SetAnimation("SYNCOPE", false, 1.0f);
            yield return 0;
        }*/
        CharacterController2D.Instance.SetAnimation("SYNCOPE", false, 1.0f);
        CharacterController2D.Instance.rb.AddForce((Vector2.left * 300f) + (Vector2.up * 300f));
        yield return new WaitForSeconds(2.0f);
        //yield return new WaitForSeconds(0.5f);

        UIManager.Instance.UIOff();
        UIManager.Instance.HelpUIOFF();


        UIManager.Instance.StartCoroutine("FadeOut");//페이드 아웃

        yield return new WaitForSeconds(1.5f);

        CameraScript.CameraState = FollowCamera.State.PLAYER;

        Spider.transform.position = new Vector3(415.5f, transform.position.y, 0);
        HelperScript.Helper.gameObject.SetActive(false);

        UIManager.Instance.HPBar.fillAmount += 1f;

        UIManager.Instance.StartCoroutine("FadeIn");//페이드 인

        yield return new WaitForSeconds(2.5f);

        //PlayerScript.SetAnimation(" ", false, 1.0f);일어나는 애니메이션
        CharacterController2D.Instance.Player.skeleton.flipX = false;
        yield return new WaitForSeconds(1.0f);
        CharacterController2D.Instance.Player.skeleton.flipX = true;
        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.TextPrintStart(49);
        UIManager.Instance.WaitCheck(49);
        yield return new WaitForSeconds(UIManager.Instance.WaitCount);

        UIManager.Instance.UIOff();

        tutorialCnt = 6;
        CharacterController2D.Instance.isAct = false;
        yield return 0;
    }

    IEnumerator Tutorial6() // 비행기 떨어지는 테스트
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.HelpCount = 4;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(4);
        yield return new WaitForSeconds(0.5f);
        CharacterController2D.Instance.isSkill = true;
        tutorialCnt = 7;
        yield return 0;
    }

    IEnumerator Tutorial7()
    {
        CharacterController2D.Instance.isAct = false;
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HelpUIOFF();
        tutorialCnt = 8;
        yield return 0;
    }

    IEnumerator Tutorial8()
    {
        UIManager.Instance.HelpCount = 5;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(5); // 세이브 포인트 아이템 설명
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 9;
        yield return 0;
    }

    IEnumerator Tutorial9()
    {
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpCount = 6;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(6); // 위험한 오브젝트
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 10;
        yield return 0;
    }
    IEnumerator Tutorial10()
    {
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpCount = 7;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(7); // 체력 아이템
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 11;
        yield return 0;
    }

    IEnumerator Tutorial11()
    {
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpCount = 8;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(8); // 로프 설명
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 12;
        yield return 0;
    }

    IEnumerator Tutorial12()
    {
        UIManager.Instance.HelpUIOFF();
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 13;
        yield return 0;
    }

    IEnumerator Tutorial13()
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);
        isSkip = false;
        for (int i = 50; i < 53; i++)// 대사
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 14;
        yield return 0;
    }

    IEnumerator Tutorial14() // 열쇠를 먹으면 작동
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);
        isSkip = false;
        for (int i = 53; i < 54; i++)// 대사
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 15;
        yield return 0;
    }
    IEnumerator Tutorial15() // 
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;

        //철컥하는 사운드 작동
        DeskBox.state.SetAnimation(0, "shake", false);//흔들리는 서랍장 애니메이션 중지하도록 트랩쪽에서 작동
        yield return new WaitForSeconds(1.5f);


        for (float i = 1f; i >= 0; i -= WalkTime)// 흔들림이 멈춘 데스크에서 내려온다
        {
            CharacterController2D.Instance.Player.skeleton.flipX = false;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.left * WalkTime * 3.5f));
            yield return 0;
        }
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.Player.skeleton.flipX = true; // 플레이어는 오른쪽을 바라본다.

        yield return new WaitForSeconds(0.5f);
        DeskBox.state.SetAnimation(0, "open", false);
        // 흔들림이 격해지든 뭐든간에 막 움직이다가 이펙트 뿜으면서 구원을 받은 인형이 그곳에서 등장한다.
        // 구원받은 인형은 흔들리는 데스크보다 소팅레이어가 뒤에 있다
        // 오른쪽을 보고있다면 왼쪽을 보게 하고 왼쪽을 보고 있다면




        yield return new WaitForSeconds(1.5f);
        isSkip = false;
        for (int i = 54; i < 65; i++)// 대사
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();

        //구원받은 인형은 오른쪽을 바라보고 빠른 속도로 도망간다.
        isSkip = false;
        for (int i = 65; i < 66; i++)// 안내 인형이 누구 또 있는가 보네
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();

        yield return new WaitForSeconds(0.5f);
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 16;
        yield return 0;
    }

    IEnumerator Tutorial16() // 댄서를 만나면 작동
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        for (int i = 66; i < 77; i++)// 댄서랑 기사에 관한 이야기를 나눈다.
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();

        // 댄서가 발로 차는 애니메이션

        CharacterController2D.Instance.SetAnimation("SYNCOPE", false, 1.0f);
        CharacterController2D.Instance.rb.AddForce((Vector2.left * 300f) + (Vector2.up * 300f));
        yield return new WaitForSeconds(1.5f);

        UIManager.Instance.StartCoroutine("FadeOut");

        UIManager.Instance.UIOff();
        UIManager.Instance.HelpCount = 9;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(9); // 도움말 창에 컴퍼스를 구해와요

        BeforeObj.SetActive(false);
        AfterObj.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        UIManager.Instance.StartCoroutine("FadeIn");

        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.meetDancer = false;
        CharacterController2D.Instance.isAct = false; // 캐릭터 움직임 제한 해제
        tutorialCnt = 17;
        yield return new WaitForSeconds(4.0f);
        UIManager.Instance.HelpUIOFF();// 도움말 제거

        yield return 0;
    }

    IEnumerator FailTutorialDancer() // 댄서를 기사 퀘스트 못깨고 만나면 작동
    {
        UIManager.Instance.HelpUIOFF();// 도움말 제거
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        yield return new WaitForSeconds(0.1f);

        for (float i = 1f; i >= 0; i -= WalkTime)// 댄서와 거리를 둔다
        {
            CharacterController2D.Instance.Player.skeleton.flipX = false;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.left * WalkTime * 2.0f));
            yield return 0;
        }
        isSkip = false;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.Player.skeleton.flipX = true;

        for (int i = 77; i < 78; i++)// 댄서랑 기사에 관한 이야기를 나눈다.
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        GameManager.Instance.meetDancer = false;
        CharacterController2D.Instance.isAct = false;

        if (!GameManager.Instance.getCompas)
        {
            UIManager.Instance.HelpCount = 9;
            UIManager.Instance.HelpInfo();
            UIManager.Instance.HelpTextPrintStart(9); // 도움말 창에 컴퍼스를 구해와요
            tutorialCnt = 17;
        }
        else
        {
            tutorialCnt = 18;
        }

        yield return new WaitForSeconds(3.5f);
        UIManager.Instance.HelpUIOFF();// 도움말 제거

        yield return 0;
    }
    IEnumerator FailTutorialKnight() // 교환할 물건없이 기사와의 만남
    {
        // 통안에 들어가면 바로 대화 시작
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        for (float i = 1f; i >= 0; i -= WalkTime)// 댄서와 거리를 둔다
        {
            CharacterController2D.Instance.Player.skeleton.flipX = true;
            CharacterController2D.Instance.SetAnimation("WALK", true, 1.0f);
            CharacterController2D.Instance.transform.Translate((Vector2.right * WalkTime * 2.0f));
            yield return 0;
        }

        isSkip = false;
        for (int i = 80; i < 81; i++)// 댄서랑 기사에 관한 이야기를 나눈다.
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        //\기사한테 줄 컴퍼스 없으면 저리 가버렸 이 바늘축 대신할 거 안가져오면 안줄꺼임 하면서 체력 닮ㅇㅁㅇㄹ

        //yield return new WaitForSeconds(3.5f);
        if (!GameManager.Instance.getCompas)
        {
            UIManager.Instance.HelpCount = 9;
            UIManager.Instance.HelpInfo();
            UIManager.Instance.HelpTextPrintStart(9); // 도움말 창에 컴퍼스를 구해와요
            tutorialCnt = 17;
        }
        else
        {
            tutorialCnt = 18;
        }
        GameManager.Instance.meetKnight = false;
        CharacterController2D.Instance.isAct = false;
        yield return 0;
    }

    IEnumerator Tutorial17() // 캠퍼스를 먹으면 작동
    {
        GameManager.Instance.getCompas = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        for (int i = 78; i < 79; i++)// 플레이어 대사 어엇 이것을 기사한테 줘야징
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        yield return new WaitForSeconds(1.5f);
        tutorialCnt = 18;
        yield return 0;
    }

    IEnumerator Tutorial18() // 기사한테 줄 캠퍼스 있는 상태에서 통안에 들어가면 바로 대화 시작 
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        for (int i = 81; i < 91; i++)// 기사한테 줄 컴퍼스 있으면 올 바꿔드림 넘겨 드림하고 넘겨줌    그리고 가보셈
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();


        // 캐릭터 통 밖으로 나감
        // 통 입구 콜리더로 봉쇄 님 못들어감

        // 축음기로 가라는 도움말 창 뜸
        CharacterController2D.Instance.isAct = false;
        tutorialCnt = 19;
        yield return 0;
    }

    IEnumerator Tutorial19() // 댄서에게 줄 아이템 있을 경우
    {
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        CharacterController2D.Instance.isAct = true;
        yield return new WaitForSeconds(1.5f);

        isSkip = false;
        for (int i = 91; i < 94; i++)// 댄서 앞으로 갔을때 구해왔으면 어머 하고 즐거운 대화 
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();

        // 댄서 콜라더 없애거나 아니면 아예 어디론가 가버리던가
        tutorialCnt = 20;
        yield return 0;
    }
    IEnumerator Tutorial20() //댄서 미션을 끝내고 거미줄 쳐진 죽어가는 서랍장 인형을 처음 만났을 때
    {
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);
        float aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime; // 테마 전달 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);
        CharacterController2D.Instance.SetAnimation("SURPRISE", false, 1.0f);
        aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime; // 테마 전달 애니메이션 길이 저장
        yield return new WaitForSeconds(aniTime);

        isSkip = false;
        for (int i = 95; i < 112; i++)// 죽어가는 서랍장 인혀오가의 대화
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpCount = 12;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(12); // 괴물과 안내 인형을 찾으러 앞으로 가세요
        tutorialCnt = 21;
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.HelpUIOFF();
        yield return 0;
    }
    IEnumerator Tutorial21() //안내 인형과 거미를 발견
    {
        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        isSkip = false;
        for (int i = 112; i < 114; i++)// 방법 구상하는 독백
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpCount = 13;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(13); // 비행기 나무 모빌 위로 기어올라가세요
        tutorialCnt = 22;
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.HelpUIOFF();
        yield return 0;
    }
    IEnumerator Tutorial22() //안내 인형을 구해주는 컷 신
    {
        UIManager.Instance.UIOff();
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HPBar.enabled = false;
        UIManager.Instance.CheckBar.enabled = false;
        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        UIManager.Instance.CutToonSkipLabel.enabled = true;
        yield return new WaitForSeconds(10.0f);

        CutToon2.SetActive(false); // 컷 신 Off
        UIManager.Instance.CutToonSkipLabel.enabled = false;

        tutorialCnt = 23;

        yield return 0;
    }
    IEnumerator Tutorial23() //안내 인형을 구해주고 대화
    {
        // 캐릭터와 거미, 헬퍼의 위치 조정

        CharacterController2D.Instance.isAct = true;
        CharacterController2D.Instance.Player.skeleton.flipX = false;
        CharacterController2D.Instance.SetAnimation("STAY", false, 1.0f);

        isSkip = false;
        for (int i = 114; i < 116; i++)// 안내 인형과의 대화
        {
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        CharacterController2D.Instance.isAct = false;
        UIManager.Instance.HelpUIOFF();
        UIManager.Instance.HelpCount = 14;
        UIManager.Instance.HelpInfo();
        UIManager.Instance.HelpTextPrintStart(14); // 거미를 피해 앞으로 도망치세요!
        UIManager.Instance.HPBar.enabled = true;
        UIManager.Instance.CheckBar.enabled = true;
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.HelpUIOFF();

        yield return 0;
    }
}