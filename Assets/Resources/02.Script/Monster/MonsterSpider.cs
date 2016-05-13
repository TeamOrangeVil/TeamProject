using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public class MonsterSpider : Monster
{
    public enum MonsterState { IDLE, TRACE, ATK, BEATEN, DIE, RAGE }; //상태 선언
    public MonsterState monsterState = MonsterState.IDLE;//기본상태로 초기화

    public Rigidbody2D mobRig2d;
    public GameObject objChecker;
    Vector2 goHead;//방향
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
                    else if(dist > 3)
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
                    SetAnimation("WRAPING", false, 1.0f);
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
                        if(slofangle > monsterTr.rotation.z)
                        {
                            monsterTr.Rotate(0, 0, rotSpeed * Time.deltaTime);
                        }
                    }
                    else if(slofDown)
                    {
                        if(slofangle < monsterTr.rotation.z)
                        {
                            monsterTr.Rotate(0, 0, -rotSpeed * Time.deltaTime);
                        }
                    }
                    else if(!slofUp && !slofDown)
                    {
                        monsterTr.rotation = Quaternion.Slerp(monsterTr.localRotation, Quaternion.identity, Time.deltaTime * 3.25f);
                        mobRig2d.isKinematic = true;
                    }
                    break;
            }
            yield return null;
        }
    }
    //외부에서 SendMessage로 실행되는 무브 세팅들
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

}