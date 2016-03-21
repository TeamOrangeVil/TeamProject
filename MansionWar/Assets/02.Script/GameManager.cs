using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // 어떤 던전이든 끼우면 적용 가능함 (이라고 예상하고 싶음)
    // 입구 콜리더와 맵 바닥 죽음 확인

    public GameObject colliderUp;
    public GameObject colliderDown;
    public GameObject colliderRight;
    public GameObject colliderLeft;
    // 게임 맵 바닥
    public Transform Floor_M;
    // 화면을 가려줄 오브젝트
    public GameObject fadeBoard;
    // 몬스터의 수를 확인하고 문을 열기위한 체크변수
    public bool doorHitUp = false;
    public bool doorHitDown = false;
    public bool doorHitRight = false;
    public bool doorHitLeft = false;

    public bool isDie = false;

   // public GameObject Manager_g;

    private static GameManager gInstance = null;

    public static GameManager Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }

    void Awake()
    {
        gInstance = this;
    }

    void Start()
    {
        StartCoroutine(MonsterCount());
    }

    IEnumerator MonsterCount()
    {
        while (!isDie)
        {
            //현재 생성된 몬스터 개수 산출
            var monsterCount = GameObject.FindGameObjectsWithTag("Monster");

            if (monsterCount.Length == 0)
            {
                //Manager_g.GetComponentInChildren<Collider>().enabled = true;
                colliderDown.GetComponent<Collider>().enabled = true;
                colliderUp.GetComponent<Collider>().enabled = true;
                colliderRight.GetComponent<Collider>().enabled = true;
                colliderLeft.GetComponent<Collider>().enabled = true;

                RaycastHit hit;
                if (Physics.Raycast(colliderUp.transform.position + (transform.forward * 40.0f), Vector3.forward, out hit, 60.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitUp = true;
                    }
                }
                if (Physics.Raycast(colliderDown.transform.position - (transform.forward * 40.0f), -Vector3.forward, out hit, 60.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitDown = true;
                    }
                }
                if (Physics.Raycast(colliderRight.transform.position , Vector3.right, out hit, 60.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitRight = true;
                    }
                }
                if (Physics.Raycast(colliderLeft.transform.position, -Vector3.right, out hit, 60.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitLeft = true;
                    }
                }
            }
            else
            {
                //Manager_g.GetComponentInChildren<Collider>().enabled = false;
                colliderDown.GetComponent<Collider>().enabled = false;
                colliderUp.GetComponent<Collider>().enabled = false;
                colliderRight.GetComponent<Collider>().enabled = false;
                colliderLeft.GetComponent<Collider>().enabled = false;
                doorHitDown = false;
                doorHitUp = false;
                doorHitLeft = false;
                doorHitRight = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    // 캐릭터 맵 이동 시 위치 이동
    // 맵 이동 시 맵 On/Off 기능
    // 맵 이동 시 FadeIn/Out 기능
    // 맵은 미리 펼쳐져 있음
    // 모든 인게임에 적용하려면 맵은 미리 펼쳐져 있고 레이 캐스트로 확인해서 있으면 이동 없으면 이동불가

    // 맵 표시 map 속성 숫자(X) 속성 숫자(Z)
    // 속성 + Add A, - Subtract S, X Unchange U 
    // ex) mapA3S3  설명 map00을 기준으로 앞으로 3번째 아래로 3번째에 위치함
    

}
