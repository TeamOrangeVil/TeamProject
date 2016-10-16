using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Spine.Unity;

struct ObjectSetup
{
    //게임오브젝트의 위치, 각 게임오브젝트의 컴포넌트 작동값 초기화를 위한 구조체
    public GameObject gameObject;
    public Vector3 beforeTr;                    //오브젝트 초기위치
    public Vector3 beforeRot;                   //옵젝 초기 회전
    public bool isEnabled;                      //작동 온오프 적용

    /* bool isKine;                             //키네마틱 작동 여부
    public bool isCollOn;                       //콜리더
    public bool isSpriteShow;                   //스프라이트 on off
    public bool isEffectShow;                   //이펙트 on off
    public bool isGotAni;                       // 상호작용 오브젝트가 애니메이션을 포함하는 경우
    */
};
public struct ReStartPlayerInfo
{
    public GameObject player;           //플레이어 정보
    public Vector3 pos;                 //재시작 위치
    public GameObject effect;           //이펙트 온 ? 오프
    public int hp;                      //체력
    public int checkCount;              //되돌리기 사용 가능 수
    public int tutoCount;               //튜토리얼 정보
    public int skinNumber;              //입은 옷(스킨,테마) 정보
};
public class GameManager : MonoBehaviour
{
    private int frameRate = 60;

    public GameInfo gameInfo;                   //게임의 현재 진행 정보를 담는다
    Stack<ObjectSetup> checkPointGameObjectStack;//되돌아갈 게임 오브젝트들을 담는 스텤

    public TutorialManager tutoManager;
    ObjectSetup tempObjectSetup;                //오브젝트를 담아두는 임시변수
    ReStartPlayerInfo restartNeedsPlayer;       //게임오버후 시스템이 재시작할때 필요한 목록들

    //public GameObject TeleportStartEff;       //텔포 시작 이펰
    //public GameObject TeleportEndEff;         //텔포 종료 이펰
    public GameObject effect;                   //저장 이펰트
    public GameObject player;                   //플레이어 ㅋ
    public GameObject Weapon;                   //캐릭터 바늘

    Vector3 playerBeforeTr;                     //체크포인트를 활성화한 위치
    Vector3 playerRestartTr;                    //초기 리스타트 위치
    Vector3 effectTr;                           //이펙트 위치

    public int hp = 100;                        //플레이어 채력
    public int checkCount = 4;                  //체크포인트 사용 가능 횟수
    int beforeHp = 100;                         //체크포인트 발동할때 채력

    public float playTime = 0;                  //플레이 시간
    public int hitTimes = 0;                    //피격 횟수
    public int deadTimes = 0;                   //사망 횟수
    public float QuestProgress = 0;             //퀘스트 진행정보 (%)

    public bool nowLoad = false;                //로드중?
    public bool nowSave = false;                //세이부중?
    public bool playerRewind = false;           //되감기중?
    public bool gameover = false;               //게임오버?
    public bool reloadStage = false;            //스테이지 재 로딩 ?

