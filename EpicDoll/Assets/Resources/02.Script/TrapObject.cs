using UnityEngine;
using System.Collections;
using Spine.Unity;

struct TrapNames//트렙의 이름들을 미리 선언해서 스위치에 활용
{
    public const string tack = "Tack1";
    public const string sawDust = "Trap_SawDust";
    public const string airplane = "AirPlane_Trap";
    public const string airplane2 = "AirPlane_Trap2";
    public const string mobleFloor = "MobleFloor";
    public const string brokenHouse = "BrokenHouse";
};
public class TrapObject : MonoBehaviour
{

    public Vector3 beforeTr; //오브젝트의 초기 위치
    public Vector3 beforeRot; //오브젝트의 초기 회전
    public Collider2D boxColl; //뺰쓰 트리거
    public GameObject objEffect; //옵젝 이펙트
    public SpriteRenderer spriteRender;//스프라이트 이미지
    public SkeletonAnimation trapAni;//애니메이션
    Rigidbody2D objRigidbody; //물리작용
    public DistanceJoint2D distJoint;//모빌 메달림

    float head; //플레이어 이동방향
    float tempZRot; // 로테이션을 위한 z값 저장

    public bool isSaved;//중복저장을 막기위해 선언
    public bool isLoading;//원래위치로 불려갈 때
    public bool isActing;//동작 중복실행을 막기위해 선언
    // Use this for initialization
    void Start()
    {
        beforeTr = GetComponent<Transform>().transform.position;
        spriteRender = GetComponent<SpriteRenderer>();
        boxColl = GetComponent<BoxCollider2D>();
        objRigidbody = GetComponent<Rigidbody2D>();
        isLoading = false;
        isSaved = false;
        isActing = false;
        tempZRot = transform.rotation.z;
    }
    public void TrapAction()
    {
        if (!isActing)
        {
            head = CharacterController2D.Instance.transform.position.x < transform.position.x == true ? 1f : -1f;
            switch (gameObject.name)
            {
                case TrapNames.tack://압정
                    isActing = true;
                    //if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                    StartCoroutine(DamageDilay(0.6f));
                    CharacterController2D.Instance.rb.AddForce(Vector2.left * 2.5f * head + Vector2.up * 5f, ForceMode2D.Impulse);
                    //CharacterController2D.Instance.StartCoroutine(PlayerHit());
                    SoundEffectManager.Instance.SoundDelay("Dameged",0);
                    GameManager.Instance.PlayerDamaged(25);
                    break;
                case TrapNames.sawDust://톱밥장애물
                    isActing = true;
                    //if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                    StartCoroutine(DamageDilay(0.6f));
                    CharacterController2D.Instance.rb.AddForce(Vector2.left * 2.5f * head + Vector2.up * 5f, ForceMode2D.Impulse);
                    //CharacterController2D.Instance.StartCoroutine(PlayerHit());
                    SoundEffectManager.Instance.SoundDelay("Dameged", 0);
                    GameManager.Instance.PlayerDamaged(25);
                    break;
                case TrapNames.airplane://비햏기
                    if (objRigidbody.isKinematic)
                    {
                        StartCoroutine(DamageDilay(0.8f));
                        if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                        objRigidbody.isKinematic = false;
                    }
                    else
                    {
                        CharacterController2D.Instance.rb.AddForce(Vector2.left * 8f * head + Vector2.up * 5f, ForceMode2D.Impulse);
                        //CharacterController2D.Instance.StartCoroutine(PlayerHit());
                        SoundEffectManager.Instance.SoundDelay("Dameged", 0);
                        GameManager.Instance.PlayerDamaged(25);
                        isActing = true;
                    }
                    break;
                case TrapNames.airplane2://비햏기2
                    if (objRigidbody.isKinematic)
                    {
                        StartCoroutine(DamageDilay(0.8f));
                        if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                        objRigidbody.isKinematic = false;
                    }
                    else
                    {
                        CharacterController2D.Instance.rb.AddForce(Vector2.left * 8f * head + Vector2.up * 5f, ForceMode2D.Impulse);
                        //CharacterController2D.Instance.StartCoroutine(PlayerHit());
                        GameManager.Instance.PlayerDamaged(25);
                        //isActing = true;
                    }
                    break;
                case TrapNames.mobleFloor://발판
                    isActing = true;
                    if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                    StartCoroutine(ShakeFloor());
                    break;
                case TrapNames.brokenHouse://박살 집
                    isActing = true;
                    if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                    boxColl.enabled = false;
                    trapAni.state.SetAnimation(0, "animation",false);
                    SoundEffectManager.Instance.SoundDelay(gameObject.name, 0);
                    break;
            }
        }
    }
    void StartReturn(Vector3 tempTr)
    {
        if (!isLoading)
        {
            beforeTr = tempTr;
            isLoading = true;
            StopAllCoroutines();
            StartCoroutine(ReturntoStartPos());
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!GameManager.Instance.playerRewind)
        {
            if (!isActing && col.gameObject.name.Equals("Player"))
            {
                TrapAction();
            }
            else if (!isActing && col.gameObject.CompareTag("FLOOR") && name.Equals(TrapNames.airplane))
            {
                isActing = true;
            }
            else if (!isActing && col.gameObject.CompareTag("FLOOR") && name.Equals(TrapNames.airplane2))
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!GameManager.Instance.playerRewind)
        {
            if (!isActing && col.gameObject.name.Equals("Player"))
            {
                TrapAction();
            }
        }
    }
    /// <summary>
    /// 트렙 행동 코루틴들
    /// </summary>
    IEnumerator ReturntoStartPos()//오브젝트 초기위치로
    {
        //if (objRigidbody != null && !name.Equals(TrapNames.mobleFloor)) { objRigidbody.isKinematic = true; }//물리 off
        if (objRigidbody != null) { objRigidbody.isKinematic = true; }//물리 off
        spriteRender.enabled = true;
        boxColl.enabled = true;
        while (isLoading)
        {
            if (transform.position == beforeTr)
            {
                if (objEffect != null) { objEffect.SetActive(false); }
                // if (distJoint != null) { distJoint.enabled = true; }
                if (trapAni != null) { trapAni.skeleton.SetToSetupPose(); }
                spriteRender.enabled = true;
                isActing = false;
                isSaved = false;
                isLoading = false;
                StopCoroutine(ReturntoStartPos());
            }
            transform.position = Vector2.Lerp(transform.position, beforeTr, (Time.deltaTime * 1.75f) + 0.15f);
            //transform.rotation = Quaternion.Euler
            yield return null;
        }
    }

