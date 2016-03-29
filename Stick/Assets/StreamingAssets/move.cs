using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour 
{
   
    //캐릭터의 Horizontal 방향 값 변수 선언
    private float h = 0.0f;
    private float v = 0.0f;
	
	private float walkSpeed = 3.3f;
	
	 void FixedUpdate()
    {
		h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
		
		if (h > 0) //만약 h 값이 0보다 클 경우
        {


                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        Debug.Log("뛰어가자");

                        TransformLimit();
                    }
                    else
                    {
                        Debug.Log("걸어가자");

                        TransformLimit();
                    }
		}
        else if (h < 0)
        {


                    if (Input.GetKey(KeyCode.LeftShift))
                    {

                        TransformLimit();
                    }
                    else
                    {

                        TransformLimit();
                    }

        }
	}
	    public void TransformLimit() 
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -14.0f, 14.0f), Mathf.Clamp(transform.position.y, -6.0f, 6.0f), Mathf.Clamp(transform.position.z, 0.0f, 8.0f));
        movement.Set(h, 0, v);
        if(Input.GetKey(KeyCode.LeftShift))
        {
            tr.Translate(movement.normalized * walkSpeed * runSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            tr.Translate(movement.normalized * walkSpeed * Time.deltaTime, Space.World);
        }
        
        
    }
}