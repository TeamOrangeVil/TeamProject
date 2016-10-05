using UnityEngine;
using System.Collections;

public class UIPosition : MonoBehaviour {

    public UISprite BlackSprite;
    public UILabel PlayerText;

    public GameObject targetPlayer; // 타겟이 되는 플레이어 캐릭터

    public Camera cameraUI; // 바라볼 카메라
    
    public Vector2 screenPos; // 월드 좌표를 스크린 좌표로 받을 벡터

    void Awake()
    {
        screenPos = Camera.main.WorldToScreenPoint(targetPlayer.transform.position);
    }

    void Update()
    {
        PlayerText.transform.position = cameraUI.ScreenToWorldPoint(new Vector2(screenPos.x, screenPos.y + 85.0f));
        BlackSprite.transform.position = cameraUI.ScreenToWorldPoint(new Vector2(screenPos.x, screenPos.y + 85.0f));
    }
}
