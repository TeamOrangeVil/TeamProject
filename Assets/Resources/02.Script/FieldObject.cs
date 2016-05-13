using UnityEngine;
using System.Collections;
using Spine.Unity;

public class FieldObject : MonoBehaviour {
    //타입 : 저장 아이템증가 Save, 움직이는 배경 애니 Bg, interactive Object - 상호작용 옵젝 ITROBJ, 함정 TRAP 
    public enum ObjectSetting { SAVE, BG, ITROBJ, TRAP };
    public ObjectSetting objectSetting;

    public Vector3 beforeTr; //오브젝트의 초기 위치
    Rigidbody2D objRigidbody; //물리작용
    public Collider2D boxTrigger; //뺰쓰 트리거
    public Collider2D boxColl; //뺰쓰 트리거
    public ObjectTriggerSetter triggerSetter;
    public SpriteRenderer spriteRender;
    SkeletonAnimation objAnimation;//오브젝트가 애니메이션이 있으면 활용하기 위해 선언
    public GameObject objEffect; //옵젝 이풰에에엨트
    CharacterController2D Player;

    public bool goingBefore; //초기 위치로 회귀하는 모드 On : Off를 위해
    public bool isSaved; // 스텍에 중복으로 정보가 들어가는걸 방지
    public bool isGotAni; // 상호작용 or 함정인대 애니메이션을 포함하는 경우


    // Use this for initialization
    void Start ()
    {
        beforeTr = GetComponent<Transform>().transform.position;
        Player  = GameObject.Find("Player").GetComponent<CharacterController2D>();
        goingBefore = false;
        isSaved = false;
        //오브젝트 타입에 따라 불러와야 되는 컴포넌트들을 switch문을 활용해서 불러온다.
        switch (objectSetting)
        {
            case ObjectSetting.BG:
                break;
            case ObjectSetting.SAVE:
                break;
            case ObjectSetting.TRAP:
                if (this.gameObject.name.Equals("Trap_SawDust"))
                {
                    boxTrigger = GameObject.Find("Trigger").GetComponent<Collider2D>();
                    triggerSetter = GetComponentInChildren<ObjectTriggerSetter>();
                }
                else if (this.gameObject.name.Equals("Tack"))
                {
                    boxTrigger = GameObject.Find("Trigger").GetComponent<Collider2D>();
                    triggerSetter = GetComponentInChildren<ObjectTriggerSetter>();
                    //PlayerController.Instance.채력감소 ㄱㄱㄱ
                }
                else if (this.gameObject.name.Equals("AirPlane_Trap"))
                {
                    boxTrigger = GameObject.Find("AirPlaneTrigger").GetComponent<Collider2D>();
                    triggerSetter = GetComponentInChildren<ObjectTriggerSetter>();
                    objRigidbody = GetComponent<Rigidbody2D>();//물리 불러오기
                    objRigidbody.isKinematic = true;
                }
                else if(this.gameObject.name.Equals("BrokenHouse"))
                {
                    spriteRender = GetComponent<SpriteRenderer>();
                    triggerSetter = GetComponentInChildren<ObjectTriggerSetter>();
                    boxTrigger = GameObject.Find("Trigger").GetComponent<Collider2D>();
                    boxColl = GetComponent<BoxCollider2D>();
                    boxTrigger.enabled = true;
                }
                else if (this.gameObject.name.Equals("MobleFloor"))
                {
                    objRigidbody = GetComponent<Rigidbody2D>();//물리 불러오기
                    objRigidbody.isKinematic = true;
                    spriteRender = GetComponent<SpriteRenderer>();
                    triggerSetter = GetComponentInChildren<ObjectTriggerSetter>();
                    boxTrigger = GameObject.Find("Trigger").GetComponent<Collider2D>();
                    boxColl = GetComponent<BoxCollider2D>();
                }
                break;
            case ObjectSetting.ITROBJ:
                if (this.gameObject.name.Equals("Gramophone"))
                {
                    boxTrigger = GameObject.Find("Trigger").GetComponent<Collider2D>();
                    triggerSetter = GetComponentInChildren<ObjectTriggerSetter>();
                }
                else if (this.gameObject.name.Equals("Key"))
                {
                    boxTrigger = GameObject.Find("Trigger").GetComponent<Collider2D>();
                    triggerSetter = GetComponentInChildren<ObjectTriggerSetter>();
                }
                break;
            default:
                break;
        }
    }

    public void AniSetOn(string aniName)//배경 오브젝트의 애니메이션 작동
    {

    }

