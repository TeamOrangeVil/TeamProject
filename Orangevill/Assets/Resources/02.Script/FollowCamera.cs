using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public enum State { PLAYER, HELPER, SPIDER }
    public State CameraState;

    public Transform targetPlayer; // 카메라가 바라볼 플레이어 타겟
    public Transform targetHelper; // 카메라가 바라볼 헬퍼 타겟
    public Transform targetSpider;
    public Transform background;
    
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
        targetSpider = GameObject.Find("Spider").GetComponent<Transform>();
    }
    
    void LateUpdate()
    {
        switch(CameraState)
        {
            case State.PLAYER:
                tr.position = Vector3.Lerp(tr.position, targetPlayer.position + (targetPlayer.up * z) - (targetPlayer.forward * x), Time.deltaTime * trace);
                tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 400.0f),
                                              Mathf.Clamp(transform.position.y, 4.7f, 18.0f),
                                              Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
                break;
            case State.HELPER:
                tr.position = Vector3.Lerp(tr.position, targetHelper.position + (targetHelper.up * z) - (targetHelper.forward * x), Time.deltaTime * trace);
                tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 400.0f),
                                              Mathf.Clamp(transform.position.y, 4.7f, 6.0f),
                                              Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
                break;
            case State.SPIDER:
                tr.position = Vector3.Lerp(tr.position, targetSpider.position + (targetSpider.up * z) - (targetSpider.forward * x), Time.deltaTime * trace);
                tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 400.0f),
                                              Mathf.Clamp(transform.position.y, 4.7f, 6.0f),
                                              Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
                break;
        }
        /*if(CameraState.Equals(State.PLAYER))
        {
            tr.position = Vector3.Lerp(tr.position, targetPlayer.position + (targetPlayer.up * z) - (targetPlayer.forward * x), Time.deltaTime * trace);
            tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 400.0f),
                                          Mathf.Clamp(transform.position.y, 4.7f, 18.0f),
                                          Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
        }
        if (CameraState.Equals(State.HELPER))
        {
            tr.position = Vector3.Lerp(tr.position, targetHelper.position + (targetHelper.up * z) - (targetHelper.forward * x), Time.deltaTime * trace);
            tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 400.0f),
                                          Mathf.Clamp(transform.position.y, 4.7f, 6.0f),
                                          Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
        }
        if (CameraState.Equals(State.SPIDER))
        {
            tr.position = Vector3.Lerp(tr.position, targetSpider.position + (targetSpider.up * z) - (targetSpider.forward * x), Time.deltaTime * trace);
            tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 400.0f),
                                          Mathf.Clamp(transform.position.y, 4.7f, 6.0f),
                                          Mathf.Clamp(transform.position.z, -15.0f, 7.0f));
        }*/
    }

    public IEnumerator CameraShake()
    {
        for (float i=0; i<1; i += 0.05f)
        {
            transform.position = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), transform.position.z);
            yield return 0;
        }
       
    }
}