using UnityEngine;
using System.Collections;

public class ProtoVillManager : MonoBehaviour {

    public GameObject poorVill;
    public GameObject richVill;

    private static ProtoVillManager gInstance = null;

    public static ProtoVillManager Instance
    {
        get
        {
            if (gInstance == null) { }
            return gInstance;
        }
    }

    void Awake()
    {
        gInstance = this;
    }



}
