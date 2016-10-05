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
    private float jumpPower = 6.5f;
    private float climbSpeed = 0.0075f;

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
        if (isSpace && isFloor && !isHang) // 점프
        {
            isJump = true;
        }

        if (!isClimb && isHang && isAct && isSpace) // 오르기
        {
            StartCoroutine(CLIMBING());
            isClimb = true;
        }

        JumpCheck(); // 점프를 할수 있는 위치인지 확인
        HangCheck(); // 잡을 수 있는 벽인지 확인
        TargetHitCheck();
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    // FixedUpdate 문++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    void FixedUpdate()
    {
        if (!isDoll && !isAct)
        {
            if (isJump && isFloor) // 점프에 제한이 없을 경우, 바닥에 있을 경우
            {
                rb.velocity = new Vector2(0, jumpPower);
                isJump = false;
            }

            // 플레이어 움직임 관련==============================================================================================================

            if (isCrawl) { SetAnimation("GUIDECRAWL", true, 1.0f); }
            //===================================================================================================================================
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // 플레이어 접촉 체크=====================
    void TargetHitCheck()
    {
        if (Helper.Skeleton.FlipX == false)
        {
            RaycastHit2D hitHang = Physics2D.Raycast(transform.position + (Vector3.up * 1.5f) + (Vector3.right * 0.3f), Vector2.right, 0.1f);
            if (hitHang.collider != null && hitHang.collider.CompareTag("PLAYER"))
            {
                isPlayer = true;
            }

        }
        else
        {
            RaycastHit2D hitHang = Physics2D.Raycast(transform.position + (Vector3.up * 1.5f) - (Vector3.right * 0.3f), -Vector2.right, 0.1f);
            if (hitHang.collider != null && hitHang.collider.CompareTag("PLAYER"))
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
        if (hitHang.collider != null)
        {
            if (hitHang.collider.CompareTag("CLIMBFLOOR") && !isHangLimit && !isHang)
            {
                hangPosition = (hitHang.collider.bounds.min) + (Vector3.up * hitHang.collider.bounds.size.y) - (Vector3.up * 2.5f);

                StartCoroutine(HANGING());
                isHangLimit = true;

            }
            else if (hitHang.collider.CompareTag("CRAWL"))
            {
                isFloor = false;
                isCrawl = true;
            }
        }

    }
    //===========================================================================================================================================

    // 점프 가능 ================================================================================================================================
    void JumpCheck()
    {
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);

        if (hitDown.collider == null)
        {
            isFloor = false;
            isCrawl = false;
        }
        else if (hitDown.collider.CompareTag("FLOOR") || hitDown.collider.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
            isCrawl = false;
        }
        else if (hitDown.collider.CompareTag("CRAWL"))
        {
            isFloor = false;
            isCrawl = true;
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
            Helper.state.SetAnimation(0, name, loop).TimeScale = speed;
            animationName = name;
        }
    }
    //===========================================================================================================================================

    // 벽 잡기 코루틴 ===========================================================================================================================
    IEnumerator HANGING()
    {
        rb.isKinematic = true;
        isAct = true;

        if (Helper.Skeleton.FlipX.Equals(true))
        {
            transform.position = hangPosition;
        }
        else
        {
            transform.position = hangPosition;
        }

        SetAnimation("GUIDEHANG", true, 1.0f);
        yield return new WaitForSeconds(0.5f);
        isHang = true;
    }
    //===========================================================================================================================================

    // 오르기 코루틴=============================================================================================================================
    IEnumerator CLIMBING()
    {
        SetAnimation("GUIDECLIMB", false, 1.0f);
        if (Helper.Skeleton.FlipX.Equals(true)) // 캐릭터가 왼쪽을 바라볼 경우
        {
            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.01f))
            {
                tr.Translate((Vector2.up * 5.0f * climbSpeed) + (Vector2.right * 1.5f * climbSpeed));
                yield return 0;
            }

            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.01f))
            {
                tr.Translate((Vector2.right * 1.0f * climbSpeed));
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
        rb.isKinematic = false;
        isAct = false;
        isHang = false;
        isClimb = false;
        isFloor = false;
    }
    //========================================================================================================
}


