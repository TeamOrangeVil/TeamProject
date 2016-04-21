using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{

    public Transform targetPlayer; // 카메라가 바라볼 플레이어 타겟
    public Transform targetHelper; // 카메라가 바라볼 헬퍼 타겟
    public Transform background;

    public bool isTargetPlayer = true;

    // 카메라의 X,Y 좌표를 넣을 변수
    public float x = 30f;
    public float z = 2.2f;
    private float trace = 200.0f;
    private Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
        targetPlayer = GameObject.Find("Player").GetComponent<Transform>();
        targetHelper = GameObject.Find("HelperDoll").GetComponent<Transform>();
    }

    void LateUpdate()
    {
        //background
        if(isTargetPlayer)
        {
            tr.position = Vector3.Lerp(tr.position, targetPlayer.position + (targetPlayer.up * z) - (targetPlayer.forward * x), Time.deltaTime * trace);
            tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 200.0f),
                                          Mathf.Clamp(transform.position.y, 4.7f, 6.0f),
                                          Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
        }
        else
        {
            tr.position = Vector3.Lerp(tr.position, targetHelper.position + (targetHelper.up * z) - (targetHelper.forward * x), Time.deltaTime * trace);
            tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 200.0f),
                                          Mathf.Clamp(transform.position.y, 4.7f, 6.0f),
                                          Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
        }
        
    }
}