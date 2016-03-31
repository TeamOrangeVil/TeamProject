using UnityEngine;
using System.Collections;

public class Woodman : MonoBehaviour
{
    [SpineAnimation]
    public string hitAnim;

    SkeletonAnimation skelAnim;

    void Start()
    {
        skelAnim = GetComponent<SkeletonAnimation>();
    }

    void Hit(string attachmentName)
    {
        if (attachmentName == "Stab")
        {
            Debug.Log("Stabbed");
        }
        else if (attachmentName == "Slice")
        {
            Debug.Log("Sliced");
        }
        skelAnim.state.SetAnimation(0, hitAnim, false);
    }
}