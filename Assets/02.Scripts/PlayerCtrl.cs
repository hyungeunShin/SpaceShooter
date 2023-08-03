using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    //컴포넌트
    private Transform tr;
    //애니메이션
    private Animation anim;

    //이동속도
    public float moveSpeed = 10.0f;
    //회전속도
    public float turnSpeed = 80.0f;

    //생명력
    private readonly float initHp = 100.0f;
    public float currHp;
    private Image hpBar;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    void Awake()
    {
        //제일 먼저 호출
        //스크립트가 비활성화되어 있어도 호출됨
    }
    
    void OnEnable()
    {
        //두번째 호출
        //스크립트가 활성화될 때마다 호출
    }

    /* void Start()
    {
        //세번째
        //Update 호출되기 전 호출
        //코루틴으로 호출될 수 있음
    } */

    IEnumerator Start()
    {
        hpBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();
        currHp = initHp;
        DisplayHealth();

        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        //애니메이션 실행
        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;
    }
    
    void Update()
    {
        //프레임마다 호출
        //호출간격 불규칙
        //화면의 렌더링 주기와 일치

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");
        //Debug.Log("h= " + h);        
        //Debug.Log("v= " + v);

        //이동 로직
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        //캐릭터 애니메이션
        PlayerAnim(h, v);
    }

    void LateUpdate()
    {
        //Update 함수가 종료된 후 호출
    }

    void FixedUpdate()
    {
        //일정한 간격으로 호출(default 0.02)
        //물리 엔진의 계산 주기와 일치
    }

    void PlayerAnim(float h, float v)
    {
        if(v >= 0.1f)
        {
            //전진
            anim.CrossFade("RunF", 0.25f);
        }
        else if(v <= -0.1f)
        {
            //후진
            anim.CrossFade("RunB", 0.25f);
        }
        else if(h >= 0.1f)
        {
            //오른쪽이동
            anim.CrossFade("RunR", 0.25f);
        }
        else if(h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            //정지
            anim.CrossFade("Idle", 0.25f);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if(currHp >= 0.0f && coll.CompareTag("PUNCH"))
        {
            currHp -= 10.0f;
            DisplayHealth();
            
            Debug.Log($"Player hp = {currHp/initHp}");

            if(currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        Debug.Log("Player Die");

        // GameObject[] monsters = GameObject.FindGameObjectWithTag("MONSTER");

        // foreach(GameObject monster in monsters)
        // {
        //     monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        // }

        OnPlayerDie();
        //GameObject.Find("GameMgr").GetComponent<GameManager>().IsGameOver = true;
        GameManager.instance.IsGameOver = true;
    }

    void DisplayHealth()
    {
        hpBar.fillAmount = currHp / initHp;
    }
}
