using UnityEngine;
using System.Collections;

public class QuestManager : MonoBehaviour {

    public UILabel questText;
    public Sprite textBackground;

    public bool questCh = false;

    private static QuestManager gInstance = null;

    public static QuestManager Instance
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
        if(Input.GetKey(KeyCode.Alpha1))//진짜 비교는 촌장님한테 말걸면
        {
            //questText.text = //xml 파일 1번
        }
    }
}
