using UnityEngine;
using System.Collections;

static class Constancts//#define대신 ㅋ
{
    //전처리할 내용을 넣어주세요 -> public const 형태 이름;
    //public const string body01 = "shop_img02";
    //public const string body02 = "stake-";
}
public class Monster : MonoBehaviour
{

    public enum MonsterState { IDLE, TRACE, ATK, BEATEN, DIE }; //상태 선언
    public MonsterState monsterState = MonsterState.IDLE;//기본상태로 초기화

    public SkeletonAnimation monsterAnimation;//spine 애니메이션
    private string curAnimation = "";//현재 실행중인 애니메이션

    public Sprite[] sprite;//이미지 교체를 위해
    [SpineSlot]
    public string slot;//교체될 이미지가들어갈 슬롯
    [SpineSkin]
    public string skin;//애니 본에 씌움

    public Collider atkColl;//공격 콜리더
    public Collider defColl;//쳐맞 콜리더
    public GameObject monsterGen;//몹 생성기
    public NavMeshAgent nvAgent;//추적을 위한 네비
    private Transform monsterTr;//몹 자신의 위치
    public Transform playerTr;//플레이어 위치

    public float traceDist = 10f; // 추적 거리
    public float dist2 = 0; // z축 추적거리
    public float attackDist = 2f; // 공격 거리
    //몬스터의 정보(외부 입력 가능)
    public string ID;
    public string Name;
    public string kName;
    public int Etype;
    public int type;
    public int Hp;
    public int Atk;
    public float Spd;
    public float Acc;
    public float AtkSpd;
    float aniTime;
    //string WeekPoint;
    //private float moveSpeed = 0.05f;//몹 이속
    private bool isDie = false;//몬스터 행동여부
    private bool isFly = false;//몬스터 비행여부
    private bool isHit = false;//몬스터 피격여부
    public bool isAtk = false;//몬스터 공격판정
    // Use this for initialization
    void Start()
    {
        //var SkeletonRender = GetComponent<SkeletonRenderer>();
        //var attachMent = SkeletonRender.skeleton.Data.AddUnitySprite(slot, sprite[0], skin);
        //SkeletonRender.skeleton.SetAttachment(slot, sprite[0].name);
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();//플레이어 위치 가져옴
        monsterTr = GetComponent<Transform>();//내 위치정보 가져옴
        monsterGen = GameObject.Find("MonsterGenerator");
        nvAgent = GetComponent<NavMeshAgent>();//네비는 몬스터의 매시에이전트
        nvAgent.destination = playerTr.position;//네비가 가리키는 목표는 플레이어
        atkColl = GameObject.FindWithTag("MobAtk").GetComponent<Collider>();//자식오브젝트에 있는 공격범위 콜리더
        defColl = GetComponent<Collider>();
        atkColl.enabled = true;
        nvAgent.Stop();
        if (transform.position.x > playerTr.position.x)
        {
            monsterAnimation.skeleton.flipX = false;
        }
        else if (transform.position.x < playerTr.position.x)
        {
            monsterAnimation.skeleton.flipX = true;
        }
        StartCoroutine(MonsterStateCheck());
        StartCoroutine(MonsterAction());
    }
    //몬스터 정보를 외부에서 초기화 하기위한 함수입니다.
    public void Insert(string i_id, string i_name, string i_kname, int i_etype, int i_type,
         int i_hp, int i_atk, float i_spd, float i_acc, float i_atkspd)
    {
        ID = i_id;
        Name = i_name;
        kName = i_kname;
        Etype = i_etype;
        type = i_type;
        Hp = i_hp;
        Atk = i_atk;
        Spd = i_spd;
        Acc = i_acc;
        AtkSpd = i_atkspd;
    }
    //몬스터의 행동을 위한 코루틴 함수입니다.
    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (monsterState)
            {
                case MonsterState.IDLE://대기때 하는거 없음
                    SetAnimation("STAY", true, 1.0f);
                    break;
                case MonsterState.DIE://죽으면 
                    nvAgent.Stop();
                    SetAnimation("APA3", false, 1.0f);
                    yield return new WaitForSeconds(aniTime);
                    QuestManager.Instance.QuestIsClear(name);//퀘 조건인지?
                    StopAllCoroutines();
                    gameObject.SetActive(false);//몬스터 사망처리
                    break;
                case MonsterState.BEATEN://맞으면
                    nvAgent.Stop();
                    SetAnimation("APA2", false, 1.0f);
                    yield return new WaitForSeconds(aniTime);
                    atkColl.enabled = true;
                    defColl.enabled = true;
                    isHit = false;
                    break;
                case MonsterState.TRACE://추적
                    if (transform.position.x > playerTr.position.x)
                    {
                        monsterAnimation.skeleton.flipX = false;
                    }
                    else if (transform.position.x < playerTr.position.x)
                    {
                        monsterAnimation.skeleton.flipX = true;
                    }
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    if (dist2 > 1) { transform.Translate(Vector3.forward * 0.25f * Time.deltaTime); } //z축을 좀더 좁힌다.
                    else if (dist2 < -1) { transform.Translate(Vector3.back * 0.25f * Time.deltaTime); } //z축을 좀더 좁힌다. 절대값 계산시 양수->양수 일때 이동 장애있음
                    SetAnimation("walk", true, 1.0f);
                    break;
                case MonsterState.ATK://공격
                    if (transform.position.x > playerTr.position.x)
                    {
                        monsterAnimation.skeleton.flipX = false;
                    }
                    else if (transform.position.x < playerTr.position.x)
                    {
                        monsterAnimation.skeleton.flipX = true;
                    }
                    SetAnimation("STAY", true, 2.0f);//선딜
                    yield return new WaitForSeconds(aniTime * 0.5f);
                    if (transform.position.x > playerTr.position.x)
                    {
                        monsterAnimation.skeleton.flipX = false;
                    }
                    else if (transform.position.x < playerTr.position.x)
                    {
                        monsterAnimation.skeleton.flipX = true;
                    }
                    nvAgent.Stop();
                    atkColl.enabled = true;
                    SetAnimation("attack2", false, 1.0f);
                    yield return new WaitForSeconds(aniTime);
                    monsterState = MonsterState.TRACE;//행동 후 일단 재 추적 부터
                    atkColl.enabled = false;
                    break;
            }
            yield return null;
        }
    }
    IEnumerator MonsterStateCheck()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.1f);
            float dist = Vector3.Distance(playerTr.transform.position, transform.position);
            dist2 = playerTr.transform.position.z - transform.position.z;

            if (Hp <= 0)
            { 
                isDie = true;
                monsterState = MonsterState.DIE;
            }
            else if (isHit)//맞을 때
            {
                monsterState = MonsterState.BEATEN;
            }
            else if (dist <= attackDist && 
                ((dist2 <= 1 && dist2 >= 0) ||
                (dist2 <= 0 && dist2 >= -1)))//공격
            {
                monsterState = MonsterState.ATK;
            }
            else if (dist <= traceDist && !isDie)//추적
            {
                monsterState = MonsterState.TRACE;
            }
            else//대기
            {
                monsterState = MonsterState.IDLE;
            }
        }
    }
    public void SetAnimation(string name, bool loop, float speed)//스켈레톤 애니 세팅 이름,루프여부,재생속도
    {
        if (name == curAnimation)
        {
            return;
        }
        else
        {
            aniTime = monsterAnimation.state.SetAnimation(0, name, loop).EndTime;
            monsterAnimation.state.SetAnimation(0, name, loop).timeScale = speed;
            Debug.Log("애니재생시간 " + aniTime);
            curAnimation = name;
        }
    }
    void OnTriggerEnter(Collider defColl)
    {
        Debug.Log("작동중");
        if (defColl.CompareTag("PlayerAtk"))
        {
            nvAgent.Stop();
            isHit = true;
            Hp = Hp - 10;
            monsterState = MonsterState.BEATEN;
            atkColl.enabled = false;
            defColl.enabled = false;
            //SetAnimation("APA3", false, 1.0f);
           /* 
            QuestManager.Instance.QuestIsClear(name);//퀘 조건인지?
            gameObject.SetActive(false);//몬스터 사망처리*/
        }
    }
}