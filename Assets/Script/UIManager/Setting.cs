using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour, HttpRequest
{
    public void TryLogOut()
    {
        StartCoroutine(PostReq("http://202.31.202.9:80/leave", "leave"));
    }
    
    public IEnumerator PostReq(string url, string data)
    {
       Debug.Log(data);
        
       string json = "{\"msg\":\"" + data + "\"}";

       using (UnityWebRequest webRequest = new UnityWebRequest(url, "Post"))
       {
           byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
           webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
           webRequest.downloadHandler = new DownloadHandlerBuffer();
           webRequest.SetRequestHeader("Content-Type", "application/json");
           
           yield return webRequest.SendWebRequest();

           if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
               webRequest.result == UnityWebRequest.Result.ProtocolError)
           {
               Debug.LogError(webRequest.error);
           }
           else
           {
               Debug.Log("정상적으로 정보 보냄");

               if (webRequest.responseCode == 200)
               {
                   SceneManager.LoadScene("Login");
               }
               else
               {
                   Debug.Log("비정상적으로 실패");
               }
           }
       }
                        
    }
}
