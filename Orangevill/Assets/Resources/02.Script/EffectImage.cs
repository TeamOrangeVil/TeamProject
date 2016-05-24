using UnityEngine;
using System.Collections;

public class EffectImage : MonoBehaviour {
    public UISprite HitImage;
    void Start()
    {
        HitImage = GameObject.Find("HitImage").GetComponent<UISprite>();
    }
	void Update()
    {
        var height = 2 * Camera.main.orthographicSize;
        var width = height * Camera.main.aspect;
        HitImage.SetScreenRect(0, 0, Screen.width, Screen.height);
    }
}
