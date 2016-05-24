using UnityEngine;
using System.Collections;

public class MonsterKnight : Monster
{
    enum MonsterState { IDLE, TRACE, ATK, GUARD, BEATEN, DIE }; //상태 선언
    MonsterState monsterState = MonsterState.IDLE;//기본상태로 초기화

    public int atkFreQuency;
    public float rushDist;
    public float siuuuuuDist;//고함을 지르는 거리
    void Start()
    {
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();//플레이어 위치 가져옴
        monsterTr = GetComponent<Transform>();
        StartCoroutine(MonsterStateCheck());
        StartCoroutine(MonsterAction());
    }
    public override IEnumerator MonsterStateCheck()
    {
        while (!isDie)
        {
            dist = Mathf.Abs(monsterTr.position.x - playerTr.position.x);
            int rand = Random.Range(0, 11);
            if (attackDist >= dist)
            {
                if (atkFreQuency >= rand)
                {
                    monsterState = MonsterState.ATK;
                }
                else
                {
                    monsterState = MonsterState.GUARD;
                }
            }
            else if (dist > 0)
            {
                monsterState = MonsterState.TRACE;
            }
            else
            {
                monsterState = MonsterState.IDLE;
            }

            yield return new WaitForSeconds(0.2f);
        }
        monsterState = MonsterState.DIE;
    }
    public override IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            int temp;
            switch (monsterState)
            {
                case MonsterState.IDLE:
                    //SetAnimation("WRAPING", false, 1.0f);
                    break;
                case MonsterState.TRACE:
                    break;
                case MonsterState.ATK:
                    temp = Random.Range(0, 11);
                    if(monsterStat.hp <= monsterStat.maxHp * 0.2f)//신념 찌르기
                    {
                        SetAnimation("spacial", false, 1.0f);
                        yield return new WaitForSeconds(aniTime);
                        break;
                    }
                    else if (siuuuuuDist >= dist)//호-우 !
                    {
                        SetAnimation("SHOUT", false, 1.0f);
                        yield return new WaitForSeconds(aniTime);
                        break;
                    }
                    else if (rushDist >= dist)//돌진
                    {
                        SetAnimation("RUSH", false, 1.0f);
                        yield return new WaitForSeconds(aniTime);
                        break;
                    }
                    else if (temp < 8)//걍 공격
                    {
                        SetAnimation("ATK", false, 1.0f);
                        yield return new WaitForSeconds(aniTime);
                        break;
                    }
                    else
                    {
                        SetAnimation("ATK2", false, 1.0f);//내리치기 !
                        yield return new WaitForSeconds(aniTime);
                        break;
                    }
                case MonsterState.GUARD:
                    temp = Random.Range(0, 2);
                    SetAnimation("GUARD", false, 1.0f);
                    yield return new WaitForSeconds(aniTime);
                    monsterState = temp == 0 ? MonsterState.ATK : MonsterState.IDLE;
                    break;
                case MonsterState.DIE:
                    SetAnimation("DIE", false, 1.0f);
                    yield return new WaitForSeconds(aniTime);
                    StopCoroutine(MonsterAction());
                    StopCoroutine(MonsterStateCheck());
                    break;
            }
            yield return null;
        }
    }
}