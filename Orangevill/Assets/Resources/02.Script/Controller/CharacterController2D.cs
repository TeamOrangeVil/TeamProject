using UnityEngine;
using System.Collections;
using Spine.Unity;
using UnityEngine.SceneManagement;

// 최소 필요한 컴포넌트들
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))] 
[RequireComponent(typeof(AudioSource))]

public class CharacterController2D : MonoBehaviour
{
    private static CharacterController2D gInstance = null;

    // 캐릭터
    //public enum State { IDLE, WALK, RUN, JUMP, FALL, HIT, ATTACK, DEAD, HANG, CLIMB, ROPE }
    //public State PlayerState;
    public SkeletonAnimation Player; // 스파인 용 캐릭터 메소드
    private string animationName = "";

    // 상태체크
    public bool isJump = false; // 점프 체크
    public bool isFloor = false; // 바닥 체크
    public bool isAct = false; // 캐릭터 움직임 제한
    public bool isHang = false; // 오를 수 있는 벽 체크
    public bool isClimb = false; // 오르기 체크
    public bool isRopeAct = false; // 로프 액션 체크
    public bool isCrawl = false; // 기어가기 체크
    public bool isSkill = false; // F 스킬 체크
    public bool isHit = false; // 피해 체크
    public bool isTutorialNeedle = false; // 바늘 체크
    public bool isRopePosition = false; // 로프 방향 체크
    public bool isWalkSound = false; // 걷기 소리 체크

    // 캐릭터 물리 컴포넌트
    public Rigidbody2D rb;
    public Transform tr;

    // 캐릭터 방향 좌표
    public float h;

    // 캐릭터 조작감
    public float jumpPow = 8.8f;
    public float Speed = 0.13f;
    private float climbSpeed = 0.0075f;

    // 캐릭터 행동 위치
    public Vector3 hangPoint = Vector3.zero;
    public Vector3 jumpPoint = Vector3.zero;

    // 캐릭터 로프 관련
    public float aniTime = 0.0f;
    public Vector3 ropeActPoint = Vector3.zero;
    public Vector3 ropeRingPoint = Vector3.zero;
    public Vector3 ropeStartPoint = Vector3.zero;
    public GameObject RopeInBody; // 로프의 몸통
    public GameObject RopeInHand; // 바늘에 달려있는 로프 
    public GameObject Needle; // 평소에 들고다니는 바늘
    public GameObject NeedlePoint; // 바늘 관련 상위 오브젝트

    public GameObject GManager; // GameManager 연결

    public GameObject jumpEffect; // 점프 효과
    public GameObject mobileEffect; // 로프 액션 모빌 효과

    // 캐릭터 사운드 관련
    private AudioSource PlayerAudio;
    public AudioClip jumpSound;
    public AudioClip climbSound;
    public AudioClip[] ropeSound;
    public AudioClip[] walkSound;
    
