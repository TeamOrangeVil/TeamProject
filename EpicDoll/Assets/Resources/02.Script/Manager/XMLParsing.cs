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
    //XML데이터 파일명, 노드 이름
    public const string PLAYINFOXML = "/PlayDB.xml";
    public const string GAMEOPTIONXML = "/GameOption.xml";
    public const string QUESTINFOXML = "/QuestInfoDB.xml";
    public const string QUESTINFONODE = "Quest_Info/Quest";
    public const string EVENTDIALOGXML = "/EventScript.xml";
    public const string EVENTDIALOGNODESTART = "Event_Progress/Progress";
    public const string EVENTDIALOGNODEEND = "/Event";
    public const string INFODIALOGXML = "/InfoScript.xml";
    public const string INFODIALOGNODESTART = "Info_Progress/Progress";
    public const string INFODIALOGNODEEND = "/Event";
}

[System.Serializable]
//[XmlRoot(ElementName = "SAVE")]
[XmlRoot("SAVEDATA")]
public class GameInfo //xml로 불러올 게임 정보를 저장할 클라스
{
    //세이브 정보----------------------------------------------------
    public int IDNumber;
    //플레이어 정보--------------------------------------------------
    public Vector3 playerTr;                        //위치
    public int hp;                                  //채력
    public int beforeHp;                            //되감기 전 채력
    public int checkCount;                          //체크포인트 사용 횟수
    public int skinNum;                             //옷 정보
    public int hitTimes;                            //피격 횟수
    public int deadTimes;                           //사망 횟수
    public float playTimes;                         //실행 시간
    public bool isSkill;                            //스킬 사용 가능
    //퀘스트 정보-----------------------------------------------------
    public int tutoCount;                           //튜토리얼 진행 정보
    public Vector3 InfoDollTr;                      //안내인형 위치
    public Vector3 spiderTr;                        //거미 위치
    public Vector3 shadowSpiderTr;                  //거미 2 위치
    public Vector3 dancerTr = Vector3.zero;         //댄서
    public bool getKey = false;                     //열쇠
    public bool meetMan = false;                    //서랍장맨 만남
    public bool mansWall = false;                   //서랍장맨 구했는지?
    public bool returnToWay = false;                //돌아가
    public bool getCompas = false;                  //컴퍼스겟?
    public bool getNeedle = false;                  //바늘 겟?
    public bool meetKnight = false;                 //기사 ㅎㅇ?
    public bool meetDancer = false;                 //댄서 ㅎㅇ?
    public bool exchange = false;                   //기사랑 템 교환 ?
    public bool spiderAct = false;                  //거미
    public bool shadowSpiderAct = false;            //거미 작동 여부
    public bool deskBox = false;                    //서랍장
    public bool gramophoneAct = false;              //축음기
    //시스템 정보-----------------------------------------------------
    public bool uiActive;                           //ui 숨김
    public int bgmNum;                              //BGM번호
}

