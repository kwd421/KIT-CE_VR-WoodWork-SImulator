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

    // �ߺ� �˻� �ð��帧
    float curTime = 0f;
    // �ߺ� �˻� �ð�
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

    // FixedJointMethod�϶� ���
    void CheckJoints()
    {
        // ����� ����Ʈ�� ������ ����
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

                // ����Ʈ�� ���������� ���� ����Ʈ�� �߰�
                if (joint1.connectedBody == null)
                {
                    Debug.Log("Broken Wood Joint Remove");
                    Destroy(joint1);
                    jointsToRemove.Add(joint1);
                    continue;
                }

                // �ߺ��� ����Ʈ ����
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

            // ������ ����Ʈ ������ ����
            foreach (FixedJoint joint in jointsToRemove)
            {
                Debug.Log("Remove");
                connectedJoints.Remove(joint);
            }
        }
    }
    
    // Nailgun���� �򶧴� �ӵ��� Y������ 4.467472 �������� �����Ұ�
    void OnTriggerEnter(Collider other)
    {
		if (triggerLock)
			return;
        // ����� ��ȣ�ۿ��ϰ�, ���� �ɷ����� ���� ��
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
    
    // �������� �浹 ������(�� ���ư�)
    void FixedJointMethod(Collider other)
    {
		Debug.Log(nailRigid.velocity.y);
		Debug.Log("Trigger");

		// �� ���� �̹� ������ ����Ǿ� �ִ��� Ȯ��
		foreach (FixedJoint joint in other.GetComponents<FixedJoint>())
		{
			if (joint.connectedBody == nailRigid)
			{
				Debug.Log("Exist, Return");
				return;
			}
		}

		Debug.Log("NailMake");
		// ���ο� ����Ʈ ���� �� ������ ���� ����
		FixedJoint woodJoint = other.gameObject.AddComponent<FixedJoint>();
		woodJoint.connectedBody = nailRigid;

		// ����� ����Ʈ ����Ʈ�� �߰�
		connectedJoints.Add(woodJoint);
	}

	// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	// collider�� Combined�� 1���� Add�Ǹ� colliders�� �������� �ұ��ϰ� ������ �ʴ� ���� �Ѥ� �߻�.
	// �ΰ��ӻ󿡼� �ڽ��� collider�� �ѹ� �� �߰����ִ� �����۵��Ͽ��� �Ѥ�
	// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    void ParentCombineMethod(Collider other)
    {
		// Debug.Log(triggerLock);
		// �� ���� �θ� Combined�� �ƴҶ� + �浹�� ������ �θ� Combined�� �ƴҶ�(�� �� ù �浹)
		// �� �浹ü�� �θ� Combined�� ����
		if (!nail.root.CompareTag("Combined") && !other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("both no");
			GameObject combined = new GameObject("Combined");
			combined.tag = "Combined";
			combined.transform.position = nail.position;
			combined.AddComponent<Combined>();
			combined.AddComponent<Rigidbody>();

			// XRGrabInteractable �߰�
			XRGrabInteractable combinedGrab = combined.AddComponent<XRGrabInteractable>();
			combinedGrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
			// �浹ü �ƹ����̳� ���� �� �ְ�
			combinedGrab.useDynamicAttach = true;			

			// �θ� ������Ʈ�� ����
			nail.SetParent(combined.transform);
			other.transform.SetParent(combined.transform);
			
			// ������ Rigidbody �� XRGrabInteractable ����(���� �и����� �ʰ�)
			Destroy(nailRigid);

			Destroy(other.GetComponent<XRGrabInteractable>());
			Destroy(other.GetComponent<Rigidbody>());

			// combined�� collider ����
			combined.GetComponent<Combined>().ColliderCheck();
		}
		// �� ���� �θ� Combined�� �ƴҶ� + �浹�� ������ �θ�� Combined�϶�(���� ù �浹)
		// �� ���� Combined�� �ڽ����� ����
		else if (!nail.root.CompareTag("Combined") && other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("nail no");
			// �浹�� ���� �θ��� XRGrab ������
			XRGrabInteractable combinedGrab = other.transform.root.GetComponent<XRGrabInteractable>();

			// Rigidbody�� ������ ���� �и��ǹǷ� ����
			Destroy(nailRigid);
			nail.SetParent(other.transform.root);

			// combined�� collider ����
			other.transform.root.GetComponent<Combined>().ColliderCheck();
		}

		// �� ���� �θ� Combined�϶� + �浹�� ������ �θ�� Combined�� �ƴҶ�(���縸 ù �浹)
		// �浹�� ���縦 Combined�� �ڽ����� ����
		else if (nail.root.CompareTag("Combined") && !other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("wood no");
			// �� �θ��� XRGrab ������
			XRGrabInteractable combinedGrab = transform.root.GetComponent<XRGrabInteractable>();

			// Rigidbody�� ������ ���� �и��ǹǷ� ����
			Destroy(other.GetComponent<XRGrabInteractable>());
			Destroy(other.GetComponent<Rigidbody>());
			other.transform.SetParent(transform.root.transform);

			// combined�� collider ����
			nail.root.GetComponent<Combined>().ColliderCheck();
		}
		// �� ���� �θ� Combined�϶� + �浹�� ������ �θ� Combined�϶�(�� �� ù �浹�� �ƴ� ��)
		// ���� �ڸ��� �ٷ� ���������� �Ͼ�� �ó�����
		else if (nail.root.CompareTag("Combined") && other.transform.root.CompareTag("Combined"))
		{
			Debug.Log("both yes");
			// �� ���� �θ� ������
			Transform combined1 = transform.root;
			XRGrabInteractable combined1grab = combined1.GetComponent<XRGrabInteractable>();
			// �浹�� ������ �θ� ������
			Transform combined2 = other.transform.root;
			XRGrabInteractable combined2grab = combined2.GetComponent<XRGrabInteractable>();

			Debug.Log("child�̵�");
			foreach (Transform child in combined2)
			{
				child.SetParent(combined1);
			}
			Debug.Log("collider�̵�");
			foreach (Collider combined2col in combined2grab.colliders)
			{
				combined1grab.colliders.Add(combined2col);
			}
/*
			// �̵� Ȯ�� �� ��������� ���� �θ� Destroy
			if(combined2.CompareTag("Combined"))
			{
				Destroy(combined2.gameObject);
			}
			*/

			// combined�� collider ����
			combined1.GetComponent<Combined>().ColliderCheck();
		}
	}

	IEnumerator DisableNail()
	{
		yield return disappearTime;
		transform.gameObject.SetActive(false);
	}
}
