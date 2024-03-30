using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Sendinfo : MonoBehaviour
{
    [SerializeField] private TMP_InputField information;

    private bool isLogin = false;
    private string url = "http://localhost:80/logininfo";

    public void SendInfo() //서버에게 텍스트 정보 보내기 [버튼 이벤트]
    {
        Debug.Log(information.text);
        StartCoroutine(PostReq(information.text));
    }

    public void GetLoginResult() //서버로부터 인증 결과를 받아옴
    {
        
    }

    IEnumerator PostReq(string postData)
    {
        // JSON 데이터 준비
        string json = "{\"data\":\"" + postData + "\"}";

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
    }
}