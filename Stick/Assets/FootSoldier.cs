using UnityEngine;
using System.Collections;

public class FootSoldier : MonoBehaviour
{
    [SpineAnimation]
    public string idleAnim;

    [SpineAnimation]
    public string[] attackAnims;

    SkeletonAnimation skelAnim;

    void Start()
    {
        skelAnim = GetComponent<SkeletonAnimation>();
        skelAnim.state.SetAnimation(0, idleAnim, true);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var attackAnim = attackAnims[Random.Range(0, attackAnims.Length)];
            skelAnim.state.SetAnimation(0, attackAnim, false);
            skelAnim.state.AddAnimation(0, idleAnim, true, .5f);
        }
    }
}