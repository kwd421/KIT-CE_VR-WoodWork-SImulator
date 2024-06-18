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

		// ���� �ð� ������ �Ѿ� �ٽ� ��Ȱ��ȭ
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
		// �浹ü�� ǥ�鿡 �ǰ�����Ʈ
		bulletImpact.forward = other.GetContact(0).normal;
		bulletImpact.position = other.GetContact(0).point;
		bulletImpact.localScale = Vector3.one * 100f;
		bulletEffect.Play();

		if (other.gameObject.CompareTag("Enemy"))
		{
			// �� ü�� -1
			other.transform.GetComponent<Enemy>().Damaged();
		}
	}
}
