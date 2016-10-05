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
    public Transform spider;
    
    // 카메라
    public Transform cam;
    
    // 처음 플레이어의 위치
    public Vector3 startPositionPlayer;
    public Vector3 startPositionHelper;
    public Vector3 startPositionSpider;
    public Vector3 startPositionCam;

    // 시작 위치를 기준으로 움직인 거리
    private float playerDistanceX;
    private float playerDistanceY;
    private float helperDistanceX;
    private float helperDistanceY;
    private float spiderDistanceX;
    private float spiderDistanceY;
    private float camDistanceX;
    private float camDistanceY;
    
    // 속도
    private float speed = 1.0f;

    // 뒷 배경의 움직일 좌표
    private float comparePosX;
    private float comparePosY;
    private Vector3 comparePos;

    // 선형보간
    private float smooth = 3.0f;

    //public Transform tr;

    // 메인카메라
   // public FollowCamera CameraScript;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        helper = GameObject.Find("HelperDoll").GetComponent<Transform>();
        spider = GameObject.Find("Spider").GetComponent<Transform>();
        //cam = GameObject.Find("Camera").GetComponent<Transform>();
        //tr = GetComponent<Transform>();
    }

    void Start()
    {
       // CameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowCamera>();

        //startPositionPlayer = player.position;
        //startPositionHelper = helper.position;
        startPositionCam = cam.position;
    }

    void FixedUpdate()
    {
        camDistanceX = (startPositionCam.x - cam.position.x);
        camDistanceY = (startPositionCam.y - cam.position.y);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            comparePosX = backgrounds[i].position.x + camDistanceX * ((i + 1) * speed * 0.3f);
            comparePosY = backgrounds[i].position.y + camDistanceY * ((i + 1) * speed);

            comparePos = new Vector3(comparePosX, comparePosY, backgrounds[i].transform.position.z);
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, comparePos, smooth * Time.deltaTime);
        }
        startPositionCam = cam.position;
    }
    //tr.position = new Vector3(Mathf.Clamp(transform.position.x, 3.37f, 600.0f),
    //                                  Mathf.Clamp(transform.position.y, -10.0f, 20.0f),
    //                                  Mathf.Clamp(transform.position.z, -20.0f, 40.0f));
    /*
    if (CameraScript.CameraState.Equals(FollowCamera.State.PLAYER))
    {
        //캐릭터의 첫 위치를 기준으로 움직인 거리를 구한다.
        playerDistanceX = (startPositionPlayer.x - player.position.x);
        playerDistanceY = (startPositionPlayer.y - player.position.y);

        //playerDistanceX = (startPosition.x - cam.position.x);
        //playerDistanceY = (startPosition.y - cam.position.y);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if(i.Equals(0))
            {
                comparePosX = backgrounds[i].position.x + playerDistanceX * ((i + 1) * speed * 0.5f);
                comparePosY = backgrounds[i].position.y + playerDistanceY * ((i + 1) * speed * 0);

                comparePos = new Vector3(comparePosX, comparePosY, backgrounds[i].transform.position.z);
            }
            else
            {
                comparePosX = backgrounds[i].position.x + playerDistanceX * ((i + 1) * speed * 0.5f);
                comparePosY = backgrounds[i].position.y + playerDistanceY * ((i + 1) * speed * 0.5f);

                comparePos = new Vector3(comparePosX, comparePosY, backgrounds[i].transform.position.z);
            }
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, comparePos, smooth * Time.deltaTime);
        }
        startPositionPlayer = player.position;

        //startPosition = cam.position;
    }
    if (CameraScript.CameraState.Equals(FollowCamera.State.HELPER))
    {
        helperDistanceX = (startPositionHelper.x - helper.position.x);
        helperDistanceY = (startPositionHelper.y - helper.position.y);


        for (int i = 0; i < backgrounds.Length; i++)
        {
            comparePosX = backgrounds[i].position.x + helperDistanceX * ((i + 1) * speed);
            comparePosY = backgrounds[i].position.y + helperDistanceY * ((i + 1) * speed*0.5f);

            comparePos = new Vector3(comparePosX, comparePosY, backgrounds[i].transform.position.z);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, comparePos, smooth * Time.deltaTime);
        }
        startPositionHelper = helper.position;
    }
    if (CameraScript.CameraState.Equals(FollowCamera.State.SPIDER))
    {
        spiderDistanceX = (startPositionSpider.x - spider.position.x);
        spiderDistanceY = (startPositionSpider.y - spider.position.y);


        for (int i = 0; i < backgrounds.Length; i++)
        {
            comparePosX = backgrounds[i].position.x + spiderDistanceX * ((i + 1) * speed);
            comparePosY = backgrounds[i].position.y + spiderDistanceY * ((i + 1) * speed * 0.5f);

            comparePos = new Vector3(comparePosX, comparePosY, backgrounds[i].transform.position.z);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, comparePos, smooth * Time.deltaTime);
        }
        startPositionSpider = spider.position;
    }
}*/

}
