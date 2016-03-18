using UnityEngine;
using System.Collections;

public class forblog : MonoBehaviour {
    
    public SkeletonAnimation Player; //스켈레톤 애니메이션
   
    private string cur_animation = "";  //현재 실행 중인 애니메이션 이름
   
    public bool limit_move = false;  //움직임을 제한하는 변수 선언
    
    public Transform tr; //캐릭터의 Transform 컴포넌트 추가를 위한 변수 선언
   
    public Vector3 movement;  //캐릭터의 움직임을 넣을 변수 선언
    
    //캐릭터의 Horizontal 방향 값 변수 선언
    public float h = 0.0f;
    public float v = 0.0f;
    
    //캐릭터의 속도
    public float walkSpeed = 15.0f;

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
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                TransformLimit();
            }*/

            // 키보드 A,D 값을 변수에 넣어준다.
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            //만약 방향 값이 0 보다 클 경우
            if (h>0)
            {
                Player.skeleton.flipX = true;
                TransformLimit();
                SetAnimation("run", true, 1.0f);
            }
            else if(h<0)
            {
                Player.skeleton.flipX = false;
                TransformLimit();
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
                // 추후 가만히 있을 때 다양한 동작 구현
                // SetAnimation("idle", true, 1.0f);
            }
        }
    }

    // 움직임 관련 함수
    public void TransformLimit()
    {
        movement.Set(h, 0, v);
        tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -25.0f, 25.0f), Mathf.Clamp(transform.position.y, -8.0f, 8.0f), Mathf.Clamp(transform.position.z, -10.0f, 10.0f));
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
    
    // 콜리더 충돌 관련 1. 입구 충돌
    void OnTriggerEnter(Collider other)
    {
        // 위로 충돌
        if(other.CompareTag("DoorUp"))
        {
            if(GameManager.Instance.doorHitUp == true)
            {
                GameManager.Instance.Floor_M.transform.position +=  new Vector3(0, 0, -40.0f);
                transform.position = new Vector3(0, 2.32f, -8.0f);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitRight = false;
                GameManager.Instance.doorHitLeft = false;
            }
        }
        // 아래로 충돌
        if (other.CompareTag("DoorDown"))
        {
            if (GameManager.Instance.doorHitDown == true)
            {
                GameManager.Instance.Floor_M.transform.position += new Vector3(0, 0, 40.0f);
                transform.position = new Vector3(0, 2.32f, 8.0f);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitRight = false;
                GameManager.Instance.doorHitLeft = false;
            }
        }
        // 오른쪽으로 충돌
        if (other.CompareTag("DoorRight"))
        {
            if (GameManager.Instance.doorHitRight == true)
            {
                GameManager.Instance.Floor_M.transform.position += new Vector3(-70.0f, 0, 0);
                transform.position = new Vector3(-20.0f, 2.32f, 0);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitRight = false;
                GameManager.Instance.doorHitLeft = false;
            }
        }
        // 왼쪽으로 충돌
        if (other.CompareTag("DoorLeft"))
        {
            if (GameManager.Instance.doorHitLeft == true)
            {
                GameManager.Instance.Floor_M.transform.position += new Vector3(70.0f, 0, 0);
                transform.position = new Vector3(20.0f, 2.32f, 0);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitRight = false;
                GameManager.Instance.doorHitLeft = false;
            }
        }
    }
}
