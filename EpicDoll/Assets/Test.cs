using UnityEngine;
using System.Collections;

//delegate void EventHandler(Color color);

delegate IEnumerator TutorialHandler(int step);

/*class TestParams
{
    public event EventHandler eventHandler;

    int _hp = 100;

    public int hp
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (hp <= 50) eventHandler(Color.red);
        }
    }
}*/

class QuestParams
{
    public event TutorialHandler tutorialHandler;

    int _step = 100;

    public int step
    {
        get { return _step; }
        set
        {
            _step = value;
        }
    }
}

public class Test : MonoBehaviour {
   /* void ChangeColor(Color color)
    {
        this.GetComponent<Renderer>().material.color = Color.red;
    }

    void ShowColor(Color color)
    {
        print(color);
    }*/
    IEnumerator test(int aa)
    {
        yield return 0;
    }

    //TestParams testParams = new TestParams();

    QuestParams questParams = new QuestParams();

    void Start()
    {
        /*testParams.eventHandler += new EventHandler(ChangeColor);
        testParams.eventHandler += new EventHandler(ShowColor);*/

        questParams.tutorialHandler += new TutorialHandler(test);
    }

    void OnMouseDown()
    {
        //testParams.hp -= 10;
    }


}
