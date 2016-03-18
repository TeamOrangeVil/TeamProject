using UnityEngine;
using System.Collections;

public class forblog : MonoBehaviour {
    //스켈레톤 애니메이션
    public SkeletonAnimation Player;
    //현재 실행 중인 애니메이션 이름
    private string cur_animation = "";
    //움직임을 제한하는 변수 선언
    public bool limit_move = false;
    //캐릭터의 Transform 컴포넌트 추가를 위한 변수 선언
    public Transform tr;
    //캐릭터의 움직임을 넣을 변수 선언
    public Vector3 movement;
    //캐릭터의 Horizontal 방향 값 변수 선언
    public float h = 0.0f;
    public float v = 0.0f;
    //캐릭터의 속도
    public float walkSpeed = 15.0f;
    public float jumpSpeed = 0.1f;
    public bool isJump = false;

    bool attacking = false;
    float attackTimer = 0;
    float attackCd = 0.4f;

    public Collider atkTrigger;

    void Awake()
    {
        //Player의 컴포넌트
        tr = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        
        //움직임이 제한되지 않을 경우
        if(!limit_move)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJump = false;
                TransformLimit();
                //Jumping();
            }
            // 키보드 A,D 값을 변수에 넣어준다.
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            //만약 h 값이 0보다 클 경우
            if (h>0)
            {
                /*Vector2 tiling = transform.localScale;
                tiling.x = -Mathf.Abs(tiling.x);
                transform.localScale = tiling;*/
                //transform.localRotation = Quaternion.Euler(0, 180, 0);
                Player.skeleton.flipX = true;
                TransformLimit();
                //애니메이션
                SetAnimation("run", true, 1.0f);
            }
            else if(h<0)
            {
                /* Vector2 tiling = transform.localScale;
                 tiling.x = Mathf.Abs(tiling.x);
                 transform.localScale = tiling;*/
                //transform.localRotation = Quaternion.Euler(0, 0, 0);
                Player.skeleton.flipX = false;
                TransformLimit();
                //애니메이션
                SetAnimation("run", true, 1.0f);
            }
            else if (v > 0)
            {
                TransformLimit();
            }
            else if (v < 0)
            {
                TransformLimit();
            }
            else
            {
                //SetAnimation("idle", true, 1.0f);
            }
        }
    }

    public void TransformLimit()
    {
        movement.Set(h, 0, v);
        tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10.0f, 10.0f), Mathf.Clamp(transform.position.y, -8.0f, 8.0f), Mathf.Clamp(transform.position.z, -8.0f, 8.0f));
    }
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
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "MOBATK")
        {
            Debug.Log("으앙아픔");
            StartCoroutine(MonsterAtk(other));
        }
    }
    public IEnumerator MonsterAtk(Collider coll)
    {
        coll.enabled = false;
        yield return new WaitForSeconds(1.5f);
        coll.enabled = true;
        yield return new WaitForSeconds(1f);
    }
}
