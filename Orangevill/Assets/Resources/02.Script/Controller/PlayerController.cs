using UnityEngine;
using System.Collections;
using Spine.Unity;

public class PlayerController : MonoBehaviour {
    //public Spine.Skeleton PlayerSkel;
    public SkeletonAnimation Player;
    //public Spine.SkeletonData PlayerData;
    //public Spine.SkeletonJson PlayerJson;
    //public Spine.SkeletonBounds PlayerBounds;
    //public Spine.Animation PlayerAni;
    //public Spine.AnimationState PlayerState;
    //public Spine.AnimationStateData PlayerStateData;

    private string animationName = ""; // 캐릭터의 애니메이션 이름을 넣을 변수

    public Rigidbody2D rb;
    public Transform tr;
    public GameObject JointHead; // 조인트의 앞부분
    public GameObject JointTail; // 조인트의 뒷부분
    public GameObject PlayerBody; // 조인트가 달릴 몸통의 위치
    public GameObject PlayerWeapon; // 조인트가 달리 손의 위치

    public UIProgressBar ProgressHPBar;


    public bool isMove = true; // 별다른 제한사항이 없을 경우 true
    public bool isJump = false; // 캐릭터가 바닥과 맞닿은 상태에서 점프를 누를 경우 true
    public bool isFloor = false; // 캐릭터가 바닥과 맞닿을 경우 true
    public bool isAct = false; // 특수 행동을 취할 때 true
    public bool isHang = false; // 벽을 붙잡을 겨우 true
    public bool isClimb = false;
    public bool isThread = false;
    public bool isRope = false;
    public bool isCrawl = false; // 캐릭터가 기어갈 경우 true

    private float v;
    private float h;
    //public float playerHP = 5.0f;
    private float jumpPower = 7f;
    private float walkSpeed = 4.0f;
    private float climbLimit = 0.0075f;
    private float ropeLimit = 0.005f;
    private float ropeSpeed = 30.0f;

    private Vector2 movement; // 캐릭터의 움직임
    private Vector3 hangPosition; // 캐릭터가 벽을 붙잡는 최종위치
    private Vector3 JointBodyTr; // 조인트를 연결할 캐릭터의 몸통
    private Vector3 JointWeaponTr; // 조인트를 연결할 무기의 위치


    private static PlayerController gInstance = null;

