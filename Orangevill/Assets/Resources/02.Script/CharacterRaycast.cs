using UnityEngine;
using System.Collections;

public class CharacterRaycast : MonoBehaviour {

    public GameObject needle;
    public float dir=1;

    public void Update()
    {
        CheckFloor();
        CheckClimb();
    }
    public void CheckFloor()
    {
        //Debug.DrawRay(transform.position + (Vector3.right * 0.32f), Vector2.down, Color.red);
        //Debug.DrawRay(transform.position - (Vector3.right * 0.32f), Vector2.down, Color.red);
        CharacterController2D.Instance.isFloor = false;
        // 바닥 체크 ================================================
        // 아래쪽 Raycast
        // 바닥체크 레이어 FLOOR, CLIMBFLOOR, ROPERING, CRAWLING
        RaycastHit2D hitDown1 = Physics2D.Raycast(transform.position + (Vector3.right * 0.32f), Vector2.down, 0.1f, 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);
        RaycastHit2D hitDown2 = Physics2D.Raycast(transform.position - (Vector3.right * 0.32f), Vector2.down, 0.1f, 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);

        if (hitDown1.collider != null)
        {
            if (hitDown1.collider.CompareTag("FLOOR") || hitDown1.collider.CompareTag("CLIMBFLOOR")) // 땅을 밟고 있다면
            {
                CharacterController2D.Instance.isFloor = true;
                CharacterController2D.Instance.isRopeAct = false;
                CharacterController2D.Instance.isCrawl = false;
                CharacterController2D.Instance.jumpEffect.SetActive(false);
                return;
            }
            if (hitDown1.collider.CompareTag("ROPERING")) // 충돌한 오브젝트 태그가 ROPERING이면
            {
                CharacterController2D.Instance.ropeActPoint = hitDown1.collider.gameObject.transform.position - CharacterController2D.Instance.RopeInHand.transform.position; //- transform.position; // 충돌한 게임 오브젝트의 위치값과 현재 위치값 비교
                CharacterController2D.Instance.ropeRingPoint = hitDown1.collider.gameObject.transform.position;
                CharacterController2D.Instance.isRopeAct = true;
                CharacterController2D.Instance.isFloor = true;
                CharacterController2D.Instance.isCrawl = false;
                CharacterController2D.Instance.jumpEffect.SetActive(false);
                if (hitDown1.collider.gameObject.transform.position.x > transform.position.x)
                {
                    dir = 1;
                    needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                    needle.transform.localPosition =  Vector3.right * 1.8f + Vector3.up * 2.77f;

                    CharacterController2D.Instance.isRopePosition = true;
                    //CharacterController2D.Instance.Player.skeleton.flipX = true;
                }
                else
                {
                    dir = -1;
                    needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                    needle.transform.localPosition = Vector3.right * -2f + Vector3.up * 2.77f;

                    CharacterController2D.Instance.isRopePosition = false;
                    //CharacterController2D.Instance.Player.skeleton.flipX = false;
                }
                return;
            }
            if (hitDown1.collider.CompareTag("CRAWL"))
            {
                CharacterController2D.Instance.isFloor = false;
                CharacterController2D.Instance.isCrawl = true;
            }
        }
        else
        {
            if (hitDown2.collider != null)
            {
                if (hitDown2.collider.CompareTag("FLOOR") || hitDown2.collider.CompareTag("CLIMBFLOOR"))
                {
                    // 땅을 밟고 있다면
                    CharacterController2D.Instance.isFloor = true;
                    CharacterController2D.Instance.isRopeAct = false;
                    CharacterController2D.Instance.isCrawl = false;
                    CharacterController2D.Instance.jumpEffect.SetActive(false);
                    return;
                }
                if (hitDown2.collider.CompareTag("ROPERING")) // 충돌한 오브젝트 태그가 ROPERING이면
                {
                    CharacterController2D.Instance.ropeActPoint = hitDown2.collider.gameObject.transform.position - CharacterController2D.Instance.RopeInHand.transform.position; //- transform.position; // 충돌한 게임 오브젝트의 위치값과 현재 위치값 비교
                    CharacterController2D.Instance.ropeRingPoint = hitDown2.collider.gameObject.transform.position;
                    CharacterController2D.Instance.isRopeAct = true;
                    CharacterController2D.Instance.isFloor = true;
                    CharacterController2D.Instance.isCrawl = false;
                    CharacterController2D.Instance.jumpEffect.SetActive(false);
                    if (hitDown1.collider.gameObject.transform.position.x > transform.position.x)
                    {
                        dir = 1;
                        needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                        needle.transform.localPosition = Vector3.right * 1.8f + Vector3.up * 2.77f;

                        CharacterController2D.Instance.isRopePosition = true;
                        //CharacterController2D.Instance.Player.skeleton.flipX = true;
                    }
                    else
                    {
                        dir = -1;
                        needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                        needle.transform.localPosition = Vector3.right * -2f + Vector3.up * 2.77f;

                        CharacterController2D.Instance.isRopePosition = false;
                        //CharacterController2D.Instance.Player.skeleton.flipX = false;
                    }
                    return;
                }
                if (hitDown2.collider.CompareTag("CRAWL"))
                {
                    CharacterController2D.Instance.isFloor = false;
                    CharacterController2D.Instance.isCrawl = true;
                }
            }
        }
    }