    public void ObjectAction(Vector3 temp)//각 오브젝트가 자신이 해야할 행동을 정의받아 동작을 취한다 Kia~~ 취한다 주모~~
    {
        switch (objectSetting)
        {
            case ObjectSetting.BG:
                AniSetOn("GO");
                break;
            case ObjectSetting.SAVE:
                //PlayerController.Instance.샬라샬라블라블라 저장 최대 횟수 ++
                this.enabled = false;
                break;
            case ObjectSetting.TRAP:
                if (this.gameObject.name.Equals("Trap_SawDust"))
                {
                    Debug.Log("아퍼 ㅠㅠ");
                    float head = Player.Player.skeleton.flipX == true ? 1f : -1f;
                    Player.rb.AddForce(Vector2.left * 5f * head + Vector2.up * 2f, ForceMode2D.Impulse);
                    //triggerSetter.SetBool(false);
                    //PlayerController.Instance.채력감소 ㄱㄱㄱ
                }
                else if (this.gameObject.name.Equals("Tack"))
                {
                    Debug.Log("아퍼 ㅠㅠ");
                    float head = Player.Player.skeleton.flipX == true ? 1f : -1f;
                    Player.rb.AddForce(Vector2.left * 5f * head + Vector2.up * 2f, ForceMode2D.Impulse);
                    StartCoroutine(Tack());
                    //triggerSetter.SetBool(false);
                    //PlayerController.Instance.채력감소 ㄱㄱㄱ
                }
                else if (this.gameObject.name.Equals("BrokenHouse"))
                {
                    Debug.Log("하우스 박살");
                    GameManager.Instance.PushIntoStack(this);
                    objEffect.SetActive(true);
                    boxTrigger.enabled = false;
                    spriteRender.enabled = false;
                    triggerSetter.SetBool(true);
                }
                else if (this.gameObject.name.Equals("AirPlane_Trap"))
                {
                    Debug.Log("비행기 추락");
                    GameManager.Instance.PushIntoStack(this);
                    objRigidbody.isKinematic = false;
                    boxTrigger.enabled = false;
                    isSaved = true;
                    triggerSetter.SetBool(true);
                }
                else if (this.gameObject.name.Equals("MobleFloor"))
                {
                    Debug.Log("발판 추락");
                    GameManager.Instance.PushIntoStack(this);
                    StartCoroutine(FallDownFloor());
                    isSaved = true;
                    //triggerSetter.SetBool(true);
                }
                break;
            case ObjectSetting.ITROBJ:
                if (this.gameObject.name.Equals("Gramophone"))
                {
                   //이벤트. sendmeesage;
                }
                else if (this.gameObject.name.Equals("Key"))
                {
                    //이벤트 트리거 값 증가. sendmeesage;
                    gameObject.SetActive(false);
                    //triggerSetter.SetBool(true);
                }
                break;
        }
        //boxTrigger.SetActive(false);
    }
    //되감기 할때 원래위치로 돌아가는 코루틴
    IEnumerator ReturntoStartPos()
    {
        if (objRigidbody != null)
        {
            objRigidbody.isKinematic = true;
        }
        while (goingBefore)
        {
            if (transform.position == beforeTr)
            {
                goingBefore = false;
                isSaved = false;
                boxTrigger.enabled = true;
                triggerSetter.SetBool(false);
                spriteRender.enabled = true;
                objEffect.SetActive(false);
                StopCoroutine(ReturntoStartPos());
            }
            this.transform.position = Vector2.Lerp(this.transform.position, beforeTr, (Time.deltaTime * 1.75f)+0.25f);
            yield return null;
        }
    }
    //오브젝트가 일정 시간 후 충돌판정 재작동
    IEnumerator Tack()
    {  
        yield return new WaitForSeconds(0.5f);
        triggerSetter.SetBool(false);
        StopCoroutine(Tack());
    }
    //오브젝트가 낙하 후 재생성
    IEnumerator FallDownFloor()
    {
        Debug.Log("떨어진다");
        beforeTr = transform.position;
        yield return new WaitForSeconds(2.0f);
        objRigidbody.isKinematic = false;//물리 적용해서 떨어뜨리기 변경요망;
        boxColl.enabled = false;
        yield return new WaitForSeconds(0.75f);
        spriteRender.enabled = false;
        //spriteRender.enabled = false;
        yield return new WaitForSeconds(6.25f);
        transform.position = beforeTr;
        objRigidbody.isKinematic = true;//물리 적용해서 떨어뜨리기 변경요망;
        boxColl.enabled = true;
        boxTrigger.enabled = true;
        spriteRender.enabled = true;
        isSaved = false;
        triggerSetter.SetBool(false);
        StopCoroutine(FallDownFloor());
    }
    //오브젝트가 박살
    IEnumerator BrokenEffect()
    {
        objEffect.SetActive(true);
        //yield return new WaitForSeconds(0.25f);
        //this.gameObject.SetActive(false);
        boxTrigger.enabled = false;
        yield return new WaitForSeconds(0.25f);
        StopCoroutine(BrokenEffect());
    }
    //외부에서 SendMessage로 상태를 조정한 뒤 코루틴을 실행시키기 위한 함수
    void StartReturn(Vector3 tempTr)
    {
        if (!goingBefore)
        {
            Debug.Log("Rewind Start");
            beforeTr = tempTr;
            goingBefore = true;
            StartCoroutine(ReturntoStartPos());
        }
    }
}
