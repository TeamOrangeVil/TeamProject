using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {

    public Transform tr;
    public Rigidbody2D rb;

    public float moveSpeed=0.001f;
    public float ChCount = 0.0f;

    public bool isAct = false;

    public Vector3 trMove;
    public Vector2 rbMove;

    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        trMove = new Vector3(0, 0, 0);
        rbMove = new Vector3(0, 0, 0);
    }
    void Start()
    {
        StartCoroutine(Move());
    }
    
    IEnumerator Move()
    {
        while(!isAct)
        {
            yield return 0;
            if (ChCount == 0)
            {
                trMove += new Vector3(0, 1, 0) * Time.deltaTime;
                tr.Translate(trMove.normalized * 0.1f);
                //rbMove += new Vector2(0, moveSpeed) * Time.deltaTime;
                //rb.velocity += rbMove;
                if(transform.position.y > 1)
                {
                    ChCount += 1.0f;
                }
            }
            if (ChCount == 1)
            {
                trMove -= new Vector3(0, 1, 0) * Time.deltaTime;
                tr.Translate(trMove.normalized * 0.1f);
                //rbMove -= new Vector2(0, moveSpeed) * Time.deltaTime;
                //rb.velocity += rbMove;
                if (transform.position.y < 0)
                {
                    ChCount -= 1.0f;
                }
            }
        }
    }
}
