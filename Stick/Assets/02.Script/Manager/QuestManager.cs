using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour {

    XML_Parsing xmlParsing;
    Quest_Info questInfo;
    Quest_Progress questProgress;

    public UILabel questText;
    public GameObject TextExit;
    public Sprite textBackground;
    public GameObject questYes;
    

    public bool questStart;//퀘스트 온오프
    public int questCount;//현재 받은 퀘스트 번호
    public int questStep;//퀘스트 진행상황
    public int ClearToQuest;//퀘스트 달성을 위해 채워야되는 할당량

    public bool TextStop = false; // 퀘스트를 받고 같은 퀘스트 받는 것을 방지

    public TypewriterEffect eft;

    private static QuestManager gInstance = null;

    public static QuestManager Instance
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
        questInfo = new Quest_Info();
        questProgress = new Quest_Progress();
        TextExit.SetActive(false);
        questStart = false;
        questCount = 1;
        questStep = 0;
        ClearToQuest = 0;
        LoadQuest();
        questYes.SetActive(false);
        
    }
    public void LoadQuest()//q번째 퀘스트 정보를 불러옵니다.
    {
        if (questInfo == null || questProgress == null)
            Debug.Log("아 없잖아;");
        Debug.Log("데이터로드 실행");
        questInfo = XML_Parsing.Instance.QuestInfoRead(Application.streamingAssetsPath + XmlConstancts.QUESTINFOXML, questCount);
        questProgress = XML_Parsing.Instance.QuestProgressRead(Application.streamingAssetsPath + XmlConstancts.QUESTDIALOGXML,
            "Quest_" + questInfo.QuestID.ToString());
    }
    public void NowQusetInfo()//현재 퀘스트가 뭔지, 무슨 대사를 표시할지 판단하여 실행
    {
        if (questInfo.QuestID == questCount)//현재 진행해야되는 퀘스트순서 판단
        {
            switch (questStep)//퀘스트의 진행단계를 분기로 합니다.
            {
                case 0://수락 여부
                    if (questStep == 0)
                        TextExit.SetActive(true);
                    questText.text = questProgress.ScriptReq;
                    eft.ResetToBeginning();
                    questYes.SetActive(true);
                    break;
                case 1://수락
                    questYes.SetActive(false);
                    questText.text = questProgress.ScriptYes;
                    eft.ResetToBeginning();
                    questStep++;
                    break;
                case 2: //대화창 끝내기
                    TextExit.SetActive(false);
                    questStep++;
                    break;
                case 3://진행중
                    TextExit.SetActive(true);
                    questText.text = questProgress.ScriptProgress;
                    eft.ResetToBeginning();
                    questStep++;
                    break;
                case 4:
                    TextExit.SetActive(false);
                    questStep--;
                    break;
                case 5://조건 충족시 완료
                    TextExit.SetActive(true);
                    questText.text = questProgress.ScriptPer;
                    eft.ResetToBeginning();
                    questStep++;
                    break;
                case 6: // 창 종료
                    TextExit.SetActive(false);
                    questStep = 0;
                    ClearToQuest = 0;
                    questCount++;
                    LoadQuest();
                    break;

            }
        }
        else//아니면 그냥 대화 or 추후 서브퀘스트
        {

        }
    }
    public bool IsQuestNpc(string npcName)//npc가 퀘스트를 부여할 수 있는 npc인지 판단합니다.
    {
        if (questInfo.QuestNpc == npcName)
            return true;
        else
            return false;
    }
    public void QuestIsClear(string itemName)//퀘스트 조건이 충족 되는지 판단
    {
        if ((questInfo.req_Target) == itemName && questCount <= 3)//주운 아이템이 퀘템이면
        {
            ClearToQuest++;
            if (questInfo.req_Howmach == ClearToQuest)//퀘스트 할당량 다 채우면
            {
                Debug.Log("퀘스트 조건 충족");
                questStep = 5;//5단계행
            }
        }
    }
    public void questStepAdd()//퀘스트 진행단계 조절
    {
        questStep += 1;
        NowQusetInfo();
    }
}
