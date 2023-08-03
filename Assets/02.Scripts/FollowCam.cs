using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    //따라갈 대상
    public Transform targetTr;

    //메인 카메라
    private Transform camTr;

    //떨어질 거리
    [Range(2.0f, 20.0f)]
    public float distance = 10.0f;

    [Range(0.0f, 10.0f)]
    public float height = 2.0f;

    //반응속도
    public float damping = 10.0f;

    public float targetOffset = 2.0f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        camTr = GetComponent<Transform>();    
    }

    void LateUpdate()
    {
        Vector3 pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);
        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}
