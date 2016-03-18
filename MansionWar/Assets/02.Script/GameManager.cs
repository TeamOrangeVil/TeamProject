using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // 던전 4방향 문 충돌체
    public Collider colliderUp;
    public Collider colliderDown;
    public Collider colliderRight;
    public Collider colliderLeft;
    
    public Transform Floor_M; // 게임 맵 바닥
    
    public GameObject fadeBoard; // 화면을 가려줄 오브젝트
    
    // 몬스터의 수를 확인하고 문을 열기위한 체크변수
    public bool doorHitUp = false;
    public bool doorHitDown = false;
    public bool doorHitRight = false;
    public bool doorHitLeft = false;

    public bool isDie = false;//몬스터 죽음 여부

    // 싱글턴 관련
    private static GameManager gInstance = null;

    public static GameManager Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }

    void Awake()
    {
        gInstance = this;
    }

    void Start()
    {
        // 코루틴 실행
        StartCoroutine(MonsterCount());
    }

    // 몬스터 존재 확인
    IEnumerator MonsterCount()
    {
        while (!isDie)
        {
            //현재 생성된 몬스터 개수 산출
            var monsterCount = GameObject.FindGameObjectsWithTag("Monster");

            // 몬스터가 없을 경우
            if (monsterCount.Length == 0)
            {
                //만약 이동할 위치가 있다면 조건문 만들어 두기
                doorHitDown = true;
                doorHitUp = true;
                doorHitLeft = true;
                doorHitRight = true;
            }
            // 몬스터가 남아있을 경우
            else
            {
                doorHitDown = false;
                doorHitUp = false;
                doorHitLeft = false;
                doorHitRight = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    // 캐릭터 맵 이동 시 위치 이동
    // 맵 이동 시 맵 On/Off 기능
    // 맵 이동 시 FadeIn/Out 기능
    // 맵은 미리 펼쳐져 있음
    // 모든 인게임에 적용하려면 맵은 미리 펼쳐져 있고 레이 캐스트로 확인해서 있으면 이동 없으면 이동불가

    // 맵 표시 map 속성 숫자(X) 속성 숫자(Z)
    // 속성 + Add A, - Subtract S, X Unchange U 
    // ex) mapA3S3  설명 map00을 기준으로 앞으로 3번째 아래로 3번째에 위치함
    

}
