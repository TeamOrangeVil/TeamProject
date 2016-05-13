using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]

public class Rope : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public GameObject targetobj;

    public Vector3 targetPoint = Vector3.zero;
    public Vector3 center = Vector3.zero;
    public Vector3 theSlerp = Vector3.zero;
    public Vector3 theLerp = Vector3.zero;

    void Start()
    {
        //라인렌더러 설정
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetColors(Color.red, Color.yellow);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetVertexCount(20);

    }

    void Update()
    {
        center = transform.position + targetobj.transform.position * 0.5f;

        Vector3 RelCenter = transform.position - center;
        Vector3 aimRelCenter = targetobj.transform.position - center;

        for (float i = 0, interval = -0.053f; interval < 1.0f;)
        {
            theSlerp = Vector3.Slerp(RelCenter, aimRelCenter, interval += 0.053f);
            lineRenderer.SetPosition((int)i++, (theSlerp) + center);
            /*if (i<3 || i >21)
            {
                theLerp = Vector3.Lerp(RelCenter, aimRelCenter, interval += 0.043f);
                lineRenderer.SetPosition((int)i++, (theSlerp) + center);
            }
            else
            {
                theSlerp = Vector3.Slerp(RelCenter, aimRelCenter, interval += 0.043f);
                lineRenderer.SetPosition((int)i++, (theSlerp) + center);
                
            }*/
            /*
            if((i%2).Equals(0))
            {
                theSlerp = Vector3.Slerp(RelCenter, aimRelCenter, interval += 0.043f);
                lineRenderer.SetPosition((int)i++, (theSlerp) + center);
            }
            else
            {
                theLerp = Vector3.Lerp(RelCenter, aimRelCenter, interval += 0.043f);
                lineRenderer.SetPosition((int)i++, (theSlerp) + center);
            }*/


        }

        //lineRenderer.SetPosition(12, center);
        // 타겟이 되는 마우스좌표같은 값

        //타겟 포인트 초기화
        //targetPoint = Vector3.zero;
        //만약 레이캐스트를 쏘면

        // 타겟포인트에 레이의 포지션 저장
        //targetPoint = ray.GetPoint(hitdist);의 역할을 대신한다
        //targetPoint = targetobj.transform.position - transform.position;

        // vector3 센터 값에   현재 위치랑 타겟포인트 값을 더하고 0.5를 곱한다
        //center = (transform.position + targetPoint) * 0.5f;
        // 그리고 센터의 y값을 -=70.0f 해준다.
        //center.y -= 70.0f;

        // quaternion targetRotation = Quaternion.lookrotation(center - transform.position);
        //Quaternion targetRotation = Quaternion.LookRotation(center - transform.position);

        //transform.rotation = quaternion.slerp(transform.rotation, targetrotation, rotationSpeed * time.deltatime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 레이캐스트힛 힛인포
        //만약 피직스.라인캐스트(transform.position, targetpoint, out hitinfo)면
        //targetpoint = hitinfo.point;
        //else 면 targetpoint = transform.position;

        //vector3 relcenter = transform.position - center;
        //Vector3 aimrelcenter = targetpoint - center;

        //for(float index = 0.0f, interval = -0.0417f; interval < 1.0f;)
        //theArc = vector3.slerp(relcenter, aimRelCenter, interval += 0.0417;
        //linerenderer.setposition((int)index++, theArc+center);


        //라인렌더러 처음위치 나중위치
        //lineRenderer.SetPosition(0, transform.position);
        //lineRenderer.SetPosition(1, targetobj.transform.position);
    }
}