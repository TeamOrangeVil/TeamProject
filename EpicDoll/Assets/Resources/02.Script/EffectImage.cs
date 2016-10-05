using UnityEngine;
using System.Collections;

public class EffectImage : MonoBehaviour {
    public UISprite HitImage;
    public UISprite SpriteImage;
    void Start()
    {
        HitImage = GameObject.Find("HitImage").GetComponent<UISprite>();
        SpriteImage = GameObject.Find("SpriteImage").GetComponent<UISprite>();
    }
	void Update()
    {
        var height = Camera.main.orthographicSize* Screen.width / Screen.height;  //2 * Camera.main.orthographicSize;
        var width = height * Screen.height / Screen.width;//Camera.main.aspect;
        //Debug.Log("orthographicSize : " + Camera.main.orthographicSize);
        //Debug.Log("aspect : " + Camera.main.aspect);
        //Debug.Log("헤이트 : "+height);
        //Debug.Log("와이드 : " + width);

        // HitImage.SetScreenRect(200, 100, Screen.width, Screen.height);
        //HitImage.SetScreenRect(0, 0, (int)HitImage.localSize.x, (int)HitImage.localSize.y);
        //HitImage.SetScreenRect((int)width, (int)height, (int)HitImage.localSize.x, (int)HitImage.localSize.y);
        //SpriteImage.SetScreenRect(0,0, (int)SpriteImage.localSize.x, (int)SpriteImage.localSize.y);
        HitImage.SetScreenRect(Screen.width/2, 0, (int)HitImage.localSize.x, (int)HitImage.localSize.y);
    }
}
