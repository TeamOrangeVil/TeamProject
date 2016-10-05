using UnityEngine;
using System.Collections;
using Spine.Unity;

//BackGroundAniObjName : 오브젝트 이름
struct Objname
{
    public const string key01 = "Key";
    public const string men = "Men_Temp";
    public const string men2 = "Men_Temp2";
    public const string hpPlus = "HPItem";
    public const string checkPointPlus = "SavePointItem";
    public const string cutOffBucket = "BucketFront";
    public const string compass = "Compass";
    public const string knight = "Knight";
    public const string dancer = "Dancer";
    public const string underGround = "UnderGround";
    public const string eventWall = "EventWall";
    public const string penguin = "small penguins";
    public const string thread = "Therad";
    public const string toolBox = "tool box";
    public const string flowerBowl = "FlowerBowl";
    public const string dollsTomb = "new dead dolls";
    public const string cloud = "cloud";
    public const string dollsdrawing = "DollPaper";
    public const string memo = "Corkfan";
    public const string gramophone = "gramophone";//축음기
    public const string safeSawdust = "sawdust";
};

public class FieldObject : MonoBehaviour
{

    public Collider2D boxColl; //뺰쓰 트리거
    public SpriteRenderer spriteRender;//스프라이트 이미지
    public GameObject oneTimeEffect; //옵젝 이펙트
    public GameObject roofEffect;//옵젝 이펰트2
    SkeletonAnimation objAnimation;//오브젝트 애니메이션
    MeshRenderer objMeshRender;//옵젝 매시렌더러
    AudioSource effectSound;

    public bool isActing;//작동여부
    bool soonOff;//곧 오프

