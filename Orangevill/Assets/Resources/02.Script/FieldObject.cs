using UnityEngine;
using System.Collections;
using Spine.Unity;

//BackGroundAniObjName : 오브젝트 이름
struct Objname
{
    public const string key01 = "Key";
    public const string men = "Men_Temp";
    public const string hpPlus = "HPItem";
    public const string checkPointPlus = "SavePointItem";
    public const string cutOffBucket = "BucketFront";
    public const string compass = "Compass";
    public const string knight = "Knight";
    public const string dancer = "Dancer";
    public const string underGround = "UnderGround";
};

public class FieldObject : MonoBehaviour
{

    public Collider2D boxColl; //뺰쓰 트리거
    public SpriteRenderer spriteRender;//스프라이트 이미지
    public GameObject objEffect; //옵젝 이펙트
    public GameObject childEffect;//옵젝 이펰트2
    SkeletonAnimation objAnimation;//오브젝트 애니메이션
    MeshRenderer objMeshRender;//옵젝 매시렌더러

    public bool isActing;//작동여부
    bool soonOff;//곧 오프

    void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        objMeshRender = GetComponent<MeshRenderer>();
        objAnimation = GetComponent<SkeletonAnimation>();
        //childEffect = GetComponentInChildren<GameObject>();
        
    }

    public void AniSetOn()//배경 오브젝트의 애니메이션 작동
    {
        objAnimation.state.SetAnimation(0, "ANIMATION", false);
    }

    void ObjAction()
    {
        switch (gameObject.name)
        {
            case Objname.key01://열쇠
                Debug.Log("겟키");
                isActing = true;
                GameManager.Instance.getKey = true;
                StartCoroutine(DilayEffect(1));
                //childEffect.SetActive(false);
                spriteRender.enabled = false;
                boxColl.enabled = false;
                break;
            case Objname.men://서랍장 인형
                if (GameManager.Instance.getKey)
                {
                    Debug.Log("ㅎㅇ");
                    isActing = true;
                    GameManager.Instance.meetMan = true;
                    objAnimation.state.SetAnimation(0, "open", false);
                    //objAnimation.state.ClearTrack(0);
                }
                else { Debug.Log("끄져"); }
                break;
            case Objname.hpPlus://채력++ 
                GameManager.Instance.HpPlusGet();
                isActing = true;
                objMeshRender.enabled = false;
                objEffect.SetActive(true);
                childEffect.SetActive(false);
                StartCoroutine(DilayEffect(1));
                break;
            case Objname.checkPointPlus://쳌포++
                GameManager.Instance.CheckPlusGet();
                isActing = true;
                spriteRender.enabled = false;
                objEffect.SetActive(true);
                childEffect.SetActive(false);
                StartCoroutine(DilayEffect(1));
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
                break;
            case Objname.dancer://댄서인형
                isActing = true;
                GameManager.Instance.meetDancer = true;
                break;
            case Objname.knight://기사
                isActing = true;
                if (GameManager.Instance.getCompas)
                {

                }
                GameManager.Instance.meetKnight = true;
                break;
            case Objname.underGround://추락
                isActing = true;
                GameManager.Instance.PlayerDamaged(100);
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
        objEffect.SetActive(true);
        yield return new WaitForSeconds(time);
        objEffect.SetActive(false);
        isActing = false;
        boxColl.enabled = false;
        StopCoroutine(DilayObject(0));
    }

}