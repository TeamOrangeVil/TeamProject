using UnityEngine;
using System.Collections;

public class BackgroundOffset : MonoBehaviour {
    // 배경 배열
    public Transform[] backgrounds;
    // 플레이어
    public Transform player;
    // 처음 플레이어의 위치
    public Vector3 startPosition;
    // 시작 위치를 기준으로 움직인 거리
    private float playerDistanceX;
    private float playerDistanceY;
    // 속도
    private float speed = 1.0f;

    private float comparePosX;
    private float comparePosY;

    private Vector3 comparePos;
    // 선형보간
    private float smooth = 3.0f;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Start()
    {
        startPosition = player.position;
    }

    void FixedUpdate()
    {
        //캐릭터의 첫 위치를 기준으로 움직인 거리를 구한다.
        playerDistanceX = (startPosition.x - player.position.x);
        playerDistanceY = (startPosition.y - player.position.y);

        for (int i=0; i< backgrounds.Length; i++)
        {
            comparePosX = backgrounds[i].position.x + playerDistanceX * ((i+1) * speed);
            comparePosY = backgrounds[i].position.y + playerDistanceY * ((i+1) * speed);

            comparePos = new Vector3(comparePosX, comparePosY, backgrounds[i].position.z);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, comparePos, smooth * Time.deltaTime);
        }
        startPosition = player.position;
    }
}
