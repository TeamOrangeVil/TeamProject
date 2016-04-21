using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour {
    List<EventScript> eventScripts;

    public int tutorialCnt = 1; // 튜토리얼 순서
    public float WalkTime = 0.01f; // NPC 걷기
  
    public bool isTutorial = false;

    public UILabel HelperText;
    public UILabel PlayerText;
    public TypewriterEffect helperWriteEffect;
    public TypewriterEffect PlayerWriteEffect;

    //public GameObject CharacterTalkBackground;
    //public GameObject HelperTalkBackground;

    //public GameObject BlackBlur;
    //public float BlackBlurHeight;
    //public float BlackBlurWidth;

    public Vector3 PlayerScreenPos;
    public Vector3 HelperScreenPos;
    public Camera cameraUI;

    public PlayerController PlayerScript;
    public HelperController HelperScript;
    public FollowCamera CameraScript;

    public float WaitCount = 0.0f; // 코루틴 대기 시간

    public Transform PlayerTr;
    public Transform HelperDollTr;

    public Vector3 TutorialDist; // NPC와 캐릭터 거리

    //public Animation anim;
    public GameObject animObj; // 컷신
    public GameObject HPbar; // 체력바

    void Awake()
    {
        

        //CharacterTalkBackground.SetActive(false);
        //HelperTalkBackground.SetActive(false);

    }
    void Start()
    {
        HelperText.enabled = false;
        PlayerText.enabled = false;

        eventScripts = new List<EventScript>();
        eventScripts = XMLParsing.Instance.EventScriptsLoad(1);
        if (eventScripts == null) { Debug.Log("아 안되잖아;"); }
        //string eventdesa = eventScripts[0].script;

        PlayerScript = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerController>();
        HelperScript = GameObject.FindGameObjectWithTag("HELPER").GetComponent<HelperController>();
        CameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();

        PlayerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        HelperDollTr = GameObject.FindGameObjectWithTag("HELPER").GetComponent<Transform>();

        
    }

    void Update()
    {
        Debug.Log("WaitCount" + WaitCount);
        //BlackBlur.transform.localScale = new Vector3(Screen.width, Screen.height * 0.5f, -1);
        
        if (!isTutorial)
        {
            TutorialCheck();
        }
        TutorialDist = HelperDollTr.transform.position - PlayerTr.transform.position; // NPC와 캐릭터 거리
    }

    void TutorialCheck()
    {
        if(tutorialCnt.Equals(1))
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
            Debug.Log("튜토리얼이 3번째래"); 
            StartCoroutine(Tutorial3());
            tutorialCnt = 0;
        }
        
    }

    IEnumerator Tutorial1()
    {

        PlayerScript.isAct = true;
        //anim = GetComponent<Animation>();
        //anim.Play(anim.clip.name);
        HPbar.SetActive(false);

        yield return new WaitForSeconds(10.0f);
        animObj.SetActive(false);
        HPbar.SetActive(true);

        HelperScript.SetAnimation("STAY", true, 1.0f);
       
        for (float i = 1f; i >= 0; i -= WalkTime)// 단상에서 일어난다.
        {
            PlayerScript.SetAnimation("DROP", true, 1.0f); // 단상에서 일어나는 애니메이션
            yield return 0;
        }
        PlayerScript.Player.skeleton.flipX = true;

        for (float i = 1f; i >= 0; i -= WalkTime)// 단상에서 걸어간다.
        {
            PlayerScript.SetAnimation("WALK", true, 1.0f);
            PlayerScript.transform.Translate((Vector2.right * WalkTime * 3.5f));
            yield return 0;
        }

        PlayerScript.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있음
        yield return new WaitForSeconds(0.5f);
        HelperScript.Helper.skeleton.flipX =true;
        yield return new WaitForSeconds(0.5f);

        for (float i = 1f; i >= 0; i -= WalkTime) //헬퍼가 캐릭터 쪽으로 걸어온다.
        {

            HelperScript.SetAnimation("WALK", true, 1.0f); // 플레이어를 향해 다가오는 헬퍼
            HelperScript.transform.Translate(-(Vector2.right * WalkTime));
            yield return 0;
        }
        
        for(int i = 0; i< 10; i++)//긴 대화를 나눈다.
        {
            HelperScript.SetAnimation("STAY", true, 1.0f);
            TutorialTextPrint(i);
            WaitCheck(i);
            if(i==9) 
            {
                HelperScript.Helper.Skeleton.flipX = false;
            }
            yield return new WaitForSeconds(WaitCount);
        }

        HelperText.enabled = false; 
        CameraScript.isTargetPlayer = false; // 카메라가 헬퍼를 비춘다.

        for (float i = 1f; i >= 0; i -= WalkTime-0.005f)// 헬퍼가 책 위로 올라간다.
        {
            
            HelperScript.transform.Translate((Vector2.right * WalkTime * 7.5f));
            if(i < 0.6f && i>0.55f)
            {
                HelperScript.SetAnimation("JUMP", true, 1.0f);
                HelperScript.isJump = true;
            }
            else
            {
                HelperScript.SetAnimation("WALK", true, 1.0f);
                HelperScript.isJump = false;
            }
            yield return 0;
        }

        HelperScript.SetAnimation("STAY", true, 1.0f);
        HelperScript.Helper.skeleton.flipX = true;

        Debug.Log("이리와, 넌 아무것도 입지 않았잖아 내가 옷을 만들어 줄께");
        TutorialTextPrint(10); //이리와, 넌 아무것도 입지 않았잖아 내가 옷을 만들어 줄게
        WaitCheck(10);
        yield return new WaitForSeconds(WaitCount);


        CameraScript.isTargetPlayer = true; // 카메라가 플레이어를 비춘다.
        yield return new WaitForSeconds(0.1f);

        Debug.Log("옷을 만들줄 알아?");
        TutorialTextPrint(11); //옷을 만들 줄 알아?
        WaitCheck(11);
        yield return new WaitForSeconds(WaitCount);

        CameraScript.isTargetPlayer = false; // 카메라가 헬퍼를 비춘다.
        yield return new WaitForSeconds(0.1f);

        Debug.Log("아주 약간이야");
        TutorialTextPrint(12);
        WaitCheck(12);
        yield return new WaitForSeconds(WaitCount);

        HelperScript.SetAnimation("STAY", true, 1.0f);

        HelperText.enabled = false;
        CameraScript.isTargetPlayer = true; // 카메라가 플레이어를 비춘다.
        PlayerScript.isAct = false;
        tutorialCnt = 2;
        yield return 0;

        
    }
    IEnumerator Tutorial2()
    {
        Debug.Log("튜토리얼 2번쨰 시작!");
        PlayerScript.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있음
        PlayerScript.isAct = true;
        yield return new WaitForSeconds(1.0f);
        CameraScript.isTargetPlayer = false;
        HelperScript.Helper.skeleton.flipX = false;

        for (float i = 1f; i >= 0; i -= WalkTime )// 헬퍼가 책 위로 올라간다.
        {
            if(!HelperScript.isHang)
            { 
                HelperScript.SetAnimation("WALK", true, 1.0f);
            }
           else
            {
                HelperScript.SetAnimation("DROP", true, 1.0f);
            }
            if (!HelperScript.isHangLimit)
            {
                HelperScript.transform.Translate((Vector2.right * WalkTime * 7.0f));
            }

            if (i < 0.7f && i > 0.3f)
            {
                HelperScript.SetAnimation("JUMP", true, 1.0f);
                HelperScript.isJump = true;
            }
            else
            {
                HelperScript.isJump = false;
            }
            yield return 0;
        }
        HelperScript.SetAnimation("DROP", true, 1.0f);
        yield return new WaitForSeconds(1.0f);
        HelperScript.StartCoroutine("CLIMBING");
        yield return new WaitForSeconds(2.0f);
        HelperScript.SetAnimation("STAY", true, 1.0f);
        

        for (float i = 1f; i >= 0; i -= WalkTime - 0.005f)
        {
            HelperScript.SetAnimation("WALK", true, 1.0f);
            HelperScript.transform.Translate((Vector2.right * WalkTime * 2.0f));
            yield return 0;
        }
        HelperScript.SetAnimation("STAY", true, 1.0f);
        HelperScript.Helper.skeleton.flipX = true;

        yield return new WaitForSeconds(1.0f);

        TutorialTextPrint(13);
        WaitCheck(13);
        yield return new WaitForSeconds(WaitCount);

        HelperText.enabled = false;
        CameraScript.isTargetPlayer = true;
        PlayerScript.isAct = false;
        tutorialCnt = 3;
        yield return 0;


    }
    //튜토리얼3========================================================
    IEnumerator Tutorial3()
    {
        PlayerScript.SetAnimation("STAY", true, 1.0f); // 헬퍼를 바라보며 가만히 있음
        PlayerScript.isAct = true;
        yield return new WaitForSeconds(1.0f);
        CameraScript.isTargetPlayer = false;
        HelperScript.Helper.skeleton.flipX = false;

        for (float i = 1f; i >= 0; i -= 0.003f) //헬퍼가 캐릭터 쪽으로 걸어온다.
        {
            HelperScript.SetAnimation("WALK", true, 1.0f); // 플레이어를 향해 다가오는 헬퍼
            HelperScript.transform.Translate((Vector2.right * WalkTime * 7.5f));
            yield return 0;
        }

        HelperScript.Helper.skeleton.flipX = true;
        HelperScript.SetAnimation("STAY", true, 1.0f); // 플레이어를 향해 다가오는 헬퍼
        yield return new WaitForSeconds(1.0f);

        TutorialTextPrint(14);
        WaitCheck(14);
        yield return new WaitForSeconds(WaitCount);

        HelperText.enabled = false;
        CameraScript.isTargetPlayer = true;
        PlayerScript.isAct = false;

        yield return 0;
    }
    //===================================================================

    public void TutorialTextPrint(int TextIndex)
    {
        PlayerScreenPos = Camera.main.WorldToScreenPoint(PlayerTr.transform.position);
        PlayerText.transform.position = cameraUI.ScreenToWorldPoint(new Vector2(PlayerScreenPos.x-50.0f, PlayerScreenPos.y + 160.0f)); // 텍스트의 위치를 플레이어 위에
        HelperScreenPos = Camera.main.WorldToScreenPoint(HelperDollTr.transform.position);
        HelperText.transform.position = cameraUI.ScreenToWorldPoint(new Vector2(HelperScreenPos.x+120.0f, HelperScreenPos.y + 160.0f));// 텍스트의 위치를 헬퍼 위에

        if (eventScripts[TextIndex].target == "Player") // 스크립트 대사의 타겟이 플레이어라면
        {
            //PlayerScript.SetAnimation("DROP", true, 1.0f); // 플레이어가 말하는 애니메이션
            HelperScript.SetAnimation("STAY", true, 1.0f);

            HelperText.enabled = false;
            PlayerText.enabled = true;

            PlayerText.text = eventScripts[TextIndex].script; // 해당 스크립트
            PlayerWriteEffect.ResetToBeginning(); // 라이팅 출력
        }
        else
        {
            PlayerScript.SetAnimation("STAY", true, 1.0f);
            //HelperScript.SetAnimation("DROP", true, 1.0f);

            HelperText.enabled = true;
            PlayerText.enabled = false;

            HelperText.text = eventScripts[TextIndex].script;
            helperWriteEffect.ResetToBeginning();
        }
    }
    public float WaitCheck(int TextIndex)
    {
        WaitCount = 0;
        int i = eventScripts[TextIndex].script.Length;
        if (i > 10 && i < 15)
        {
            WaitCount = 2.8f;
        }
        else if (i > 14 && i < 25)
        {
            WaitCount = 3.3f;
        }
        else if (i > 24 && i < 35)
        {
            WaitCount = 3.8f;
        }
        else
        {
            WaitCount = 4.3f;
        }
        return WaitCount;
    }
}

