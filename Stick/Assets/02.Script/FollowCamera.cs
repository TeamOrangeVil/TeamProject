using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
    
    public Transform target; // 카메라가 바라볼 타겟

    // 카메라의 X,Y 좌표를 넣을 변수
    public float x=20f;
    public float z=2.2f;
    public float trace=200.0f;
    private Transform tr;

    void Awake()
    {
        tr = GetComponent<Transform>();
        target = GameObject.Find("Player").GetComponent<Transform>();
    }

    void LateUpdate()
    {// forward(0,0,1) 위아래   up(0,1,0) 카메라 높이
        tr.position = Vector3.Lerp(tr.position, target.position - (target.forward * x) + (Vector3.up * z), Time.deltaTime * trace);
        //tr.position = new Vector3(Mathf.Clamp(transform.position.x, -13.0f, 13.0f),
                                //Mathf.Clamp(transform.position.y, -5.0f, 17.0f),
                               // Mathf.Clamp(transform.position.z, -40.0f, -10.5f));
        //tr.localRotation = Quaternion.Euler(15, 0, 0);
    }
}