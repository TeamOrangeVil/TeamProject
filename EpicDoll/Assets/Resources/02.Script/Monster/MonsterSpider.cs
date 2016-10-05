using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public class MonsterSpider : Monster
{
    public enum MonsterState { IDLE, TRACE, ATK, RAGE, BEATEN, JUMP, DIE  }; //상태 선언
    MonsterState monsterState = MonsterState.IDLE;             //기본상태로 초기화

    public GameObject rayCastLag;                           //레이케스트 위치 동적변경을 위해 선언
    public GameObject rayCastBody;                          //레이케스트 위치 동적변경을 위해 선언
    public Transform bodyTr;                                //몬스터 몸통 회전 기준점
    Vector3 beforeBodyTr;
    Vector3 beforeBodyRt;
    Vector3 slofVector;                                     //오브젝트에 설정되있는 오름 각
    Vector3 objChack;                                       //오브젝트를 체크할 방향을 설정한 벡터
    Vector3 goHead;                                         //진행 방향

    public string curObj;                                   //이전 오브젝트
    public float rotSpeed;                                  //회전 속도
    public float objHeight;                                 //몸 올라갈 높이
    public float slofangle;                                 //각

    public bool slofUp = false;                             //오름 내림 상태 판정
    public bool slofDown = false;                           //
    public bool jump = false;                           //점프 패턴
    bool tempDelay = false;                                 //동작간 임시 딜레이 값

    float rayLenght = 2.0f;                                 //레이케스트 길이

    void Start()
    {
        //플레이어 위치 가져옴
        monsterAnimation = GetComponent<SkeletonAnimation>();
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        bodyTr = GameObject.Find("body [Override]").GetComponent<Transform>();
        beforeBodyTr = bodyTr.transform.position;
        objChack = transform.TransformDirection(Vector3.down);
        //mobRig = GetComponent<Rigidbody>();
        //boxColl = GetComponent<BoxCollider>();

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
            /*switch (EventStep)
            {
                case 0:
                    if (attackDist >= dist)
                    {
                        monsterState = MonsterState.ATK;
                    }
                    else
                    {
                        monsterState = MonsterState.IDLE;
                    }
                    break;
                case 1:*/
                    if (attackDist >= dist)
                    {
                        monsterState = MonsterState.ATK;
                    }
                    /*else if (monsterTr.position.x > 774f)
                    {
                        monsterState = MonsterState.RAGE;
                        StopCoroutine("MonsterStateCheck");     //몬스터 쫒아오는 이벤트 종료
                    }*/
                    else if (dist > 0)
                    {
                        monsterState = MonsterState.TRACE;
                    }
                    //break;
            //}
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
                    SetAnimation("ATTACK", false, 1.0f);
                    SoundEffectManager.Instance.SoundDelay("SpiderAtk", 0);
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
                        bodyTr.Translate(Vector3.up * 5f * Time.deltaTime);
                    }
                    else if (slofDown)
                    {
                        bodyTr.Translate(Vector3.down * 5f * Time.deltaTime);
                    }
                    /* 이전 등반 애니메이션 따로 있을 때
                    if (slofUp)//올라가야 할 때
                    {//살짝 느려짐
                        boxColl.center = Vector3.up * 0.8f;
                        boxColl.enabled = false;
                        mobRig.useGravity = false;
                        monsterTr.Translate(goHead * moveSpeed * 1.75f * Time.deltaTime);
                    }//평지
                    else if(!slofUp && !slofDown && !jumpDown)
                    {
                        boxColl.enabled = true;
                        //if (monsterTr.position.y < -0.4f || (monsterTr.position.y > -0.4f && monsterTr.position.y < 1.0f)) { boxColl.center = Vector3.up * 0.115f; mobRig.useGravity = true; }
                        //else { boxColl.center = Vector3.up * 0.8f; mobRig.useGravity = false;}
                        if (monsterTr.rotation.z < 0.0001f && monsterTr.rotation.z > -0.0001f) { mobRig.isKinematic = false; }
                        else { monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, Quaternion.identity, Time.deltaTime * moveSpeed * 0.75f); }
                        monsterTr.Translate(goHead * moveSpeed * Time.deltaTime);
                    }
                    //if (jumpDown) { SetAnimation("jump", false, 1f); }
                    if (slofUp) { SetAnimation("CLIMB", false, 0.775f); }
                    else { SetAnimation("WALK", true, 1.0f); }*/
                    else {  }
                    monsterTr.Translate(goHead * moveSpeed * Time.deltaTime);
                    SetAnimation("WALK", true, 1.0f);
                    break;
            }
            yield return null;
        }
    }
    //상황별 동적 레이케스트 발사
    IEnumerator RayShot()
    {
        Ray ray;
        RaycastHit hit;
        hit = new RaycastHit();
        while (!isDie)
        {
            //평소
            if (!slofUp && !slofDown && !jump)
            {
                Debug.Log("노말 레이케스트");
                rayLenght = 1f;
                ray = new Ray(rayCastLag.transform.position, objChack);
                if (Physics.Raycast(ray, out hit, rayLenght, 1 << 12))
                    Debug.DrawRay(rayCastLag.transform.position, objChack * rayLenght, Color.blue, 0.1f);
                else { Debug.DrawRay(rayCastLag.transform.position, objChack * rayLenght, Color.red, 0.1f); }
            }
            else
            {
                if (slofUp)
                {
                    Debug.Log("업 레이케스트");
                    rayLenght = 3f;
                    ray = new Ray(rayCastLag.transform.position, objChack);
                    if (Physics.Raycast(ray, out hit, rayLenght, 1 << 7))
                        Debug.DrawRay(rayCastLag.transform.position, objChack * rayLenght, Color.blue, 0.1f);
                }
                else if (slofDown)
                {
                    Debug.Log("다운 레이케스트");
                    rayLenght = 3f;
                    ray = new Ray(rayCastLag.transform.position, objChack);
                    if (Physics.Raycast(ray, out hit, rayLenght, 1 << 12))
                        Debug.DrawRay(rayCastLag.transform.position, objChack * rayLenght, Color.blue, 0.1f);
                }
            }
            //바닥이 무슨 바닥인지 판정 후 행동 결정
            if (hit.collider != null)
            {
                //up
                if (hit.collider.CompareTag("CLIMBFLOOR") && hit.collider.tag != curObj)
                {
                    curObj = hit.collider.name;
                    Debug.Log("오르막");
                    SetUp(hit.transform.localScale.y);
                }
                else if (hit.collider.CompareTag("FLOOR") && hit.collider.tag != curObj)
                {
                    Debug.Log("바닥");
                    curObj = hit.collider.tag;
                    SetDown(hit.transform.localScale.y);
                }/*
                else if (hit.collider.name.Contains("MobJump") && hit.collider.name != curObj)
                {
                    curObj = hit.collider.name;
                    Debug.Log("점프");
                    SetJump();
                    hit.collider.gameObject.SetActive(false);
                }*///평지
            }
            else
            {
                SetGround();
            }
            yield return null;
        }
    }
    public void SetUp(float height)
    {
        slofUp = true;
        objHeight = height;
        slofangle = height * 0.6f;
    }
    public void SetDown(float height)
    {
        slofDown = true;
        objHeight = 0;
        slofangle = height * -0.6f;
    }
    public void SetGround()//회전, 점프력 초기화
    {
        slofUp = false;
        slofDown = false;
        jump = false;
        slofangle = 0f;
        slofVector = Vector3.zero;
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