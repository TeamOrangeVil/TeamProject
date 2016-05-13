using UnityEngine;
using System.Collections;
using Spine.Unity;

struct TrapNames//트렙의 이름들을 미리 선언해서 스위치에 활용
{
    public const string tack = "Tack";
    public const string sawDust = "Trap_SawDust";
    public const string airplane = "AirPlane_Trap";
    public const string mobleFloor = "MobleFloor";
};
public class TrapObject : MonoBehaviour
{

    public Vector3 beforeTr; //오브젝트의 초기 위치
    public Collider2D boxColl; //뺰쓰 트리거
    public GameObject objEffect; //옵젝 이펙트
    public SpriteRenderer spriteRender;//스프라이트 이미지
    Rigidbody2D objRigidbody; //물리작용
    float head; //플레이어 이동방향

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
    }
    public void TrapAction()
    {
        if (!isActing)
        {
            head = CharacterController2D.Instance.transform.position.x < transform.position.x == true ? 1f : -1f;
            switch (gameObject.name)
            {
                case TrapNames.tack:
                    isActing = true;
                    if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                    StartCoroutine(DamageDilay(0.6f));
                    CharacterController2D.Instance.rb.AddForce(Vector2.left * 2.5f * head + Vector2.up * 5f, ForceMode2D.Impulse);
                    GameManager.Instance.PlayerDamaged(25);
                    break;
                case TrapNames.sawDust:
                    isActing = true;
                    if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                    StartCoroutine(DamageDilay(0.6f));
                    CharacterController2D.Instance.rb.AddForce(Vector2.left * 2.5f * head + Vector2.up * 5f, ForceMode2D.Impulse);
                    GameManager.Instance.PlayerDamaged(25);
                    break;
                case TrapNames.airplane:
                    if (objRigidbody.isKinematic)
                    {
                        Debug.Log("추락");
                        StartCoroutine(DamageDilay(0.5f));
                        if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                        objRigidbody.isKinematic = false;
                    }
                    else
                    {
                        Debug.Log("충돌");
                        CharacterController2D.Instance.rb.AddForce(Vector2.left * 8f * head, ForceMode2D.Impulse);
                        GameManager.Instance.PlayerDamaged(25);
                        isActing = true;
                    }
                    break;
                case TrapNames.mobleFloor:
                    isActing = true;
                    if (!isSaved && GameManager.Instance.nowSave) { GameManager.Instance.PushIntoStack(this); isSaved = true; }// 중복세이브 방지
                    StartCoroutine(FallDownFloor());
                    break;
            }
        }
    }
    void StartReturn(Vector3 tempTr)
    {
        if (!isLoading)
        {
            Debug.Log("Rewind Start");
            beforeTr = tempTr;
            isLoading = true;
            StopAllCoroutines();
            //StopCoroutine(FallDownFloor()); 왜 작동 안하는지 체크할 것 !
            StartCoroutine(ReturntoStartPos());
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isActing && col.gameObject.name.Equals("Player"))
        {
            TrapAction();
        }
        else if (!isActing && col.gameObject.CompareTag("FLOOR") && name.Equals(TrapNames.airplane))
        {
            isActing = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isActing && col.gameObject.name.Equals("Player"))
        {
            TrapAction();
        }
    }
    /// <summary>
    /// 트렙 행동 코루틴들
    /// </summary>
    IEnumerator ReturntoStartPos()
    {
        if (objRigidbody != null) { objRigidbody.isKinematic = true; }//물리 off
        spriteRender.enabled = true;
        boxColl.enabled = true;
        while (isLoading)
        {
            if (transform.position == beforeTr)
            {
                if (objEffect != null) { objEffect.SetActive(false); }
                spriteRender.enabled = true;
                isActing = false;
                isSaved = false;
                isLoading = false;
                StopCoroutine(ReturntoStartPos());
            }
            transform.position = Vector2.Lerp(transform.position, beforeTr, (Time.deltaTime * 1.75f) + 0.15f);
            yield return null;
        }
    }

    IEnumerator FallDownFloor()
    {
        while (isActing)
        {
            beforeTr = transform.position;
            if (!isLoading) { yield return new WaitForSeconds(1.0f); }
            else { Debug.Log("Stop1"); yield break; }
            objRigidbody.isKinematic = false;
            boxColl.enabled = false;
            if (!isLoading) { yield return new WaitForSeconds(0.75f); }
            else { Debug.Log("Stop2"); yield break; }
            spriteRender.enabled = false;
            if (!isLoading) { yield return new WaitForSeconds(3.25f); }
            else { Debug.Log("Stop3"); yield break; }
            transform.position = beforeTr;
            objRigidbody.isKinematic = true;
            spriteRender.enabled = true;
            boxColl.enabled = true;
            isActing = false;
            StopCoroutine(FallDownFloor());
        }
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