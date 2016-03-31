using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
    public SkeletonAnimation Player;  //스켈레톤 애니메이션
    
    private string cur_animation = ""; //현재 실행 중인 애니메이션 이름
    
    public bool limit_move = false; //움직임을 제한하는 변수
    
    public Transform tr; //캐릭터의 Transform 컴포넌트 추가를 위한 변수 선언
    public Rigidbody2D rb;
    
    public Vector3 movement; //캐릭터의 움직임을 넣을 변수 선언

    //캐릭터의 방향
    public float h = 0.0f;
    public float v = 0.0f;

    //캐릭터의 속도
    private float walkSpeed = 6.0f;
    private float jumpPower = 4.0f;
    public bool isJump = false;

    void Awake()
    {
        tr = GetComponent<Transform>(); //Player의 컴포넌트
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!limit_move) //움직임이 제한되지 않을 경우
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            if (!isJump)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rb.AddForce(Vector2.up * jumpPower * 100.0f);
                    isJump = true;
                }
            }
            if (h > 0) //만약 h 값이 0보다 클 경우
            {
                SetAnimation("run", true, 1.0f);
                TransformLimit();
                Player.Skeleton.flipX = false;
                if (!isJump)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        rb.AddForce(Vector2.up * jumpPower * 100.0f);
                        isJump = true;
                    }
                }
            }
            else if (h < 0)
            {
                SetAnimation("run", true, 1.0f);
                TransformLimit();
                Player.Skeleton.flipX = true;
                if (!isJump)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        rb.AddForce(Vector2.up * jumpPower * 100.0f);
                        isJump = true;
                    }
                }
            }
            else { SetAnimation("STAY", true, 1.0f); }
        }
    }
    // 캐릭터 움직임
    public void TransformLimit() 
    {
        movement.Set(h, 0,0);
        tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World);
    }

    // 스파인 애니메이션 관련 함수
    void SetAnimation(string name, bool loop, float speed)
    {
        if(name == cur_animation)
        {
            return;
        }
        else
        {
            Player.state.SetAnimation(0, name, loop).timeScale = speed;
            cur_animation = name;
        }
    }

    // 충돌 체크
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Floor"))
        {
            isJump = false;

        }
    }
    

}