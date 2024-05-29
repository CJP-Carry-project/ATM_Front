using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendURLInfo : MonoBehaviour, HttpRequest
{
    [SerializeField] private TMP_InputField info;
    [SerializeField] private TMP_Text youtube_info;
    [SerializeField] private Image thumnail_img;
    [SerializeField] private TMP_InputField title_info;
    [SerializeField] private GameObject successAudio;
    private string save_title = "";
    private string save_img = "";
    private GameObject child;
    private bool isPost = false;

    private void Awake()
    {
        child = transform.Find("YoutubeInfoLayer").gameObject;
    }

    public void SendURL()
    {
        StartCoroutine(PostURLInfoReq("http://202.31.202.9:80/music", info.text));
    }

    public void SendInfoForMusic() //서버에게 URL 정보 보내기 [음원 악보 채보 버튼 이벤트]
    {
        if (!isPost)
        {
            Debug.Log(save_title);
            StartCoroutine(PostReq("http://202.31.202.9:80/save", save_title));
        }
        else
        {
            Debug.Log("사전 Post 작업 진행 중");
        }
    }

    public void SendInfoForPiano() //서버에게 URL 정보 보내기 [피아노 악보 채보 버튼 이벤트]
    {
        if (!isPost)
        {
            Debug.Log(save_title);
            StartCoroutine(PostReq("http://202.31.202.9:80/piano", save_title));
        }
        else
        {
            Debug.Log("사전 Post 작업 진행 중");
        }
    }
    //음악 정보 보내기
    public IEnumerator PostReq(string url, string data)
    {
        isPost = true;
        JObject req = new JObject();
        // JSON 데이터 준비
        req["title_info"] = data;
        req["id"] = MyIDInfo.myId; //ID도 같이 보냄
        req["thumbnail"] = save_img;
        req["youtube_url"] = info.text;
        req["response"] = "yes";
        string json = req.ToString();
        Debug.Log(json);
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
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
                    Debug.Log("여기까지 정상 성공");
                    JObject rec = JObject.Parse(webRequest.downloadHandler.text);
                    string result = (string)rec["message"];
                    Debug.Log(result);
                    successAudio.GetComponent<AudioSource>().Play();
                }
                else
                {
                    Debug.Log("수신 실패");
                }
            }

            isPost = false;
        }
    }
    //URL 정보 보내기
    public IEnumerator PostURLInfoReq(string url, string data)
    {
        //Json 데이터 준비
        string json = "{\"youtube_url\":\"" + data + "\"}";
        
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST")) 
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
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
                Debug.Log("데이터 전달 완료");

                if (webRequest.responseCode == 200)
                {
                    Debug.Log("요청 받았음");
                    JObject res = JObject.Parse(webRequest.downloadHandler.text);
                    Debug.Log(res.ToString());
                    string author = (string)res["author"];
                    string img = (string)res["thumbnail"];
                    string title = (string)res["title"];
                    youtube_info.text = "작가: "+author + "\n"+"\n" + "제목: "+ title;
                    ChangeImg(img);
                    child.SetActive(true);
                    Debug.Log("정상 response 수신 완료");
                }
                else
                {
                    Debug.Log(webRequest.error);
                }
            }
        }
    }
    //이미지 변환
    public void ChangeImg(string img_url)
    {
        save_img = img_url;
        StartCoroutine(GetTexture(img_url));
    }
    //코루틴을 사용해서 WWW객체를 통해 URL에 접근 -> 이미지 불러옴 + 텍스처 변환 및 Sprite 변환 + 매칭
    public IEnumerator GetTexture(string url)
    {
        // UnityWebRequest를 사용하여 이미지 가져오기
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                // 가져온 이미지를 텍스처로 변환하여 렌더러에 적용
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                thumnail_img.sprite = sprite;
            } else {
                Debug.LogError("Failed to load image: " + www.error);
            }
        }
    }
    //확인 버튼 이벤트
    public void OnCheck()
    {
        save_title = title_info.text;
        child.SetActive(false);
    }
}