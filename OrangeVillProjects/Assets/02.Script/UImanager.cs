using UnityEngine;
using System.Collections;

public class UImanager : MonoBehaviour {
    public GameObject InputBox;
    UIInput uiinput;
    string text;
    float zMove;
    public void getMessage()
    {
        uiinput = InputBox.GetComponent<UIInput>();
        text = uiinput.label.text;
        print(text);
        zMove = float.Parse(text);
        FollowCamera.instance.z = zMove;
    }
}
