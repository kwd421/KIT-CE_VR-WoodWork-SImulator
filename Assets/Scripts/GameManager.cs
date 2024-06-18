using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform player;
    // 안전한곳 체크
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
