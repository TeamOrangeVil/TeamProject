using UnityEngine;
using System.Collections;

public class DirectionUI : MonoBehaviour {
    public GameObject TextBox;
    public GameObject PlayerText;
    // public Camera guiCam = NGUITools.FindCameraForLayer()
    public GameObject target;
    
    void Update()
    {
        
        Camera worldCam = NGUITools.FindCameraForLayer(target.layer);
        Camera guiCam = NGUITools.FindCameraForLayer(gameObject.layer);

        //var height = Camera.main.orthographicSize;//UICamera.mainCamera.orthographicSize;
        //var width = height * Camera.main.aspect;//UICamera.mainCamera.aspect;

        Vector3 pos = guiCam.ViewportToWorldPoint(worldCam.WorldToViewportPoint(target.transform.position));
        //Vector3 pos = guiCam.ViewportToWorldPoint(worldCam.WorldToViewportPoint(new Vector3((height * Camera.main.aspect) * 0.5f, Camera.main.orthographicSize, 0)));
        pos.z = 1;

        transform.position = pos;
    }

}
/*
        // 캐릭터 따라가기
        Camera worldCam = NGUITools.FindCameraForLayer(target.layer);
        Camera guiCam = NGUITools.FindCameraForLayer(gameObject.layer);
        Vector3 pos = guiCam.ViewportToWorldPoint(worldCam.WorldToViewportPoint(target.transform.position));
        pos.z = 1;
        transform.position = pos;

        화면 위치 유지
        var height = Camera.main.orthographicSize;//UICamera.mainCamera.orthographicSize;
        var width = height * Camera.main.aspect;//UICamera.mainCamera.aspect;
        Vector3 pos = guiCam.ViewportToWorldPoint(worldCam.WorldToViewportPoint(new Vector3((height * Camera.main.aspect) * 0.5f, Camera.main.orthographicSize, 0)));
         pos.z = 1;
        transform.position = pos;
*/
