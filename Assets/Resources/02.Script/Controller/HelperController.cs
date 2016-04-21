using UnityEngine;
using System.Collections;
using Spine.Unity;

public class HelperController : MonoBehaviour
{
    public SkeletonAnimation Helper;

    private string animationName = ""; // 캐릭터의 애니메이션 이름을 넣을 변수

    public Rigidbody2D rb;
    public Transform tr;


    public bool isMove = true; // 별다른 제한사항이 없을 경우 true
    public bool isJump = false; // 캐릭터가 바닥과 맞닿은 상태에서 점프를 누를 경우 true
    public bool isFloor = false; // 캐릭터가 바닥과 맞닿을 경우 true
    public bool isAct = false; // 특수 행동을 취할 때 true
    public bool isHang = false; // 벽을 붙잡을 겨우 true
    public bool isClimb = false;
    public bool isThread = false;
    public bool isRope = false;
    public bool isCrawl = false; // 캐릭터가 기어갈 경우 true
    public bool isDoll = false;
    public bool isPlayer = false;
    public bool isHangLimit = false;
    public bool isSpace = false;

    public float v;
    public float h;
    private float jumpPower = 7f;
    private float walkSpeed = 4.0f;
    private float runSpeed = 7.0f;
    private float climbLimit = 0.0075f;
    private float crawlSpeed = 2.0f;
    private float ropeLimit = 0.005f;
    private float ropeSpeed = 30.0f;

    public Vector2 movement; // 캐릭터의 움직임
    public Vector3 hangPosition; // 캐릭터가 벽을 붙잡는 최종위치

    private static HelperController gInstance = null;

