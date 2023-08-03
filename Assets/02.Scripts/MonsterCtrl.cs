using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터 상태
    public enum State
    {
        IDLE,
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.IDLE;

    public float traceDist = 10.0f;

    public float attackDist = 2.0f;

    public bool isDie = false;

    //컴포넌트
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");

    //혈흔 효과
    private GameObject bloodEffect;
    private int hp = 100;

    void OnEnable()
    {
        //이벤트 발생 시 함수 연결
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    void OnDisable()
    {
        //연결 함수 해제
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        anim = GetComponent<Animator>();
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        //추적 대상의 위치로 추적 시작
        //agent.destination = playerTr.position;
    }

    void update()
    {
        if(agent.remainingDistance >= 2.0f)
        {
            Vector3 direction = agent.desiredVelocity;
            Quaternion rot = Quaternion.LookRotation(direction);

            monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, rot, Time.deltaTime * 10.0f);
        }
    }

    IEnumerator CheckMonsterState()
    {
        while(!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            if(state == State.DIE) yield break;

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if(distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if(distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while(!isDie)
        {
            switch(state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                
                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;

                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;

                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;

                    yield return new WaitForSeconds(3.0f);
                    
                    hp = 100;
                    isDie = false;

                    GetComponent<CapsuleCollider>().enabled = true;
                    this.gameObject.SetActive(false);
                    
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.CompareTag("BULLET"))
        {
            Destroy(coll.gameObject);
        }
    }

    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        anim.SetTrigger(hashHit);

        //총알의 충돌 지점
        //Vector3 pos = coll.GetContact(0).point;
        //Quaternion rot = Quaternion.LookRotation(-coll.GetContact(0).normal);

        Quaternion rot = Quaternion.LookRotation(normal);
        ShowBloodEffect(pos, rot);

        hp -= 30;
        if(hp <= 0)
        {
            state = State.DIE;
            GameManager.instance.DisplayScore(50);
        }
    }

    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }

    void onDrawGizmos()
    {
        //추적 사정거리
        if(state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }

        //공격 사정거리
        if(state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.gameObject.name);
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        anim.SetTrigger(hashPlayerDie);
    }
}
