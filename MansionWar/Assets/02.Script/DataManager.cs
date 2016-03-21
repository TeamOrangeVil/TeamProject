using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour {
    
    public UIProgressBar HpBar; //캐릭터 체력

    public GameObject BloodEfBackground;
    public GameObject BloodEfForeground;
    public UISprite test;

    public float fadeSpeed = 0.05f;
    public float fadeFloat = 1.0f;

    public float timer = 0.0f;

    private static DataManager gInstance = null;

    public static DataManager Instance
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
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);

    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if(timer<1)
        {
            Debug.Log("timer : " + timer);
            Debug.Log("alpha : " + test.alpha);
            test.alpha += 0.06f;
        }
        else if(timer>0)
        {
            Debug.Log("timer : " + timer);
            Debug.Log("alpha : " + test.alpha);
            test.alpha -= 0.06f;
        }
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
    }

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
}
