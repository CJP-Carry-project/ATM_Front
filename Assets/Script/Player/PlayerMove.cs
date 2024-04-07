using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 10f;

    private CharacterController cc;

    private float gravity = -20f;
    private float yVelocity = 0f;
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // 좌 우
        float v = Input.GetAxis("Vertical"); // 앞 뒤

        Vector3 dir = new Vector3(h, 0, v);

        dir = dir.normalized; //방향 정규화

        //카메라 기준으로 방향을 지정 [기준을 명확하게]
        dir = Camera.main.transform.TransformDirection(dir);
        
        // 캐릭터 수직 속도에 중력값 적용
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        cc.Move(dir * moveSpeed * Time.deltaTime);
    }
}