    //이벤 ---------------------------------------------------------
    public bool getKey = false;                 //열쇠
    public bool meetMan = false;                //서랍장맨
    public bool mansWall = false;               //서랍장맨 구했는지?
    public bool returnToWay = false;            //돌아가
    public bool getCompas = false;              //컴퍼스겟?
    public bool getNeedle = false;              //바늘 겟?
    public bool meetKnight = false;             //기사 ㅎㅇ?
    public bool meetDancer = false;             //댄서 ㅎㅇ?
    public bool exchange = false;               //기사랑 템 교환 ?
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
        checkPointGameObjectStack = new Stack<ObjectSetup>();//스텤생성
        checkPointGameObjectStack.Clear();
    }
    void Start()
    {
        //현재 게임 진행 데이터를 퀘스트 정보와 같이 담아두기 위해 선언
        gameInfo = new GameInfo();
        playerRestartTr = player.transform.position;//재시작 위치
        StartCoroutine(AsnycSceneLoad());
    }
    //현재 게임 진행 데이터를 gameInfo에 저장
    public void GameInfoDataSave(int saveNumber)
    {
        Debug.Log("데이터 저장");
        TutorialManager.Instance.TutoDataSave();
        gameInfo.IDNumber = saveNumber;
        gameInfo.hp = hp;
        gameInfo.beforeHp = beforeHp;
        gameInfo.checkCount = checkCount;
        gameInfo.hitTimes = hitTimes;
        gameInfo.deadTimes = deadTimes;
        gameInfo.playTimes = playTime;
        gameInfo.isSkill = CharacterController2D.Instance.isSkill;

        gameInfo.getKey = getKey;
        gameInfo.meetMan = meetMan;
        gameInfo.mansWall = mansWall;
        gameInfo.returnToWay = returnToWay;
        gameInfo.getCompas = getCompas;
        gameInfo.getNeedle = getNeedle;
        gameInfo.meetKnight = meetKnight;
        gameInfo.meetDancer = meetDancer;
        gameInfo.exchange = exchange;
        gameInfo.uiActive = UIManager.Instance.CheckBar.enabled;
        XMLParsing.Instance.XmlSaveGameData(gameInfo, saveNumber);
    }
    public void GameInfoDataLoad(int loadNumber)
    {
        if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.PLAYINFOXML))
        {
            Debug.Log("데이터 불러오기");
            TutorialManager.Instance.CutToon1.SetActive(false);
            gameInfo = XMLParsing.Instance.XmlLoadGameData(loadNumber);
            if (gameInfo.playerTr == Vector3.zero)
            {
                TutorialManager.Instance.CutToon1.SetActive(true);
                SoundEffectManager.Instance.BGMStart(1);
            }
            else
            {
                CharacterController2D.Instance.tr.position = gameInfo.playerTr;
                hp = gameInfo.hp;
                beforeHp = gameInfo.beforeHp;
                checkCount = gameInfo.checkCount;
                hitTimes = gameInfo.hitTimes;
                deadTimes = gameInfo.deadTimes;
                playTime = gameInfo.playTimes;

                getKey = gameInfo.getKey;
                meetMan = gameInfo.meetMan;
                mansWall = gameInfo.mansWall;
                returnToWay = gameInfo.returnToWay;
                getCompas = gameInfo.getCompas;
                getNeedle = gameInfo.getNeedle;
                meetKnight = gameInfo.meetKnight;
                meetDancer = gameInfo.meetDancer;
                exchange = gameInfo.exchange;
                CharacterController2D.Instance.isSkill = gameInfo.isSkill;
                TutorialManager.Instance.TutoDataLoad(gameInfo.tutoCount, gameInfo.skinNum, gameInfo.bgmNum, gameInfo.InfoDollTr, gameInfo.spiderTr, gameInfo.shadowSpiderTr,
                    gameInfo.spiderAct, gameInfo.shadowSpiderAct, gameInfo.dancerTr, gameInfo.gramophoneAct);

                UIManager.Instance.HPBar.enabled = gameInfo.uiActive;
                UIManager.Instance.CheckBar.enabled = gameInfo.uiActive;
                UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
                UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
            }
        }
        else
        {
            TutorialManager.Instance.CutToon1.SetActive(true);
            SoundEffectManager.Instance.BGMStart(1);
        }
        TutorialManager.Instance.tutoDataSet = true;
        Debug.Log("데이터 불러오기 완료");
    }
    public void CheckStart(Vector3 pos)//체크포인트를 설정하는 함 수
    {
        checkPointGameObjectStack.Clear();
        beforeHp = hp;//채력
        playerBeforeTr = pos;//세이브 한 위치 저장
        nowSave = true;//세이브모드 작동
        effect.SetActive(false);//이전 쳌포 이펙트는 끄고
        StartCoroutine(EffectDelay());//새로 실행
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
        }
    }
    //되감기 체크 후 오브젝트를 건들때마다 실행되는 함수
    public void PushIntoStack(FieldObject temp)
    {
        if (nowSave && !playerRewind)//이동 중 자동 세이브 방지를 위해 !되감기
        {
            tempObjectSetup.gameObject = temp.gameObject;       //게임옵젝
            tempObjectSetup.isEnabled = temp.enabled;           //작동 여부
            checkPointGameObjectStack.Push(tempObjectSetup);    //스텍에 입력
        }
    }
    //되감기 체크 후 이동된 오브젝트를 원래 위치로 돌리기 위한 함수
    void PopFromStack()
    {
        for (int i = checkPointGameObjectStack.Count; i > 0; i--)
        {
            tempObjectSetup = checkPointGameObjectStack.Pop();  //스텍에서 하나 빼서
            tempObjectSetup.gameObject.SetActive(true);         //작동시켜서
            //자기위치로 돌아가도록 한다.
            tempObjectSetup.gameObject.SendMessage("StartReturn", tempObjectSetup.beforeTr, 
                SendMessageOptions.DontRequireReceiver);
            
        }
        //끝나면 임시변수 정보 초기화
        tempObjectSetup.gameObject = null;
        tempObjectSetup.beforeTr = Vector3.zero;
        checkPointGameObjectStack.Clear();
    }
    //되돌리기를 실행하면 작동하는 함수
    public void RewindStart()
    {
        nowLoad = true;                                         //되감기 함수 실행을위해 체크
        nowSave = false;
        playerRewind = true;                                    //플레이어 이전위치 이동위해 체크
        CharacterController2D.Instance.isAct = true;            //컨트롤러 조작 x
        CharacterController2D.Instance.rb.isKinematic = true;   //물리 off
        CharacterController2D.Instance.SetAnimation("DRAG", false, 1.0f);
        SoundEffectManager.Instance.SoundDelay("Rewind", 0);
        PlayerChecked(1);                                       //쳌포인트 사용
        PopFromStack();                                         //오브젝트들 원상복귀
        //StartCoroutine(Teleport());                           //쳌포로 텔포
        StartCoroutine(Rewind());                               //쳌포로 귀환
    }
    public void PlayerDamaged(int minusHp)
    {
        hp -= minusHp;
        UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
        CharacterController2D.Instance.StartCoroutine("PlayerHit");
        hitTimes++;
    }
    public void PlayerChecked(int minusCheck)
    {
        checkCount -= minusCheck;
        UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
    }
    public void HpPlusGet()
    {
        /*if (hp < 100)
        {
            hp += 25;
            UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
        }*/
        hp = 100;
        UIManager.Instance.HPBar.fillAmount = 1f;
    }
    public void CheckPlusGet()
    {
        /*if (checkCount < 4)
        {
            checkCount++;
            UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
        }*/
        checkCount = 4;
        UIManager.Instance.CheckBar.fillAmount = 1f;
    }
    //게임오버후 재시작 할 위치를 지정합니다.
    public void RestartPointSet()
    {
        if (tutoManager == null) { tutoManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>(); }
        restartNeedsPlayer.hp = hp;
        restartNeedsPlayer.checkCount = checkCount;
        restartNeedsPlayer.pos = player.transform.position;
        //restartNeedsPlayer.tutoCount = tutoManager.tutorialCnt;
        restartNeedsPlayer.skinNumber = AniSpriteChange.Instance.skinNum;
    }
    //게임오버후 재시작 할 위치를 받아옵니다.
    public void RestartToSetPoint()
    {
        //이전에 입력한 정보가 없으면 처음부터
        if (tutoManager == null)
        {
            hp = 100;
            checkCount = 4;
            player.transform.position = playerRestartTr;
            AniSpriteChange.Instance.SpriteChange(0);
            UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
            UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
            checkPointGameObjectStack.Clear();
        }
        //셋팅 된 위치가 있을 땐 셋팅된곳 부터
        else 
        {
            hp = restartNeedsPlayer.hp;
            checkCount = restartNeedsPlayer.checkCount;
            player.transform.position = restartNeedsPlayer.pos;
            AniSpriteChange.Instance.SpriteChange(restartNeedsPlayer.skinNumber);
            UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
            UIManager.Instance.CheckBar.fillAmount = checkCount * 0.25f;
            checkPointGameObjectStack.Clear();
        }
        //스테이지 리로드 완료
        reloadStage = false;
    }
    void Update()
    {
        //키 입력시 되감기 모드 ㄱㄱ
        if (Input.GetKeyDown(KeyCode.W) && !nowLoad && nowSave && !playerRewind && !gameover &&
            CharacterController2D.Instance.isSkill && !CharacterController2D.Instance.isAct)
        {
            RewindStart();
        }
        playTime = playTime + Time.deltaTime;
        /*else if (Input.GetKeyDown(KeyCode.F) && checkCount > 0 && !playerRewind &&
            CharacterController2D.Instance.isSkill && !CharacterController2D.Instance.isAct)//쳌포인트가 한개도 설정 안됬을 때 작동
        {
            Debug.Log("save");
            CheckStart(player.transform.position);
            beforeHp = hp;
        }*/
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F11))
        {
            RestartPointSet();
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            GameInfoDataSave(SlotDataNumberSave.Instance.slotNum);
        }
        // if (Input.GetKey(KeyCode.O)) { Time.timeScale = 3; }
        // else if (Input.GetKey(KeyCode.L)) { Time.timeScale = 0.5f; }
        // else { Time.timeScale = 1; }