    public static HelperController Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }

    void Awake()
    {
        Helper = GetComponent<SkeletonAnimation>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        

        gInstance = this;
    }

    // Update 문+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    void Update()
    {
        if (isSpace) // 점프
        {
            if (isFloor && !isHang)
            {
                isJump = true;
            }
        }

        if (!isClimb && isHang && isAct) // 오르기
        {
            if (isSpace)
            {
                StartCoroutine(CLIMBING());
                isClimb = true;
            }
        }
        JumpCheck(); // 점프를 할수 있는 위치인지 확인
        HangCheck(); // 잡을 수 있는 벽인지 확인
        TargetHitCheck();

    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    // FixedUpdate 문++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    void FixedUpdate()
    {
        //v = Input.GetAxis("Vertical");
        //h = Input.GetAxis("Horizontal");

        //movement.Set(h, 0);
        if(!isDoll)
        {
            if (!isAct)
            {
                if (isJump && isFloor) // 점프에 제한이 없을 경우, 바닥에 있을 경우
                {
                    rb.velocity = new Vector2(0, jumpPower);
                    isJump = false;
                }
 
                // 플레이어 움직임 관련==============================================================================================================
                if (h != 0)
                {
                    if (isFloor) { SetAnimation("WALK", true, 1.0f); }
                    if (!isFloor) { SetAnimation("JUMP", false, 1.0f); }
                }
                if (h == 0)
                {
                    //if (isFloor) { SetAnimation("STAY", true, 1.0f); }
                    //if (!isFloor) { SetAnimation("JUMP", false, 1.0f); }
                }
                if (!isFloor) { SetAnimation("JUMP", false, 1.0f); }
                if(isHang) { SetAnimation("HAINGING", false, 1.0f); }
                //===================================================================================================================================
            }
        }
        
        
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // 플레이어 접촉 체크=====================
    void TargetHitCheck()
    {
        if (Helper.Skeleton.flipX == false)
        {
            RaycastHit2D hitHang = Physics2D.Raycast(transform.position + (Vector3.up * 2.0f) + (Vector3.right * 0.5f), Vector2.right, 0.2f);
            if (hitHang.collider.CompareTag("PLAYER"))
            {
                isPlayer = true;
            }
        }
        else
        {
            RaycastHit2D hitHang = Physics2D.Raycast(transform.position + (Vector3.up * 1.5f) - (Vector3.right * 0.5f), -Vector2.right, 0.5f);
            if (hitHang.collider.CompareTag("PLAYER"))
            {
                isPlayer = true;
            }
        }
    }
    //========================================


        // 벽 잡기 가능 & 기어가기 체크 =============================================================================================================================
        void HangCheck()
    {
            RaycastHit2D hitHang = Physics2D.Raycast(transform.position + (Vector3.up * 1.0f) + (Vector3.right * 0.5f), Vector2.right, 0.5f);

        if (hitHang.collider.CompareTag("CLIMBFLOOR"))
        {
            if (!isHangLimit)
            {
                if (!isHang)
                {
                    hangPosition = (hitHang.collider.bounds.min) + (Vector3.up * hitHang.collider.bounds.size.y) - (Vector3.up*3.0f);

                    StartCoroutine(Hanging());
                    isHangLimit = true;
                }
            }
        }
    }
    //===========================================================================================================================================

    // 점프 가능 ================================================================================================================================
    void JumpCheck()
    {
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);

        if (hitDown.collider == null)
        {
            isFloor = false;
        }
        else if (hitDown.collider.CompareTag("FLOOR") || hitDown.collider.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
        }
    }
    //===========================================================================================================================================
    
    // 스파인 애니메이션 관련 함수===============================================================================================================
    public void SetAnimation(string name, bool loop, float speed)
    {
        if (name == animationName)
        {
            return;
        }
        else
        {
            Helper.state.SetAnimation(0, name, loop).timeScale = speed;
            animationName = name;
        }
    }
    //===========================================================================================================================================

    // 벽 잡기 코루틴 ===========================================================================================================================
    IEnumerator Hanging()
    {
        Debug.Log("벽을 잡아서 내 ㅡ트레스를 없애버려라");
        rb.isKinematic = true;
        isAct = true;

        if (Helper.Skeleton.flipX == false)
        {
            transform.position = hangPosition;
        }
        else
        {
            transform.position = hangPosition;
        }

        SetAnimation("HAINGING", true, 1.0f);
        yield return new WaitForSeconds(0.5f);
        isHang = true;
    }
    //===========================================================================================================================================

    // 오르기 코루틴=============================================================================================================================
    IEnumerator CLIMBING()
    {
        Debug.Log("올라가지롱");
        SetAnimation("CLIMB", false, 1.0f);
        if (Helper.Skeleton.flipX == false) // 캐릭터가 왼쪽을 바라볼 경우
        {
            for (float i = 1f; i >= 0; i -= 0.03f)
            {
                tr.Translate((Vector2.up * 2.0f * climbLimit ));
                yield return 0;
            }
            for (float i = 1f; i >= 0; i -= 0.025f)
            {
                tr.Translate((Vector2.up * 3.0f * climbLimit));
                yield return 0;
            }
            for (float i = 1f; i >= 0; i -= (0.015f + 0.01f))
            {
                tr.Translate((Vector2.up * 3.0f * climbLimit) + (Vector2.right * 3.0f * climbLimit));
                yield return 0;
            }
        }
        else // 오른쪽을 바라보지 않을 경우
        {
            for (float i = 1f; i >= 0; i -= climbLimit + 0.1f)
            {
                tr.Translate((Vector2.up * 2.3f * climbLimit));
                yield return 0;
            }
            for (float i = 1f; i >= 0; i -= climbLimit + 0.1f)
            {
                tr.Translate((Vector2.up * 2.0f * climbLimit));
                yield return 0;
            }
            for (float i = 1f; i >= 0; i -= (climbLimit + 0.1f))
            {
                tr.Translate((Vector2.up * 1.0f * climbLimit) - (Vector2.right * 3.0f * climbLimit));
                yield return 0;
            }
        }
        rb.isKinematic = false;
        isAct = false;
        isHang = false;
        isClimb = false;
        isFloor = false;
    }
    //========================================================================================================
}