using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //데미지
    public float damage = 20.0f;

    //발사
    public float force = 1500.0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);    
    }
}
