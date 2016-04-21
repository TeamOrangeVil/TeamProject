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
    public const string PLAYINFOXML = "/PlayerDB.xml";
    public const string MOBDBXML = "/MonsterDB.xml";
    public const string MOBXMLNODE = "MonsterInfo/Monster";
    public const string OBJBINDXML = "/ObjectDB.xml";
    public const string BINDXMLNODE = "ObjectInfo/Object";
    public const string QUESTINFOXML = "/QuestInfoDB.xml";
    public const string QUESTINFONODE = "Quest_Info/Quest";
    //public const string QUESTDIALOGXML = "/QuestScript.xml";
    //public const string QUESTDIALOGNODE = "Quest_Progress/Progress";
    public const string EVENTDIALOGXML = "/EventScript.xml";
    public const string EVENTDIALOGNODESTART = "Event_Progress/Progress";
    public const string EVENTDIALOGNODEEND = "/Event";
}

[System.Serializable]
[XmlRoot(ElementName = "Name")]
public class Player_Info //xml로 불러올 몬스터의 정보를 저장할 클라스
{
    [XmlElement("Name")]
    public string Name;
    [XmlElement("Hp")]
    public int Hp;
    [XmlElement("Weapon01")]
    public string Weapon01;
    [XmlElement("Weapon02")]
    public string Weapon02;
    [XmlElement("Weapon03")]
    public string Weapon03;
    [XmlElement("NowWeapon")]
    public string NowWeapon;
}
public struct EventScript
{
    public int ID;//번호
    public string target;//출력 대상 이름
    public string script;//대사
};
public class Quest_Info//퀘스트 항목을 정의합니다.
{
    public int QuestID;//퀘스트 번호
    public string Quest_NameK;//게임내에서 띄워줄 퀘스트 이름
    public string QuestNpc;//퀘를 주는 npc이름
    public int Unlock_Condition;//퀘스트 해재조건
    public int QueType;//퀘스트 타입
    public int QueProgress;//대사 처리순서
    public string req_Target;//필요 조건
    public int req_Howmach;//얼마나?
    public int ResultExp;//보상 겸치
    public int ResultGold;//보상 골드
}
/*public class Quest_Progress//대사 처리
{
    public string QuestID;//퀘스트 번호
    public string ScriptReq;//퀘스트 줄때
    public string ScriptYes;//수락시
    public string ScriptNo;//거절시
    public string ScriptProgress;//진행중
    public string ScriptPer;//완료
}*/
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
public class Object_Info
{
    public int ID;
    public string Object_NameK;
    public string Object_Name;
    public int Object_Type;
    public string Mix_Result;
}