#endif
    }
    void FixedUpdate()
    {
        //게임오버 여부 체크
        //게임오버 체크 1 - 채력 체크
        if (!gameover) {gameover = hp <= 0 ? true : false; }
        //게임오버 체크 2 - 낙하 체크
        else if (CharacterController2D.Instance.tr.position.y < -11.5f)
        {
            if (!SoundEffectManager.Instance.effectAudio.isPlaying)
            {
                SoundEffectManager.Instance.SoundDelay("GameOver", 0);
            }
            gameover = true;
        }
        if (gameover && !playerRewind)// 게임오버 & !순간이동중
        {
            CharacterController2D.Instance.isAct = false;
            UIManager.Instance.enabled = false;
            //사망 전 최종 저장위치로, 중복 실행 방지위해 if(!reloadStage)처리
            if (!reloadStage) { StartCoroutine(GameOver()); reloadStage = true; }
        }
    }
    //씬 로드 코루틴
    IEnumerator AsnycSceneLoad()
    {
        AsyncOperation asyncOp;
        asyncOp = SceneManager.LoadSceneAsync(03, LoadSceneMode.Additive);
        asyncOp.allowSceneActivation = false;
        while (!asyncOp.isDone)
        {
            if (asyncOp.progress == 0.9f) { asyncOp.allowSceneActivation = true; }
            yield return null;
        }
        //if (tutoManager == null) { tutoManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>(); }
        Debug.Log("데이터 체크");
        if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.PLAYINFOXML))
        {
            Debug.Log("데이터 불러오기");
            TutorialManager.Instance.CutToon1.SetActive(false);
            GameInfoDataLoad(SlotDataNumberSave.Instance.slotNum);
        }
        TutorialManager.Instance.tutoDataSet = true;
        Debug.Log("데이터 불러오기 완료");
        if (System.IO.File.Exists(Application.streamingAssetsPath + XmlConstancts.GAMEOPTIONXML))
        {
            SoundEffectManager.Instance.SoundOptionSet();
        }
        StopCoroutine("AsnycSceneLoad");
    }
    //게임오버후 게임을 재시작 하면 재시작 위치로 돌리는 함수 
    IEnumerator GameOver()
    {
        if (!playerRewind)//순간 이동 중이 아닐때 실행
        {
            CharacterController2D.Instance.rb.isKinematic = true;
            SoundEffectManager.Instance.SoundDelay("GameOver", 0);
            UIManager.Instance.StartCoroutine("FadeOut");
            yield return new WaitForSeconds(0.5f);
            RestartToSetPoint();
            //게임 처음 상태로 초기화
            gameover = false;
            nowLoad = false;
            nowSave = false;
            playerRewind = false;
            playerBeforeTr = Vector3.zero;
            effect.SetActive(false);
            CharacterController2D.Instance.isAct = false;//조작 on
            CharacterController2D.Instance.rb.isKinematic = false;
            UIManager.Instance.enabled = true;
            deadTimes++;
        }
        yield return new WaitForSeconds(3f);
        UIManager.Instance.StartCoroutine("FadeIn");
        StopAllCoroutines();
    }
    //이펙트의 타이밍을 맞춘다
    IEnumerator EffectDelay()
    {
        //CharacterController2D.Instance.SetAnimation("CHECKPOINT", false, 1.0f);
        yield return new WaitForSeconds(0.65f);
        effect.SetActive(true);
        effect.transform.position = playerBeforeTr;
        SoundEffectManager.Instance.SoundDelay("CheckPoint", 0);
        StopCoroutine("EffectDelay");
    }
    //되감기
    IEnumerator Rewind()
    {
        SoundEffectManager.Instance.SoundDelay("Rewind", 0);
        while (Vector2.Distance(player.transform.position, playerBeforeTr) > 0.09375f)
        {
            player.transform.position = Vector2.Lerp(player.transform.position, playerBeforeTr, (Time.deltaTime * 1.5f) + 0.05f);
            if (player.transform.position.x > playerBeforeTr.x) { CharacterController2D.Instance.Player.skeleton.FlipX = true; }
            else if (player.transform.position.x < playerBeforeTr.x) { CharacterController2D.Instance.Player.skeleton.FlipX = false; }
            yield return null;
        }
        playerRewind = false;
        SoundEffectManager.Instance.SoundEnd();
        CharacterController2D.Instance.rb.isKinematic = false;//물리 on
        CharacterController2D.Instance.isAct = false;//조작 on
        nowLoad = false;
        //gameover = false;
        hp = beforeHp;
        UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
        //이동 완료시 애니메이션 기본자세로
        effect.SetActive(false);
        StopCoroutine("Rewind");
    }
    //순간이동
    /*IEnumerator Teleport()
    {
        if (!gameover)
        {
            Debug.Log("이동 대기");
            CharacterController2D.Instance.isAct = true;//조작 off
            TeleportStartEff.SetActive(true);
            TeleportStartEff.transform.position = player.transform.position + Vector3.up * 2.5f;
            yield return new WaitForSeconds(0.05f);
            CharacterController2D.Instance.SetAnimation("save2", false, 1.0f);//출발 애니
            yield return new WaitForSeconds(0.35f);
            playerMesh.enabled = false; //캐릭터 숨기기
            Weapon.SetActive(false);    //바늘도
            Debug.Log("이동");
            player.transform.position = playerBeforeTr;
            TeleportStartEff.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            TeleportEndEff.SetActive(true);
            TeleportEndEff.transform.position = player.transform.position + Vector3.up * 3f;
            CharacterController2D.Instance.SetAnimation("save", false, 1.0f);//출발 애니
            yield return new WaitForSeconds(0.4f);
            TeleportEndEff.SetActive(false);
            playerMesh.enabled = true; //캐릭터 보이기
            Weapon.SetActive(true);    //바늘도
            //셋팅 초기화
            hp = beforeHp;
            UIManager.Instance.HPBar.fillAmount = hp * 0.01f;
            playerAni.skeleton.SetToSetupPose();//이동 완료시 애니메이션 기본자세로
            effect.SetActive(false);//완료 후 이펙트는 제거
            playerRewind = false;
            nowLoad = false;
            gameover = false;
            playerRid.isKinematic = false;//물리 on  
            CharacterController2D.Instance.isAct = false;//조작 on
            Debug.Log("완료");
            StopCoroutine("Teleport");
        }
    }*/
}