using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform player;
    // �����Ѱ� üũ
    public bool isSafePlace;

    public AudioClip combinedBreakSound;
    public AudioClip combinedhitSound;
    
    
	void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

}
