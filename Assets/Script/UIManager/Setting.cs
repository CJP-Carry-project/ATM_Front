using System.Collections;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour, HttpRequest
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject player;
    public void TryLogOut()
    {
        SceneManager.LoadScene("Login");
    }
    public void TrySignOut()
    {
        StartCoroutine(PostReq("https://202.31.202.9:80/leave", "leave"));
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
            
            // 인증서 검증을 무시하기 위해 CertificateHandler 설정
            webRequest.certificateHandler = new BypassCertificate();
            
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
                    Debug.Log("받은 정보:" + webRequest.downloadHandler.text);
                    JObject rec = JObject.Parse(webRequest.downloadHandler.text);
                    string result = (string)rec["message"];
                    Debug.Log(result);
                    SceneManager.LoadScene("Login");
                }
                else
                {
                    Debug.Log("비정상적으로 실패");
                }
            }
        }
    }
    public void ReturnToPage()
    {
        PlayerMove playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        this.GameObject().SetActive(false);
        playerMove.moveSpeed = 10f;
    }
}