    public static PlayerController Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }

    void Awake()
    {
        Player = GetComponent<SkeletonAnimation>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        JointHead = GameObject.FindGameObjectWithTag("JOINTHEAD");
        JointTail = GameObject.FindGameObjectWithTag("JOINTTAIL");
        PlayerBody = GameObject.FindGameObjectWithTag("BODY");
        PlayerWeapon = GameObject.FindGameObjectWithTag("WEAPON");

        gInstance = this;
    }

    // Update 문+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    void Update()
    {
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position - Vector3.up * 1.5f, Vector2.down, 4.0f);
        if (hitDown.collider.CompareTag("FLOOR") || hitDown.collider.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
        }
        else
        {
            isFloor = false;
        }

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + (Vector3.right * 1.0f), Vector2.right, 8.0f);

        if (hitRight.collider.CompareTag("CLIMBFLOOR"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isHang)
                {
                    hangPosition = (hitRight.collider.bounds.min) + (Vector3.up * hitRight.collider.bounds.size.y) - (Vector3.up * 3.0f);
                    Player.Skeleton.flipX = false;
                    StartCoroutine(Hanging());

                }
            }
        }

        JumpCheck(); // 점프를 할수 있는 위치인지 확인
        //HangCheck(); // 잡을 수 있는 벽인지 확인


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) // 점프
        {
            if (isFloor && !isHang)
            {
                isFloor = false;
                isJump = true;
            }
        }

        if (!isClimb && isHang && isAct) // 오르기
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                isClimb = true;
                StartCoroutine(CLIMBING());
                
            }
        }




    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    // FixedUpdate 문++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    void FixedUpdate()
    {
        
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        movement.Set(h, 0);
        if (!isAct)
        {
            //if (isJump && isFloor) // 점프에 제한이 없을 경우, 바닥에 있을 경우
            if(Input.GetKeyDown(KeyCode.Space) && isFloor)
            {
                isFloor = false;
                //rb.velocity = new Vector2(0, jumpPower);
                rb.velocity = Vector2.up * jumpPower;
                //rb.AddForce(Vector2.up * jumpPower);
                isJump   = false;
            }

            if (isMove) // 움직임에 제한이 없을 경우
            {
                
                    tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World); // 걷기

                
            }

            if (h > 0) // 오른쪽 방향일 경우
            {
                Player.Skeleton.flipX = true;
            }
            if (h < 0) // 왼쪽 방향일 경우
            {
                Player.Skeleton.flipX = false;
            }



            // 플레이어 움직임 관련==============================================================================================================
            if (h != 0)
            {
                
                if (isFloor) { SetAnimation("WALK", true, 1.0f); }
                if (!isFloor) { SetAnimation("JUMP", false, 1.0f); }
            }
            if (h.Equals(0))
            {
                if (isFloor) { SetAnimation("STAY", true, 1.0f); }
                if (!isFloor) { SetAnimation("JUMP", false, 1.0f); }
            }
            if (!isFloor) { SetAnimation("JUMP", false, 1.0f); }
            //===================================================================================================================================
        }
       
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    /*
        void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("FLOOR") || coll.gameObject.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
        }
        else
        {
            isFloor = false;
        }
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("FLOOR") || coll.gameObject.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
        }
        else
        {
            isFloor = false;
        }
    }
    void OnTriggerEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("FLOOR") || coll.gameObject.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
        }
        else
        {
            isFloor = false;
        }
    }*/

    // 벽 잡기 가능 & 기어가기 체크 =============================================================================================================================
    /*void HangCheck()
    {
        
            Debug.Log("정면에서 당신은 충돌합니다");
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                if (!isHang)
                {
                    Player.Skeleton.flipX = false;
                    //hangPosition = (hitHang.collider.bounds.min) + (Vector3.up * hitHang.collider.bounds.size.y) - (Vector3.up * 3.0f);
                    //hangPosition = hitHang.collider.bounds.min - (Vector3.up * hitHang.collider.bounds.size.y) - (Vector3.up * 2.0f);
                    StartCoroutine(Hanging());
                }
            }

        
    }*/
    //===========================================================================================================================================

    // 점프 가능 ================================================================================================================================
    void JumpCheck()
    {
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 2.0f);
        if (hitDown.collider.CompareTag("FLOOR") || hitDown.collider.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
        }
        else
        {
            isFloor = false;
        }
    }
    //===========================================================================================================================================

    /*void RopeCheck()
    {
        if (isRope)
        {
            RaycastHit2D hitFloor = Physics2D.Raycast(new Vector2(PlayerWeapon.transform.position.x, PlayerWeapon.transform.position.y), Vector2.up, 0.5f);

            if (hitFloor.collider.CompareTag("FLOOR"))
            {
                //JointHead.transform
            }
        }
    }
   */
    
    /*
    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("FLOOR") || coll.gameObject.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
        }
        else
        {
            isFloor = false;
        }
    }*/

    // 스파인 애니메이션 관련 함수===============================================================================================================
    public void SetAnimation(string name, bool loop, float speed)
    {
        if (name.Equals(animationName))
        {
            return;
        }
        else
        {
            Player.state.SetAnimation(0, name, loop).timeScale = speed;
            animationName = name;
        }
    }
    //===========================================================================================================================================

    // 벽 잡기 코루틴 ===========================================================================================================================
    IEnumerator Hanging()
    {
        rb.isKinematic = true;
        isAct = true;

        if (Player.Skeleton.flipX.Equals(true))
        {
            transform.position = hangPosition;
        }
        else
        {
            Player.skeleton.flipX = true;
            transform.position = hangPosition;
        } 

        SetAnimation("STAY", true, 1.0f);
        yield return new WaitForSeconds(0.5f);
        isHang = true;
    }
    //===========================================================================================================================================

    // 오르기 코루틴=============================================================================================================================
    IEnumerator CLIMBING()
    {
        SetAnimation("CLIMB", false, 1.0f);
        if (Player.Skeleton.flipX.Equals(true)) // 캐릭터가 왼쪽을 바라볼 경우
        {
            Player.skeleton.flipX = true;
            for (float i = 1f; i >= 0; i -= (climbLimit+0.01f ))
            {
                tr.Translate((Vector2.up * 3.0f * climbLimit));
                yield return 0;
            }
           /* for (float i = 1f; i >= 0; i -= climbLimit)
            {
                tr.Translate((Vector2.up * 2.0f * climbLimit));
                yield return 0;
            }*/
            for (float i = 1f; i >= 0; i -= (climbLimit + 0.01f))
            {
                tr.Translate((Vector2.up * 1.0f * climbLimit) + (Vector2.right * 3.0f * climbLimit));
                yield return 0;
            }
        }
        else // 오른쪽을 바라보지 않을 경우
        {
            for (float i = 1f; i >= 0; i -= climbLimit + 0.006f)
            {
                tr.Translate((Vector2.up * 2.3f * climbLimit));
                yield return 0;
            }
           /* for (float i = 1f; i >= 0; i -= climbLimit)
            {
                tr.Translate((Vector2.up * 2.0f * climbLimit));
                yield return 0;
            }*/
            for (float i = 1f; i >= 0; i -= (climbLimit + 0.006f))
            {
                tr.Translate((Vector2.up * 1.0f * climbLimit) - (Vector2.right * 3.0f * climbLimit));
                yield return 0;
            }
        }
        rb.isKinematic = false;
        isAct = false;
        isHang = false;
        isClimb = false;
        //isFloor = false;
    }
    //========================================================================================================
    // 캐릭터 로프 액션=======================================================================================
    IEnumerator RopeAct()
    {
        isRope = true;
        rb.isKinematic = true;
        isAct = true;

        SetAnimation("CLIMB", false, 1.0f); // 로프 액션과 관련된 애니메이션

        for (float i = 1f; i >= 0; i -= 0.05f)// 턴 대기
        {
            yield return new WaitForSeconds(0.1f);
        }
        for (float i = 1f; i >= 0; i -= ropeLimit)
        {
            PlayerWeapon.transform.Translate(Vector3.up * ropeLimit * ropeSpeed);

            
            yield return 0;
        }
          
    }
    //==============================================================================================
}