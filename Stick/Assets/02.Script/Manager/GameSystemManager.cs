using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSystemManager : MonoBehaviour
{
    public GameObject BloodEfBackground;
    public GameObject BloodEfForeground;
    public float fadeSpeed = 0.05f;
    public float fadeFloat = 1.0f;

    public float timer = 0.0f;

    private static GameSystemManager gInstance = null;

    public static GameSystemManager Instance
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
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        SceneManager.LoadScene("Stage_01", LoadSceneMode.Additive);
    }
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer < 1)
        {
            //Debug.Log("timer : " + timer);
            //Debug.Log("alpha : " + test.alpha);
            //test.alpha += 0.06f;
        }
        else if (timer > 0)
        {
            //Debug.Log("timer : " + timer);
            //Debug.Log("alpha : " + test.alpha);
            //test.alpha -= 0.06f;
        }
    }
}

        /*public UIProgressBar HpBar; //캐릭터 체력

        public GameObject BloodEfBackground;
        public GameObject BloodEfForeground;
        public UISprite test;

        public float fadeSpeed = 0.05f;
        public float fadeFloat = 1.0f;

        public float timer = 0.0f;

        public UILabel labelTest;

        private static SystemManager gInstance = null;

        public static SystemManager Instance
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


        void FixedUpdate()
        {
            timer += Time.deltaTime;

            if(timer<1)
            {
                //Debug.Log("timer : " + timer);
                //Debug.Log("alpha : " + test.alpha);
                test.alpha += 0.06f;
            }
            else if(timer>0)
            {
                //Debug.Log("timer : " + timer);
                //Debug.Log("alpha : " + test.alpha);
                test.alpha -= 0.06f;
            }*/

        //else if (timer)
        /*
        if(test.alpha > 0)
        {
            test.alpha += 0.05f;
        }
        else if( test.alpha < 1)
        {

        }
        for (fadeFloat = 1.0f; fadeFloat >= 0; fadeFloat -= fadeSpeed*Time.deltaTime)
        {
            Debug.Log(test.alpha);

            test.alpha -= fadeSpeed;
        }*/


        //IEnumerator Fade()
        //{

        /*for (float i = 0.0f; i <= 1; i += fadeSpeed)
        {
            Debug.Log("보여진다");
            //Color color = BloodEfBackground.GetComponent<SpriteRenderer>().color;
            //color = new Vector4(0, 0, 0, i);
            //BloodEfBackground.GetComponent<SpriteRenderer>().color = color;

            //BloodEfBackground.GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 0, i);
            Color color = BloodEfBackground.GetComponent<Renderer>().material.color;
            color = new Vector4(0, 0, 0, i);
            BloodEfBackground.GetComponent<Renderer>().material.color = color;
            yield return 0;
        }*/
        //yield return 0;
        //}


        /*
    public GameObject bBoard; // 화면을 페이드 인 아웃 할 검은 판

    private float fadeSpeed = 0.005f; // 페이드 인 아웃 스피드

    // 인트로 화면 관련
    IEnumerator Start()
    {
        bBoard.transform.localScale = new Vector3(Screen.width, Screen.height * 0.5f, -1); // 검은 판을 해상도 사이즈에 알맞게 확대
        yield return StartCoroutine("FadeIn"); // 검은 화면의 alpha 값이 점점 0으로 감소
        yield return new WaitForSeconds(0.5f); // 0.5초 정지
        yield return StartCoroutine("FadeOut"); // 검은 화면의 alpha 값이 점점 1로 증가\

    }

    IEnumerator FadeIn()
    {
        for (float i = 1f; i >= 0; i -= fadeSpeed)
        {
            Color color = bBoard.GetComponent<Renderer>().material.color; // 검은 판의 마테리얼 컬러 값을 color에 저장
            color = new Vector4(0, 0, 0, i); // R G B A 중 변해야 할 Alpha 값에 for문 용 임시 변수 i를 저장
            bBoard.GetComponent<Renderer>().material.color = color; // 저장 된 값을 계속 저장
            yield return 0;
        }
    }

    IEnumerator FadeOut()
    {
        for (float i = 0f; i <= 1; i += fadeSpeed)
        {
            Color color = bBoard.GetComponent<Renderer>().material.color;
            color = new Vector4(0, 0, 0, i);
            bBoard.GetComponent<Renderer>().material.color = color;
            yield return 0;
        }
    }
    }
    */

