using UnityEngine;
using System.Collections;

public class CharacterRaycast : MonoBehaviour {


    public void Update()
    {
        CheckFloor();
        CheckClimb();
    }
    public void CheckFloor()
    {
        CharacterController2D.Instance.isFloor = false;
        // 바닥 체크 ================================================
        // 아래쪽 Raycast
        // 바닥체크 레이어 FLOOR, CLIMBFLOOR, ROPERING, CRAWLING
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);
        
        if(hitDown.collider !=null)
        {
            if (hitDown.collider.CompareTag("FLOOR") || hitDown.collider.CompareTag("CLIMBFLOOR"))
            {
                // 땅을 밟고 있다면
                CharacterController2D.Instance.isFloor = true;
                CharacterController2D.Instance.isRopeAct = false;
                CharacterController2D.Instance.isCrawl = false;
                return;
            }
            if (hitDown.collider.CompareTag("ROPERING")) // 충돌한 오브젝트 태그가 ROPERING이면
            {
                CharacterController2D.Instance.ropeActPoint = hitDown.collider.gameObject.transform.position - CharacterController2D.Instance.RopeInHand.transform.position; //- transform.position; // 충돌한 게임 오브젝트의 위치값과 현재 위치값 비교
                CharacterController2D.Instance.ropeRingPoint = hitDown.collider.gameObject.transform.position;
                CharacterController2D.Instance.isRopeAct = true;
                CharacterController2D.Instance.isFloor = true;
                CharacterController2D.Instance.isCrawl = false;
                return;
            }
            if (hitDown.collider.CompareTag("CRAWL"))
            {
                CharacterController2D.Instance.isFloor = false;
                CharacterController2D.Instance.isCrawl = true;
            }
        }
        
        
    }

    public void CheckClimb()
    {
        // 오르기 체크 ===============================================
        // 오른쪽 Raycast
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + (Vector3.up * 1.0f) + (Vector3.right * 1.5f), Vector2.right, -3f, 1 << 9);
        if(hitRight.collider != null)
        {
            if (hitRight.collider.CompareTag("CLIMBFLOOR") && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("클라임 충돌은 됨여");

                if (CharacterController2D.Instance.Player.Skeleton.flipX.Equals(true) && !CharacterController2D.Instance.isHang && hitRight.collider.gameObject.transform.position.x > transform.position.x)
                {
                    CharacterController2D.Instance.hangPoint = (hitRight.collider.bounds.min) + (Vector3.up * hitRight.collider.bounds.size.y) - (Vector3.up * 3.0f);
                    CharacterController2D.Instance.StartCoroutine("HANGING");
                }
                if (CharacterController2D.Instance.Player.skeleton.flipX.Equals(false) && !CharacterController2D.Instance.isHang && hitRight.collider.gameObject.transform.position.x < transform.position.x)
                {
                    CharacterController2D.Instance.hangPoint = (hitRight.collider.bounds.max) + (Vector3.up * hitRight.collider.bounds.size.y) - (Vector3.up * 6.0f);
                    CharacterController2D.Instance.StartCoroutine("HANGING");
                }
            }
        }


    }
}
