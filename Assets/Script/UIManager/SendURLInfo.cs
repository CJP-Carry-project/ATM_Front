using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class SendURLInfo : MonoBehaviour, HttpRequest
{
    [SerializeField] private TMP_InputField info;
    private bool isPost = false;

    public void sendInfoForMusic() //서버에게 URL 정보 보내기 [음원 악보 채보 버튼 이벤트]
    {
        if (!isPost)
        {
            Debug.Log(info.text);
            StartCoroutine(PostReq("http://localhost:80/music",info.text));
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
            StartCoroutine(PostReq("http://localhost:80/piano",info.text));
        }
        else
        {
            Debug.Log("사전 Post 작업 진행 중");
        }
    }

    public IEnumerator PostReq(string url, string data)
    {
        isPost = true;
        
        Debug.Log(url);
        //Json 데이터 준비
        string json = "{\"link\":\"" + data + "\"}";
        
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
                Debug.Log("데이터 전달 완료");

                if (webRequest.responseCode == 200)
                {
                    Debug.Log("악보 정상적으로 수신");
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
        }
        isPost = false;
    }
}
