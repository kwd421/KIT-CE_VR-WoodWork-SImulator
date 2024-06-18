using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform player;
    NavMeshAgent agent;

    enum State { Idle, Move, Attack , Damaged, Dead }
    // �ʱ����
    State state = State.Idle;

    [SerializeField]
    int hp;
	[SerializeField]
	int maxHp = 10;
	[SerializeField]
    float attackRange = 2f;

    [SerializeField]
    WaitForSeconds attackTime = new WaitForSeconds(2f);
    float curTime = 0f;

    [SerializeField]
    float distance;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

	void OnEnable()
	{
        hp = maxHp;
	}

	void Start()
    {
        // �ٷ� Ȱ��ȭ�ϸ� ���� ������ ����. false�� ����
        agent.enabled = false;
        // �ӵ� ����
        agent.speed = moveSpeed;
        // Player ����
        player = GameManager.instance.player;
    }

    void Update()
    {
        // �÷��̾ �� �ȿ� �������� �̵���������
        distance = Vector3.Distance(transform.position, player.position);
        if (GameManager.instance.isSafePlace)
        {
            state = State.Idle;
        }
        else if(!GameManager.instance.isSafePlace && distance > 2f)
        {
            state = State.Move;
        }

        switch(state)
        {
            case State.Idle:
                agent.enabled = false;
                break;

            case State.Move:
                Move();
                break;

            case State.Attack:
                Attack();
                //StartCoroutine(AttackCoroutine());
                break;
            case State.Damaged:
                Damaged();
                break;
            case State.Dead:
                Dead();
                break;
        }
    }

    public void Move()
    {
        if (agent == null)
        {
            return;
        }
        else
        {
            agent.enabled = true;
            agent.SetDestination(player.position);
            if(Vector3.Distance(transform.position, player.position) < attackRange)
            {
                state = State.Attack;
                agent.enabled = false;
            }
            if(hp < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    // 2�ʸ��� ����
    public void Attack()
    {
        if (curTime == 0f)
        {
            Debug.Log("Attack");
        }
        curTime += Time.deltaTime;
        if(curTime > 2f)
        {
            curTime = 0f;
        }
        
    }


    IEnumerator AttackCoroutine()
    {        
        yield return attackTime;
        Debug.Log("Attack");
    }

    public void Damaged()
    {
        // ü�°���
        hp--;
        Debug.Log(hp);
        // ���� hp�� ��������
        if(hp >= 0)
        {
            StartCoroutine(DamagedCoroutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }
	IEnumerator DamagedCoroutine()
	{
		// �� ã�� ����
		agent.enabled = false;
		// �ڽ� ��ü�� MeshRenderer�κ��� ���͸��� ������
		Material mat = GetComponentInChildren<MeshRenderer>().material;
		// ���� ���� ����
		Color originalColor = mat.color;
		// ���͸����� �� ����
		mat.color = Color.red;

		// 0.1�� ��ٸ���
		yield return new WaitForSeconds(0.1f);

		// ���͸����� �� �������
		mat.color = originalColor;
		// ���¸� Idle�� ��ȯ
		state = State.Move;
	}

	void Dead()
    {
        agent.enabled = false;
        Destroy(gameObject);
    }
}
