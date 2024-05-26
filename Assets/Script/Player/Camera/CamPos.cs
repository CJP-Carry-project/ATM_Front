using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPos : MonoBehaviour
{
    public Transform target;
    public Transform otherCam;
    private bool isPlayView = true;
    
    

    void Update()
    {
        if (isPlayView)
        {
            transform.position = target.position; //기본 상태
        }
        if (Input.GetKey(KeyCode.V)) //V키를 눌렀을 때
        {
            isPlayView = false;
            transform.position = otherCam.position;
            transform.rotation = otherCam.rotation;
            //시점 전환
        }
        if (!isPlayView && Input.GetKey(KeyCode.V))
        {
            isPlayView = true;
        }
    }
}