using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrashBox : MonoBehaviour
{
    AudioSource audio;
    public AudioClip clip;

	void Start()
	{
		audio = transform.AddComponent<AudioSource>();
        audio.clip = clip;
	}
	private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Wood") || collider.gameObject.CompareTag("Nail"))
        {
            StartCoroutine(BreakCoroutine(collider));
        }        
    }
    
    IEnumerator BreakCoroutine(Collider collider)
    {
        Destroy(collider.gameObject);
        audio.Play();
        yield return null;
    }
}
