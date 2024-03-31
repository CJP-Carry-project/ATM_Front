using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class SendURLInfo : MonoBehaviour
{
    [SerializeField] private TMP_InputField info;
    private string url = "http://localhost:80/";
    private bool isPost = false;

    public void sendInfoForMusic() //서버에게 URL 정보 보내기 [음원 악보 채보 버튼 이벤트]
    {
        if (!isPost)
        {
            Debug.Log(info.text);
            StartCoroutine(PostReq(info.text, 0));
        }
        else
        {
            Debug.Log("사전 Post 작업 진행 중");
        }
    }

    public void sendInfoForPiano() //서버에게 URL 정보 보내기 [피아노 악보 채보 버튼 이벤트]
    {
        if (!isPost)
        {
            Debug.Log(info.text);
            StartCoroutine(PostReq(info.text, 1));
        }
        else
        {
            Debug.Log("사전 Post 작업 진행 중");
        }
    }

    IEnumerator PostReq(string postData, int type)
    {
        isPost = true;
        if (type == 0)
        {
            url += "music"; // 일반 음원 악보 채보 데이터 보냄
        }

        if (type == 1)
        {
            url += "piano"; // 피아노 음원 악보 채보 데이터 보냄
        }
        Debug.Log(url);
        //Json 데이터 준비
        string json = "{\"link\":\"" + postData + "\"}";
        
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //요청 보내기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
        url = "http://localhost:80/";
        isPost = false;
    }
}
