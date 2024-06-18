using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.XR.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class Gun : MonoBehaviour
{
	[SerializeField]
	GameObject bulletFactory;
	[SerializeField]
	int bulletCount;
	[SerializeField]
	Transform bulletSpawnPoint;
	[SerializeField]
	float fireSpeed;
	[SerializeField]
	AudioSource[] bulletAudio;
	[SerializeField]
	List<GameObject> bullets = new List<GameObject>();
	[SerializeField]
	int channels;
	[SerializeField]
	AudioClip bulletSound;

	void Awake()
	{
		bulletAudio = new AudioSource[channels];
	}

	void Start()
	{ 
		XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
		// XRGRabInteractable�� activated �̺�Ʈ�� ���� Trigger�� �������� �ߵ���
		grabbable.activated.AddListener(FireBullet);

		for (int i = 0; i < channels; i++)
		{
			bulletAudio[i] = transform.AddComponent<AudioSource>();
			bulletAudio[i].playOnAwake = false;
			bulletAudio[i].clip = bulletSound;
		}

		// ������Ʈ Ǯ��
		for (int i = 0; i < bulletCount; i++)
		{
			GameObject bullet = Instantiate(bulletFactory);
			bullet.SetActive(false);
			bullets.Add(bullet);
		}
	}



	public void FireBullet(ActivateEventArgs arg)
	{
		foreach (GameObject bullet in bullets)
		{
			// ���� bullet�� ������� �ƴ϶��(��Ȱ��ȭ)
			if (!bullet.activeSelf)
			{
				PlayFireSound();
				bullet.SetActive(true);
				bullet.transform.position = bulletSpawnPoint.position;
				bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * fireSpeed;
				break;
			}
		}
	}

	void PlayFireSound()
	{
		for(int i=0; i<channels; i++)
		{
			if (bulletAudio[i].isPlaying)
			{
				continue;
			}
			else
			{
				bulletAudio[i].Play();
				break;
			}
		}
	}
}
