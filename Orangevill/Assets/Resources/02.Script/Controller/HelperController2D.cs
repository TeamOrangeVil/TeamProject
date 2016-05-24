using UnityEngine;
using System.Collections;
using Spine.Unity;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(BoxCollider2D))]

public class HelperController2D : MonoBehaviour
{
    public SkeletonAnimation Helper;
    private string animationName = "";

    // 상태체크
    public bool isJump = false; // 점프 체크
    public bool isFloor = false; // 바닥 체크
    public bool isAct = false; // 캐릭터 움직임 제한
    public bool isHang = false;
    public bool isRopeAct = false; // 로프 액션 체크
    public bool isCrawl = false;
    public bool isHangLimit = false;
    public bool isSpace = false;

    // 캐릭터 물리 컴포넌트
    public Rigidbody2D rb;
    public Transform tr;

    // 캐릭터 방향 좌표
    public float h;
    private float v;

    // 캐릭터 조작감
    public float jumpPow = 7.0f;
    public float Speed = 0.2f;
    private float climbSpeed = 0.0075f;

    // 캐릭터 행동 위치
    public Vector3 hangPoint = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        Helper = GetComponent<SkeletonAnimation>();
    }

    void Update()
    {
        // 캐릭터의 방향
        if (h > 0) { Helper.Skeleton.flipX = true; }
        else if (h < 0) { Helper.Skeleton.flipX = false; }

        if (!isAct)
        {
                if (isFloor && isSpace)
                {
                    isFloor = false;
                    isJump = true; // 점프 작동
                }
        }
        if (isAct)
        {
                StartCoroutine(CLIMBING()); // 오르기 시작   
        }
        CheckFloor();
        CheckClimbRight();
    }

    void FixedUpdate()
    {
        if (!isAct && isJump)
        {
                rb.velocity = Vector2.up * jumpPow;
                isJump = false;
        }
        if (h != 0)
        {
            if (isFloor && !isAct && !isCrawl) { SetAnimation("WALK", true, 1.0f);Debug.Log("걸어"); }
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
        isHang = false;
        SetAnimation("CLIMB", true, 1.0f);
        if (Helper.Skeleton.flipX.Equals(true)) // 캐릭터가 왼쪽을 바라볼 경우
        {
            Helper.skeleton.flipX = true;
            for (float i = 1f; i >= 0; i -= (climbSpeed + 0.01f))
            {
                tr.Translate((Vector2.up * 5.0f * climbSpeed));
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
        rb.isKinematic = false;
    }
    

    public void CheckFloor()
    {
        isFloor = false;
        // 바닥 체크 ================================================
        // 아래쪽 Raycast
        // 바닥체크 레이어 FLOOR, CLIMBFLOOR, ROPERING, CRAWLING
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);
        if (hitDown.collider.CompareTag("FLOOR") || hitDown.collider.CompareTag("CLIMBFLOOR"))
        {
            isFloor = true;
            isRopeAct = false;
            isCrawl = false;
            return;
        }
        if (hitDown.collider.CompareTag("CRAWL"))
        {
            isFloor = false;
            isCrawl = true;
        }
    }

    public void CheckClimbRight()
    {
        // 오르기 체크 ===============================================
        // 오른쪽 Raycast
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + (Vector3.up * 1.0f) + (Vector3.right * 2.0f), Vector2.right, -4.0f, 1 << 9);

        if (hitRight.collider.CompareTag("CLIMBFLOOR"))
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isAct)
            {
                if (Helper.Skeleton.flipX.Equals(true))
                {
                    if (hitRight.collider.gameObject.transform.position.x > transform.position.x)// 충돌체가 캐릭터보다 오른쪽에 있다면
                    {
                        hangPoint = (hitRight.collider.bounds.min) + (Vector3.up * hitRight.collider.bounds.size.y) - (Vector3.up * 3.0f);

                        StartCoroutine(HANGING());
                    }

                }
                if (Helper.skeleton.flipX.Equals(false))
                {
                    if (hitRight.collider.gameObject.transform.position.x < transform.position.x)// 충돌체가 캐릭터보다 오른쪽에 있다면
                    {
                        hangPoint = (hitRight.collider.bounds.max) + (Vector3.up * hitRight.collider.bounds.size.y) - (Vector3.up * 6.0f);

                        StartCoroutine(HANGING());
                    }
                }

            }
        }

       
    }

    // 스파인 애니메이션 =========================================
    public void SetAnimation(string name, bool loop, float speed)
    {
        if (name.Equals(animationName)) { return; }
        else
        {
            Helper.skeleton.SetToSetupPose(); 
            Helper.state.SetAnimation(0, name, loop).timeScale = speed;
            animationName = name;
        }
    }
    //===========================================================
}

