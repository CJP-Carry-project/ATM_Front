using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class Login : MonoBehaviour, HttpRequest
{
    private bool isLogin = false;
    private bool isClick = false;

    public void ExitProgram()
    {
        Debug.Log("프로그램을 종료합니다.");
        Application.Quit();
    }
    public void TryLogin() //서버에게 로그인 요청하기
    {
        if (!isLogin) //로그인이 되어있지않다면
        {
            if (!isClick)
            {
                isClick = true;
                StartCoroutine(PostReq("http://202.31.202.9:80/authorize", "login"));
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
        // JSON 데이터 준비
        string json = "{\"msg\":\"" + data + "\"}";

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
                Debug.Log("정상적으로 정보 보냄");

                if (webRequest.responseCode == 200)
                {
                    //URL 열기 -> 사용자가 카카오 인증하도록
                    // JSON 데이터 파싱
                    JObject url_json = JObject.Parse(webRequest.downloadHandler.text);
                    string openUrl = (string)url_json["open_url"];
                    Debug.Log(openUrl);
                    Application.OpenURL(openUrl);
                }

                if (webRequest.responseCode == 200)
                {
                    SceneManager.LoadScene("MusicRoom");
                }
                else
                {
                    Debug.Log("비정상적으로 로그인을 실패했습니다.");
                }
            }
        }
    }
}
