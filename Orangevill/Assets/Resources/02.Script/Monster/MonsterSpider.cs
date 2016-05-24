using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public class MonsterSpider : Monster
{
    public enum MonsterState { IDLE, TRACE, ATK, BEATEN, DIE, RAGE }; //상태 선언
    public MonsterState monsterState = MonsterState.IDLE;//기본상태로 초기화

    public Rigidbody2D mobRig2d;
    BoxCollider2D boxColl;
    //public GameObject objChecker;
    Vector2 goHead;//방향
    Vector2 objChack;//오브젝트를 체크할 방향벡터

    public string curObj;
    public bool slofUp = false;
    public bool slofDown = false;
    public int EventStep = 0;
    public float moveSpeed;//이동 속도
    public float rotSpeed;//회전 속도
    public float slofangle;//각

    void Start()
    {
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();//플레이어 위치 가져옴
        monsterTr = GetComponent<Transform>();
        mobRig2d = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
        objChack = Vector2.down;
        if (transform.position.x > playerTr.position.x)
        {
            monsterAnimation.skeleton.flipX = false;
            goHead = Vector2.left;
        }
        else if (transform.position.x < playerTr.position.x)
        {
            monsterAnimation.skeleton.flipX = true;
            goHead = Vector2.right;
        }
        monsterAnimation.skeleton.flipX = true;
        monsterAnimation.skeleton.SetAttachment("1", null);//거미 + 비행기모빌에서 비행기 제거

        SetAnimation("WALK", true, 1.0f);
        mobRig2d.isKinematic = true;
        StartCoroutine(MonsterStateCheck());
        StartCoroutine(MonsterAction());

    }
    public override IEnumerator MonsterStateCheck()
    {
        while (!isDie)
        {
            //상태 체크
            dist = Mathf.Abs(monsterTr.position.x - playerTr.position.x);
            switch (EventStep)
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
                case 1:
                    if (attackDist >= dist)
                    {
                        monsterState = MonsterState.ATK;
                    }
                    else if (dist > 0)
                    {
                        monsterState = MonsterState.TRACE;
                    }
                    else if (dist > 3)
                    {
                        monsterState = MonsterState.RAGE;
                    }
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
            if (transform.position.x > playerTr.position.x)
            {
                goHead = Vector2.left;
                monsterAnimation.skeleton.flipX = false;
            }
            else if (transform.position.x < playerTr.position.x)
            {
                goHead = Vector2.right;
                monsterAnimation.skeleton.flipX = true;
            }
            //행동
            switch (monsterState)
            {
                case MonsterState.IDLE:
                    //SetAnimation("WRAPING", false, 1.0f);
                    yield return new WaitForSeconds(1f);
                    break;
                case MonsterState.ATK:
                    //공격 애니 실행 해
                    Debug.Log("호-우");
                    SetAnimation("ATTACK", false, 1.0f);
                    yield return new WaitForSeconds(aniTime);
                    monsterState = MonsterState.IDLE;
                    break;
                case MonsterState.BEATEN:
                    break;
                case MonsterState.TRACE:
                    SetAnimation("WALK", true, 1.0f);
                    monsterTr.Translate(goHead * moveSpeed * Time.deltaTime);
                    if (slofUp)
                    {
                        boxColl.enabled = false;
                        if (slofangle > monsterTr.rotation.z)
                        {
                            monsterTr.Rotate(0, 0, rotSpeed * Time.deltaTime);
                        }
                    }
                    else if (slofDown)
                    {
                        boxColl.enabled = false;
                        if (slofangle < monsterTr.rotation.z)
                        {
                            monsterTr.Rotate(0, 0, -rotSpeed * Time.deltaTime);
                        }
                    }
                    else if (!slofUp && !slofDown)
                    {
                        boxColl.enabled = true;
                        monsterTr.rotation = Quaternion.Slerp(monsterTr.localRotation, Quaternion.identity, Time.deltaTime * 3.25f);
                        //mobRig2d.isKinematic = false;
                    }
                    break;
            }
            yield return null;
        }
    }

    public void SetUp(float angle)
    {
        slofUp = true;
        slofangle = angle * 0.6f;
    }
    public void SetDown(float angle)
    {
        slofDown = true;
        slofangle = angle * 0.6f;
    }
    public void SetGround()
    {
        slofUp = false;
        slofDown = false;
        slofangle = 0f;
    }
    //게임 이벤트 매니져가 몬스터 작동 실행 할 수 있도록 하는 함수
    public void MonsterActivate()
    {
        StartCoroutine(MonsterStateCheck());
        StartCoroutine(MonsterAction());
    }
    void FixedUpdate()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position + (Vector3.right * 0.35f), objChack, 5f);
        Debug.DrawRay(transform.position + (Vector3.right * 0.35f), objChack, Color.cyan, 0.5f);
        //바닥이 무슨 바닥인지 판정 후 행동 결정
        if (hit.collider != null)
        {
            if (hit.collider.name.Contains("UpBook") && hit.collider.name != curObj)
            {
                curObj = hit.collider.name;
                Debug.Log("오르막");
                SetUp(hit.transform.rotation.z);
            }
            else if (hit.collider.name.Contains("DownBook") && hit.collider.name != curObj)
            {
                curObj = hit.collider.name;
                Debug.Log("내리막");
                SetDown(hit.transform.rotation.z);
            }
            else if ((!hit.collider.name.Contains("DownBook") && !hit.collider.name.Contains("UpBook") && hit.collider.tag != curObj)
               && (hit.collider.CompareTag("CLIMBFLOOR") || hit.collider.CompareTag("FLOOR")))
            {
                curObj = hit.collider.tag;
                Debug.Log("평지");
                SetGround();
            }
        }
    }
}