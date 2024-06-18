using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Combined : MonoBehaviour
{
    XRGrabInteractable xr;
    List<Collider> temp = new List<Collider>();
	AudioSource hitAudio;
	AudioSource breakAudio;
	WaitForSeconds breakTime = new WaitForSeconds(1f);
	int hp = 10;
	bool broken = false;

	void Awake()
	{
		breakAudio = transform.AddComponent<AudioSource>();
		hitAudio = transform.AddComponent<AudioSource>();
	}

	void Start()
    {
        xr = GetComponent<XRGrabInteractable>();
		breakAudio.clip = GameManager.instance.combinedBreakSound;
		hitAudio.clip = GameManager.instance.combinedhitSound;
	}

	void Update()
	{
		if(transform.childCount == 0)
		{
			Destroy(gameObject);
		}
		if(hp < 0 && !broken)
		{
			broken = true;
			StartCoroutine(BreakCoroutine());
		}
	}

	// �ݶ��̴� �ߺ�üũ
    public void ColliderCheck()
	{		
		StartCoroutine(ColCheckCoroutine());
	}


	IEnumerator ColCheckCoroutine()
	{
		// null reference exception ���ϱ����� ���������ӿ� collider�߰� ����
		yield return null;
		//xr.colliders.Clear();
		foreach (Transform child in transform)
		{
			// ��
			if (child.GetComponent<Collider>() == null)
			{
				foreach (Transform grandChild in child)
				{
					xr.colliders.Add(grandChild.GetComponent<Collider>());
					xr.interactionManager.UnregisterInteractable(xr.GetComponent<IXRInteractable>());
					xr.interactionManager.RegisterInteractable(xr.GetComponent<IXRInteractable>());
				}
			}
			// ����
			else
			{
				xr.colliders.Add(child.GetComponent<Collider>());
				xr.interactionManager.UnregisterInteractable(xr.GetComponent<IXRInteractable>());
				xr.interactionManager.RegisterInteractable(xr.GetComponent<IXRInteractable>());
			}
		}
		// �������� hp �ʱ�ȭ
		hp = 10;
		Debug.Log(xr.colliders.Count);
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.CompareTag("Enemy"))
		{
			hp--;
			collision.transform.GetComponent<Enemy>().Damaged();
			hitAudio.Play();
		}
	}

	IEnumerator BreakCoroutine()
	{
		breakAudio.Play();
		GetComponentInChildren<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
		yield return breakTime;
		Destroy(transform.gameObject);
	}
}
