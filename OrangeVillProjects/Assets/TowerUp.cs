using UnityEngine;
using System.Collections;

public class TowerUp : MonoBehaviour {

    public GameObject beggarHouse;
    public GameObject richHouse;

    void FixedUpdate()
    {
        if(QuestManager.Instance.questCount ==2)
        {
            //beggarHouse.SetActive(false);
            //richHouse.SetActive(true);
        }
    }
}
