using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 300f;
    
    //유니티 특성상 0~360으로 값을 처리 => -1도가 되는 경우 359도로 자동 연산 처리
    //회전값 변수 -> 회전 값 누적을 위함.
    private float mx = 0;
    
    void FixedUpdate()
    {
        // 플레이어의 경우 좌우 회전만 하면 된다. [범위는 따로 정할 필요 없음]
        float mouse_X = Input.GetAxis("Mouse X"); // 마우스 x 축에 관한 입력 

        //마우스 입력값 만큼 누적
        mx += mouse_X * rotSpeed * Time.deltaTime;
        
        // 회전 방향으로 물체를 회전
        transform.eulerAngles = new Vector3(0, mx, 0);
    }
}
