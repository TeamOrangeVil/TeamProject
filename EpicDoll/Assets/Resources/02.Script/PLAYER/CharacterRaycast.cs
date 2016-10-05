using UnityEngine;
using System.Collections;

public class CharacterRaycast : MonoBehaviour
{
    public GameObject needle; // 바늘
    public float dir;
    public float climbDir = 1; // 좌우 체크
    public void Update()
    {
        CheckFloor(); // 바닥 확인 함수
        CheckClimb(); // 오르기 가능 확인 함수
        Debug.DrawRay(transform.position + (Vector3.up * 2.4f) + (Vector3.right * 1.5f * climbDir), Vector2.down, Color.red, 1.6f);
    }
    public void CheckFloor()
    {
        CharacterController2D.Instance.isFloor = false;
        // 바닥 체크 ================================================
        // 아래쪽 Raycast
        // 바닥체크 레이어 FLOOR, CLIMBFLOOR, ROPERING, CRAWLING
        RaycastHit2D hitDown1 = Physics2D.Raycast(transform.position + (Vector3.up * 0.1f) + (Vector3.right * 0.25f), Vector2.down, 0.2f, 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);
        RaycastHit2D hitDown2 = Physics2D.Raycast(transform.position + (Vector3.up * 0.1f) - (Vector3.right * 0.25f), Vector2.down, 0.2f, 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);

        if (hitDown1.collider != null)
        {
            if (hitDown1.collider.CompareTag("ROPERING")) // 충돌한 오브젝트 태그가 ROPERING이면
            {
                UIManager.Instance.HintAlarmR.enabled = true; // 상호작용 표시
                CharacterController2D.Instance.isRopeAct = true; // 로프액션 가능 상태
                CharacterController2D.Instance.isFloor = true; // 바닥 확인
                CharacterController2D.Instance.isCrawl = false; // 기어가기 불가
                CharacterController2D.Instance.jumpEffect.SetActive(false); // 점프 이펙트 제거
                CharacterController2D.Instance.ropeActPoint = hitDown1.collider.gameObject.transform.position - CharacterController2D.Instance.RopeInHand.transform.position; // 충돌한 게임 오브젝트의 위치값과 현재 위치값 비교
                CharacterController2D.Instance.ropeRingPoint = hitDown1.collider.gameObject.transform.position; // 로프를 걸 오브젝트의 위치
                if (hitDown1.collider.gameObject.transform.position.x > transform.position.x) // 로프를 걸 오브젝트가 캐릭터보다 오른쪽에 있을 경우
                {
                    dir = 1;
                    needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                    needle.transform.localPosition = Vector3.right * 1.8f + Vector3.up * 2.77f;

                    CharacterController2D.Instance.isRopePosition = true;
                }
                else
                {
                    dir = -1;
                    needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                    needle.transform.localPosition = Vector3.right * -2f + Vector3.up * 2.77f;

                    CharacterController2D.Instance.isRopePosition = false;
                }
                return;
            }
            else
            {
                UIManager.Instance.HintAlarmR.enabled = false; // 상호작용 R키 Off
            }
            if (hitDown1.collider.CompareTag("FLOOR") || hitDown1.collider.CompareTag("CLIMBFLOOR")) // 땅을 밟고 있다면
            {
                CharacterController2D.Instance.isFloor = true; // 바닥 확인
                CharacterController2D.Instance.isRopeAct = false; // 로프 액션 불가
                CharacterController2D.Instance.isCrawl = false; //기어가기 불가
                CharacterController2D.Instance.jumpEffect.SetActive(false); // 점프 이펙트 제거
                return;
            }

            if (hitDown1.collider.CompareTag("CRAWL")) // 기어갈 곳이라면
            {
                CharacterController2D.Instance.isFloor = false;
                CharacterController2D.Instance.isCrawl = true; // 기어가기 가능
                CharacterController2D.Instance.jumpEffect.SetActive(false); // 점프 이펙트 제거
            }
        }
        else if (hitDown2.collider != null)
        {
            if (hitDown2.collider.CompareTag("ROPERING")) // 충돌한 오브젝트 태그가 ROPERING이면
            {
                UIManager.Instance.HintAlarmR.enabled = true;
                CharacterController2D.Instance.ropeActPoint = hitDown2.collider.gameObject.transform.position - CharacterController2D.Instance.RopeInHand.transform.position; //- transform.position; // 충돌한 게임 오브젝트의 위치값과 현재 위치값 비교
                CharacterController2D.Instance.ropeRingPoint = hitDown2.collider.gameObject.transform.position;
                CharacterController2D.Instance.isRopeAct = true;
                CharacterController2D.Instance.isFloor = true;
                CharacterController2D.Instance.isCrawl = false;
                CharacterController2D.Instance.jumpEffect.SetActive(false);
                if (hitDown2.collider.gameObject.transform.position.x > transform.position.x)
                {
                    dir = 1;
                    needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                    needle.transform.localPosition = Vector3.right * 1.8f + Vector3.up * 2.77f;

                    CharacterController2D.Instance.isRopePosition = true;
                }
                else
                {
                    dir = -1;
                    needle.transform.localRotation = Quaternion.Euler(0, 0, dir * 143.68f);
                    needle.transform.localPosition = Vector3.right * -2f + Vector3.up * 2.77f;

                    CharacterController2D.Instance.isRopePosition = false;
                }
                return;
            }
            else
            {
                UIManager.Instance.HintAlarmR.enabled = false;
            }
            if (hitDown2.collider.CompareTag("FLOOR") || hitDown2.collider.CompareTag("CLIMBFLOOR"))
            {
                // 땅을 밟고 있다면
                CharacterController2D.Instance.isFloor = true;
                CharacterController2D.Instance.isRopeAct = false;
                CharacterController2D.Instance.isCrawl = false;
                CharacterController2D.Instance.jumpEffect.SetActive(false);
                return;
            }

            if (hitDown2.collider.CompareTag("CRAWL"))
            {
                CharacterController2D.Instance.isFloor = false;
                CharacterController2D.Instance.isCrawl = true;
                CharacterController2D.Instance.jumpEffect.SetActive(false);
            }
        }
        else
        {
            UIManager.Instance.HintAlarmR.enabled = false;
        }
    }

