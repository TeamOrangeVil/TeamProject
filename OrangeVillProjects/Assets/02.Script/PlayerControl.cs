using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {
   
    public SkeletonAnimation Playerani;  //스켈레톤 애니메이션
    
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

    XML_Parsing xmlParsing;
    public GameObject Player;//게임 플레이어
    public Transform PlayerHand;//무기 위치 싱크를위한 손의 위치
    public GameObject[] MyWeapon;//가지고있는 조합템 배열
    public GameObject NowWeapon;//현재 무기
    public GameObject BefoWeapon;//무기 교체를 위한임시 
    public GameObject temp;//검색을 위해 설정
    public Collider Coll;
    public string bindName;
    public int atk;
    public bool isatk;
    public int WeaponSwitcher;//배열 이동을 위해 설정
    List<Bind_Info> BindList = new List<Bind_Info>();

    // Use this for initialization
    void Start()
    {
        WeaponSwitcher = 0;
        Coll = GetComponent<Collider>();
        Coll.enabled = false;
        xmlParsing = GameObject.Find("DataManager").GetComponent<XML_Parsing>();
        BindList = xmlParsing.BindDBListRead(Application.streamingAssetsPath + XmlConstancts.OBJBINDXML);
        PlayerHand = GameObject.Find("Hand").GetComponent<Transform>();
        MyWeapon = GetComponents<GameObject>();//배열임, 그래서 겟컴포넌s
        NowWeapon = GetComponent<GameObject>();
        BefoWeapon = GetComponent<GameObject>();

    }
     void Awake()
    {
        tr = GetComponent<Transform>(); //Player의 컴포넌트
        //if(GameManager.Instance.Player==null)
        
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

                //Player.skeleton.flipX = true;
                TransformLimit();
                //애니메이션
                //SetAnimation("run", true, 1.0f);
            }
            else if(h<0)
            {
                /* Vector2 tiling = transform.localScale;
                 tiling.x = Mathf.Abs(tiling.x);
                 transform.localScale = tiling;*/
                //transform.localRotation = Quaternion.Euler(0, 0, 0);

               // Player.skeleton.flipX = false;
                TransformLimit();
                //애니메이션
                //SetAnimation("run", true, 1.0f);
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
        if (Input.GetKeyDown(KeyCode.T))//조합키
        {
            StartCoroutine(CollAction());
            Debug.Log("뭐지");
        }
        if (Input.GetKeyDown(KeyCode.Q))//공격
        {
            Debug.Log("공격");
            isatk = true;
            StartCoroutine(CollAction());
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1");
            BefoWeapon = NowWeapon;
            BefoWeapon.SetActive(false);
            NowWeapon = MyWeapon[0];
            NowWeapon.SetActive(true);
            MyWeapon[0] = BefoWeapon;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))//임시 무기 스위치--
        {
            Debug.Log("2");
            BefoWeapon = NowWeapon;
            BefoWeapon.SetActive(false);
            NowWeapon = MyWeapon[1];
            NowWeapon.SetActive(true);
            MyWeapon[1] = BefoWeapon;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))//임시 무기 스위치--
        {
            Debug.Log("3");
            BefoWeapon = NowWeapon;
            BefoWeapon.SetActive(false);
            NowWeapon = MyWeapon[2];
            NowWeapon.SetActive(true);
            MyWeapon[2] = BefoWeapon;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(CollAction());
        }
            if (NowWeapon != null)
            NowWeapon.transform.position = PlayerHand.transform.position;//무기 위치
    }
    // 움직임 제한
    public void TransformLimit() 
    {
        movement.Set(h, 0, v);
        tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10.0f, 10.0f), Mathf.Clamp(transform.position.y, -6.0f, 6.0f), Mathf.Clamp(transform.position.z, 1.0f, 5.0f));
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
            //Player.state.SetAnimation(0, name, loop).timeScale = speed;
            cur_animation = name;
        }
    }
    // 충돌 관련

    void OnTriggerStay(Collider other)
    {
        Debug.Log("충돌 되는중!");
        if (other.tag == "Object" && isatk == false)
        {//공격 x 몬스터 x
            bindName = other.name;
            BindSerch();
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Monster" && NowWeapon.name == "Weapon_003(Clone)" && isatk == true)
        {//공격 o 몬스터 o
            Debug.Log("잡았다");
            other.gameObject.SetActive(false);
            QuestManager.Instance.QuestIsClear(other.gameObject.name);
        }
    }
    void BindSerch()//무슨 옵젝인지검색
    {
        for (int i = 0; i < BindList.Count; i++)
        {
            if (BindList[i].ID == bindName)
            {
                temp = GameObject.Find(BindList[i].MixResult);
                Debug.Log("앗 무기닷");
                Debug.Log(bindName);
                if (NowWeapon == null)//든 무기가 없으면
                {
                    NowWeapon = Instantiate(temp, PlayerHand.position, Quaternion.identity) as GameObject;
                    MyWeapon[WeaponSwitcher] = NowWeapon.gameObject;
                    if (WeaponSwitcher < 2)
                        WeaponSwitcher++;
                    else if (WeaponSwitcher > 0)
                        WeaponSwitcher--;
                }
                else
                {//이미 들고있는 무기가 있을 때
                    MyWeapon[WeaponSwitcher] = NowWeapon.gameObject;
                    MyWeapon[WeaponSwitcher].SetActive(false);
                    if (WeaponSwitcher < 2)
                        WeaponSwitcher++;
                    else if (WeaponSwitcher > 0)
                        WeaponSwitcher--;
                    NowWeapon = Instantiate(temp, PlayerHand.position, Quaternion.identity) as GameObject;
                }
                bindName = null;
            }
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("WarPortal"))
        {
            SceneManager.LoadScene("03");
        }
        if (other.CompareTag("VillPortal"))
        {
            SceneManager.LoadScene("ProtoVill");
        }
            if (other.CompareTag("Monster"))
        {
            //HpBar1.fillAmount -= 1f;
            //HpBar.fillDirection -= 1f;
            //HpBar.value -= 1.0f;
            //DataManager.Instance.HpBar.value -= 0.1f;
        }
        if(other.CompareTag("DoorUp")) // 위로 충돌
        {
            if(DungeunDoorManager.Instance.doorHitUp == true)
            {
                DungeunDoorManager.Instance.Floor_M.transform.position -=  Vector3.forward * 80.0f;
                transform.position = new Vector3(0, 2.32f, -4.0f);
                DungeunDoorManager.Instance.doorHitDown = false;
                DungeunDoorManager.Instance.doorHitUp = false;
                DungeunDoorManager.Instance.doorHitLeft = false;
                DungeunDoorManager.Instance.doorHitRight = false;
            }
        }
        if (other.CompareTag("DoorDown")) // 아래로 충돌
        {
            if (DungeunDoorManager.Instance.doorHitDown == true)
            {
                DungeunDoorManager.Instance.Floor_M.transform.position += Vector3.forward * 80.0f;
                transform.position = new Vector3(0, 2.32f, 4.0f);
                DungeunDoorManager.Instance.doorHitDown = false;
                DungeunDoorManager.Instance.doorHitUp = false;
                DungeunDoorManager.Instance.doorHitLeft = false;
                DungeunDoorManager.Instance.doorHitRight = false;
            }
        }
        if (other.CompareTag("DoorRight")) // 오른쪽으로 충돌
        {
            if (DungeunDoorManager.Instance.doorHitRight == true)
            {
                DungeunDoorManager.Instance.Floor_M.transform.position -= new Vector3(70.0f, 0, 0);
                transform.position = new Vector3(-20.0f, 2.32f, 0);
                DungeunDoorManager.Instance.doorHitDown = false;
                DungeunDoorManager.Instance.doorHitUp = false;
                DungeunDoorManager.Instance.doorHitLeft = false;
                DungeunDoorManager.Instance.doorHitRight = false;
            }
        }
        if (other.CompareTag("DoorLeft")) // 왼쪽으로 충돌
        {
            if (DungeunDoorManager.Instance.doorHitLeft == true)
            {
                DungeunDoorManager.Instance.Floor_M.transform.position += new Vector3(70.0f, 0, 0);
                transform.position = new Vector3(20.0f, 2.32f, 0);
                DungeunDoorManager.Instance.doorHitDown = false;
                DungeunDoorManager.Instance.doorHitUp = false;
                DungeunDoorManager.Instance.doorHitLeft = false;
                DungeunDoorManager.Instance.doorHitRight = false;
            }
        }
        if (other.CompareTag("NPC"))//npc에게 말건 경우
        {
            if (QuestManager.Instance.IsQuestNpc(other.gameObject.name))// 퀘스트 주는 NPC가 맞는지
            {
                Debug.Log("퀘스트다!");
                QuestManager.Instance.NowQusetInfo();
            }
        }
    }
    IEnumerator CollAction()//콜리더 온오프
    {
        Coll.enabled = true;
        yield return new WaitForSeconds(0.05f);
        if (isatk == true)
            isatk = false;
        Coll.enabled = false;
    }
}