public class XMLParsing : MonoBehaviour
{
    public string path;//xml파일 경로
    private static XMLParsing gInstance = null;
    public static XMLParsing Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }
    [XmlArray("Player"), XmlArrayItem("Player_Info")]
    public List<Player_Info> m_playerlist = new List<Player_Info>();
    public List<Monster_Info> monsters = new List<Monster_Info>();

    // Use this for initialization
    void Awake()
    {
        gInstance = this;//싱글톤 재귀
        path = Application.streamingAssetsPath;//xml파일 경로
        //monsters = Read();//시작시 리스트에 xml데이터를 넣는다.
    }
    public Player_Info InfoInsert(string name, int hp, string wpn01,
        string wpn02, string wpn03, string nowWpn)//값을 얻어오자
    {
        Player_Info temp = new Player_Info();
        temp.Name = name;
        temp.Hp = hp;
        temp.Weapon01 = wpn01;
        temp.Weapon02 = wpn02;
        temp.Weapon03 = wpn03;
        temp.NowWeapon = nowWpn;
        return temp;
    }
    public void InfoSave(Player_Info tempInfo)
    {
        if (m_playerlist.Count <= 0)
            m_playerlist.Add(tempInfo);

        m_playerlist[0].Hp = tempInfo.Hp;
        m_playerlist[0].Name = tempInfo.Name;
        m_playerlist[0].Weapon01 = tempInfo.Weapon01;
        m_playerlist[0].Weapon02 = tempInfo.Weapon02;
        m_playerlist[0].Weapon03 = tempInfo.Weapon03;
        m_playerlist[0].NowWeapon = tempInfo.NowWeapon;
    }
    public Player_Info InfoOutput(List<Player_Info> temp)
    {
        Player_Info tempInfo = new Player_Info();
        tempInfo.Weapon01 = temp[0].Weapon01.Remove(10);
        tempInfo.Weapon02 = temp[0].Weapon02.Remove(10);
        tempInfo.Weapon03 = temp[0].Weapon03.Remove(10);
        tempInfo.NowWeapon = temp[0].NowWeapon.Remove(10);
        return tempInfo;
    }
    public List<Monster_Info> Read()//추후 재정의 해서 쓸 수 있도록 작업 할 것
    {
        XmlDocument Document = new XmlDocument();
        Document.Load(path + XmlConstancts.MOBDBXML);
        //XmlElement KeyList = Document.DocumentElement;//키 리스트를 문서의 항목을 사용한다?
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
    public List<EventScript> EventScriptsLoad(int stageNumber)//스테이지별 대화 전송
    {
        XmlDocument Document = new XmlDocument();
        Document.Load(path + XmlConstancts.EVENTDIALOGXML);
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.EVENTDIALOGNODESTART + stageNumber + XmlConstancts.EVENTDIALOGNODEEND);
        List<EventScript> tempList = new List<EventScript>();
        foreach (XmlNode xn in Nodes)
        {
            EventScript es_Info = new EventScript();
            es_Info.ID = int.Parse(xn["ID"].InnerText);
            es_Info.target = xn["Target"].InnerText;
            es_Info.script = xn["Scrpit"].InnerText;
            tempList.Add(es_Info);
        }
        return tempList;
    }
    public List<Object_Info> BindDBListRead()//리스트로 조합템인지 아닌지 비교용
    {
        List<Object_Info> bind_list = new List<Object_Info>();
        
        XmlDocument Document = new XmlDocument();
        Document.Load(path + XmlConstancts.OBJBINDXML);
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.BINDXMLNODE);
        foreach (XmlNode xn in Nodes)
        {
            Object_Info temp_info = new Object_Info();
            temp_info.ID = int.Parse(xn["Object_ID"].InnerText);
            temp_info.Object_Name = xn["Object_Name"].InnerText;
            temp_info.Object_NameK = xn["Object_NameK"].InnerText;
            temp_info.Object_Type = int.Parse(xn["Object_Type"].InnerText);
            temp_info.Mix_Result = xn["Mix_Result"].InnerText;
            bind_list.Add(temp_info);
        }
        return bind_list;
    }
    public Object_Info BindDBRead(string objectName)//아이템
    {
        Object_Info Object_Info = new Object_Info();
        XmlDocument Document = new XmlDocument();
        Document.Load(path + XmlConstancts.OBJBINDXML);
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.BINDXMLNODE);
        foreach (XmlNode xn in Nodes)
        {
            Object_Info temp_info = new Object_Info();
            temp_info.ID = int.Parse(xn["Object_ID"].InnerText);
            temp_info.Object_Name = xn["Object_Name"].InnerText;
            temp_info.Object_NameK = xn["Object_NameK"].InnerText;
            temp_info.Object_Type = int.Parse(xn["Object_Type"].InnerText);
            temp_info.Mix_Result = xn["Mix_Result"].InnerText;
            if (temp_info.Object_Name == objectName)
            { 
                Object_Info = temp_info;
                break;
            }
            //else
            //Debug.Log("아직 못찾음");
        }
        return Object_Info;
    }
    /*public Quest_Progress QuestProgressRead(string id)//대사처리
    {
        Quest_Progress Progress_info = new Quest_Progress();
        XmlDocument Document = new XmlDocument();
        Document.Load(path+XmlConstancts.QUESTDIALOGXML);
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
            if (temp_info.QuestID == id) { 
                Progress_info = temp_info;
                break;
            }
            //else
            // Debug.Log("아직 못찾음");
        }
        return Progress_info;
    }*/
    /*public Quest_Info QuestInfoRead(int id)//퀘스트 정보
    {
        Quest_Info Progress_info = new Quest_Info();
        XmlDocument Document = new XmlDocument();
        Document.Load(Application.streamingAssetsPath + XmlConstancts.QUESTINFOXML);
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.QUESTINFONODE);
        foreach (XmlNode xn in Nodes)
        {
            Quest_Info temp_info = new Quest_Info();
            temp_info.QuestID = int.Parse(xn["Quest_Code"].InnerText);
            temp_info.Quest_NameK = xn["Quest_NameK"].InnerText;
            temp_info.QuestNpc = xn["Quest_Npc"].InnerText;
            temp_info.Unlock_Condition = int.Parse(xn["Quest_Unlock_Condition"].InnerText);
            temp_info.QueType = int.Parse(xn["Quest_Type"].InnerText);
            temp_info.req_Target = xn["req_Target"].InnerText;
            temp_info.req_Howmach = int.Parse(xn["req_Howmach"].InnerText);
            temp_info.ResultExp = int.Parse(xn["ResultExp"].InnerText);
            temp_info.ResultGold = int.Parse(xn["ResultGold"].InnerText);
            if (temp_info.QuestID == id) { 
                Progress_info = temp_info;
                break;
            }
            else
                Debug.Log("아직 못찾음");
        }
        return Progress_info;
    }*/

    public Monster_Info MonsterDbRead()//list사용 안하도록 재정의 하는 중
    {
        Monster_Info m_Info = new Monster_Info();
        return m_Info;
    }
    /// <summary>
    /// Xml파일을 XmlSerializer로 저장할때 윈도우에서 파일 인코딩이 자동으로 UTF-8로 되지 않기 때문에
    /// 저장, 불러오기 할때 인코딩 형식을 utf-8로 고정했습니다.
    /// </summary>
    public void XmlSaveUTF8()
    {
        var serializer = new XmlSerializer(typeof(List<Player_Info>));
        using (var stream = new FileStream(Application.streamingAssetsPath + XmlConstancts.PLAYINFOXML, FileMode.Create))
        {
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(streamWriter, this.m_playerlist);
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        Debug.Log("저장 완료");
    }
    public List<Player_Info> XmlLoadUTF8()
    {
        var serializer = new XmlSerializer(typeof(List<Player_Info>));
        using (var stream = new FileStream(Application.streamingAssetsPath + XmlConstancts.PLAYINFOXML, FileMode.Open))
        {
            var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);
            return (List<Player_Info>)serializer.Deserialize(streamReader);
        }
    }
}
