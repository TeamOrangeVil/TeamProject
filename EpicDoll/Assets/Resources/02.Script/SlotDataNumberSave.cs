using UnityEngine;
using System.Collections;

public class SlotDataNumberSave : MonoBehaviour {

    private static SlotDataNumberSave gInstance = null;
    public int slotNum;

    public static SlotDataNumberSave Instance
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
        DontDestroyOnLoad(this);
    }

}
