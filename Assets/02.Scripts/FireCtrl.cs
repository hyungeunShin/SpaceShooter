using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;

    public Transform firePos;

    public AudioClip fireSfx;

    private new AudioSource audio;

    private MeshRenderer muzzleFlash;

    private RaycastHit hit;

    void Start()
    {
        audio = GetComponent<AudioSource>();

        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
    }

    void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * 10.0f, Color.green);

        //마우스 왼쪽
        if(Input.GetMouseButtonDown(0))
        {
            Fire();

            if(Physics.Raycast(firePos.position, firePos.forward, out hit, 10.0f, 1 << 6))
            {
                Debug.Log($"Hit={hit.transform.name}");
                hit.transform.GetComponent<MonsterCtrl>()?.OnDamage(hit.point, hit.normal);
            }
        }    
    }

    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
        audio.PlayOneShot(fireSfx, 1.0f);

        //총구 화염 효과 코루틴
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        //오프셋 좌표값 랜덤 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;

        //muzzleFlash 회전
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        //muzzleFlash 크기
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        muzzleFlash.enabled = true;
        
        //0.2초 동안 대기하는 동안 메시지 루프로 제어권 양보
        yield return new WaitForSeconds(0.2f);

        muzzleFlash.enabled = false;
    }
}
