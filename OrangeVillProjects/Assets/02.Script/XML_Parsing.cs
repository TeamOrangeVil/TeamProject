using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

static class XmlConstancts
{
    //전처리할 내용을 넣어주세요 -s> public const 형태 이름;
    //xml 관련해서 사용할 전처리입니다.
    public const string PLAYINFOXML = "/Player_db_Test.xml";
    public const string MOBDBXML = "/MonsterDB.xml";
    public const string MOBXMLNODE = "MonsterInfo/Monster";
    public const string OBJBINDXML = "/ObjBindDB.xml";
    public const string BINDXMLNODE = "BindInfo/Bind";
    public const string QUESTINFOXML = "/Quest_Info_Schmar.xml";
    public const string QUESTINFONODE = "Quest_Info/Quest";
    public const string QUESTDIALOGXML = "/Quest.xml";
    public const string QUESTDIALOGNODE = "Quest_Progress/Progress";
}

[System.Serializable]
[XmlRoot(ElementName = "XMLSerializer")]
public class PlayerInfo //xml로 불러올 몬스터의 정보를 저장할 클라스
{
    [XmlElement("name")]
    public string name;
    [XmlElement("hp")]
    public float hp;
    [XmlElement("atk")]
    public float atk;
    [XmlElement("def")]
    public float def;
    //[XmlElement("def")]
    //public float def;
    //[XmlElement("def")]
    //public float def;
    //[XmlElement("def")]
    //public float def;
}
public class Quest_Info//퀘스트 항목을 정의합니다.
{
    public int QuestID;//퀘스트 번호
    public string Quest_NameK;//게임내에서 띄워줄 퀘스트 이름
    public string QuestNpc;//퀘를 주는 npc이름
    public int Unlock_Condition;//퀘스트 해재조건
    public int QueProgress;//대사 처리순서
    public string req_Item;//필요 조건
    public int req_Howmach;//얼마나?
    public int ResultExp;//보상 겸치
    public int ResultGold;//보상 골드
}
public class Quest_Progress//대사 처리
{
    public string QuestID;//퀘스트 번호
    public string ScriptReq;//퀘스트 줄때
    public string ScriptYes;//수락시
    public string ScriptNo;//거절시
    public string ScriptProgress;//진행중
    public string ScriptPer;//완료
}
public class Monster_Info//데이터 로드 리스트 정의
{
    public string ID;
    public string Name;
    public string kName;
    public int Etype;
    public int type;
    public int Hp;
    public int Atk;
    public float Spd;
    public float Acc;
    public float AtkSpd;
}
public class Bind_Info
{
    public string ID;
    public string WeaponState;
    public string ObjectCode;
    public string MixResult;
}
public class XML_Parsing : MonoBehaviour
{
    static string path;//xml파일 경로


    [XmlArray("Player"), XmlArrayItem("PlayerInfo")]
    public List<PlayerInfo> m_playerlist = new List<PlayerInfo>();
    public List<Monster_Info> monsters = new List<Monster_Info>();
    // Use this for initialization
    void Awake()
    {
        path = Application.streamingAssetsPath;//xml파일 경로
        monsters = Read(path + XmlConstancts.MOBDBXML);//시작시 리스트에 xml데이터를 넣는다.
    }

    // Update is called once per frame
    void Start()
    {
        /*if (monsters.Count > 0)//재대로 들어갔는지 확인
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                Debug.Log("id " + monsters[i].ID);
                Debug.Log("이름 " + monsters[i].kName);
                Debug.Log("영문 이름 " + monsters[i].Name);
                Debug.Log("hp " + monsters[i].Hp);
                Debug.Log("속성" + monsters[i].Etype);
                Debug.Log("공중형" + monsters[i].type);
                Debug.Log("spd " + monsters[i].Spd);
                Debug.Log("atkspd " + monsters[i].AtkSpd);
                Debug.Log("가속도 " + monsters[i].Acc);
                Debug.Log("atk " + monsters[i].Atk);
            }
        }*/
    }
    public List<Monster_Info> Read(string Path)//추후 재정의 해서 쓸 수 있도록 작업 할 것
    {
        XmlDocument Document = new XmlDocument();
        Document.Load(Path);
        XmlElement KeyList = Document.DocumentElement;//키 리스트를 문서의 항목을 사용한다?
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.MOBXMLNODE);//monsterinfo아래 Monster항목을 노드로 설정하여 하위항목을 불러오자
        List<Monster_Info> tempList = new List<Monster_Info>();//반환을 위한 임시 리스트

