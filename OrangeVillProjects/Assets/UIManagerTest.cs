using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManagerTest : MonoBehaviour {

	void Start()
    {
        SceneManager.LoadScene("ProtoVill", LoadSceneMode.Additive);
    }
}
