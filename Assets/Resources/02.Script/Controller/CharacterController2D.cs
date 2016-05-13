using UnityEngine;
using System.Collections;
using Spine.Unity;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BoxCollider2D))]

public class CharacterController2D : MonoBehaviour
{
    private static CharacterController2D gInstance = null;

    // 캐릭터
    //public enum State { IDLE, WALK, RUN, JUMP, FALL, HIT, ATTACK, DEAD, HANG, CLIMB, ROPE }
    //public State PlayerState;
    public SkeletonAnimation Player;
    private string animationName = "";

    // 상태체크
    public bool isJump = false; // 점프 체크
    public bool isFloor = false; // 바닥 체크
    public bool isAct = false; // 캐릭터 움직임 제한
    public bool isHang = false;
    public bool isClimb = false;
    public bool isRopeAct = false; // 로프 액션 체크
    public bool isCrawl = false;
    public bool isSkill = false;

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

    // 캐릭터 로프 관련
    public float aniTime = 0.0f;
    public Vector3 ropeActPoint = Vector3.zero;
    public Vector3 ropeRingPoint = Vector3.zero;
    public Vector3 ropeStartPoint = Vector3.zero;
    public GameObject RopeInBody; // 로프의 몸통
    public GameObject RopeInHand; // 바늘에 달려있는 로프 
    public GameObject Needle;
    public GameObject NeedlePoint;
    public GameObject Weapon;

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
        Player = GetComponent<SkeletonAnimation>();
        RopeInBody = GameObject.Find("RopeInBody");
        RopeInHand = GameObject.Find("RopeInHand");
        NeedlePoint = GameObject.Find("NeedlePoint");
        Weapon = GameObject.Find("Weapon");
        RopeInHand.SetActive(false);
        Weapon.SetActive(false);
    }

    void Update()
    {
        
        // 캐릭터의 방향
        if (h > 0) { Player.Skeleton.flipX = true; }
        else if (h < 0) { Player.Skeleton.flipX = false; }

        if (h != 0)
        {
            if (isFloor && !isAct && !isCrawl) { SetAnimation("WALK2", true, 1.0f); }
            if (!isFloor && !isAct && !isCrawl) { SetAnimation("JUMP", false, 1.0f); }
            if (isCrawl) { SetAnimation("CRAWL", true, 1.0f); }
        }
        if (h.Equals(0))
        {
            if (isFloor && !isAct && !isCrawl) { SetAnimation("STAY", true, 1.0f); }
            if (!isFloor && !isAct && !isCrawl) { SetAnimation("JUMP", false, 1.0f); }
            if (isCrawl) { SetAnimation("CRAWL", true, 1.0f); }
        }
        if (!isFloor && !isAct && !isCrawl) { SetAnimation("JUMP", false, 1.0f); }
        if (isCrawl) { SetAnimation("CRAWL", true, 1.0f); }

        if (!isAct)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isFloor)
                {
                    isFloor = false;
                    isJump = true; // 점프 작동
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && isRopeAct)
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
            if (Input.GetKeyDown(KeyCode.Space) && isHang && !isClimb)
            {
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
                rb.velocity = Vector2.up * jumpPow;
                isJump = false;
            }

            
        }
        
        
    }
        // 매달리기 =================================================
        IEnumerator HANGING()
    {
        isAct = true;
        rb.isKinematic = true;
        transform.position = hangPoint;
        
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
        SetAnimation("CLIMB", true, 1.0f);
        if (Player.Skeleton.flipX.Equals(true)) // 캐릭터가 왼쪽을 바라볼 경우
        {
            Player.skeleton.flipX = true;
            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.01f))
            {
                tr.Translate((Vector2.up * 6.0f * climbSpeed));
                yield return 0;
            }

            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.01f))
            {
                tr.Translate((Vector2.up * 2.0f * climbSpeed) + (Vector2.right * 3.0f * climbSpeed));
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
    }
    // 로프액션 ===================================================
    IEnumerator ROPEACTING()
    {
        isAct = true; // 캐릭터 움직임 제한
        SetAnimation("ROPE", false, 1.0f);

        aniTime = Player.state.GetCurrent(0).EndTime; // 로프 액션 애니메이션의 길이를 저장

        RopeInBody.GetComponent<DistanceJoint2D>().enabled = true;

        RopeInBody.transform.position = transform.position + Vector3.up * 2.0f + Vector3.right * 1.0f;
        RopeInHand.transform.position = transform.position + Vector3.up * 2.0f + Vector3.right * 1.0f;
        RopeInBody.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        yield return new WaitForSeconds(aniTime - 0.3f); // 로프 액션 애니메이션 만큼 대기
        Needle.SetActive(false);
        RopeInHand.SetActive(true);
        for (float i = 1.0f; i >= 0; i -= 0.035f)
        {
            ropeStartPoint = transform.position; // 현재 위치를 저장
            float dis = Vector3.Distance(ropeRingPoint, RopeInHand.transform.position); // 로프를 걸 링과 바늘이 달려있는 로프의 끝의 거리를 확인

            if (dis > 0.9f) // 거리가 많이 가까워 지면
            {
                RopeInHand.transform.Translate((Vector2.right * ropeActPoint.x * 0.05f) + (Vector2.up * ropeActPoint.y * 0.05f)); // 아니면 계속 로프를 걸 링으로 이동                                                                                                            //RopeInHand.transform.loca                                                                                                                  //RopeInHand.transform.localPosition = Vector3.Lerp(RopeInHand.transform.position, ropeRingPoint)
            }
            if (dis < 0.9f)
            {
                RopeInHand.transform.position = ropeRingPoint; // 바늘이 달려있는 로프의 위치를 로프를 걸 링 위치에 고정
            }
            RopeInBody.GetComponent<DistanceJoint2D>().distance = Vector3.Distance(RopeInBody.transform.position, ropeRingPoint) - 1.5f; // 연결 길이를 캐릭터와 바늘과의 거리만큼
            yield return 0;
        }
        yield return new WaitForSeconds(0.1f);

        NeedlePoint.transform.DetachChildren(); // NeedlePoint의 자식을 분리
        transform.SetParent(RopeInBody.transform); // Player를 RopeInBody의 자식으로
        yield return new WaitForSeconds(1.0f);

        RopeInBody.GetComponent<DistanceJoint2D>().enabled = false;
        RopeInBody.GetComponent<DistanceJoint2D>().enabled = true; // DistanceJoint2D 버그? 제한
        RopeInBody.transform.position = transform.position + Vector3.up * 2.0f - (Vector3.right * 2.0f);
        RopeInBody.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        rb.isKinematic = true;
        yield return new WaitForSeconds(0.1f);

        SetAnimation("FLY", false, 1.0f);
        RopeInBody.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 600.0f); // 로프 액션을 위한 힘
        yield return new WaitForSeconds(1.0f);

        rb.isKinematic = false;
        rb.AddForce((Vector2.right * 400.0f) + (Vector2.up * 400.0f)); // 로프 액션 이후 힘
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
        isAct = false;
        yield return 0;
    }
  
    IEnumerator PlayerHit()
    {
        //Player.GetComponent<MeshRenderer>().
        yield return 0;
    }

    // 스파인 애니메이션 =========================================
    public void SetAnimation(string name, bool loop, float speed)
    {
        if (name.Equals(animationName)) { return; }
        else
        {
            Player.skeleton.SetToSetupPose();
            Player.state.SetAnimation(0, name, loop).timeScale = speed;
            animationName = name;
        }
    }
    //===========================================================
}





//tr.TransformDirection 어떤 방향이든 로컬 무시하고 그냥 월드좌표
//rb.velocity = Vector2.right * h * Speed; 이동은 가능하나 점프랑 같이 두번 적용하면 점프가 순간이동함
//rb.MovePosition(Vector2.right * h * Speed); 이동은 가능하나 원래 위치로 돌아옴
// public LayerMask 연구 필요