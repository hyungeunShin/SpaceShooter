using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //몬스터 출현 위치
    public List<Transform> points = new List<Transform>();

    //몬스터를 미리 생성해 저장
    public List<GameObject> monsterPool = new List<GameObject>();
    public int maxMonsters = 10;

    public GameObject monster;

    //생성 간격
    public float createTime = 3.0f;

    private bool isGameOver;

    public bool IsGameOver
    {
        get{return isGameOver;}
        set{
            isGameOver = value;
            if(isGameOver)
            {
                CancelInvoke("CreateMonster");
            }
        }
    }

    //싱글턴
    public static GameManager instance = null;
    
    //점수
    public TMP_Text scoreText;
    private int totScore;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(this.gameObject);
        }

        //다른 화면으로 전환되도 유지
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        CreateMonsterPool();

        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        foreach(Transform point in spawnPointGroup)
        {
            points.Add(point);
        }

        InvokeRepeating("CreateMonster", 2.0f, createTime);

        totScore = PlayerPrefs.GetInt("TOT_SCORE", 0);
        DisplayScore(0);
    }

    void CreateMonster()
    {
        int idx = Random.Range(0, points.Count);
        //Instantiate(monster, points[idx].position, points[idx].rotation);

        GameObject _monster = GetMonsterInPool();
        _monster?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
        _monster?.SetActive(true);
    }

    void CreateMonsterPool()
    {
        for(int i = 0; i < maxMonsters; i++)
        {
            var _monster = Instantiate<GameObject>(monster);
            _monster.name = $"Monster_{i:00}";
            _monster.SetActive(false);

            monsterPool.Add(_monster);
        }
    }

    public GameObject GetMonsterInPool()
    {
        foreach(var _monster in monsterPool)
        {
            if(_monster.activeSelf == false)
            {
                return _monster;
            }
        }

        return null;
    }

    public void DisplayScore(int score)
    {
        totScore += score;
        scoreText.text = $"SCORE: {totScore:#,##0}";
        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }
}
