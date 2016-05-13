using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*struct ObjectSetup
{
    public GameObject gameObject;
    public Vector3 beforeTr;//오브젝트 초기위치
}*/
public class CheckPoint : MonoBehaviour {

    Stack<ObjectSetup> checkPointGameObjectStack;
    ObjectSetup tempObjectSetup;
    Vector3 playerBeforeTr;
    public GameObject Player;
    bool nowLoad = false;
    bool nowSave = true;
    bool playerRewind = false;
    // Use this for initialization
    void Start () {
        checkPointGameObjectStack = new Stack<ObjectSetup>();
        checkPointGameObjectStack.Clear();
        playerBeforeTr.Set(-14.41f, -2.56f, 0);//플레이어가 키 입력시 위치 저장 할 수 있도록 수정 예정
	}
    
    public void CheckStart(Vector3 pos)
    {
        if (!nowSave)
        {
            nowSave = true;
            playerBeforeTr = pos;
            checkPointGameObjectStack.Clear();
        }
    }

    void PushIntoStack(FieldObject temp)//옵젝을 건들때마다 실행되는 함수
    {
        if (nowSave && !playerRewind)
        {
            tempObjectSetup.gameObject = temp.gameObject;
            tempObjectSetup.beforeTr = temp.beforeTr;
            checkPointGameObjectStack.Push(tempObjectSetup);
            Debug.Log("옵젝 초기위치 저-장");
            Debug.Log(checkPointGameObjectStack.Count);
        }
    }
    void PopFromStack()//옵젝을 원래 위치로 돌리기 위한 함수
    {
        for(int i = checkPointGameObjectStack.Count; i > 0; i--)
        {
            tempObjectSetup = checkPointGameObjectStack.Pop();
            tempObjectSetup.gameObject.SendMessage("StartReturn", tempObjectSetup.beforeTr,SendMessageOptions.DontRequireReceiver);
            Debug.Log("호오오오오오오우 !");
        }
        nowLoad = false;
        tempObjectSetup.gameObject = null;
        tempObjectSetup.beforeTr = Vector3.zero;
        checkPointGameObjectStack.Clear();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.L) && !nowLoad)//키 입력시 되감기 모드 ㄱㄱ
        {
            nowSave = false;
            nowLoad = true;
            playerRewind = true;
            PopFromStack();
        }
        if (playerRewind)//플레이어 되감기
        {
            Player.transform.position = Vector2.Lerp(Player.transform.position, playerBeforeTr,Time.deltaTime * 0.75f);
            Debug.Log("핫오브워리어어어어");
            if (Player.transform.position.x == playerBeforeTr.x){ playerRewind = false; }
        }
    }
}
