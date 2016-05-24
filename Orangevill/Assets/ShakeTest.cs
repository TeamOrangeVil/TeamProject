using UnityEngine;
using System.Collections;

public class ShakeTest : MonoBehaviour {

    
    float temp = 1;
    public float tempXPos;
    public float tempZRot;
    // Use this for initialization
    void Start() {
        tempXPos = transform.position.x;
        tempZRot = transform.rotation.z;
    }

	// Update is called once per frame
	void Update ()
    {
        if(transform.position.x > tempXPos + 0.5f)
        {
            Debug.Log("호우");
            temp = -1f;
        }
        else if (transform.position.x < tempXPos - 0.5f)
        {
            Debug.Log("날두");
            temp = 1f;
        }
        transform.Translate(Vector3.right * temp * Time.deltaTime);
        Debug.Log("실행실행");
        if (transform.rotation.z > tempZRot + 5f)
        {
            Debug.Log("호우");
            temp = -1f;
        }
        else if (transform.rotation.z < tempZRot - 5f)
        {
            Debug.Log("날두");
            temp = 1f;
        }
        transform.Rotate(Vector3.forward * temp * 15.0f * Time.deltaTime);
    }
}
