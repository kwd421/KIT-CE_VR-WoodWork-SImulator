using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.XR.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class NailGun : MonoBehaviour
{
	[SerializeField]
	GameObject nailFactory;
	[SerializeField]
	int nailCount;
	[SerializeField]
	Transform nailSpawnPoint;
	[SerializeField]
	float fireSpeed;
	[SerializeField]
	int nailNum;
	[SerializeField]
	AudioClip[] nailGunSFXs;
	[SerializeField]
	int channels;
	[SerializeField]
	AudioSource[] nailAudio;

	int soundNum;

	List<GameObject> nails = new List<GameObject>();

	void Awake()
	{
		nailAudio = new AudioSource[channels];
	}


	void Start()
	{
		XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
		// XRGRabInteractable의 activated 이벤트는 보통 Trigger를 눌렀을때 발동됨
		grabbable.activated.AddListener(FireNail);

		// 소리 여러번 가능하게
		for (int i = 0; i < channels; i++)
		{
			nailAudio[i] = transform.AddComponent<AudioSource>();
			nailAudio[i].playOnAwake = false;
		}

		// 오브젝트 풀링
		for (int i = 0; i < nailCount; i++)
		{
			GameObject nail = Instantiate(nailFactory);
			nail.SetActive(false);
			nails.Add(nail);
		}
	}



	public void FireNail(ActivateEventArgs arg)
	{
		NailGunSFXPlay();
		nails[nailNum].SetActive(true);
		// 총구가 바라보는 방향으로 발사
		nails[nailNum].transform.position = nailSpawnPoint.position;
		nails[nailNum].transform.rotation = nailSpawnPoint.rotation;
		nails[nailNum].GetComponent<Rigidbody>().velocity = nailSpawnPoint.forward * fireSpeed;
		nailNum++;
		if (nailNum >= nails.Count)
		{
			nailNum = 0; // 인덱스 초과 방지를 위해 nailNum 초기화
		}
	}

	// 효과음 랜덤재생
	void NailGunSFXPlay()
	{
		soundNum = Random.Range(0, nailGunSFXs.Length);
		for(int i=0; i<channels; i++)
		{
			if(nailAudio[i].isPlaying)
			{
				continue;
			}
			else
			{
				nailAudio[i].clip = nailGunSFXs[soundNum];
				nailAudio[i].Play();
				break;
			}
		}
	}
}
