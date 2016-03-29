using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DungeunDoorManager : MonoBehaviour
{

    public GameObject doorUp;
    public GameObject doorDown;
    public GameObject doorRight;
    public GameObject doorLeft;
    //플레이어
    public Transform Player;
    // 화면을 가려줄 오브젝트
    public GameObject fadeBoard;
    // 몬스터의 수를 확인하고 문을 열기위한 체크변수
    public bool doorHitUp = false;
    public bool doorHitDown = false;
    public bool doorHitRight = false;
    public bool doorHitLeft = false;

    public int floorX = 0;
    public int floorY = 0;

    public bool isDie = false;

    public GameObject Floor_M;
    public GameObject Floor3D1;
    public GameObject Floor3D2;
    public GameObject Floor3D3;

    public GameObject Alltree;

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
        Player = GameObject.Find("Player").GetComponent<Transform>();
        StartCoroutine(MonsterCount());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("충돌해따!");
            if ((other.transform.position.x - this.transform.position.x) > 15)
            {
                if (doorHitRight == true)
                {
                    DoorRightOpen();
                }
            }
            if ((other.transform.position.x - this.transform.position.x) < -15)
            {
                if (doorHitLeft == true)
                {
                    DoorLeftOpen();
                    
                }
            }
            if((other.transform.position.x - this.transform.position.x) < 10 &&
                (other.transform.position.z - this.transform.position.z) > 5.5 )
            {
                if (doorHitUp == true) { DoorUpOpen(); }
            }
            else
            {
                if(doorHitDown == true) { DoorDownOpen(); }
            }
        }
    }

        IEnumerator MonsterCount()
    {
        while (!isDie)
        {
            Debug.Log("몬스터 확인!");

            
            var monsterCount = GameObject.FindGameObjectsWithTag("Monster");
            Debug.Log("몬스터 수" +monsterCount.Length);
            if (monsterCount.Length == 0)
                //if (monsterCount == 0) // 몬스터가 한마리도 없다면
            {
                Debug.Log("문을 열어라!");
                // 모든 문을 연다.
                //doorDown.GetComponent<Collider>().enabled = true;
                //doorUp.GetComponent<Collider>().enabled = true;
                doorRight.GetComponent<Collider>().enabled = true;
                doorLeft.GetComponent<Collider>().enabled = true;

                //doorHitDown = true;
                doorHitLeft = true;
                //doorHitUp = true;
                doorHitRight = true;
                
            }
            else // 문을 잠근다.
            {
                //doorDown.GetComponent<Collider>().enabled = false;
                //doorUp.GetComponent<Collider>().enabled = false;
                doorRight.GetComponent<Collider>().enabled = false;
                doorLeft.GetComponent<Collider>().enabled = false;
                //doorHitDown = false;
                //doorHitUp = false;
                doorHitLeft = false;
                doorHitRight = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
   
    // 입구 열기 함수
    public void DoorUpOpen() // 위쪽 문 출입
    {
        
            floorY += 1; // 미니맵 위치 값 저장
            //Floor_M.transform.position -= new Vector3(70.0f, 0, 0); // 터레인의 이동
            //doorHitDown = false;
            //doorHitUp = false;
            doorHitLeft = false;
            doorHitRight = false;

    }
    public void DoorDownOpen() // 아래쪽 문 출입
    {
        if (doorHitDown == true)
        {
            floorY -= 1;
            //Floor_M.transform.position -= new Vector3(70.0f, 0, 0);
            //doorHitDown = false;
            //doorHitUp = false;
            doorHitLeft = false;
            doorHitRight = false;
        }
    }
    public void DoorRightOpen() // 오른쪽 문 출입
    {
        if (doorHitRight == true)
        {
            floorX += 1;
            Floor_M.transform.position -= new Vector3(41.5f, 0, 0);
            Player.transform.position -= new Vector3(35.0f, 0, 0);
            Alltree.transform.position -= new Vector3(5.0f, 0, 0);
            //doorHitDown = false;
            //doorHitUp = false;
            doorHitLeft = false;
            doorHitRight = false;
            //this.transform.position += new Vector3(40, 0, 0);
            //Player.transform.position += new Vector3(6, 0, 0);
            Debug.Log("오른쪼");
        }
    }
    public void DoorLeftOpen() // 왼쪽 문 출입
    {
        if (doorHitLeft == true)
        {
            floorX -= 1;
            Floor_M.transform.position += new Vector3(41.5f, 0, 0);
            Player.transform.position += new Vector3(35.0f, 0, 0);
            Alltree.transform.position += new Vector3(10.0f, 0, 0);
            //Floor3D1.transform.position += new Vector3(40.0f, 0, 0);
            //Floor3D2.transform.position += new Vector3(40.0f, 0, 0);
            //Floor3D3.transform.position += new Vector3(40.0f, 0, 0);
            //doorHitDown = false;
            //doorHitUp = false;
            doorHitLeft = false;
            doorHitRight = false;
            //this.transform.position += new Vector3(-40, 0, 0);
            //Player.transform.position += new Vector3(6, 0, 0);
            Debug.Log("왼쪼");
        }
    }

}
