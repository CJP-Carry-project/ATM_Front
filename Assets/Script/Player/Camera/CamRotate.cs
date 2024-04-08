using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 200f;
    
    //유니티 특성상 0~360으로 값을 처리 => -1도가 되는 경우 359도로 자동 연산 처리
    //회전값 변수 -> 회전 값 누적을 위함.
    private float mx = 0;
    private float my = 0;
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        
        float mouse_X = Input.GetAxis("Mouse X"); // 마우스 x 축에 관한 입력 
        float mouse_Y = Input.GetAxis("Mouse Y"); // 마우스 y 축에 관한 입력 

        //마우스 입력값 만큼 누적
        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;
        
        //y 값에 대해서 최대 최소 값의 범위를 부여
        my = Mathf.Clamp(my, -90f, 90f); //최소 ~ 최대의 사이 값으로 지정해준다.
        
        // 회전 방향으로 물체를 회전
        transform.eulerAngles = new Vector3(-my, mx, 0);
    }
}
