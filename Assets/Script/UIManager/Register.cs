using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Register : MonoBehaviour, HttpRequest
{
    [SerializeField] private TMP_InputField id;
    [SerializeField] private TMP_InputField pw;
    [SerializeField] private TMP_InputField phoneNum;
    [SerializeField] private GameObject failIcon;
    private float delay = 5f;
    public void TryRegister()
    {
        if(!string.IsNullOrWhiteSpace(id.text) && !string.IsNullOrWhiteSpace(pw.text) && !string.IsNullOrWhiteSpace(phoneNum.text)){
            JObject json = new JObject();
            json["id"] = id.text;
            json["pw"] = pw.text;
            json["phone"] = phoneNum.text;
            StartCoroutine(PostReq("http://202.31.202.9:80/register", json.ToString()));
        }
        else
        {
            SceneManager.LoadScene("init");
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
                        SceneManager.LoadScene("Login");
                    }
                    else
                    {
                        yield return StartCoroutine(failIconDisableAfterSeconds());
                        id.text = "";
                        pw.text = "";
                        phoneNum.text = "";
                        SceneManager.LoadScene("init"); //실패 시 초기 화면으로
                    }
                }
            }
        }
    }

    private IEnumerator failIconDisableAfterSeconds()
    {
        failIcon.SetActive(true);
        yield return new WaitForSeconds(delay);
        failIcon.SetActive(false);
    }
}