public struct EventScript
{
    public int ID;                                  //번호
    public string target;                           //말하는 사람
    public string other;                            //듣는 대상
    public string script;                           //스크립트
};
public struct InfoScript
{
    public int ID;                                  //번호
    public string script;                           //스크립트
};
public struct SoundOption
{
    public float bgmValue;                          //BGM 볼륨
    public float effValue;                          //이펙트 볼륨
    public bool bgmMute;                            //BGM 음소거
    public bool effMute;                            //이펙트 음소거
};
public class XMLParsing : MonoBehaviour
{
    //xml파일 경로
    public string path;
    private static XMLParsing gInstance = null;
    public static XMLParsing Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }
    //xml데이터 선언, GameInfo라는 항목 밑으로 데이터가 묶임 
    //[XmlArray("GameSave"), XmlArrayItem("GameInfo")]
    [XmlArray("GameSave"), XmlArrayItem("IDNumber")]
    public GameInfo[] m_Gamelnfo = new GameInfo[3];
    public SoundOption S_op;
    public int Datanum = 0;                         //데이터 넘버

    void Awake()
    {
        gInstance = this;
        path = Application.streamingAssetsPath;     //xml파일 경로
    }
    //스테이지별 대화 로드
    public List<EventScript> EventScriptsLoad(int stageNumber)
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
            es_Info.other = xn["Other"].InnerText;
            es_Info.script = xn["Scrpit"].InnerText;
            tempList.Add(es_Info);
        }
        return tempList;
    }
    //스테이지 안내문 로드
    public List<InfoScript> InfoScriptsLoad(int stageNumber)
    {
        XmlDocument Document = new XmlDocument();
        Document.Load(path + XmlConstancts.INFODIALOGXML);
        XmlNodeList Nodes = Document.SelectNodes(XmlConstancts.INFODIALOGNODESTART + stageNumber + XmlConstancts.INFODIALOGNODEEND);
        List<InfoScript> tempList = new List<InfoScript>();
        foreach (XmlNode xn in Nodes)
        {
            InfoScript es_Info = new InfoScript();
            es_Info.ID = int.Parse(xn["ID"].InnerText);
            es_Info.script = xn["Scrpit"].InnerText;
            tempList.Add(es_Info);
        }
        return tempList;
    }
    /// <summary>
    /// Xml파일을 XmlSerializer로 저장할때 윈도우에서 파일 인코딩이 자동으로 UTF-8로 되지 않기 때문에
    /// 저장, 불러오기 할때 인코딩 형식을 utf-8로 고정했습니다.
    /// </summary>
    public void XmlSaveGameData(GameInfo gameInfo, int saveNumber)
    {
        Debug.Log("xml로 데이터 저장");
        m_Gamelnfo[saveNumber] = gameInfo;
        var serializer = new XmlSerializer(typeof(GameInfo[]));
        using (var stream = new FileStream(path + XmlConstancts.PLAYINFOXML, FileMode.Create))
        {
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(streamWriter, this.m_Gamelnfo);
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        Debug.Log("저장 완료");
    }
    public GameInfo XmlLoadGameData(int loadNumber)
    {
        Debug.Log("xml로 데이터 불러오기");
        var serializer = new XmlSerializer(typeof(GameInfo[]));
        using (var stream = new FileStream(path + XmlConstancts.PLAYINFOXML, FileMode.Open))
        {
            var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);

            m_Gamelnfo = (GameInfo[])serializer.Deserialize(streamReader);
            return m_Gamelnfo[loadNumber];
        }
    }
    public List<GameInfo> XmlLoadGameDatas()
    {
        Debug.Log("xml로 데이터 불러오기");
        var serializer = new XmlSerializer(typeof(List<GameInfo>));
        List<GameInfo> tempList = new List<GameInfo>();
        using (var stream = new FileStream(path + XmlConstancts.PLAYINFOXML, FileMode.Open))
        {
            var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);

            tempList = (List<GameInfo>)serializer.Deserialize(streamReader);
            return tempList;
        }
    }
    public void XmlSaveOption(float EffVol, float BgmVol, bool EffMute, bool BgmMute)
    {
        Debug.Log("게임 옵션 세이브");
        S_op.effValue = EffVol;
        S_op.bgmValue = BgmVol;
        S_op.effMute = EffMute;
        S_op.bgmMute = BgmMute;

        var serializer = new XmlSerializer(typeof(SoundOption));
        using (var stream = new FileStream(path + XmlConstancts.GAMEOPTIONXML, FileMode.Create))
        {
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            serializer.Serialize(streamWriter, this.S_op);
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        Debug.Log("저장 완료");
    }
    public SoundOption XmlLoadOption()
    {
        Debug.Log("게임 옵션 로드");
        var serializer = new XmlSerializer(typeof(SoundOption));
        using (var stream = new FileStream(path + XmlConstancts.GAMEOPTIONXML, FileMode.Open))
        {
            var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);
            return (SoundOption)serializer.Deserialize(streamReader);
        }
    }
}
