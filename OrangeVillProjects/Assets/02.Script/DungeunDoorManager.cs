using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DungeunDoorManager : MonoBehaviour
{

    public GameObject doorUp;
    public GameObject doorDown;
    public GameObject doorRight;
    public GameObject doorLeft;
    // 게임 맵 바닥 상위 오브젝트
    public GameObject Floor_M;
    // 화면을 가려줄 오브젝트
    public GameObject fadeBoard;
    // 몬스터의 수를 확인하고 문을 열기위한 체크변수
    public bool doorHitUp = false;
    public bool doorHitDown = false;
    public bool doorHitRight = false;
    public bool doorHitLeft = false;

    public bool isDie = false;

    public Collider[] tempColiiders;

    public GameObject Player;

    private static DungeunDoorManager gInstance = null;

    public static DungeunDoorManager Instance
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
        Player = GameObject.Find("Player");
        // Manager_g = gameObject.GetComponentsInChildren<Collider>();
        tempColiiders = Floor_M.GetComponentsInChildren<Collider>();
    }

    IEnumerator MonsterCount()
    {
        while (!isDie)
        {
            //현재 생성된 몬스터 개수 산출
            int monsterCount = GameObject.FindGameObjectsWithTag("Monster").Length;
            //var monsterCount = GameObject.FindGameObjectsWithTag("Monster");
                //if (monsterCount.Length == 0)
                if (monsterCount == 0) // 몬스터가 한마리도 없다면
            {
                // 모든 문을 연다.
                doorDown.GetComponent<Collider>().enabled = true;
                doorUp.GetComponent<Collider>().enabled = true;
                doorRight.GetComponent<Collider>().enabled = true;
                doorLeft.GetComponent<Collider>().enabled = true;

                RaycastHit hit;
               
                // 위쪽으로 레이를 쏴서 맵이 존재할 경우 문을 연다.
                if (Physics.Raycast(doorUp.transform.position + (transform.forward * 40.0f), Vector3.forward, out hit, 10.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitUp = true;
                    }
                }
                // 아래쪽으로 레이를 쏴서 맵이 존재할 경우 문을 연다.
                if (Physics.Raycast(doorDown.transform.position - (transform.forward * 40.0f), -Vector3.forward, out hit, 10.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitDown = true;
                    }
                }
                // 오른쪽으로 레이를 쏴서 맵이 존재할 경우 문을 연다.
                if (Physics.Raycast(doorRight.transform.position , Vector3.right, out hit, 10.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitRight = true;
                    }
                }
                // 왼쪽으로 레이를 쏴서 맵이 존재할 경우 문을 연다.
                if (Physics.Raycast(doorLeft.transform.position, -Vector3.right, out hit, 10.0f))
                {
                    if (hit.collider.gameObject.CompareTag("Floor"))
                    {
                        doorHitLeft = true;
                    }
                }
            }
            else // 문을 잠근다.
            {
                /*for(int i = 0; i<4; i++)
                {
                    if(tempColiiders//name.Contains("Floor"))
                    {

                    }
                }*/
                doorDown.GetComponent<Collider>().enabled = false;
                doorUp.GetComponent<Collider>().enabled = false;
                doorRight.GetComponent<Collider>().enabled = false;
                doorLeft.GetComponent<Collider>().enabled = false;
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