    public void CheckClimb()
    {
        // 오르기 체크 ===============================================
        // 오른쪽 Raycast
        RaycastHit2D hitRight1 = Physics2D.Raycast(transform.position + (Vector3.up * 0.8f) + (Vector3.right * 1.5f), Vector2.right, -3f, 1 << 9);
        RaycastHit2D hitRight2 = Physics2D.Raycast(transform.position + (Vector3.up * 2.4f) + (Vector3.right * 1.5f), Vector2.right, -3f, 1 << 9);
        if (hitRight1.collider != null && hitRight1.collider.CompareTag("CLIMBFLOOR") && Input.GetKeyDown(KeyCode.Space))
        {

            if (CharacterController2D.Instance.Player.Skeleton.flipX.Equals(true) && !CharacterController2D.Instance.isHang && hitRight1.collider.gameObject.transform.position.x > transform.position.x)
            {
                CharacterController2D.Instance.hangPoint = (hitRight1.collider.bounds.min) + (Vector3.up * hitRight1.collider.bounds.size.y) - (Vector3.up * 3.0f);
                CharacterController2D.Instance.StartCoroutine("HANGING");
            }
            if (CharacterController2D.Instance.Player.skeleton.flipX.Equals(false) && !CharacterController2D.Instance.isHang && hitRight1.collider.gameObject.transform.position.x < transform.position.x)
            {
                CharacterController2D.Instance.hangPoint = (hitRight1.collider.bounds.max) - (Vector3.up * 3.0f);
                CharacterController2D.Instance.StartCoroutine("HANGING");
            }
        }
        else
        {
            if (hitRight2.collider != null && hitRight2.collider.CompareTag("CLIMBFLOOR") && Input.GetKeyDown(KeyCode.Space))
            {

                if (CharacterController2D.Instance.Player.Skeleton.flipX.Equals(true) && !CharacterController2D.Instance.isHang && hitRight2.collider.gameObject.transform.position.x > transform.position.x)
                {
                    CharacterController2D.Instance.hangPoint = (hitRight2.collider.bounds.min) + (Vector3.up * hitRight2.collider.bounds.size.y) - (Vector3.up * 3.0f);
                    CharacterController2D.Instance.StartCoroutine("HANGING");
                }
                if (CharacterController2D.Instance.Player.skeleton.flipX.Equals(false) && !CharacterController2D.Instance.isHang && hitRight2.collider.gameObject.transform.position.x < transform.position.x)
                {
                    CharacterController2D.Instance.hangPoint = (hitRight2.collider.bounds.max) - (Vector3.up * 3.0f);
                    CharacterController2D.Instance.StartCoroutine("HANGING");
                }

            }
        }
    }
}
