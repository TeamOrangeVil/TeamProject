using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text;

public class PlayerControl : MonoBehaviour {

    public SkeletonAnimation Player;  //스켈레톤 애니메이션
    
    private string cur_animation = ""; //현재 실행 중인 애니메이션 이름
    
    public bool limit_move = false; //움직임을 제한하는 변수 선언
    
    public Transform tr; //캐릭터의 Transform 컴포넌트 추가를 위한 변수 선언
    
    public Vector3 movement; //캐릭터의 움직임을 넣을 변수 선언
    
    //캐릭터의 Horizontal 방향 값 변수 선언
    private float h = 0.0f;
    private float v = 0.0f;
    //캐릭터의 속도
    private float walkSpeed = 3.3f;
    private float runSpeed = 1.5f;
    private float jumpSpeed = 0.1f;
    public bool isJump = false;

    public bool communicationCheck = false;
    public UISprite textPanel;

    public GameObject inventoryPanel;

    XML_Parsing xmlParsing;
    //public GameObject Player;//게임 플레이어
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

    private StringBuilder sb;

    //public GameObject[] Slots = new GameObject[3];

    private static PlayerControl gInstance = null;

    public static PlayerControl Instance
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
        tr = GetComponent<Transform>(); //Player의 컴포넌트
        sb = new StringBuilder();
        
    }
    
    void Start()
    {
        WeaponSwitcher = 0;
        Coll = GetComponent<Collider>();
        Coll.enabled = false;
        BindList = XML_Parsing.Instance.BindDBListRead(XmlConstancts.OBJBINDXML);
        PlayerHand = GameObject.Find("left Hand").GetComponent<Transform>();
        MyWeapon = new GameObject[3];
        NowWeapon = GetComponent<GameObject>();
        BefoWeapon = GetComponent<GameObject>();

        //Slots[0] = GameObject.Find("Slot0").GetComponent<GameObject>();
        //Slots[1] = GameObject.FindGameObjectWithTag("Slot2").GetComponent<GameObject>();
        //Slots[2] = GameObject.Find("Slot2").GetComponent<GameObject>();

    }

    void FixedUpdate()
    {
        if (!limit_move) //움직임이 제한되지 않을 경우
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            // 대화 시 캐릭터 움직임 제한
            if (textPanel.isActiveAndEnabled == true) { communicationCheck = true; }
            if (textPanel.isActiveAndEnabled == false) { communicationCheck = false; }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJump = false;
                TransformLimit();
            }
            if (h > 0) //만약 h 값이 0보다 클 경우
            {

                if (communicationCheck == false)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        SetAnimation("run", true, 1.0f);
                        TransformLimit();
                    }
                    else
                    {
                        SetAnimation("Walk", true, 1.0f);
                        TransformLimit();
                    }
                }
                Player.Skeleton.flipX = false;
            }
            else if (h < 0)
            {
                if (communicationCheck == false)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        SetAnimation("run", true, 1.0f);
                        TransformLimit();
                    }
                    else
                    {
                        SetAnimation("Walk", true, 1.0f);
                        TransformLimit();
                    }
                }

                 Player.Skeleton.flipX = true;
            }
            else if (v > 0) { if (communicationCheck == false) { TransformLimit(); } SetAnimation("Walk", true, 1.0f); }
            else if (v < 0) { if (communicationCheck == false) { TransformLimit(); } SetAnimation("Walk", true, 1.0f); }
            else { SetAnimation("STAY", true, 1.0f); }
        }

        //조합키
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(CollAction());
        }
        //공격
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
            isatk = true;
            limit_move = true;
            StartCoroutine(CollAction());
        }
        
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(inventoryPanel.activeInHierarchy == true)
            {
                inventoryPanel.SetActive(false);
            }
            else
            {
                inventoryPanel.SetActive(true);
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

                BefoWeapon = NowWeapon;
                NowWeapon = MyWeapon[0];
                MyWeapon[0] = BefoWeapon;
                BefoWeapon = null;
                InvenChanger(MyWeapon[0].name, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
                BefoWeapon = NowWeapon;
                NowWeapon = MyWeapon[1];
                MyWeapon[1] = BefoWeapon;
                BefoWeapon = null;
                InvenChanger(MyWeapon[1].name, 1);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
                BefoWeapon = NowWeapon;
                NowWeapon = MyWeapon[2];
                MyWeapon[2] = BefoWeapon;
                BefoWeapon = null;
                InvenChanger(MyWeapon[2].name, 2);
        }
    }
    // 캐릭터 움직임 관련
    public void TransformLimit() 
    {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -19.0f, 19.0f),
                                        Mathf.Clamp(transform.position.y, -6.0f, 6.0f),
                                        Mathf.Clamp(transform.position.z, 0.0f, 6.5f));
        
        movement.Set(h, 0, v);
        tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            tr.Translate(movement.normalized * walkSpeed * runSpeed * Time.deltaTime, Space.World);
        }   
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

    void BindSerch(GameObject other, string id)//무슨 옵젝인지검색
    {
        for (int i = 0; i < BindList.Count; i++)
        {
            if (BindList[i].ID == id)
            {
                temp = GameObject.Find(BindList[i].MixResult) as GameObject;
                    NowWeapon = temp;
            }
        }
    }
    void InvenChanger(string WeaponName,int point)
    {
        //sb.Append("Slot");

        //GameObject changeSprite = Slots[point];
        GameObject changeSprite = GameObject.Find("Slot" + point.ToString());
        //GameObject changeSprite = GameObject.Find(sb.Append("Slot");)
        if (changeSprite.GetComponent<UISprite>().spriteName == "ButtonBrakeUpSprite")
        {
            changeSprite.GetComponent<UISprite>().spriteName = WeaponName;
        }
        else
        {
            changeSprite.GetComponent<UISprite>().spriteName = "ButtonBrakeUpSprite";
        }
    }
    // 충돌 관련

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Object" && isatk == false)
        {//공격 x 몬스터 x
            bindName = other.name;
            BindSerch(other.gameObject, other.name);
            other.gameObject.SetActive(false);
        }
        /*else if (other.tag == "Monster" && NowWeapon.name == "Weapon_003" && isatk == true)
        {//공격 o 몬스터 o
            other.gameObject.SetActive(false);
            QuestManager.Instance.QuestIsClear(other.gameObject.name);
        }*/
    }
    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("WarPortal"))
        {
            SceneManager.LoadScene("Stage_01");
            //SceneManager.LoadScene("03");
        }
        if (other.CompareTag("VillPortal"))
        {
            XML_Parsing.Instance.XmlSaveUTF8();
            SceneManager.LoadScene("ProtoVill");
            var temp = XML_Parsing.Instance.InfoOutput(
                XML_Parsing.Instance.XmlLoadUTF8());
            MyWeapon[0] = Resources.Load(temp.Weapon01) as GameObject;
            MyWeapon[1] = Resources.Load(temp.Weapon02) as GameObject;
            MyWeapon[2] = Resources.Load(temp.Weapon03) as GameObject;
            NowWeapon = Instantiate(Resources.Load(temp.NowWeapon), PlayerHand.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        }

        if (other.CompareTag("Monster")) // 몬스터에게 타격 당할 시 체력 감소
        {
            //HpBar1.fillAmount -= 1f;
            //HpBar.fillDirection -= 1f;
            //HpBar.value -= 1.0f;
            //DataManager.Instance.HpBar.value -= 0.1f;
        }

        if (other.CompareTag("NPC"))//npc에게 말건 경우
        {
            if (QuestManager.Instance.IsQuestNpc(other.gameObject.name))// 퀘스트 주는 NPC가 맞는지
            {
                //Debug.Log("퀘스트다!");
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
        limit_move = false;
        SetAnimation("attack 1", true, 1.0f);
    }
}