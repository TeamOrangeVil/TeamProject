using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BindControl : MonoBehaviour {
    /// <summary>
    /// 추후 Forbolg라는 케릭터 스크립트와 병합 할 것
    /// </summary>
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
    void Start () {
        WeaponSwitcher = 0;
        Coll = GetComponent<Collider>();
        Coll.enabled = false;
        xmlParsing = GameObject.Find("DataManager").GetComponent<XML_Parsing>();
        BindList = xmlParsing.BindDBListRead(Application.streamingAssetsPath + XmlConstancts.OBJBINDXML);
        PlayerHand = GameObject.Find("Hand").GetComponent<Transform>();
        MyWeapon = GetComponents<GameObject>();//배열임, 그래서 겟컴포넌s
        NowWeapon = GetComponent<GameObject>();
        BefoWeapon = GetComponent<GameObject>();

        for (int i = 0; i < BindList.Count; i++)
        {
            Debug.Log("리스트  " + BindList[i].ID);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
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

        if (NowWeapon!=null)
            NowWeapon.transform.position = PlayerHand.transform.position;//무기 위치
	}
    void OnTriggerStay(Collider other)
    {
        //나중에 거리별로 우선순위 매겨서 저장할 것
       
        if (other.tag == "Object" && isatk==false)
        {//공격 x 몬스터 x
            bindName = other.name;
            BindSerch();
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Monster" && NowWeapon.name == "Weapon_003(Clone)" && isatk == true)
        {//공격 o 몬스터 o
            Debug.Log("잡았다");
            other.gameObject.SetActive(false);
            QuestManager.Instance.QuestIsClear(other.name);
        }
        
    }
    void BindSerch()//무슨옵젝인지검색
    {
        for (int i = 0; i < BindList.Count; i++)
        {
            if (BindList[i].ID == bindName)
            {
                temp = GameObject.Find(BindList[i].MixResult);
                Debug.Log("앗 무기닷");
                Debug.Log(bindName);
                if(NowWeapon == null)
                {
                    NowWeapon = Instantiate(temp, PlayerHand.position, Quaternion.identity) as GameObject;
                    MyWeapon[WeaponSwitcher] = NowWeapon.gameObject;
                    if (WeaponSwitcher < 2)
                        WeaponSwitcher++;
                    else if (WeaponSwitcher > 0)
                        WeaponSwitcher--;
                }
                else
                {
                    MyWeapon[WeaponSwitcher] = NowWeapon.gameObject;
                    MyWeapon[WeaponSwitcher].SetActive(false);
                    if (WeaponSwitcher<2)
                    WeaponSwitcher++;
                    else if(WeaponSwitcher>0)
                    WeaponSwitcher--;
                    NowWeapon = Instantiate(temp, PlayerHand.position, Quaternion.identity) as GameObject;
                }
                //atk = 99;
                bindName = null;
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

