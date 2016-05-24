using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Spine.Unity;

struct ObjectSetup
{
    //게임오브젝트의 위치, 각 게임오브젝트의 컴포넌트 작동값 초기화를 위한 구조체
    public GameObject gameObject;
    public Vector3 beforeTr;//오브젝트 초기위치
    public Vector3 beforeRot;//옵젝 초기 회전
    public bool isEnabled;//작동 온오프 적용

    /* bool isKine;//키네마틱 작동 여부
    public bool isCollOn;//콜리더
    public bool isTriggerOn;//트리거
    public bool TriggerSetterSet;//트리거세터 옵션
    public bool isSpriteShow;//스프라이트 on off
    public bool isEffectShow;//이펙트 on off
    public bool isGotAni; // 상호작용 or 함정인대 애니메이션을 포함하는 경우*/

};
public struct ReStartPlayerInfo
{
    public GameObject player; //플레이어 정보
    public Vector3 pos; //재시작 위치
    public GameObject effect; //이펙트 온 ? 오프
    public int hp;
    public int checkCount;
    public int tutoCount; //튜토리얼 정보
    public int skinNumber; //입은 옷(스킨,테마) 정보
};
public class GameManager : MonoBehaviour
{
    private int frameRate = 60;

    Stack<ObjectSetup> checkPointGameObjectStack;//되돌아갈 게임 오브젝트들을 담는 스텤
    List<ObjectSetup> resetGameObject;

    public TutorialManager tutoManager;
    ObjectSetup tempObjectSetup;//오브젝트를 담아두는 임시변수
    ReStartPlayerInfo restartNeedsPlayer;//게임오버후 시스템이 재시작할때 필요한 목록들

    public GameObject effect;//저장 이펰트
    public GameObject TeleportStartEff;//텔포 시작 이펰
    public GameObject TeleportEndEff;//텔포 종료 이펰
    public GameObject player;//플레이어 ㅋ
    public GameObject Weapon;//캐릭터 바늘

    MeshRenderer playerMesh;//플레이어 캐릭터 이미지
    Rigidbody2D playerRid;//물리
    SkeletonAnimation playerAni;//애니메이션
    AniSpriteChange aniChange;//애니 스킨체인지
    Vector3 playerBeforeTr;//체크포인트를 활성화한 위치
    Vector3 playerRestartTr;//초기 리스타트 위치
    Vector3 effectTr;//이펙트 위치

    public int hp = 100;//플레이어 채력
    public int checkCount = 4;
    int beforeHp;//체크포인트 발동할때 채력

    public bool nowLoad = false;//로드중?
    public bool nowSave = false;//세이부중?
    public bool playerRewind = false;//되감기중?
    public bool gameover = false;//게임오버?
    public bool reloadStage = false;//스테이지 재 로딩 ?