    void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        objMeshRender = GetComponent<MeshRenderer>();
        objAnimation = GetComponent<SkeletonAnimation>();
        if (gameObject.name.Equals(Objname.men))
        {
            effectSound = GetComponent<AudioSource>();
            SoundEffectManager.Instance.GetAudio(effectSound);
            effectSound.Play();
        }
        if (gameObject.name.Equals(Objname.knight))
        {
            objAnimation.skeleton.FlipX = true;
        }
    }
    public void AniSetOn()//배경 오브젝트의 애니메이션 작동
    {
        objAnimation.state.SetAnimation(0, "ANIMATION", false);
    }
    public void StartEffect()
    {
        oneTimeEffect.SetActive(true);
    }
    void ObjAction()
    {
        switch (gameObject.name)
        {
            case Objname.key01://열쇠
                //if (GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); }// 중복세이브 방지
                isActing = true;
                GameManager.Instance.getKey = true;
                SoundEffectManager.Instance.SoundDelay(gameObject.name, 1);
                StartCoroutine(DilayEffect(1));
                roofEffect.SetActive(false);
                spriteRender.enabled = false;
                boxColl.enabled = false;
                break;
            case Objname.men://서랍장 인형
                if (GameManager.Instance.getKey)
                {
                    isActing = true;
                    GameManager.Instance.meetMan = true;
                    effectSound.Stop();
                    //SoundEffectManager.Instance.SoundDelay(gameObject.name, 0);
                    //objAnimation.state.ClearTrack(0);
                }
                else { }
                break;
            case Objname.men2://다시 만난 서랍장 인형
                isActing = true;
                GameManager.Instance.meetMan = true;
                break;
            case Objname.hpPlus://채력++ 
                GameManager.Instance.HpPlusGet();
                SoundEffectManager.Instance.SoundDelay(gameObject.name, 1);
                isActing = true;
                objMeshRender.enabled = false;
                roofEffect.SetActive(false);
                StartCoroutine(DilayEffect(0.9f));
                break;
            case Objname.checkPointPlus://쳌포++
                GameManager.Instance.CheckPlusGet();
                SoundEffectManager.Instance.SoundDelay(gameObject.name, 1);
                isActing = true;
                objMeshRender.enabled = false;
                //spriteRender.enabled = false;
                roofEffect.SetActive(false);
                StartCoroutine(DilayEffect(0.9f));
                break;
            case Objname.cutOffBucket://빠께스 단면
                if (!GameManager.Instance.exchange)
                {
                    if (isActing)//입장
                    {
                        spriteRender.enabled = true;
                        isActing = false;
                    }
                    else//퇴장
                    {
                        spriteRender.enabled = false;
                        isActing = true;
                    }
                }
                else
                {

                }
                break;
            case Objname.compass://컴퍼스 템
                isActing = true;
                GameManager.Instance.getCompas = true;
                SoundEffectManager.Instance.SoundDelay(gameObject.name, 1);
                //roofEffect.SetActive(false);
                StartCoroutine(DilayEffect(1));
                spriteRender.enabled = false;
                boxColl.enabled = false;
                break;
            case Objname.dancer://댄서인형
                isActing = true;
                GameManager.Instance.meetDancer = true;
                break;
            case Objname.knight://기사
                isActing = true;
                GameManager.Instance.meetKnight = true;
                break;
            case Objname.underGround://추락
                isActing = true;
                GameManager.Instance.PlayerDamaged(100);
                break;
            case Objname.eventWall:
                if (GameManager.Instance.meetMan)//서랍장맨을 구해주면
                {
                    boxColl.enabled = false;
                }
                else//아닐 경우
                {
                    GameManager.Instance.mansWall = true;
                }
                break;
            case Objname.thread://털실
                objAnimation.state.SetAnimation(0, "show", false);
                isActing = true;
                break;
            case Objname.memo://메모
                objAnimation.state.SetAnimation(0, "show", false);
                isActing = true;
                break;
            case Objname.toolBox://공구통
                objAnimation.state.SetAnimation(0, "toolbox", false);
                isActing = true;
                break;
            case Objname.dollsTomb://인형 무덤
                objAnimation.state.SetAnimation(0, "show", false);
                isActing = true;
                break;
            case Objname.dollsdrawing://인형 도면
                objAnimation.state.SetAnimation(0, "show", false);
                isActing = true;
                break;
            case Objname.gramophone://축음기
                //if(퀘스트 완료 조건)
                objAnimation.state.SetAnimation(0, "show", false);
                isActing = true;
                break;
            case Objname.cloud://구름
                //objAnimation.state.SetAnimation(0, "show", false);
                oneTimeEffect.SetActive(true);
                isActing = true;
                break;
            case Objname.safeSawdust://안전 톱밥
                SoundEffectManager.Instance.SoundDelay(gameObject.name, 0);
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!GameManager.Instance.playerRewind)
        {
            if (!isActing && col.gameObject.name.Equals("Player"))
            {
                ObjAction();
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (!GameManager.Instance.playerRewind && isActing && col.gameObject.name.Equals("Player"))
        {
            if (gameObject.name.Equals(Objname.cutOffBucket))
            {
                ObjAction();
            }
            else if (gameObject.name.Equals(Objname.dancer))
            {
                GameManager.Instance.meetDancer = false;
                isActing = false;
            }
            else if (gameObject.name.Equals(Objname.knight))
            {
                GameManager.Instance.meetKnight = false;
                isActing = false;
            }
            else if (gameObject.name.Equals(Objname.cloud))
            {
                oneTimeEffect.SetActive(false);
                isActing = false;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!GameManager.Instance.playerRewind)
        {
            if (!isActing && col.gameObject.name.Equals("Player"))
            {
                ObjAction();
            }
        }
    }
    void StartReturn(Vector3 tempTr)
    {
        if (this.name.Equals(Objname.key01) && GameManager.Instance.getKey)
        {
            GameManager.Instance.getKey = false;
            spriteRender.enabled = true;
            boxColl.enabled = true;
        }
    }
    IEnumerator DilayObject(float time)
    {
        boxColl.enabled = false;
        yield return new WaitForSeconds(time);
        boxColl.enabled = true;
        isActing = false;
        StopCoroutine(DilayObject(0));
    }
    IEnumerator DilayEffect(float time)
    {
        oneTimeEffect.SetActive(true);
        yield return new WaitForSeconds(time);
        oneTimeEffect.SetActive(false);
        isActing = false;
        boxColl.enabled = false;
        StopCoroutine(DilayEffect(0));
    }
    public void EffectOn()
    {
        //oneTimeEffect.SetActive(true);
    }
}