using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class Login : MonoBehaviour, HttpRequest
{
    private bool isLogin = false;
    
    public void TryLogin() //서버에게 로그인 요청하기
    {
        if (!isLogin) //로그인이 되어있지않다면
        {
            Debug.Log("Button Click");
            StartCoroutine(PostReq("http://202.31.202.9:80/authorize", "로그인 시도"));
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
                    Debug.Log("정보 수신 완료");
                    //URL 열기 -> 사용자가 카카오 인증하도록
                    // JSON 데이터 파싱
                    JObject jsonObject = JObject.Parse(json);
                    string openUrl = (string)jsonObject[webRequest.downloadHandler.text];
                    Debug.Log(openUrl);
                    Application.OpenURL(openUrl);
                    
                    //인증될 때까지 대기
                    if (isLogin == true)
                    {
                        SceneManager.LoadScene("MusicRoom");
                    }
                }
                else
                {
                    Debug.Log("비정상적으로 로그인을 실패했습니다.");
                }
            }
        }
    }
}
