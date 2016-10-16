using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public class MonsterSpider : Monster
{
    public enum MonsterState { IDLE, TRACE, ATK, RAGE, BEATEN, JUMP, DIE  }; //상태 선언
    MonsterState monsterState = MonsterState.IDLE;             //기본상태로 초기화

    public GameObject rayCastFront;                         //레이케스트 위치 동적변경을 위해 선언
    public GameObject rayCastRear;                          //레이케스트 위치 동적변경을 위해 선언
    GameObject rayCastPos;                                  //레이케스트 위치
    public Transform bodyTr;                                //몬스터 몸통 회전 기준점
    Vector3 objChack;                                       //오브젝트를 체크할 방향을 설정한 벡터
    Vector3 goHead;                                         //진행 방향
    RaycastHit2D hit;
   
    public string curObj;                                   //이전 오브젝트
    public string curTag;
    public float rotSpeed;                                  //회전 속도
    public float rotAngle;                                  //회전 각
    public float objHeight;                                 //몸 올라갈 높이
    int eventStep = 0;

    public bool slofUp = false;                             //오름 내림 상태 판정
    public bool slofDown = false;                           //
    public bool jump = false;                               //점프 패턴
    bool moveDelay = false;                                 //동작간 임시 딜레이 값

    float rayLenght = 2.0f;                                 //레이케스트 길이

    void Start()
    {
        //플레이어 위치 가져옴
        monsterAnimation = GetComponent<SkeletonAnimation>();
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        bodyTr = GameObject.Find("body [Override]").GetComponent<Transform>();
        rayCastPos = rayCastFront;
        objChack = transform.TransformDirection(Vector3.down);
        hit = new RaycastHit2D();
        //케릭터가 있는 방향을 쳐다보도록
        if (monsterTr.position.x > playerTr.position.x)
        {
            monsterAnimation.skeleton.FlipX = false;
            goHead = Vector3.left;
        }
        else if (monsterTr.position.x < playerTr.position.x)
        {
            monsterAnimation.skeleton.FlipX = true;
            goHead = Vector3.right;
        }
        monsterAnimation.skeleton.FlipX = true;
        SetAnimation("WALK", true, 1.0f);
        
        MonsterActivate();

    }
    public override IEnumerator MonsterStateCheck()
    {
        while (!isDie)
        {
            //상태 체크를 위한 거리 측정
            dist = Mathf.Abs(monsterTr.position.x - playerTr.position.x);
            switch (eventStep)
            {
                case 0:
                    if (attackDist >= dist)
                    {
                        monsterState = MonsterState.ATK;
                    }
                    else if (dist > 0)
                    {
                        monsterState = MonsterState.TRACE;
                    }
                    break;
                case 1:
                    monsterState = MonsterState.DIE;
                    break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    public override IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            //플레이어 위치 체크
            if (monsterTr.position.x > playerTr.position.x)
            {
                goHead = Vector3.left;
                monsterAnimation.skeleton.FlipX = false;
            }
            else if (monsterTr.position.x < playerTr.position.x)
            {
                goHead = Vector3.right;
                monsterAnimation.skeleton.FlipX = true;
            }
            //행동
            switch (monsterState)
            {
                case MonsterState.IDLE:
                    break;
                case MonsterState.ATK:
                    //공격 애니 실행
                    Debug.Log("공격");
                    SetAnimation("ATTACK", true, 1.0f);
                    //SoundEffectManager.Instance.SoundDelay("SpiderAtk", 0);
                    yield return new WaitForSeconds(aniTime * 0.5f);
                    if(dist < 0.25f) { GameManager.Instance.PlayerDamaged(100); }
                    yield return new WaitForSeconds(aniTime * 0.5f);
                    monsterState = MonsterState.IDLE;
                    break;
                case MonsterState.RAGE:
                    SetAnimation("RAGE", false, 1.0f);
                    yield return new WaitForSeconds(aniTime);
                    break;
                case MonsterState.TRACE:
                    //올라가야 할 때
                    if (slofUp)
                    {
                        bodyTr.transform.localPosition = new Vector3(1.670014f, bodyTr.transform.localPosition.y, bodyTr.transform.localPosition.z);
                        bodyTr.Translate(Vector3.up * moveSpeed * 1.5f * Time.deltaTime);
                        if (bodyTr.localRotation.z > rotAngle)
                            bodyTr.Rotate(Vector3.back * rotSpeed * moveSpeed * Time.deltaTime);
                        if(bodyTr.localPosition.y > objHeight)
                            slofUp = false;
                    }
                    else if (slofDown)
                    {
                        bodyTr.transform.localPosition = new Vector3(1.670014f, bodyTr.transform.localPosition.y, bodyTr.transform.localPosition.z);
                        bodyTr.Translate(Vector3.down * moveSpeed * 1.5f * Time.deltaTime);
                        if (bodyTr.localRotation.z < rotAngle)
                            bodyTr.Rotate(Vector3.forward * rotSpeed * moveSpeed * Time.deltaTime);
                        if (bodyTr.localPosition.y < objHeight)
                        {
                            slofDown = false;
                            SetGround();
                        } 
                    }
                    else
                    {
                        bodyTr.rotation = Quaternion.Slerp(bodyTr.rotation, Quaternion.identity, Time.deltaTime * moveSpeed);
                    }
                    monsterTr.Translate(goHead * moveSpeed * Time.deltaTime);
                    SetAnimation("WALK", true, 1.0f);
                    break;
                case MonsterState.DIE:
                    break;
            }
            yield return null;
        }
    }
    //상황별 동적 레이케스트 발사
    IEnumerator RayShot()
    {
        while (!isDie)
        {
            Debug.Log("레이케스트");
            rayLenght = 5f;
            hit = Physics2D.Raycast(rayCastPos.transform.position, objChack, rayLenght, 1 << 12);
            //바닥이 무슨 바닥인지 판정 후 행동 결정
            if (hit.collider != null)
            {
                Debug.DrawRay(rayCastPos.transform.position, objChack * rayLenght, Color.green, 0.5f);
                //up
                if (hit.collider.CompareTag("MOBCLIMB") && hit.collider.tag != curTag)
                {
                    if (!slofUp)
                    {
                        Debug.Log("오름");
                        curObj = hit.collider.name;
                        curTag = hit.collider.tag;
                        SetUp(hit.transform.localScale.y);
                        Debug.DrawRay(rayCastPos.transform.position, objChack * rayLenght, Color.green, 3f);
                    }
                }
                //down
                else if (hit.collider.CompareTag("FLOOR") && hit.collider.tag != curTag)
                {
                    if (!slofDown)
                    {
                        Debug.Log("바닥");
                        curObj = hit.collider.name;
                        curTag = hit.collider.tag;
                        SetDown(hit.transform.localScale.y);
                        Debug.DrawRay(rayCastPos.transform.position, objChack * rayLenght, Color.green, 3f);
                    }
                }
                else if (hit.collider.CompareTag("MOBTRACEEND"))
                {
                    monsterState = MonsterState.DIE;
                    StopCoroutine("RayShot");
                }
            }
            else
            {
                Debug.DrawRay(rayCastPos.transform.position, objChack * rayLenght, Color.red, 0.1f);
                SetGround();
            }
            yield return null;
        }
    }
    public void SetUp(float height)
    {
        slofUp = true;
        slofDown = false;
        objHeight = height * 1f;
        rotAngle = height * -0.75f;
    }
    public void SetDown(float height)
    {
        //rayCastPos = rayCastRear;
        slofUp = false;
        slofDown = true;
        objHeight = height * -0.85f;
        rotAngle = height * 0.75f;
    }
    public void SetGround()//회전, 점프력 초기화
    {
        rayCastPos = rayCastFront;
        slofUp = false;
        slofDown = false;
        jump = false;
        rotAngle = 0f;
    }
    void SetJump()
    {
        jump = true;
    }
    //게임 이벤트 매니져가 몬스터 작동 실행 할 수 있도록 하는 함수
    public void MonsterActivate()
    {
        StartCoroutine(MonsterStateCheck());
        StartCoroutine(MonsterAction());
        StartCoroutine(RayShot());
    }
}