using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    //폭발 효과
    public GameObject expEffect;

    public Texture[] textures;
    private new MeshRenderer renderer;

    //폭발 반경
    public float radius = 10.0f;

    private Transform tr;
    private Rigidbody rb;

    //총알 맞은 횟수
    private int hitCount = 0;

    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        renderer = GetComponentInChildren<MeshRenderer>();

        int idx = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[idx];
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.CompareTag("BULLET"))
        {
            if(++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        //폭발 효과 파티클 생성
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        //폭발 효과 파티클 5초 후에 제거
        Destroy(exp, 5.0f);

        //Rigidbody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함
        //rb.mass = 1.0f;
        //위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * 1500.0f);

        //간접 폭발력 전달
        IndirectDamage(tr.position);

        //3초 후에 드럼통 제거
        Destroy(gameObject, 3.0f);
    }

    // 폭발력을 주변에 전달하는 함수
    void IndirectDamage(Vector3 pos)
    {
        //주변에 있는 드럼통을 모두 추출
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);

        foreach(var coll in colls)
        {
            //폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
            rb = coll.GetComponent<Rigidbody>();
            //드럼통의 무게를 가볍게 함
            rb.mass = 1.0f;
            //freezeRotation 제한값을 해제
            rb.constraints = RigidbodyConstraints.None;
            //폭발력을 전달
            rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);
        }
    }
}
