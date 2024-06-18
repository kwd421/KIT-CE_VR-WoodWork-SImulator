using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField]
	float disappearTime = 5f;
	float curTime = 0f;


	[SerializeField]
	Transform bulletImpact;
	[SerializeField]
	ParticleSystem bulletEffect;


	Rigidbody rigid;

	void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
	}

	void Update()
	{
		curTime += Time.deltaTime;

		// 일정 시간 지나면 총알 다시 비활성화
		if (curTime > disappearTime)
		{
			curTime = 0f;
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
			transform.gameObject.SetActive(false);
		}
	}

	void OnCollisionEnter(Collision other)
	{
		// 충돌체의 표면에 피격이펙트
		bulletImpact.forward = other.GetContact(0).normal;
		bulletImpact.position = other.GetContact(0).point;
		bulletImpact.localScale = Vector3.one * 100f;
		bulletEffect.Play();

		if (other.gameObject.CompareTag("Enemy"))
		{
			// 적 체력 -1
			other.transform.GetComponent<Enemy>().Damaged();
		}
	}
}
