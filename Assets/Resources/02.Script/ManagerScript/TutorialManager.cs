using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{

    public int tutorialCnt = 1; // 튜토리얼 순서
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

    public SkeletonAnimation Spider;
    public GameObject BlackBoard;

    void Start()
    {
        HelperScript = GameObject.FindGameObjectWithTag("HELPER").GetComponent<HelperController>();
        CameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();

        PlayerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        HelperDollTr = GameObject.FindGameObjectWithTag("HELPER").GetComponent<Transform>();
        Spider = GameObject.Find("Spider").GetComponent<SkeletonAnimation>();
    }
    void Update()
    {
        if (!isTutorial)
        {
            TutorialCheck();
        }
        TutorialDist = HelperDollTr.transform.position - PlayerTr.transform.position; // NPC와 캐릭터 거리

        if(Input.GetKeyDown(KeyCode.Return))
        {
            isSkip = true;
        }
    }

    void TutorialCheck()
    {
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
    }

    IEnumerator Tutorial1()
    {
        CharacterController2D.Instance.isAct = true; // 플레이어 움직임 제한
        CameraScript.CameraState = FollowCamera.State.PLAYER;
        yield return new WaitForSeconds(10.0f);
        CutToon1.SetActive(false); // 컷 신 Off

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

        isSkip = false;
        for (int i = 0; i < 25; i++)//긴 대화를 나눈다. 25
        {
            HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);

            if(isSkip)
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
            if (i < 0.65f && i > 0.64f)
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
        UIManager.Instance.HelpTextPrintStart(0);
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
                HelperScript.SetAnimation("GUIDEJUMP", true, 1.0f);
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

        UIManager.Instance.HelpTextPrintStart(1);
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
        yield return new WaitForSeconds(0.3f);
        HelperScript.SetAnimation("GUIDESTAY", false, 1.0f);
        
        //옷 주는 이펙트 효과++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                             //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
        AniSpriteChange.Instance.currentName = AniSpriteChange.Instance.Thema02; //캐릭터 복장 테마 복장으로 변신
        AniSpriteChange.Instance.SpriteChange();
        yield return new WaitForSeconds(2.0f);

        isSkip = false;
        for (int i = 31; i < 43; i++)//긴 대화를 나눈다.
        {
            HelperScript.SetAnimation("GUIDESTAY", true, 1.0f);
            UIManager.Instance.TextPrintStart(i);
            UIManager.Instance.WaitCheck(i);
            if (i.Equals(42))
            {
                CharacterController2D.Instance.Weapon.SetActive(true);
            }
            if (isSkip)
            {
                isSkip = false;
                break;
            }
            yield return new WaitForSeconds(UIManager.Instance.WaitCount);
        }
        UIManager.Instance.UIOff();
        UIManager.Instance.HelpTextPrintStart(2);
        UIManager.Instance.HelpCount = 3;
        UIManager.Instance.HelpInfo();
        CharacterController2D.Instance.isSkill = true;
        UIManager.Instance.StateUICount = 1;
        UIManager.Instance.StateUI();
        tutorialCnt = 5;
        
    }
    // 처음 체크포인트 행동한 상태
    IEnumerator Tutorial5()
    {
        CharacterController2D.Instance.SetAnimation("CHECKPOINT", true, 1.0f);
        UIManager.Instance.CheckBar.fillAmount -= 0.25f;
        float aniTime = CharacterController2D.Instance.Player.state.GetCurrent(0).EndTime;
        yield return new WaitForSeconds(aniTime + 1.0f);
        CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f);
        UIManager.Instance.HelpUIOFF();
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

        for (float i = 1f; i >= 0; i -= WalkTime)// 거미가 유저를 향해 온다
        {
            CharacterController2D.Instance.transform.Translate((Vector2.right * WalkTime * -0.5f));
            CharacterController2D.Instance.SetAnimation("SYNCOPE", false, 1.0f);
            yield return 0;
        }
        yield return new WaitForSeconds(0.5f);

        UIManager.Instance.UIOff();
        UIManager.Instance.HelpUIOFF();
        CameraScript.CameraState = FollowCamera.State.PLAYER;

        UIManager.Instance.StartCoroutine("FadeOut");//페이드 아웃

        yield return new WaitForSeconds(3.0f);

        Spider.transform.position = new Vector3(297.4f, transform.position.y, 0);
        HelperScript.Helper.transform.position = new Vector3(300, transform.position.y, 0);

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
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HelpTextPrintStart(3);
        UIManager.Instance.HelpCount = 4;
        UIManager.Instance.HelpInfo();
        tutorialCnt = 7;
        yield return 0;
    }

    IEnumerator Tutorial7()
    {
        CharacterController2D.Instance.isAct = false;
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HelpUIOFF();

        yield return 0;
    }

    //로프 설명하는 헬프 유아이 작동
    //로프 사용하면 헬프 유아이 종료
}
