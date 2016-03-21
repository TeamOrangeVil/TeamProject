using UnityEngine;
using System.Collections;

public class forblog : MonoBehaviour {
   
    public SkeletonAnimation Player;  //스켈레톤 애니메이션
    
    private string cur_animation = ""; //현재 실행 중인 애니메이션 이름
    
    public bool limit_move = false; //움직임을 제한하는 변수 선언
    
    public Transform tr; //캐릭터의 Transform 컴포넌트 추가를 위한 변수 선언
    
    public Vector3 movement; //캐릭터의 움직임을 넣을 변수 선언
    
    //캐릭터의 Horizontal 방향 값 변수 선언
    public float h = 0.0f;
    public float v = 0.0f;
    //캐릭터의 속도
    public float walkSpeed = 15.0f;
    public float jumpSpeed = 0.1f;
    public bool isJump = false;
    
    void Awake()
    {
        tr = GetComponent<Transform>(); //Player의 컴포넌트
    }

    void FixedUpdate()
    {
        if(!limit_move) //움직임이 제한되지 않을 경우
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJump = false;
                TransformLimit();
            }
            if (h>0) //만약 h 값이 0보다 클 경우
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
                SetAnimation("stay", true, 1.0f);
            }
        }
    }
    // 움직임 제한
    public void TransformLimit() 
    {
        movement.Set(h, 0, v);
        tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -25.0f, 25.0f), Mathf.Clamp(transform.position.y, -8.0f, 8.0f), Mathf.Clamp(transform.position.z, -10.0f, 10.0f));
    }
    // 스파인 애니메이션 제한
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
    // 충돌 관련
    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Monster"))
        {
            //HpBar1.fillAmount -= 1f;
            // HpBar.fillDirection -= 1f;
            //HpBar.value -= 1.0f;
           DataManager.Instance.HpBar.value -= 0.1f;
        }
        if(other.CompareTag("DoorUp")) // 위로 충돌
        {
            if(GameManager.Instance.doorHitUp == true)
            {
                GameManager.Instance.Floor_M.transform.position -=  Vector3.forward * 80.0f;
                transform.position = new Vector3(0, 2.32f, -4.0f);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitLeft = false;
                GameManager.Instance.doorHitRight = false;
            }
        }
        if (other.CompareTag("DoorDown")) // 아래로 충돌
        {
            if (GameManager.Instance.doorHitDown == true)
            {
                GameManager.Instance.Floor_M.transform.position += Vector3.forward * 80.0f;
                transform.position = new Vector3(0, 2.32f, 4.0f);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitLeft = false;
                GameManager.Instance.doorHitRight = false;
            }
        }
        if (other.CompareTag("DoorRight")) // 오른쪽으로 충돌
        {
            if (GameManager.Instance.doorHitRight == true)
            {
                GameManager.Instance.Floor_M.transform.position -= new Vector3(70.0f, 0, 0);
                transform.position = new Vector3(-20.0f, 2.32f, 0);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitLeft = false;
                GameManager.Instance.doorHitRight = false;
            }
        }
        if (other.CompareTag("DoorLeft")) // 왼쪽으로 충돌
        {
            if (GameManager.Instance.doorHitLeft == true)
            {
                GameManager.Instance.Floor_M.transform.position += new Vector3(70.0f, 0, 0);
                transform.position = new Vector3(20.0f, 2.32f, 0);
                GameManager.Instance.doorHitDown = false;
                GameManager.Instance.doorHitUp = false;
                GameManager.Instance.doorHitLeft = false;
                GameManager.Instance.doorHitRight = false;
            }
        }
    }
}