using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {
    private static UIManager gInstance = null;
    List<EventScript> eventScripts;
    List<InfoScript> InfoScripts;

    public TypewriterEffect WriteEffect;
    public UILabel TextLabel;
    public UISprite HelperSprite;
    public UISprite PlayerSprite;
    public UISprite Theme01PlayerSprite;
    public UISprite TextLogSprite;
    public UILabel SkipLabel;

    public UISprite HelpTextLogSprite;
    public UILabel HelpTextLabel;
    public TypewriterEffect HelpWriteEffect;

    public UISprite ThemeSlot1;
    public UISprite ThemeSlot2;
    public UISprite ThemeSlot3;
    public UISprite ThemeSlot4;
    public UISprite[] ThemeSlots;
    public Vector3[] ThemeSlotPos;

    public GameObject target;

    public UISprite FKey;
    public UISprite DKey;
    public UISprite WKey;
    public UISprite SpaceKey;
    public UISprite RightKey;
    public UISprite LeftKey;

    public UISprite HPBar;
    public UISprite CheckBar;

    public UISprite BlackBoard;

    public bool isSlots = false;
    public bool isSlotMotion = false;

    public float WaitCount = 0.0f; // 코루틴 대기 시간
    public int HelpCount = 0;
    public int StateUICount = 0;

    public static UIManager Instance
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
        eventScripts = new List<EventScript>();
        eventScripts = XMLParsing.Instance.EventScriptsLoad(1);
        InfoScripts = new List<InfoScript>();
        InfoScripts = XMLParsing.Instance.InfoScriptsLoad(1);

        TextLabel = GameObject.Find("TextLabel").GetComponent<UILabel>();
        WriteEffect = GameObject.Find("TextLabel").GetComponent<TypewriterEffect>();
        HelperSprite = GameObject.Find("HelperSprite").GetComponent<UISprite>();
        PlayerSprite = GameObject.Find("PlayerSprite").GetComponent<UISprite>();
        SkipLabel = GameObject.Find("SkipLabel").GetComponent<UILabel>();

        Theme01PlayerSprite = GameObject.Find("Theme01PlayerSprite").GetComponent<UISprite>();
        TextLogSprite = GameObject.Find("TextLogSprite").GetComponent<UISprite>();
        ThemeSlot1 = GameObject.Find("ThemeSlot01").GetComponent<UISprite>();
        ThemeSlot2 = GameObject.Find("ThemeSlot02").GetComponent<UISprite>();
        ThemeSlot3 = GameObject.Find("ThemeSlot03").GetComponent<UISprite>();
        ThemeSlot4 = GameObject.Find("ThemeSlot04").GetComponent<UISprite>();

        target = GameObject.FindGameObjectWithTag("PLAYER");
        ThemeSlots = new UISprite[] { ThemeSlot1, ThemeSlot2, ThemeSlot3, ThemeSlot4 };
        ThemeSlotPos = new Vector3[4];
        
        FKey = GameObject.Find("FKey").GetComponent<UISprite>();
        DKey = GameObject.Find("DKey").GetComponent<UISprite>();
        WKey = GameObject.Find("WKey").GetComponent<UISprite>();
        SpaceKey = GameObject.Find("SpaceKey").GetComponent<UISprite>();
        RightKey = GameObject.Find("RightKey").GetComponent<UISprite>();
        LeftKey = GameObject.Find("LeftKey").GetComponent<UISprite>();

        BlackBoard = GameObject.Find("BlackBoard").GetComponent<UISprite>();

        TextLabel.enabled = false;
        HelperSprite.enabled = false;
        PlayerSprite.enabled = false;
        Theme01PlayerSprite.enabled = false;
        TextLogSprite.enabled = false;
        SkipLabel.enabled = false;

        HelpTextLogSprite.enabled = false;
        HelpTextLabel.enabled = false;

        ThemeSlot1.enabled = false;
        ThemeSlot2.enabled = false;
        ThemeSlot3.enabled = false;
        ThemeSlot4.enabled = false;

        FKey.enabled = false;
        DKey.enabled = false;
        WKey.enabled = false;
        SpaceKey.enabled = false;
        RightKey.enabled = false;
        LeftKey.enabled = false;

        HPBar.enabled = false;
        CheckBar.enabled = false;

        BlackBoard.enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D) && CharacterController2D.Instance.isFloor) //테마 슬롯 기능
        {
            for (int i = 0; i < 4; i++)
            {
                Camera worldCam = NGUITools.FindCameraForLayer(target.layer);
                Camera guiCam = NGUITools.FindCameraForLayer(ThemeSlots[i].gameObject.layer);
                Vector3 pos = guiCam.ViewportToWorldPoint(worldCam.WorldToViewportPoint(target.transform.position)) + Vector3.up * 0.1f;
                pos.z = 1;
                ThemeSlots[i].transform.position = pos;
                ThemeSlotPos[i] = pos - ((Vector3.up * 0.2f) + (Vector3.left * 0.25f) + (Vector3.up * 0.13f * i) + (Vector3.right * 0.02f * i * i));
            }
            CharacterController2D.Instance.SetAnimation("STAY", true, 1.0f);
            CharacterController2D.Instance.isAct = true;
            if (isSlots.Equals(false)) { isSlots = true;  }
            else { isSlots = false; CharacterController2D.Instance.isAct = false; }
        }
        if (isSlots)
        {
            ThemeSlot1.enabled = true;
            ThemeSlot2.enabled = true;
            ThemeSlot3.enabled = true;
            ThemeSlot4.enabled = true;
            for (int i = 0; i < 4; i++) // 휘며 움직이는 테마 슬롯
            {
                ThemeSlots[i].transform.position = Vector3.Slerp(ThemeSlots[i].transform.position, ThemeSlotPos[i], Time.deltaTime * 10f);
            }
        }
        else if (!isSlots)
        {
            ThemeSlot1.enabled = false;
            ThemeSlot2.enabled = false;
            ThemeSlot3.enabled = false;
            ThemeSlot4.enabled = false;
            isSlotMotion = false;
        }
    }

    IEnumerator FadeOut() // 점점 어두워지는 효과
    {
        BlackBoard.transform.position = CharacterController2D.Instance.transform.position - Vector3.forward;
        BlackBoard.transform.localScale = new Vector3(Screen.width, Screen.height * 0.5f, -1);
        BlackBoard.enabled = true;
        for (float i = 0f; i <= 1; i += 0.005f)
        {
            float color = BlackBoard.GetComponent<UISprite>().alpha;
            color = i;
            BlackBoard.GetComponent<UISprite>().alpha = color;
            //Color color = BlackBoard.GetComponent<UISprite>().material.color;
            //color = new Vector4(0, 0, 0, i);
            //BlackBoard.GetComponent<Renderer>().material.color = color;
            yield return 0;
        }
    }

    IEnumerator FadeIn() // 점점 밝아지는 효과
    {
        for (float i = 1f; i >= 0; i -= 0.005f)
        {
            float color = BlackBoard.GetComponent<UISprite>().alpha;
            color = i;
            BlackBoard.GetComponent<UISprite>().alpha = color;
            //Color color = BlackBoard.GetComponent<UISprite>().material.color;
            //color = new Vector4(0, 0, 0, i);
            //BlackBoard.GetComponent<Renderer>().material.color = color;
            yield return 0;
        }
        BlackBoard.enabled = false;
    }

    public void TextPrintStart(int TextIndex) // 대화 시작
    {        
        TextLogSprite.enabled = true; //대화 배경 ON
        TextLabel.enabled = true; // 대사 ON
        SkipLabel.enabled = true; 

        if (eventScripts[TextIndex].target.Equals("Player")) // 스크립트 대사의 타겟이 플레이어라면
        {
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema01)) //플레이어의 테마가 1일 경우
            { 
                PlayerSprite.enabled = true;
               PlayerSprite.color = new Vector4(1, 1, 1, 1);
            }
            if(AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema02)) // 플레이어의 테마가 2일 경우
            {
                Theme01PlayerSprite.enabled = true;
                Theme01PlayerSprite.color = new Vector4(1, 1, 1, 1);
            }
            if(eventScripts[TextIndex].other.Equals("Helper"))
            {
                HelperSprite.enabled = true;
                HelperSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1); // 듣는 역할은 어두운 이미지
            }
            
            TextLabel.text = eventScripts[TextIndex].script; // 해당 스크립트
            WriteEffect.ResetToBeginning(); // 라이팅 출력
        }
        if (eventScripts[TextIndex].target.Equals("Helper"))
        {
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema01))
            {
                PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                PlayerSprite.enabled = true;
            }
            if (AniSpriteChange.Instance.currentName.Equals(AniSpriteChange.Instance.Thema02))
            {
                Theme01PlayerSprite.color = new Vector4(0.4f, 0.4f, 0.4f, 1);
                Theme01PlayerSprite.enabled = true;
            }
            HelperSprite.enabled = true;
            HelperSprite.color = new Vector4(1, 1, 1, 1);

            TextLabel.text = eventScripts[TextIndex].script;
            WriteEffect.ResetToBeginning();
        }
        if (eventScripts[TextIndex].target.Equals("SaveDoll"))
        {
            TextLabel.text = eventScripts[TextIndex].script;
            WriteEffect.ResetToBeginning();
        }
    }
    public float WaitCheck(int TextIndex)
    {
        WaitCount = 0;
        int i = eventScripts[TextIndex].script.Length;
        WaitCount = i * 0.13f;
        
        return WaitCount;
    }
    public void UIOff() // 대화 UI OFF
    {
        TextLabel.enabled = false;
        HelperSprite.enabled = false;
        PlayerSprite.enabled = false;
        Theme01PlayerSprite.enabled = false;
        TextLogSprite.enabled = false;
        SkipLabel.enabled = false;
    }

    public void HelpTextPrintStart(int TextIndex) // 안내 대사 ON
    {
        HelpTextLogSprite.enabled = true;
        HelpTextLabel.enabled = true;
        HelpTextLabel.text = InfoScripts[TextIndex].script; // 해당 스크립트
        
    }
    public void HelpUIOFF() // 모든 안내 UI OFF
    {
        HelpTextLogSprite.enabled = false;
        HelpTextLabel.enabled = false;
        FKey.enabled = false;
        DKey.enabled = false;
        WKey.enabled = false;
        SpaceKey.enabled = false;
        RightKey.enabled = false;
        LeftKey.enabled = false;
    }

    public void HelpInfo()
    {
        switch (HelpCount)
        {
            case 1:
                RightKey.enabled = true;
                LeftKey.enabled = true;
                SpaceKey.enabled = true;
                break;
            case 2:
                SpaceKey.enabled = true;
                SpaceKey.width = SpaceKey.width / 2;
                SpaceKey.height = SpaceKey.height / 2;
                SpaceKey.transform.localPosition -= Vector3.right * 235.0f;
                break;
            case 3:
                FKey.enabled = true;
                break;
            case 4:
                WKey.enabled = true;
                break;
        }
    }

    public void StateUI()
    {
        switch(StateUICount)
        {
            case 0:
                HPBar.enabled = false;
                CheckBar.enabled = false;
                break;
            case 1:
                HPBar.enabled = true;
                CheckBar.enabled = true;
                break;
        }
    }
    /*public void HelpInfo01() // 방향 점프 UI ON
    {
        RightKey.enabled = true;
        LeftKey.enabled = true;
        SpaceKey.enabled = true;
        
    }
    public void HelpInfo02() // 점프 UI ON
    {
        SpaceKey.enabled = true;
        SpaceKey.width = SpaceKey.width / 2;
        SpaceKey.height = SpaceKey.height / 2;
        SpaceKey.transform.localPosition -= Vector3.right * 235.0f;
    }
    public void HelpInfo03() // FKey UI ON
    {
        FKey.enabled = true;
    }
    public void HelpInfo04() // WKey UI ON
    {
        WKey.enabled = true;
    }*/

}
