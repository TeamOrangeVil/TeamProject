using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Spine.Unity;

struct ObjectSetup
{
    //게임오브젝트의 위치, 각 게임오브젝트의 컴포넌트 작동값 초기화를 위한 구조체
    public GameObject gameObject;
    public Vector3 beforeTr;//오브젝트 초기위치
    public bool isEnabled;//작동 온오프 적용
    public bool isKine;//키네마틱 작동 여부
    public bool isCollOn;//콜리더
    public bool isTriggerOn;//트리거
    public bool TriggerSetterSet;//트리거세터 옵션
    public bool isSpriteShow;//스프라이트 on off
    public bool isEffectShow;//이펙트 on off
    public bool isGotAni; // 상호작용 or 함정인대 애니메이션을 포함하는 경우
}
public class GameManager : MonoBehaviour
{
    Stack<ObjectSetup> checkPointGameObjectStack;//되돌아갈 게임 오브젝트들을 담는 스텤
    ObjectSetup tempObjectSetup;//오브젝트를 담아두는 임시변수
    Vector3 playerBeforeTr;//체크포인트를 활성화한 위치

    public GameObject effect;//저장 이펰트
    public GameObject player;//플레이어 ㅋ
    public SpriteRenderer gameoverSprite;
    CharacterController2D controller;//플레이어 조작
    Rigidbody2D playerRid;//물리
    SkeletonAnimation playerAni;//애니메이션

    public int hp = 100;//플레이어 채력
    public int checkCount = 4;
    int beforeHp;//체크포인트 발동할때 채력

    public bool nowLoad = false;//로드중?
    public bool nowSave = false;//세이부중?
    public bool playerRewind = false;//되감기중?
    public bool gameover = false;

    private static GameManager gInstance = null;

    public static GameManager Instance
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
        DontDestroyOnLoad(this.gameObject);
        checkPointGameObjectStack = new Stack<ObjectSetup>();//스텤생성
        checkPointGameObjectStack.Clear();
    }

    void Start()
    {
        SceneManager.LoadScene(01, LoadSceneMode.Additive);
        //player = GameObject.FindGameObjectWithTag("player").GetComponent<GameObject>();
        controller = player.GetComponent<CharacterController2D>();
        playerRid = player.GetComponent<Rigidbody2D>();
        playerAni = player.GetComponent<SkeletonAnimation>();
        gameoverSprite = GameObject.Find("gameoverTemp").GetComponent<SpriteRenderer>();
        gameoverSprite.enabled = false;
        //처음에 진입할 게임 씬
    }
    public void CheckStart(Vector3 pos)//체크포인트를 설정하는 함 수
    {
        PlayerChecked(1);
        playerBeforeTr = pos;//세이브 한 위치 저장
        checkPointGameObjectStack.Clear();
        effect.SetActive(false);
        effect.SetActive(true);
        effect.transform.position = playerBeforeTr;
        CharacterController2D.Instance.SetAnimation("CHECKPOINT", false, 1.0f);
        nowSave = true;//세이브모드 작동
    }

    public void PushIntoStack(FieldObject temp)//옵젝을 건들때마다 실행되는 함수
    {
        if (nowSave && !playerRewind)//이동 중 자동 세이브 방지를 위해 !되감기
        {
            tempObjectSetup.gameObject = temp.gameObject;//게임옵젝
            tempObjectSetup.beforeTr = temp.beforeTr;//초기위치
            tempObjectSetup.isEnabled = temp.enabled;//작동 여부
            //tempObjectSetup.isKine = temp.isKine;
            checkPointGameObjectStack.Push(tempObjectSetup);//스텍에 입력
            Debug.Log("옵젝 초기위치 저-장");
            Debug.Log(checkPointGameObjectStack.Count);
        }
    }
    public void PushIntoStack(TrapObject temp)//옵젝을 건들때마다 실행되는 함수
    {
        if (nowSave && !playerRewind)//이동 중 자동 세이브 방지를 위해 !되감기
        {
            tempObjectSetup.gameObject = temp.gameObject;//게임옵젝
            tempObjectSetup.beforeTr = temp.beforeTr;//초기위치
            tempObjectSetup.isEnabled = temp.enabled;//작동 여부
            //tempObjectSetup.isKine = temp.isKine;
            checkPointGameObjectStack.Push(tempObjectSetup);//스텍에 입력
            Debug.Log("옵젝 초기위치 저-장");
            Debug.Log(checkPointGameObjectStack.Count);
        }
    }
    void PopFromStack()//옵젝을 원래 위치로 돌리기 위한 함수
    {
        for (int i = checkPointGameObjectStack.Count; i > 0; i--)
        {
            tempObjectSetup = checkPointGameObjectStack.Pop();//스텍에서 하나 빼서
            tempObjectSetup.gameObject.SetActive(true);//작동시켜서
            tempObjectSetup.gameObject.SendMessage("StartReturn", tempObjectSetup.beforeTr, SendMessageOptions.DontRequireReceiver);
            //자기위치로 돌아가도록 한다.
        }
        //끝나면 임시변수 정보 초기화
        tempObjectSetup.gameObject = null;
        tempObjectSetup.beforeTr = Vector3.zero;
        checkPointGameObjectStack.Clear();
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W) && !nowLoad && nowSave && !playerRewind && CharacterController2D.Instance.isSkill)//키 입력시 되감기 모드 ㄱㄱ
        {
            Debug.Log("load");
            nowLoad = true;//현재 되감기중
            nowSave = false;
            playerRewind = true;//플레이어 되감기중
            controller.enabled = false;//컨트롤러 조작 x
            playerRid.isKinematic = true;//물리 x
            PopFromStack();
        }
        else if (Input.GetKeyDown(KeyCode.F) && checkCount > 0 && !playerRewind && CharacterController2D.Instance.isSkill)//쳌포인트가 한개도 설정 안됬을 때 작동
        {
            Debug.Log("save");
            CheckStart(player.transform.position);
            beforeHp = hp;
            effect.SetActive(true);
            effect.transform.position = player.transform.position;
        }
        gameover = hp <= 0 ? true : false;
        if (playerRewind)//플레이어 되감기
        {
            player.transform.position = Vector2.Lerp(player.transform.position, playerBeforeTr, Time.deltaTime * 1.25f);
            Debug.Log("이동중");
            if (Vector2.Distance(player.transform.position, playerBeforeTr) < 0.25f)
            {
                playerRewind = false;
                nowLoad = false;
                controller.enabled = true;//조작 on
                playerRid.isKinematic = false;//물리 on
                hp = beforeHp;
                UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
                playerAni.skeleton.SetToSetupPose();//이동 완료시 애니메이션 기본자세로
                effect.SetActive(false);
            }
        }
        else if (gameover)
        {
            CharacterController2D.Instance.enabled = false;
            gameoverSprite.enabled = true;
            UIManager.Instance.gameObject.SetActive(false);
            Debug.Log("꼐임오버");
        }
    }
    //임시들 입니다.
    public void PlayerDamaged(int minusHp)
    {
        hp -= minusHp;
        UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
        //CharacterController2D.Instance.GetComponent<>
    }
    public void PlayerChecked(int minusCheck)
    {
        checkCount -= minusCheck;
        UIManager.Instance.CheckBar.fillAmount = hp * 0.25f;
    }
}

