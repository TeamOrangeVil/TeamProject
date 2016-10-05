using UnityEngine;
using System.Collections;

public class JointTint : MonoBehaviour {

    public GameObject[] Joints;

    public Transform Target;

    public bool isDie;

    public int arraySize = 10;

    void Awake()
    {
       //Joints = new GameObject[arraySize];
    }

    void FixedUpdate()
    {
        if(!isDie)
        {
            /*for(int i=0; i < arraySize; i++)
            {
                Debug.Log("알파");
                if(Joints[arraySize].transform.position.y > Target.transform.position.y)
                {
                    Color color = Joints[arraySize].GetComponent<Renderer>().material.color;
                    color = new Vector4(0, 0, 0, 0);
                    Joints[arraySize].GetComponent<Renderer>().material.color = color;
                }
                if (Joints[arraySize].transform.position.y < Target.transform.position.y)
                {
                    Color color = Joints[arraySize].GetComponent<Renderer>().material.color;
                    color = new Vector4(255, 255, 255, 255);
                    Joints[arraySize].GetComponent<Renderer>().material.color = color;
                }
            }*/
        }
    }
}
