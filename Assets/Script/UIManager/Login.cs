using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour, HttpRequest
{
    private bool isLogin = false;
    
    public void TryLogin() //서버에게 로그인 요청하기
    {
        if (!isLogin) //로그인이 되어있지않다면
        {
            Debug.Log("Button Click");
            // StartCoroutine(PostReq("http://localhost:80/kakaoLogin", "로그인 시도"));
            //성공했다고 가정
            Debug.Log("성공했다고 가정했음 코드에서 주석 확인 바람");
            SceneManager.LoadScene("MusicRoom");
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
                    isLogin = true;
                    Debug.Log("정상적으로 로그인 완료");
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
