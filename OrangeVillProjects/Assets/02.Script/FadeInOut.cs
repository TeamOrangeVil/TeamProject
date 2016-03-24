using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour {
    //
    public GameObject bBoard; // 화면을 페이드 인 아웃 할 검은 판
    public GameObject collegeLogo; // 청강대 로고
    public GameObject teamLogo; // 팀 로고
    public GameObject mainLogo; // 게임 컨셉 원화
    public GameObject labelPanel1; // Press Any Button
    public GameObject labelPanel2; // 임시 게임 제목
    public UIPanel uipanel1; // 페이드 인 아웃 
    private bool isEnter = false;
    private float fadeSpeed = 0.005f; // 페이드 인 아웃 스피드
    private float panelFadeSpeed = 0.025f; // Press Any Button 페이드 속도
 
    // 인트로 화면 관련
    IEnumerator Start()
    {
        bBoard.transform.localScale = new Vector3(Screen.width, Screen.height*0.5f, -1); // 검은 판을 해상도 사이즈에 알맞게 확대
        yield return StartCoroutine("FadeIn"); // 검은 화면의 alpha 값이 점점 0으로 감소
        yield return new WaitForSeconds(0.5f); // 0.5초 정지
        yield return StartCoroutine("FadeOut"); // 검은 화면의 alpha 값이 점점 1로 증가
        collegeLogo.SetActive(false); // 대학 로고 OFF
        teamLogo.SetActive(true); // 팀 로고 ON
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine("FadeIn");
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine("FadeOut");
        teamLogo.SetActive(false); // 팀 로고 OFF
        mainLogo.SetActive(true); // 게임 컨셉 원화 ON
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine("FadeIn");
        NGUITools.SetActive(labelPanel1, true); // 임시 게임 제목 ON
        NGUITools.SetActive(labelPanel2, true); // Press Any Button ON
        yield return StartCoroutine("PressFadeInOut"); // labelPanel2가 페이드 인 아웃 효과, 아무 키나 누를 경우 씬 이동
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
    // 씬 이동 관련
    IEnumerator PressFadeInOut()
    {
        while (!isEnter) // 씬 이동 전까지 반복
        {
            for (float i = 1f; i >= 0; i -= panelFadeSpeed)
            {
                if (Input.anyKey)
                {
                    SceneManager.LoadScene("GameUI");
                }
                uipanel1.alpha = i; // uipanel1의 alpha 값에 i 저장
                yield return 0;
            }
            for (float i = 0f; i <= 1; i += panelFadeSpeed)
            {
                if (Input.anyKey)
                {
                    SceneManager.LoadScene("GameUI");

                }
                uipanel1.alpha = i;
                yield return 0;
            }
        }
    }

    IEnumerator SceneChange()
    {
        SceneManager.LoadScene("Main");
        yield return 0;
    }
}