    IEnumerator FallDownFloor()//추락하는 발판
    {
        SoundEffectManager.Instance.SoundDelay(gameObject.name, 0);
        while (isActing)
        {
            beforeTr = transform.position;
            //if (!isLoading) { yield return new WaitForSeconds(1.0f); }
            //else { Debug.Log("Stop1"); yield break; }
            if (distJoint != null) { distJoint.enabled = false; }
            objRigidbody.isKinematic = false;
            boxColl.enabled = false;
            if (!isLoading) { yield return new WaitForSeconds(0.75f); }
            else { Debug.Log("Stop2"); yield break; }
            spriteRender.enabled = false;
            if (!isLoading) { yield return new WaitForSeconds(3.25f); }
            else { Debug.Log("Stop3"); yield break; }
            transform.position = beforeTr;
            if (distJoint != null) { distJoint.enabled = true; }
            objRigidbody.velocity = Vector2.zero;
            transform.rotation = Quaternion.Euler(beforeRot);
            objRigidbody.isKinematic = true;
            spriteRender.enabled = true;
            boxColl.enabled = true;
            isActing = false;
            StopCoroutine(FallDownFloor());
        }
    }
    IEnumerator ShakeFloor()//흔들리는 발판
    {
        float temp = 1;
        int shakeTime = 0;
        while (shakeTime <= 40)
        {
            if (transform.rotation.z > tempZRot + 0.03f)
            {
                temp = -1f;
            }
            else if (transform.rotation.z < tempZRot - 0.03f)
            {
                temp = 1f;
            }
            transform.Rotate(Vector3.forward * temp * 35.0f * Time.deltaTime);
            //transform.localRotation = Quaternion.Euler(0, 0, temp * Time.deltaTime * 500.0f);
            yield return null;
            shakeTime++;
        }
        StartCoroutine(FallDownFloor());
        StopCoroutine("ShakeFloor");
    }

    IEnumerator DamageDilay(float time)
    {
        boxColl.enabled = false;
        yield return new WaitForSeconds(time);
        boxColl.enabled = true;
        isActing = false;
        StopCoroutine(DamageDilay(0));
    }

}