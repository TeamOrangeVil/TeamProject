using UnityEngine;
using System.Collections;

public class ObjBind : MonoBehaviour
{

    public string ID;//조합 식이름
    public string WeaponState;//플레이어의 장비
    public string ObjectCode;//오브젝트 이름
    public string MixResult;//조합 결과 무기
    //public int MaxUsing;//사용횟수

    void Start()
    {
        var temp = XML_Parsing.Instance.BindDBRead(XmlConstancts.OBJBINDXML, this.name);
        Insert(temp);

    }

    void Insert(Bind_Info temp)
    {
        ID = temp.ID;
        WeaponState = temp.WeaponState;
        ObjectCode = temp.ObjectCode;
        MixResult = temp.MixResult;
    }
    public string MixResultReturn()
    {
        return MixResult;
    }
}
