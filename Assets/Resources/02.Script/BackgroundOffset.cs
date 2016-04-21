using UnityEngine;
using System.Collections;

public class BackgroundOffset : MonoBehaviour
{
    // 배경 배열
    public Transform[] backgrounds;
    // 플레이어
    public Transform player;
    //npc
    public Transform helper;
    // 카메라
    public Transform cam;
    // 처음 플레이어의 위치
    public Vector2 startPositionPlayer;
    public Vector2 startPositionHelper;
    // 시작 위치를 기준으로 움직인 거리
    private float playerDistanceX;
    private float playerDistanceY;
    // 속도
    private float speed = 1.0f;

    private float comparePosX;
    private float comparePosY;

    private Vector2 comparePos;
    // 선형보간
    private float smooth = 3.0f;

    public Transform tr;

    public FollowCamera CameraScript;

    void Awake()
    {
         player = GameObject.Find("Player").GetComponent<Transform>();
        helper = GameObject.Find("HelperDoll").GetComponent<Transform>();
        //cam = GameObject.Find("Camera").GetComponent<Transform>();
        tr = GetComponent<Transform>();
    }

    void Start()
    {
        CameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();

        startPositionPlayer = player.position;
        startPositionHelper = helper.position;
        //startPosition = cam.position;
    }

    void FixedUpdate()
    {
        tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 200.0f),
                                          Mathf.Clamp(transform.position.y, 7.0f, 15.0f),
                                          Mathf.Clamp(transform.position.z, -15.0f, 20.0f));

        if (CameraScript.isTargetPlayer)
        {
            //캐릭터의 첫 위치를 기준으로 움직인 거리를 구한다.
            playerDistanceX = (startPositionPlayer.x - player.position.x);
            //playerDistanceY = (startPositionPlayer.y - player.position.y);

            //playerDistanceX = (startPosition.x - cam.position.x);
            //playerDistanceY = (startPosition.y - cam.position.y);

            for (int i = 0; i < backgrounds.Length; i++)
            {
                comparePosX = backgrounds[i].position.x + playerDistanceX * ((i + 1) * speed);
                //comparePosY = backgrounds[i].position.y + playerDistanceY * ((i + 1));

                comparePos = new Vector2(comparePosX, comparePosY);
                //comparePos = Vector2.right * comparePosX;

                backgrounds[i].position = Vector2.Lerp(backgrounds[i].position, comparePos.x * Vector2.right, smooth * Time.deltaTime);
            }
            startPositionPlayer = player.position;

            //startPosition = cam.position;
        }
        else
        {
            //캐릭터의 첫 위치를 기준으로 움직인 거리를 구한다.
            playerDistanceX = (startPositionHelper.x - helper.position.x);
           // playerDistanceY = (startPositionHelper.y - player.position.y);

            //playerDistanceX = (startPosition.x - cam.position.x);
            //playerDistanceY = (startPosition.y - cam.position.y);

            for (int i = 0; i < backgrounds.Length; i++)
            {
                comparePosX = backgrounds[i].position.x + playerDistanceX * ((i + 1) * speed);
               // comparePosY = backgrounds[i].position.y + playerDistanceY * ((i + 1));

                comparePos = new Vector2(comparePosX, comparePosY);
                //comparePos = Vector2.right * comparePosX;

                backgrounds[i].position = Vector2.Lerp(backgrounds[i].position, comparePos.x * Vector2.right, smooth * Time.deltaTime);
            }
            startPositionHelper = helper.position;

            //startPosition = cam.position;
        }

    }
}