    public void CheckClimb() // 오르기 체크
    {
        //FlipX가 True이면 오른쪽을 바라보기 때문에 1, 아니라면 -1
        climbDir = CharacterController2D.Instance.Player.skeleton.FlipX == true ? 1 : -1; 

        // 오를수 있는 벽인지 체크, 스파인 애니메이션 캐릭터의 중심좌표가 발 아래이기 때문에  
        RaycastHit2D hitClimb = Physics2D.Raycast(transform.position + (Vector3.up * 2.4f) + (Vector3.right * 1.5f * climbDir), Vector2.down, 1.6f, 1 << 9);

        // 충돌체크가 되고, 해당 오브젝트의 태그가 CLIMBFLOOR 일 경우
        if (hitClimb.collider != null && hitClimb.collider.CompareTag("CLIMBFLOOR")) 
        {
            if (!CharacterController2D.Instance.isClimb) // isClimb이 false 일 경우
            {
                UIManager.Instance.HintAlarmSpace.enabled = true; // UI 스페이스 이미지 출력
            }
            // 스페이스바를 누르고, 행동 제한이 없고, 매달리지 않았을 경우
            if (Input.GetKeyDown(KeyCode.Space) && !CharacterController2D.Instance.isAct && !CharacterController2D.Instance.isHang) 
            {
                // 스파인 모델이 FlipX가 True이고 충돌되는 오브젝트가 캐릭터 좌표보다 크면
                if (CharacterController2D.Instance.Player.Skeleton.FlipX.Equals(true) && hitClimb.collider.gameObject.transform.position.x > transform.position.x) 
                {
                    // 충돌된 오브젝트의 오른쪽 위 가장자리의 높이를 저장한다
                    CharacterController2D.Instance.hangPoint = (hitClimb.collider.bounds.min) + (Vector3.up * hitClimb.collider.bounds.size.y) - (Vector3.up * 2.3f); 
                    CharacterController2D.Instance.StartCoroutine("HANGING"); // 매달리기 작동
                }
                else
                {
                    UIManager.Instance.HintAlarmSpace.enabled = false; // UI 스페이스 이미지 미출력
                }
                if (CharacterController2D.Instance.Player.skeleton.FlipX.Equals(false) && hitClimb.collider.gameObject.transform.position.x < transform.position.x)
                {
                    CharacterController2D.Instance.hangPoint = (hitClimb.collider.bounds.max) - (Vector3.up * 2.3f);
                    CharacterController2D.Instance.StartCoroutine("HANGING");
                }
                else
                {
                    UIManager.Instance.HintAlarmSpace.enabled = false;
                }
            }
        }
        else
        {
            UIManager.Instance.HintAlarmSpace.enabled = false;
        }
    }
}


