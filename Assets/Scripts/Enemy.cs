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
    // 초기상태
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
        // 바로 활성화하면 오류 날수도 있음. false로 시작
        agent.enabled = false;
        // 속도 설정
        agent.speed = moveSpeed;
        // Player 설정
        player = GameManager.instance.player;
    }

    void Update()
    {
        // 플레이어가 집 안에 있을때는 이동하지않음
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

    // 2초마다 공격
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
        // 체력감소
        hp--;
        Debug.Log(hp);
        // 아직 hp가 남았을떄
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
		// 길 찾기 중지
		agent.enabled = false;
		// 자식 객체의 MeshRenderer로부터 매터리얼 얻어오기
		Material mat = GetComponentInChildren<MeshRenderer>().material;
		// 원래 색을 저장
		Color originalColor = mat.color;
		// 매터리얼의 색 변경
		mat.color = Color.red;

		// 0.1초 기다리기
		yield return new WaitForSeconds(0.1f);

		// 매터리얼의 색 원래대로
		mat.color = originalColor;
		// 상태를 Idle로 전환
		state = State.Move;
	}

	void Dead()
    {
        agent.enabled = false;
        Destroy(gameObject);
    }
}
