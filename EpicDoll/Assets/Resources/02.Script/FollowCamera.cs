using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public enum State { PLAYER, HELPER, SPIDER, SPIDERFOOT,SPIDERTRACE, FREEZE }    // 카메라 타겟
    public State CameraState;                                               // 열거형 선언

    public Transform targetPlayer;      // 카메라가 바라볼 플레이어 타겟
    public Transform targetHelper;      // 카메라가 바라볼 헬퍼 타겟
    public Transform targetSpider;      // 카메라가 바라볼 거미 타겟
    public Transform targetSpiderFoot;  // 카메라가 바라볼 연출거미 타겟

    // 카메라의 X,Y 좌표를 넣을 변수
    public float x = 30f;
    public float y = 2.2f;
    private float trace = 200.0f;
    private Transform tr;

    // 카메라 흔들림
    public float shakeRadius = 0.7f;

    void Start()
    {
        tr = GetComponent<Transform>();
        targetPlayer = GameObject.Find("Player").GetComponent<Transform>();
        targetHelper = GameObject.Find("HelperDoll").GetComponent<Transform>();
        targetSpider = GameObject.Find("Spider").GetComponent<Transform>();
        targetSpiderFoot = GameObject.Find("right leg 2").GetComponent<Transform>();
    }

    void LateUpdate()
    {

        switch (CameraState)
        {
            case State.PLAYER: // 플레이어
                tr.position = Vector3.Lerp(tr.position, targetPlayer.position + (targetPlayer.up * y) - (targetPlayer.forward * x), Time.deltaTime * trace);
                tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 800.0f),
                                              Mathf.Clamp(transform.position.y, 4.2f, 800.0f),
                                              Mathf.Clamp(transform.position.z, -15.0f, -3.0f));
                break;
            case State.HELPER: // 안내 인형
                tr.position = Vector3.Lerp(tr.position, targetHelper.position + (targetHelper.up * y) - (targetHelper.forward * x), Time.deltaTime * trace);
                tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 800.0f),
                                             Mathf.Clamp(transform.position.y, 4.2f, 800.0f),
                                              Mathf.Clamp(transform.position.z, -15.0f, -3.0f));
                break;
            case State.SPIDER: // 튜토리얼 단계에서 플레이어를 공격하는 거미
                tr.position = Vector3.Lerp(tr.position, targetSpider.position + (targetSpider.up * y) - (targetSpider.forward * x) + Random.insideUnitSphere * shakeRadius * 0.2f, Time.deltaTime * trace);
                //tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 800.0f),
                //                              Mathf.Clamp(transform.position.y, 4.2f, 18.0f),
                //                              Mathf.Clamp(transform.position.z, -15.0f, -3.0f));
                break;
            case State.SPIDERFOOT:
                tr.position = Vector3.Lerp(tr.position, targetSpiderFoot.position + (targetSpiderFoot.up * y) - (targetSpiderFoot.forward * x) + (targetSpiderFoot.up), Time.deltaTime * trace);
                break;
            case State.SPIDERTRACE: // 추격전 거미
                tr.position = Vector3.Lerp(tr.position, targetPlayer.position + (targetPlayer.up * y) - (targetPlayer.forward * x) + Random.insideUnitSphere * shakeRadius * 0.2f, Time.deltaTime * trace);
                tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 800.0f),
                                              Mathf.Clamp(transform.position.y, 4.2f, 800.0f),
                                              Mathf.Clamp(transform.position.z, -15.0f, -3.0f));
                break;
            case State.FREEZE: // 카메라 고정
                break;
        }
    }
}