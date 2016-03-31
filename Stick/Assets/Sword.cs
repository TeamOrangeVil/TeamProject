using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    BoundingBoxFollower follower;

    void Start()
    {
        follower = GetComponent<BoundingBoxFollower>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        collider.SendMessage("Hit", follower.CurrentAttachmentName, SendMessageOptions.DontRequireReceiver);
    }
}