/*
튜토리얼 기획서
오렌지빌
튜토리얼 전체 진행

2.	스크립트 진행 후, 안내 인형은 천 조각들이 가득한 곳으로 먼저 이동한다. 캐릭터는 안내 인형이 이동하는 것을 보며 뒤 따라 이동한다.
(책 1권 점프로 넘기 > 책3권 쌓인 것 매달려서 넘기 > 책 아래로 기어가기)
3.	캐릭터가 천 조각이 가득한 곳에 도착하면 안내 인형과 대화 스크립트 진행 후, 캐릭터에게 옷을 입혀준다.
4.	그 후, 거미가 화면 밖에서 나타난다. (바깥에 있던 거미를 카메라가 비춘다.)
카메라가 거미를 비춘 후, 대화 스크립트 진행 후, 안내 인형을 납치해간다.
5.	캐릭터는 자신의 옆에 있던 바늘을 뽑아 뒤쫓아간다. (바늘 무기로 지급, 앞으로 이동)
6.	앞에 책 더미 아래, 하늘에 달려 있는 비행기 모빌 아래에 안내 인형을 실로 묶고 있는 거미거 있는 것을 보여준다.
7.	캐릭터는 책 더미를 오르고 비행기 모빌 위로 매달려서 오른 후, 모빌을 지탱해주던 실을 공격으로 자른다. 비행기 모빌이 거미 위로 떨어진다.
8.	충격으로 인해 정신을 잃은 거미, 그 사이 안내 인형은 실에 묶여있는 상태로 거미 약간 바깥쪽으로 구른다.
9.	(첫 번 째 전투) 화면 상단에 안내 인형을 묶은 실의 HP 바가 나타난다.
캐릭터는 바늘 공격으로 실에 묶인 안내 인형을 공격하면 HP 바가 오른쪽에서 왼쪽으로 줄어든다. 정신차린 거미는 캐릭터가 안내 인형을 풀어주려고 다가올 때 마다 앞 다리로 공격한다. (앞 다리에 공격 당하면 데미지를 입음. 거미는 캐릭터의 공격에 대미지를 입지 않음.)
10.	(전투 성공 조건: 소지 체력이 0이 되지 않고 안내 인형을 묶은 실의 체력을 0으로 만듬.)
(전투 패배 조건: 거미의 공격에 캐릭터의 소지 체력이 0이 됨.) (패배 시 비행기 모빌에 오르기 전에서 재 시작)
11.	전투 성공 시, 안내 인형이 풀려나는 모습을 보여준다. 그 후 거미가 모빌에서 빠져 나오는 모습을 보고 캐릭터가 안내 인형을 업는 모습을 보여줌.
12.	(도주 전 시작) 캐릭터는 안내 인형을 업은 상태로 앞으로 이동한다. 뒤에선 거미가 쫓아온다.
13.	(도주전 성공 조건: 소지 체력이 0이 되지 않은 상태로 커피씨앗 더미 앞(로프 액션 가능 범위)에서 로프 액션 성공하여 커피씨앗 더미 반대 쪽으로 이동 시)
(도주전 실패 조건: 도주 도중 거미와 2번 이상 접촉 시)(패배 시 도주전 처음부터 재 진행)
14.	도주전 종료 후, 카메라가 거미는 커피 씨앗 더미를 건너지 못한 것을 보여주고 거미가 보이지 않게 카메라 이동 한다.
그 후, 캐릭터는 안내 인형을 내려주고 대화 스크립트를 진행한다. 스크립트 진행 후, 튜토리얼 종료.

튜토리얼 게임 속 요소 정리
체력 UI
현재 캐릭터의 체력을 보여주는 UI이다. 거미의 공격에 당할 때 마다 일정량씩 소모된다.
(체력 UI 미정)
단상
캐릭터가 깨어나는 곳. 위에 올라 갈 수 있는 등 게임 오브젝트 취급.
단상 위에서 내려오는 컷 씬 후
시작 컷 씬 이후, 안내 인형과의 대화 스크립트가 진행된 후,
캐릭터가 플레이어의 조종을 받는 상태가 된다.
안내 인형(대화 스크립트 진행 후 선행 이동)
게임 시작 전부터 이미 정해진 위치(단상 앞)에 존재하고 있는 NPC.
‘위의 단상 위에서 내려오는 컷 씬 후’ 이후에 앞의 이곳 까지 선행 이동(인게임 상의 애니메이션으로)한다.
(단, 캐릭터가 안내인형을 앞지르는 상황을 만들지 않게 하기 위해 안내인형의 속도는 캐릭터보다 빠르게 한다.)
게임 내 책
일종의 발판이다. 플레이어가 통과하여 지나갈 수 없는 오브젝트.
단, 캐릭터는 책 위에 점프로 올라 설 수 있다. 약간 높은 곳(맵 기획서 참고)은 매달리기로 올라갈 수 있다.
매달리기 가능한 범위
캐릭터가 해당 부분과 겹쳐진 상태에서 ‘매달리기’ 시전 시 캐릭터는 매달리기 애니메이션을 취하며 해당 책(또는 비행기 모빌) 위로 올라간다.
이동해서 착지
아래로 착지.
책 5권
매달리기가 불가능해 위로 못 올라가게 되어 있는 오브젝트.
기어가기로 통과 가능
기어가기 액션으로 통과 가능.(기어가기 모션)
(기어가기 버튼 미정)
이곳
도착 시 ‘옷 입혀주기 애니 + 스크립트’ 진행된다.
그 후, ‘거미 등장해(카메라 이동) 스크립트 진행 후,
‘안내 인형 잡아서 도망가는 모습’을 보여준다.
안내 인형 잡고 있는 거미
‘안내 인형을 휘감고 있는 모습’을 취하고 있는 거미의 모습이다.
(레벨디자인 문서에 나온 대로 비행기에 맞지 않고 캐릭터가 다른 곳으로 착지했을 경우, 캐릭터를 접촉하여 사망시킨다.)
비행기 모빌(과 이어져 있는 선)
책과 같이 캐릭터가 위에 올라갈 수 있는 오브젝트이다. 점프로는 올라갈 수 없는 위치에 있어 매달리기로 올라가야 한다.
올라간 후, 캐릭터가 공격으로 ‘이어져 있는 선’을 맞췄을 경우,
‘비행기는 떨어지고’
아래에 ‘안내 인형 잡고 있는 거미’가 ‘비행기에 붙잡힌 상태의 거미’가 된다.
붙잡혀 있던 안내 인형은 ‘거미와 분리된 상태’ 가 된다.
비행기에 붙잡힌 상태의 거미
비행기에 붙잡힌 상태의 거미이다.
앞 다리 파츠에 공격 판정이 들어가 있다. 해당 파츠의 공격에 캐릭터가 당할 경우, 체력이 줄어든다. (공격 타이밍은 캐릭터가 다가왔을 시마다 공격)
거미가 해당 상태가 될 경우, 화면 상단에 ‘체력 바’가 나타난다.
캐릭터의 공격을 받을 수 있는 상태이지만 공격 받아도 아무 일도 일어나지 않는다.
거미와 분리된 상태의 안내 인형
거미줄에 둘러 쌓인 안내 인형이 캐릭터의 공격을 받을 수 있는 상태가 되었다.
캐릭터의 공격에 당할 경우, 상단의 체력 바가 줄어든다.(붉은 바가 오른쪽에서 왼쪽으로)
체력이 0이 되면 안내 인형은 ‘풀려나는 애니메이션’을 취한 후, 대화 스크립트 진행 후,
‘캐릭터의 등에 업히는 애니메이션’을 취한다. 그 후 캐릭터는 ‘안내 인형을 업은 모습’이 된다.
도망치는 길(도주전)
거미와 도주전 시작. 캐릭터는 플레이어의 동작에 따라 움직인다.
캐릭터는 거미와 부딪힐 경우, 데미지를 입게 된다.
캐릭터는 플레이어의 조종을 받게 되어 움직이는 상태가 된다.
오른쪽으로 이동하다가 ‘로프 액션 가능 범위’ 에서
플레이어가 ‘로프 액션 버튼’ 입력 시 도주전은 끝나게 된다. (그 외의 곳에서 로프 액션 입력 시 아무 일도 일어나지 않는다.)
그 후, 안내 인형을 업은 상태로 건너편으로 이동하게 된다.
안내 인형을 내려주는 모션 취하며 대화 스크립트가 진행 된 후, 튜토리얼이 종료된다.

*/