        foreach (XmlNode xn in Nodes)
        {
            Monster_Info m_Info = new Monster_Info();
            m_Info.ID = xn["Monster_Code"].InnerText;
            m_Info.kName = xn["KName"].InnerText;
            m_Info.Name = xn["Name"].InnerText;
            m_Info.Etype = int.Parse(xn["EType"].InnerText);
            m_Info.type = int.Parse(xn["Type"].InnerText);
            m_Info.Hp = int.Parse(xn["HP"].InnerText);
            m_Info.Atk = int.Parse(xn["ATK"].InnerText);
            m_Info.Spd = float.Parse(xn["Speed"].InnerText);
            m_Info.Acc = float.Parse(xn["accel"].InnerText);
            m_Info.AtkSpd = float.Parse(xn["AtkSpeed"].InnerText);
            tempList.Add(m_Info);
        }
        return tempList;
    }
    public Bind_Info BindDBRead(string path, string id)
    {
        Bind_Info bind_info = new Bind_Info();
        XmlDocument Document = new XmlDocument();
        Document.Load(path);
        XmlElement KeyList = Document.DocumentElement;
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.BINDXMLNODE);
        foreach (XmlNode xn in Nodes)
        {
            Bind_Info temp_info = new Bind_Info();
            temp_info.ID = xn["MixCode"].InnerText;
            temp_info.WeaponState = xn["Weapon_State"].InnerText;
            temp_info.ObjectCode = xn["Object_Code"].InnerText;
            temp_info.MixResult = xn["Mix_Result"].InnerText;
            if (temp_info.ID == id)
                bind_info = temp_info;
            else
                Debug.Log("아직 못찾음");
        }
        return bind_info;
    }
    public Quest_Progress QuestProgressRead(string path, string id)
    {
        Quest_Progress Progress_info = new Quest_Progress();
        XmlDocument Document = new XmlDocument();
        Document.Load(path);
        XmlElement KeyList = Document.DocumentElement;
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.QUESTDIALOGNODE);
        foreach (XmlNode xn in Nodes)
        {
            Quest_Progress temp_info = new Quest_Progress();
            temp_info.QuestID = xn["Quest"].InnerText;
            temp_info.ScriptReq = xn["Script_Req"].InnerText;
            temp_info.ScriptYes = xn["Script_Yes"].InnerText;
            temp_info.ScriptNo = xn["Script_No"].InnerText;
            temp_info.ScriptProgress = xn["Script_Progress"].InnerText;
            temp_info.ScriptPer = xn["Script_Per"].InnerText;
            if (temp_info.QuestID == id)
                Progress_info = temp_info;
            else
                Debug.Log("아직 못찾음");
        }
        return Progress_info;
    }
    public Quest_Info QuestInfoRead(string path, int id)
    {
        Quest_Info Progress_info = new Quest_Info();
        XmlDocument Document = new XmlDocument();
        Document.Load(path);
        XmlElement KeyList = Document.DocumentElement;
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.QUESTINFONODE);
        foreach (XmlNode xn in Nodes)
        {
            Quest_Info temp_info = new Quest_Info();
            temp_info.QuestID = int.Parse(xn["Quest_Code"].InnerText);
            temp_info.Quest_NameK = xn["Quest_NameK"].InnerText;
            temp_info.QuestNpc = xn["Quest_Npc"].InnerText;
            temp_info.Unlock_Condition = int.Parse(xn["Quest_Unlock_Condition"].InnerText);
            temp_info.req_Item = xn["req_Item"].InnerText;
            temp_info.req_Howmach = int.Parse(xn["req_Howmach"].InnerText);
            temp_info.ResultExp = int.Parse(xn["ResultExp"].InnerText);
            temp_info.ResultGold = int.Parse(xn["ResultGold"].InnerText);
            if (temp_info.QuestID == id)
                Progress_info = temp_info;
            else
                Debug.Log("아직 못찾음");
        }
        return Progress_info;
    }
    public List<Bind_Info> BindDBListRead(string path)
    {
        List<Bind_Info> bind_list = new List<Bind_Info>();
        XmlDocument Document = new XmlDocument();
        Document.Load(path);
        XmlElement KeyList = Document.DocumentElement;
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.BINDXMLNODE);
        foreach (XmlNode xn in Nodes)
        {
            Bind_Info temp_info = new Bind_Info();
            temp_info.ID = xn["MixCode"].InnerText;
            temp_info.WeaponState = xn["Weapon_State"].InnerText;
            temp_info.ObjectCode = xn["Object_Code"].InnerText;
            temp_info.MixResult = xn["Mix_Result"].InnerText;
            bind_list.Add(temp_info);
        }
        return bind_list;
    }
    public Monster_Info MonsterDbRead(string Path)//list사용 안하도록 재정의 하는 중
    {
        Monster_Info m_Info = new Monster_Info();
        return m_Info;
    }
    /// <summary>
    /// Xml파일을 XmlSerializer로 저장할때 윈도우에서 파일 인코딩이 자동으로 UTF-8로 되지 않기 때문에
    /// 저장, 불러오기 할때 인코딩 형식을 utf-8로 고정했습니다.
    /// </summary>
    void XmlSaveUTF8(string path)
    {
        var serializer = new XmlSerializer(typeof(List<PlayerInfo>));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(streamWriter, this.m_playerlist);
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        Debug.Log("저장 완료");
    }
    public List<PlayerInfo> XmlLoadUTF8(string path)
    {
        var serializer = new XmlSerializer(typeof(List<PlayerInfo>));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);
            return (List<PlayerInfo>)serializer.Deserialize(streamReader);
        }
    }
}
