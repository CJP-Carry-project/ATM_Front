using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine.UIElements;

public class Login : MonoBehaviour, HttpRequest
{
    private bool isLogin = false;
    private bool isClick = false;
    [SerializeField] private TMP_InputField id;
    [SerializeField] private TMP_InputField pw;
    
    public void TryLogin() //서버에게 로그인 요청하기
    {
        if (!isLogin) //로그인이 되어있지않다면
        {
            if (!isClick)
            {
                JObject json = new JObject();
                json["id"] = id.text;
                json["pw"] = pw.text;
                MyIDInfo.myId = id.text;
                isClick = true;
                StartCoroutine(PostReq("http://202.31.202.9:80/login", json.ToString()));
                SceneManager.LoadScene("MusicRoom");
            }
            else
            {
                Debug.Log("현재 실행 중입니다.");
            }
        }
        else
        {
            Debug.Log("사용자가 이미 로그인 상태입니다.");
        }
    }

    public IEnumerator PostReq(string url, string data)
    {
        Debug.Log(data);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            // 인증서 검증을 무시하기 위해 CertificateHandler 설정
            webRequest.certificateHandler = new BypassCertificate();
            
            //요청 보내기
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
                    JObject login_json = JObject.Parse(webRequest.downloadHandler.text);
                    bool result = (bool)login_json["result"];
                    if (result)
                    {
                        LoadSceneController.LoadScene("MusicRoom");
                    }
                    else
                    {
                        SceneManager.LoadScene("init");
                    }
                }
            }
        }
        isClick = false;
    }
}
