using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

    public SpriteRenderer BlackBoard;       // 페이드 박스 블랙
    public SpriteRenderer WhiteBoard;       // 페이드 박스 화이트
    public SpriteRenderer ChungKangLogo;    // 청강 로고
    public SpriteRenderer OrangeVillLogo;   // 오렌지빌 로고

    public float fadeSpeed = 0.005f;

    private int frameRate = 60;
    void Awake()
    {
        BlackBoard = GameObject.Find("Black").GetComponent<SpriteRenderer>();
        WhiteBoard = GameObject.Find("White").GetComponent<SpriteRenderer>();
        ChungKangLogo = GameObject.Find("ChungKangLogo").GetComponent<SpriteRenderer>();
        OrangeVillLogo = GameObject.Find("OrangeVillLogo").GetComponent<SpriteRenderer>();
        Application.targetFrameRate = frameRate;
    }

    IEnumerator Start() // 인트로 화면, 페이드 연출을 하며 두개의 로고를 순서대로 보여줌
    {
        // 뒤에 나올 오렌지빌 로고의 알파값을 0으로
        OrangeVillLogo.color = new Color(1f, 1f, 1f, 0f);

        // 화면 전체를 가리고 있는 검은 화면의 알파값을 0으로
        for (float i = 1; i >= 0; i -= fadeSpeed)
        {
            BlackBoard.color = new Color(1f, 1f, 1f, i);
            yield return new WaitForSeconds(fadeSpeed);
        }
        // 화면 전체를 가리고 있는 검은 화면의 알파값을 1으로
        for (float i = 0; i <= 1; i += fadeSpeed)
        {
            BlackBoard.color = new Color(1f, 1f, 1f, i);
            yield return new WaitForSeconds(fadeSpeed);
        }

        yield return new WaitForSeconds(1f);

        // 오렌지빌 로고의 알파값을 1로
        // 청강대 로고의 알파값을 0으로
        OrangeVillLogo.color = new Color(1f, 1f, 1f, 1f);
        ChungKangLogo.color = new Color(1f, 1f, 1f, 0f);

        // 화면 전체를 가리고 있는 검은 화면의 알파값을 0으로
        for (float i = 1; i >= 0; i -= fadeSpeed)
        {
            BlackBoard.color = new Color(1f, 1f, 1f, i);
            yield return new WaitForSeconds(fadeSpeed);
        }
        // 화면 전체를 가리고 있는 검은 화면의 알파값을 1으로
        for (float i = 0; i <= 1; i += fadeSpeed)
        {
            BlackBoard.color = new Color(1f, 1f, 1f, i);
            yield return new WaitForSeconds(fadeSpeed);
        }

        SceneManager.LoadScene(01, LoadSceneMode.Single);
        yield return 0;
    }

    void FixedUpdate()
    {
        // Enter 혹은 ESC를 누르면 바로 다음 화면으로 전환
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(01, LoadSceneMode.Single);
        }
    }
}
