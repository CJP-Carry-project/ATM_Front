using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecordList : MonoBehaviour, HttpRequest
{
    [SerializeField] private TMP_Text music_name;
    [SerializeField] Image thumbnail;
    private JArray lists = null;
    private int count = 0;
    public int idx = 0;
    private void Start()
    {
        StartCoroutine(PostReq("http://202.31.202.9/sheet_all", "MusicList"));
    }

    public IEnumerator PostReq(string url, string data)
    {
        JObject req = new JObject();
        req["message"] = data;
        req["id"] = MyIDInfo.myId;
        string json = req.ToString();

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
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
                if (webRequest.responseCode == 200)
                {
                    
                    JObject recordsJson = JObject.Parse(webRequest.downloadHandler.text);
                    count = (int)recordsJson["length"];
                    lists = (JArray)recordsJson["all_sheet_info"];
                    
                    if (count != 0)
                    {
                        showMusicList();
                    }
                }
                else
                {
                    Debug.Log("수신 오류");
                }
            }
        }
    }
    private void ChangeImg(string img_url)
    {
        StartCoroutine(GetTexture(img_url));
    }
    private IEnumerator GetTexture(string url)
    {
        // UnityWebRequest를 사용하여 이미지 가져오기
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) {
            // 가져온 이미지를 텍스처로 변환하여 렌더러에 적용
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            thumbnail.sprite = sprite;
        } else {
            Debug.LogError("Failed to load image: " + www.error);
        }
    }

    public void goNext()
    {
        idx++;
        if (idx >= count)
        {
            idx = 0;
        }

        showMusicList();
    }

    public void goPrev()
    {
        idx--;
        if (idx < 0)
        {
            idx = count - 1;
        }

        showMusicList();
    }

    private void showMusicList()
    {
        Record.recordList = lists;
        Record.idx = idx;
        ChangeImg((string)lists[idx]["music_img"]);
        music_name.text = (string)lists[idx]["music_name"];
    }
}