    public static CharacterController2D Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }

    void Awake()
    {
        gInstance = this;

        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        Player = GetComponent<SkeletonAnimation>(); // 스파인 스켈레톤 애니메이션 연결
        RopeInBody = GameObject.Find("RopeInBody"); // 캐릭터 몸 좌표에 위치
        RopeInHand = GameObject.Find("RopeInHand"); // 캐릭터 손 좌표에 위치
        NeedlePoint = GameObject.Find("NeedlePoint"); // 바늘

        RopeInHand.SetActive(false);
        Needle.SetActive(false);

        //jumpEffect = GameObject.Find("jump full").GetComponent<GameObject>();
        GManager = GameObject.Find("GameManager");

        PlayerAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 캐릭터의 방향
        if (h > 0) { Player.Skeleton.flipX = true; }
        else if (h < 0) { Player.Skeleton.flipX = false; }

        if (h != 0) // 움직임이 있다면
        {

            if (isFloor && !isAct && !isCrawl) { SetAnimation("WALK2", true, 1.0f); }
            if (!isFloor && !isAct && !isCrawl && !isHit) { SetAnimation("JUMP", false, 1.0f); }
            if (isCrawl) { SetAnimation("CRAWL", true, 1.0f); }

            if (isHit) { SetAnimation("FLY", true, 1.0f); } // 아픈 애니메이션
        }
        if (h.Equals(0)) // 움직임이 없다면
        {
            if (isFloor && !isAct && !isCrawl) { SetAnimation("STAY", true, 1.0f); }
            if (!isFloor && !isAct && !isCrawl && !isHit) { SetAnimation("JUMP", false, 1.0f); }
            if (isCrawl) { SetAnimation("CRAWL", false, 1.0f); }

            if (isHit) { SetAnimation("FLY", true, 1.0f); }
        }

        if (!isFloor && !isAct && !isCrawl && !isHit) { SetAnimation("JUMP", false, 1.0f); }
        if (isCrawl) { SetAnimation("CRAWL", false, 1.0f); }
        if (isHit) { SetAnimation("FLY", true, 1.0f); }

        if (!isAct)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isFloor) // 점프
            {
                isFloor = false;
                isJump = true; // 점프 작동

            }

            if(Input.GetKeyDown(KeyCode.F) && isFloor && GameManager.Instance.checkCount > 0 && isSkill) // 체크 포인트
            {
                StartCoroutine(SAVING());
            }

            if (Input.GetKeyDown(KeyCode.R) && isRopeAct && isFloor) // 로프액션
            {
                StartCoroutine(ROPEACTING());
            }
            if (!isRopeAct)
            {
                //RopeInHand.transform.position = PlayerHand.transform.position; // 바늘의 위치를 캐릭터의 손으로
            }
        }
        if(isAct)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isHang && !isClimb) // 오르기
            {
                PlayerAudio.clip = climbSound;
                PlayerAudio.Play();
                StartCoroutine(CLIMBING()); // 오르기 시작   
            }
        }
    }

    void FixedUpdate()
    {
        if (!isAct)
        {
            // 캐릭터 방향키
            h = Input.GetAxis("Horizontal");
            if (h != 0 && !isWalkSound && isFloor)
            {
                StartCoroutine(WalkSound());
                isWalkSound = true;
            }
            // 캐릭터 좌우 움직임
            if(!isCrawl)
            {
                tr.Translate(Vector3.right * h * Speed );
            }
            else
            {
                tr.Translate(Vector3.right * h * Speed * 0.5f);
            }
            

            if (isJump )
            {
                jumpPoint = transform.position;
                jumpEffect.transform.position = jumpPoint;
                jumpEffect.SetActive(true);

                rb.velocity = Vector2.up * jumpPow; // 점프 힘

                PlayerAudio.clip = jumpSound;
                PlayerAudio.Play();

                isJump = false;
            }

            
        }
        
        
    }

    // 세이브 포인트 만들기 ===============================
    IEnumerator SAVING()
    {
        isAct = true;
        SetAnimation("CHECKPOINT", false, 1.0f);
        yield return new WaitForSeconds(0.6f);
        isAct = false;
        yield return new WaitForSeconds(0.2f);
    }
        // 매달리기 =================================================
        IEnumerator HANGING()
    {
        isAct = true;
        rb.isKinematic = true;
        transform.position = hangPoint;
        Needle.SetActive(false);
        SetAnimation("HANG", true, 1.0f);
        yield return new WaitForSeconds(0.2f);
        isHang = true;
        yield return 0;
    }
    // 오르기 ===================================================
    IEnumerator CLIMBING()
    {
        isClimb = true;
        //PlayerState = State.CLIMB;
        SetAnimation("CLIMB", false, 1.0f);
        if (Player.Skeleton.flipX.Equals(true)) // 캐릭터가 왼쪽을 바라볼 경우
        {
            Player.skeleton.flipX = true;
            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.022f))
            {
                tr.Translate((Vector2.up * 6.0f * climbSpeed) + (Vector2.right * 1.5f * climbSpeed));
                yield return 0;
            }

            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.02f))
            {
                tr.Translate((Vector2.up * 3.0f * climbSpeed) + (Vector2.right * 2.0f * climbSpeed));
                yield return 0;
            }
            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.02f))
            {
                tr.Translate((Vector2.up * 2.0f * climbSpeed) + (Vector2.right * 1.0f * climbSpeed));
                yield return 0;
            }
        }
        else // 오른쪽을 바라보지 않을 경우
        {
            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.01f))
            {
                tr.Translate((Vector2.up * 5.0f * climbSpeed));
                yield return 0;
            }

            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.01f))
            {
                tr.Translate((Vector2.up * 1.2f * climbSpeed) + (Vector2.left * 3.0f * climbSpeed));
                yield return 0;
            }
        }
        isAct = false;
        isJump = false;
        isHang = false;
        isClimb = false;
        rb.isKinematic = false;

        if(isTutorialNeedle)
        {
            Needle.SetActive(true);
        }
        
    }
    // 로프액션 ===================================================
    IEnumerator ROPEACTING()
    {
        int dir;
        dir = isRopePosition == false ? -1 : 1;
        
        isAct = true; // 캐릭터 움직임 제한
        SetAnimation("ROPE", false, 1.0f);
        PlayerAudio.clip = ropeSound[0];
        PlayerAudio.Play();

        aniTime = Player.state.GetCurrent(0).EndTime; // 로프 액션 애니메이션의 길이를 저장

        RopeInBody.GetComponent<DistanceJoint2D>().enabled = true;

        RopeInBody.transform.position = transform.position + Vector3.up * 2.0f + Vector3.right * 1.0f;
        RopeInHand.transform.position = transform.position + Vector3.up * 2.0f + Vector3.right * 1.0f;
        RopeInBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        yield return new WaitForSeconds(aniTime - 0.3f); // 로프 액션 애니메이션 만큼 대기
        Needle.SetActive(false);
        RopeInHand.SetActive(true);
        PlayerAudio.clip = ropeSound[1];
        PlayerAudio.Play();
        for (float i = 1.0f; i >= 0; i -= 0.035f)
        {
            ropeStartPoint = transform.position; // 현재 위치를 저장
            float dis = Vector3.Distance(ropeRingPoint, RopeInHand.transform.position); // 로프를 걸 링과 바늘이 달려있는 로프의 끝의 거리를 확인

            if (dis > 0.9f) // 거리가 많이 가까워 지면
            {
                RopeInHand.transform.Translate((Vector2.right * ropeActPoint.x * 0.1f) + (Vector2.up * ropeActPoint.y * 0.1f)); // 아니면 계속 로프를 걸 링으로 이동                                                                                                            //RopeInHand.transform.loca                                                                                                                  //RopeInHand.transform.localPosition = Vector3.Lerp(RopeInHand.transform.position, ropeRingPoint)
            }
            if (dis < 0.9f)
            {
                RopeInHand.transform.position = ropeRingPoint; // 바늘이 달려있는 로프의 위치를 로프를 걸 링 위치에 고정
                mobileEffect.transform.position = ropeRingPoint;
                mobileEffect.SetActive(true);
                break;
            }
            
            yield return 0;
        }
        RopeInBody.GetComponent<DistanceJoint2D>().distance = Vector3.Distance(RopeInBody.transform.position, ropeRingPoint); // 연결 길이를 캐릭터와 바늘과의 거리만큼
        yield return new WaitForSeconds(0.1f);

        SetAnimation("FLY", false, 1.0f);
        NeedlePoint.transform.DetachChildren(); // NeedlePoint의 자식을 분리
        transform.SetParent(RopeInBody.transform); // Player를 RopeInBody의 자식으로
        transform.localPosition += (Vector3.right * dir * 0.5f) + (Vector3.up * 0.5f);
        yield return new WaitForSeconds(0.001f);

        
        RopeInBody.GetComponent<DistanceJoint2D>().enabled = false;
        RopeInBody.GetComponent<DistanceJoint2D>().enabled = true; // DistanceJoint2D 버그? 제한
        RopeInBody.transform.position = transform.position; //+ Vector3.up * 2.0f - (Vector3.right * 2.0f);
        RopeInBody.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        rb.isKinematic = true;
        
        yield return new WaitForSeconds(0.001f);

        
        RopeInBody.GetComponent<Rigidbody2D>().AddForce(Vector2.right * dir * 600.0f); // 로프 액션을 위한 힘
        yield return new WaitForSeconds(1.0f);

        rb.isKinematic = false;
        rb.AddForce((Vector2.right * dir * 400.0f) + (Vector2.up * 400.0f)); // 로프 액션 이후 힘
        SetAnimation("FALL", false, 1.0f);
        RopeInBody.transform.DetachChildren(); // RopeInBody의 자식을 분리
        RopeInBody.transform.SetParent(NeedlePoint.transform); // RopeInBody를 NeedlePoint의 자식으로
        RopeInHand.transform.SetParent(NeedlePoint.transform); // RopeInHand를 NeedlePoint의 자식으로
        RopeInBody.transform.position = transform.position;
        RopeInHand.transform.position = transform.position;

        isAct = false; // 캐릭터 움직임 제한 해제

        RopeInBody.GetComponent<DistanceJoint2D>().enabled = false;

        Needle.SetActive(true);
        RopeInHand.SetActive(false);
        RopeInBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition; // RopeInBody 움직임 Freeze
        RopeInBody.GetComponent<Rigidbody2D>().gravityScale = 0f;

        transform.SetParent(GManager.transform);

        isAct = false;
        yield return new WaitForSeconds(2.0f);
        mobileEffect.SetActive(false);
        yield return 0;
    }
  
    IEnumerator PlayerHit()
    {
        isHit = true;
        yield return new WaitForSeconds(0.3f);
        isHit = false;
        yield return 0;
    }
    IEnumerator WalkSound()
    {
        isWalkSound = true;
        PlayerAudio.clip = walkSound[Random.Range(0, 3)]; // 걷는 소리 랜덤으로 값을 받음
        PlayerAudio.PlayOneShot(walkSound[Random.Range(0, 3)]);
        float soundTime = PlayerAudio.clip.length; // 현재 가지고 있는 클립 사운드의 길이
        yield return new WaitForSeconds(soundTime + 0.12f); 
        isWalkSound = false;
        yield return 0;
    }

    // 스파인 애니메이션 =========================================
    public void SetAnimation(string name, bool loop, float speed)
    {
        if (name.Equals(animationName)) { return; } // 받은 name의 값이 animationName의 값과 같다면 return
        else
        {
            Player.skeleton.SetToSetupPose(); // 동작 초기화
            Player.state.SetAnimation(0, name, loop).timeScale = speed; // 애니메이션의 속도
            animationName = name; // name의 값을 animationName에 넣음
        }
    }
    //===========================================================
}





//tr.TransformDirection 어떤 방향이든 로컬 무시하고 그냥 월드좌표
//rb.velocity = Vector2.right * h * Speed; 이동은 가능하나 점프랑 같이 두번 적용하면 점프가 순간이동함
//rb.MovePosition(Vector2.right * h * Speed); 이동은 가능하나 원래 위치로 돌아옴
// public LayerMask 연구 필요