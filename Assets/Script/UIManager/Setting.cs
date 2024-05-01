using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Setting : MonoBehaviour, HttpRequest
{
    public void TryLogOut()
    {
        StartCoroutine(PostReq("http://202.31.202.9:80/leave", "leave"));
    }
    
    public IEnumerator PostReq(string url, string data)
    {
        throw new System.NotImplementedException();
    }
}
