using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePlace : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name.Equals("Player"))
        {
            GameManager.instance.isSafePlace = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name.Equals("Player"))
        {
            GameManager.instance.isSafePlace = false;
        }
    }
}