    //이벤 ---------------------------------------------------------
    public bool getKey = false; //열쇠
    public bool meetMan = false; //서랍장맨
    public bool returnToWay = false; //돌아가
    public bool getCompas = false; //컴퍼스겟?
    public bool getNeedle = false; //바늘 겟?
    public bool meetKnight = false; //기사 ㅎㅇ?
    public bool meetDancer = false; //댄서 ㅎㅇ?
    public bool exchange = false; //기사랑 템 교환 ?
    //--------------------------------------------------------------

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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
        gInstance = this;
        //DontDestroyOnLoad(this.gameObject);
        checkPointGameObjectStack = new Stack<ObjectSetup>();//스텤생성
        resetGameObject = new List<ObjectSetup>();
        checkPointGameObjectStack.Clear();
    }

    void Start()
    {
        //처음에 진입할 게임 씬
        SceneManager.LoadScene(01, LoadSceneMode.Additive);
        //StartCoroutine(AsnycSceneLoad());
        //if (tutoManager == null) { tutoManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>(); }
        //player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<GameObject>();
        //tutoManager.TutoStart(false);
        playerRid = player.GetComponent<Rigidbody2D>();
        playerAni = player.GetComponent<SkeletonAnimation>();
        aniChange = player.GetComponent<AniSpriteChange>();
        playerMesh = player.GetComponent<MeshRenderer>();
        playerRestartTr = player.transform.position;//재시작 위치
        //effectTr = player.transform.position + Vector3.up * 2f;      

    }
    public void CheckStart(Vector3 pos)//체크포인트를 설정하는 함 수
    {
        playerBeforeTr = pos;//세이브 한 위치 저장
        checkPointGameObjectStack.Clear();
        effect.SetActive(false);//이전 쳌포 이펙트는 끄고
        StartCoroutine(EffectDelay());//새로 실행
        CharacterController2D.Instance.SetAnimation("CHECKPOINT", false, 1.0f);
        nowSave = true;//세이브모드 작동
    }
    public void PushIntoStack(TrapObject temp)//옵젝을 건들때마다 실행되는 함수
    {
        if (nowSave && !playerRewind)//이동 중 자동 세이브 방지를 위해 !되감기
        {
            tempObjectSetup.gameObject = temp.gameObject;//게임옵젝
            tempObjectSetup.beforeTr = temp.beforeTr;//초기위치
            tempObjectSetup.isEnabled = temp.enabled;//작동 여부
            tempObjectSetup.beforeRot = temp.beforeRot;
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
    public void PlayerDamaged(int minusHp)
    {
        hp -= minusHp;
        UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
    }
    public void PlayerChecked(int minusCheck)
    {
        checkCount -= minusCheck;
        UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
    }
    public void HpPlusGet()
    {
        if (hp < 100)
        {
            hp += 25;
            UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
        }
    }
    public void CheckPlusGet()
    {
        if (checkCount < 4)
        {
            checkCount++;
            UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
        }
    }
    public void RestartPointSet()//게임오버후 재시작 할 위치를 지정합니다.
    {
        if (tutoManager == null) { tutoManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>(); }
        restartNeedsPlayer.hp = hp;
        restartNeedsPlayer.checkCount = checkCount;
        restartNeedsPlayer.pos = player.transform.position;
        restartNeedsPlayer.tutoCount = tutoManager.tutorialCnt;
        restartNeedsPlayer.skinNumber = AniSpriteChange.Instance.skinNum;
    }
    public void RestartToSetPoint()//게임오버후 재시작 할 위치를 받아옵니다.
    {
        /*tutoManager = null;
        tutoManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();*/
        if (restartNeedsPlayer.pos == null)//이전에 입력한 정보가 없으면 처음부터
        {
            hp = 100;
            checkCount = 4;
            player.transform.position = playerRestartTr;
            
            AniSpriteChange.Instance.SpriteChange(0);
            UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
            UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
        }
        else // 셋팅 된 위치가 있을 땐 셋팅된곳 부터
        {
            hp = restartNeedsPlayer.hp;
            checkCount = restartNeedsPlayer.checkCount;
            player.transform.position = restartNeedsPlayer.pos;
            tutoManager.tutorialCnt = restartNeedsPlayer.tutoCount;
            AniSpriteChange.Instance.SpriteChange(restartNeedsPlayer.skinNumber);
            UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
            UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
        }
        //tutoManager.RestartGame();
        //tutoManager.TutoStart(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !nowLoad && nowSave && !playerRewind && CharacterController2D.Instance.isSkill)//키 입력시 되감기 모드 ㄱㄱ
        {
            Debug.Log("load");
            nowLoad = true;//되감기 함수 실행을위해 체크
            nowSave = false;
            playerRewind = true;//플레이어 이전위치 이동위해 체크
            CharacterController2D.Instance.enabled = false;//컨트롤러 조작 x
            playerRid.isKinematic = true;//물리 x
            PlayerChecked(1);//쳌포인트 사용
            PopFromStack();//오브젝트들 원상복귀
            StartCoroutine(Teleport());//쳌포로 텔포
        }
        else if (Input.GetKeyDown(KeyCode.F) && checkCount > 0 && !playerRewind && CharacterController2D.Instance.isSkill)//쳌포인트가 한개도 설정 안됬을 때 작동
        {
            Debug.Log("save");
            CheckStart(player.transform.position);
            beforeHp = hp;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("리스타트 포인트 세팅");
            RestartPointSet();
        }
        if (Input.GetKey(KeyCode.O)) { Time.timeScale = 3; }
        else { Time.timeScale = 1; }
    }
    void FixedUpdate()
    {
        gameover = hp <= 0 ? true : false;//게임오버 여부 체크
        if (gameover && !playerRewind)
        {
            Debug.Log("꼐임오버");
            CharacterController2D.Instance.enabled = false;
            UIManager.Instance.enabled = false;
            if (!reloadStage) { StartCoroutine(GameOver()); reloadStage = true; }
        }
    }
    //게임오버후 게임을 재시작 하면 재시작 위치로 돌리는 함수 
    IEnumerator GameOver()
    {
        Debug.Log("게임 리셋");
        RestartToSetPoint();
        yield return new WaitForSeconds(1.0f);
        //게임 처음 상태로 초기화
        reloadStage = false;
        gameover = false;
        nowLoad = false;
        nowSave = false;
        playerRewind = false;
        playerBeforeTr = Vector3.zero;
        effect.SetActive(false);
        CharacterController2D.Instance.enabled = true;//조작 on
        playerRid.isKinematic = false;//물리 on
        UIManager.Instance.enabled = true;
        StopAllCoroutines();
    }
    //이펙트의 타이밍을 맞춘다
    IEnumerator EffectDelay()
    {
        Debug.Log("이펙");
        yield return new WaitForSeconds(0.65f);
        Debug.Log("딜레이");
        effect.SetActive(true);
        effect.transform.position = playerBeforeTr;
        StopCoroutine("EffectDelay");
    }
    //씬 로드 코루틴
    IEnumerator AsnycSceneLoad()
    {
        AsyncOperation asyncOp;
        asyncOp = SceneManager.LoadSceneAsync(01, LoadSceneMode.Additive);
        asyncOp.allowSceneActivation = false;
        while (!asyncOp.isDone)
        {
            if (asyncOp.progress == 0.9f) { asyncOp.allowSceneActivation = true; }
            Debug.Log((asyncOp.progress * 100) + 10 + " % 로드");
            yield return null;
        }
        if (tutoManager == null) { tutoManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>(); }
        //tutoManager.TutoStart(false);
        StopCoroutine("AsnycSceneLoad");
    }
    IEnumerator Teleport()
    {
        float time = 0;
        float temp = 1;
        float tempXPos = player.transform.localPosition.x;
        Debug.Log("이동 대기");
        CharacterController2D.Instance.enabled = false;//조작 off
        //TeleportStartEff.SetActive(true);
        //TeleportStartEff.transform.position = player.transform.position + Vector3.up * 3f;
        while (time <= 90)
        {
            if (player.transform.localPosition.x > tempXPos + 0.15f)
            {
                Debug.Log("호우");
                temp = -1f;
            }
            else if (player.transform.localPosition.x < tempXPos - 0.15f)
            {
                Debug.Log("날두");
                temp = 1f;
            }
            player.transform.Translate(Vector3.up * 0.25f *Time.deltaTime);
            if(time>30)
                player.transform.Translate(Vector3.right * 1.25f * temp * Time.deltaTime);
            Debug.Log("쉐이킷");
            yield return null;
            time++;
        }
        //yield return new WaitForSeconds(0.8f);
        playerMesh.enabled = false; //캐릭터 숨기기
        Weapon.SetActive(false);    //바늘도
        player.transform.position = playerBeforeTr;//케릭터 이동
        Debug.Log("이동");
        //TeleportStartEff.SetActive(false);
        //TeleportEndEff.SetActive(true);
        //TeleportEndEff.transform.position = player.transform.position + Vector3.up * 3f;
        yield return new WaitForSeconds(0.8f);
        //TeleportEndEff.SetActive(false);
        playerMesh.enabled = true; //캐릭터 보이기
        Weapon.SetActive(true);    //바늘도
        //셋팅 초기화
        nowLoad = false;
        gameover = false;
        hp = beforeHp;
        UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
        playerAni.skeleton.SetToSetupPose();//이동 완료시 애니메이션 기본자세로
        effect.SetActive(false);//완료 후 이펙트는 제거
        CharacterController2D.Instance.enabled = true;//조작 on
        playerRid.isKinematic = false;//물리 on  
        playerRewind = false;
        Debug.Log("완료");
        StopCoroutine("Teleport");
    }
}
