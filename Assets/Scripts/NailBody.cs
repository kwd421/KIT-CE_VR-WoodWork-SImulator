using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Interaction.Toolkit;

public class NailBody : MonoBehaviour
{
    [Header("Nail")]
    [SerializeField]
    Transform nail;
	[SerializeField]
	WaitForSeconds disappearTime = new WaitForSeconds(10f);

    [SerializeField]
    Rigidbody nailRigid;

    [Header("Connected Woods' Joint")]
    [SerializeField]
    List<FixedJoint> connectedJoints = new List<FixedJoint>();
	bool triggerLock = false;

    // 중복 검사 시간흐름
    float curTime = 0f;
    // 중복 검사 시간
    float checkTime = 0.2f;

    void Start()
    {
        nail = transform.root;
        nailRigid = nail.GetComponent<Rigidbody>();
    }

    void Update()
    {
		//CheckJoints();
		StartCoroutine(DisableNail());
    }

    // FixedJointMethod일때 사용
    void CheckJoints()
    {
        // 연결된 조인트가 없으면 리턴
        if (connectedJoints.Count == 0)
            return;

        curTime += Time.deltaTime;
        if (curTime > checkTime)
        {
            curTime = 0f;
            List<FixedJoint> jointsToRemove = new List<FixedJoint>();

            for (int k = 0; k < connectedJoints.Count; k++)
            {
                FixedJoint joint1 = connectedJoints[k];

                // 조인트가 끊어졌으면 제거 리스트에 추가
                if (joint1.connectedBody == null)
                {
                    Debug.Log("Broken Wood Joint Remove");
                    Destroy(joint1);
                    jointsToRemove.Add(joint1);
                    continue;
                }

                // 중복된 조인트 제거
                for (int l = k + 1; l < connectedJoints.Count; l++)
                {
                    FixedJoint joint2 = connectedJoints[l];
                    if (joint1.connectedBody == joint2.connectedBody)
                    {
                        Debug.Log("Duplicated Wood Joint Remove");
                        Destroy(joint2);
                        jointsToRemove.Add(joint2);
                    }
                }
            }

            // 제거할 조인트 실제로 제거
            foreach (FixedJoint joint in jointsToRemove)
            {
                Debug.Log("Remove");
                connectedJoints.Remove(joint);
            }
        }
    }
    
    // Nailgun에서 쏠때는 속도를 Y축으로 4.467472 언저리로 설정할것
    void OnTriggerEnter(Collider other)
    {
		if (triggerLock)
			return;
        // 목재와 상호작용하고, 락이 걸려있지 않을 때
        if (other.gameObject.CompareTag("Wood"))
        {
			triggerLock = true;
			// FixedJoint Method
			// FixedJointMethod(other); 

			// Parent Combine Method
			ParentCombineMethod(other);
			triggerLock = false;
		}
    }
    
    // 물리엔진 충돌 에러남(휙 날아감)
    void FixedJointMethod(Collider other)
    {
		Debug.Log(nailRigid.velocity.y);
		Debug.Log("Trigger");

		// 이 못이 이미 나무와 연결되어 있는지 확인
		foreach (FixedJoint joint in other.GetComponents<FixedJoint>())
		{
			if (joint.connectedBody == nailRigid)
			{
				Debug.Log("Exist, Return");
				return;
			}
		}

		Debug.Log("NailMake");
		// 새로운 조인트 생성 및 나무를 못과 연결
		FixedJoint woodJoint = other.gameObject.AddComponent<FixedJoint>();
		woodJoint.connectedBody = nailRigid;

		// 연결된 조인트 리스트에 추가
		connectedJoints.Add(woodJoint);
	}

	// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	// collider가 Combined에 1번만 Add되면 colliders가 있음에도 불구하고 잡히지 않는 버그 ㅡㅡ 발생.
	// 인게임상에서 자식의 collider를 한번 더 추가해주니 정상작동하였음 ㅡㅡ
	// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    void ParentCombineMethod(Collider other)
    {
		// Debug.Log(triggerLock);
		// 이 못의 부모가 Combined가 아닐때 + 충돌한 목재의 부모도 Combined가 아닐때(둘 다 첫 충돌)
		// 두 충돌체의 부모를 Combined로 설정
		if (!nail.root.CompareTag("Combined") && !other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("both no");
			GameObject combined = new GameObject("Combined");
			combined.tag = "Combined";
			combined.transform.position = nail.position;
			combined.AddComponent<Combined>();
			combined.AddComponent<Rigidbody>();

			// XRGrabInteractable 추가
			XRGrabInteractable combinedGrab = combined.AddComponent<XRGrabInteractable>();
			combinedGrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
			// 충돌체 아무곳이나 잡을 수 있게
			combinedGrab.useDynamicAttach = true;			

			// 부모 오브젝트로 설정
			nail.SetParent(combined.transform);
			other.transform.SetParent(combined.transform);
			
			// 기존의 Rigidbody 및 XRGrabInteractable 제거(따로 분리되지 않게)
			Destroy(nailRigid);

			Destroy(other.GetComponent<XRGrabInteractable>());
			Destroy(other.GetComponent<Rigidbody>());

			// combined의 collider 갱신
			combined.GetComponent<Combined>().ColliderCheck();
		}
		// 이 못의 부모가 Combined가 아닐때 + 충돌한 목재의 부모는 Combined일때(못만 첫 충돌)
		// 이 못을 Combined의 자식으로 설정
		else if (!nail.root.CompareTag("Combined") && other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("nail no");
			// 충돌한 목재 부모의 XRGrab 가져옴
			XRGrabInteractable combinedGrab = other.transform.root.GetComponent<XRGrabInteractable>();

			// Rigidbody가 있으면 따로 분리되므로 제거
			Destroy(nailRigid);
			nail.SetParent(other.transform.root);

			// combined의 collider 갱신
			other.transform.root.GetComponent<Combined>().ColliderCheck();
		}

		// 이 못의 부모가 Combined일때 + 충돌한 목재의 부모는 Combined가 아닐때(목재만 첫 충돌)
		// 충돌한 목재를 Combined의 자식으로 설정
		else if (nail.root.CompareTag("Combined") && !other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("wood no");
			// 못 부모의 XRGrab 가져옴
			XRGrabInteractable combinedGrab = transform.root.GetComponent<XRGrabInteractable>();

			// Rigidbody가 있으면 따로 분리되므로 제거
			Destroy(other.GetComponent<XRGrabInteractable>());
			Destroy(other.GetComponent<Rigidbody>());
			other.transform.SetParent(transform.root.transform);

			// combined의 collider 갱신
			nail.root.GetComponent<Combined>().ColliderCheck();
		}
		// 이 못의 부모가 Combined일때 + 충돌한 목재의 부모도 Combined일때(둘 다 첫 충돌이 아닐 때)
		// 목재 자리에 바로 생성했을때 일어나는 시나리오
		else if (nail.root.CompareTag("Combined") && other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("both yes");
			// 이 못의 부모 가져옴
			Transform combined1 = transform.root;
			XRGrabInteractable combined1grab = combined1.GetComponent<XRGrabInteractable>();
			// 충돌한 목재의 부모 가져옴
			Transform combined2 = other.transform.root;
			XRGrabInteractable combined2grab = combined2.GetComponent<XRGrabInteractable>();

			Debug.Log("child이동");
			foreach (Transform child in combined2)
			{
				child.SetParent(combined1);
			}
			Debug.Log("collider이동");
			foreach (Collider combined2col in combined2grab.colliders)
			{
				combined1grab.colliders.Add(combined2col);
			}
/*
			// 이동 확인 후 쓸모없어진 목재 부모 Destroy
			if(combined2.CompareTag("Combined"))
			{
				Destroy(combined2.gameObject);
			}
			*/

			// combined의 collider 갱신
			combined1.GetComponent<Combined>().ColliderCheck();
		}
	}

	IEnumerator DisableNail()
	{
		yield return disappearTime;
		transform.gameObject.SetActive(false);
	}
}
