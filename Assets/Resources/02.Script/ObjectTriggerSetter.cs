using UnityEngine;
using System.Collections;
//각종 작동에 사용되는 트리거 박스를 가진 게임 오브젝트의 스크립트입니다 ^오^

public class ObjectTriggerSetter : MonoBehaviour{

    public FieldObject Parent;//부모 오브젝트
    public bool isSaved;
    // Use this for initialization
    void Start()
    {
        Parent = GetComponentInParent<FieldObject>();
        isSaved = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isSaved && !GameManager.Instance.playerRewind)
        {
            Parent.ObjectAction(Parent.transform.position);
            isSaved = true;
        }
    }
    public void SetBool(bool isTrue)
    {
        isSaved = isTrue;
    }
}
