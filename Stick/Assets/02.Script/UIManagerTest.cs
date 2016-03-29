using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManagerTest : MonoBehaviour {

    public GameObject bBoard; // 화면을 페이드 인 아웃 할 검은 판
    
    private float fadeSpeed = 0.005f; // 페이드 인 아웃 스피드

    // 인트로 화면 관련
    IEnumerator Start()
    {
        bBoard.transform.localScale = new Vector3(Screen.width, Screen.height * 0.5f, -1); // 검은 판을 해상도 사이즈에 알맞게 확대
        yield return StartCoroutine("FadeIn"); // 검은 화면의 alpha 값이 점점 0으로 감소
        yield return new WaitForSeconds(0.5f); // 0.5초 정지
        yield return StartCoroutine("FadeOut"); // 검은 화면의 alpha 값이 점점 1로 증가\
        SceneManager.LoadScene("ProtoVill", LoadSceneMode.Additive);
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
