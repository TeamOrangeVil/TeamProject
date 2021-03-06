﻿using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

/// <summary>
/// 몬스터의 기본이 되는 클래스입니다. 
/// 몬스터의 기본기능은 이 클래스를 상속 받아 사용합니다.
/// </summary>
public struct MonsterStat                           // 몬스터 스-텟
{
    public string name;                             // ui 표기 한글 이름(필요?)
    public float hp;                                // 체력
    public float maxHp;                             // 최대 체력
    public int atk;                                 // 공격력
    public float atkSpd;                            // 공속
};
public class Monster : MonoBehaviour {

    public SkeletonAnimation monsterAnimation;      //spine 애니메이션
    Spine.TrackEntry trackEntry;                    //애니메이션 트렉 정보
    Spine.Skin monsterNowSkin;                      //몬스터 현재 스킨
    private string curAnimation = "";               //현재 실행중인 애니메이션
    public float aniTime;                           //현재 애니메이션의 재생에 걸리는 시간 저장
    [SpineSlot]                                     
    public string monsterSpineSlot;                 //이미지 변경할 슬롯
    [SpineSkin]                                     
    public string monsterSpineSkin;                 //이미지 변경에 쓰일 스킨 정보

	int EventStep = 0;						        //이벤트 제어를 위한 변수
    public Transform monsterTr;                     //몹 자신의 위치
    public Transform playerTr;                      //플레이어 위치
    public float traceDist;                         //시야 범위
    public float attackDist;                        //공격 범위
    public float dist;                              //플레이어와 몬스터간의 거리
	public float moveSpeed;							//이동 속도

    public MonsterStat monsterStat;                 //스텟
    public Collision2D atkColl;                     //공격 콜리더
    public Collision2D defColl;                     //쳐맞 콜리더
    public bool isDie = false;                      //몬스터 작동(사망)
    public bool isHit = false;                      //몬스터 피격여부
    public bool isAtk = false;                      //몬스터 공격판정

    // Use this for initialization
    void Start()
    {
        monsterStat = new MonsterStat();
        monsterAnimation = GetComponent<SkeletonAnimation>();
        monsterTr = GetComponent<Transform>();
        trackEntry = new Spine.TrackEntry();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
    }
    public void SetAnimation(string name, bool loop, float speed)//스켈레톤 애니 세팅 이름,루프여부,재생속도
    {
        if (name == curAnimation)
        {
            return;//현재 애니메이션 == 실행 입력 애니메이션 일때 set생략
        }
        else
        {
            //trackEntry.TimeScale = 0.0f;
            monsterAnimation.skeletonDataAsset.Reset();
            monsterAnimation.skeleton.SetToSetupPose();
            monsterAnimation.skeleton.SetSlotsToSetupPose();
            monsterAnimation.state.SetAnimation(0, name, loop).TimeScale = speed;
            aniTime = monsterAnimation.state.GetCurrent(0).EndTime;
            curAnimation = name;
        }
    }
    void SpriteChange()//몬스터의 이미지를 바꿉니다.
    {
        if (monsterAnimation == null)
            monsterAnimation = GetComponent<SkeletonAnimation>();

        Spine.SkeletonData data = monsterAnimation.skeleton.Data;

        if (monsterNowSkin == null)
            monsterNowSkin = new Spine.Skin("Generated Monster Skin");

        //1. 현재 본 데이터에 씌울 스킨을 불러온다?
        int SlotIndex = data.FindSlotIndex(monsterSpineSlot);
        Spine.Skin AttachmentSource = data.FindSkin(monsterSpineSkin);

        //2. 스킨을 적용하고 스킨 이미지를 애니메이션 트렉에 적용한다.
        monsterAnimation.skeleton.SetSkin(monsterNowSkin);
        monsterAnimation.skeleton.SetSlotsToSetupPose();

        Spine.TrackEntry currentTrack = monsterAnimation.state.GetCurrent(0);//재생 애니메이션 정보
        if (currentTrack != null)
        {                               
            currentTrack.Animation.Apply(monsterAnimation.skeleton, 0f, currentTrack.Time, currentTrack.Loop, null);
        }
    }
    //몬스터 행동
    public virtual IEnumerator MonsterAction()
    {
        yield return null;
    }
    //몬스터 상황판단
    public virtual IEnumerator MonsterStateCheck()
    {
        yield return null;
    }
    public int EventStepChange(int inputEventStep)
    {
        return EventStep = inputEventStep;
    }
}
