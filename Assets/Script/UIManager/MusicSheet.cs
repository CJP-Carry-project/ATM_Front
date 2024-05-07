using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class MusicSheet : MonoBehaviour, HttpRequest
{
    [SerializeField] private Image leftSheet; //왼쪽
    [SerializeField] private Image rightSheet; //오른쪽
    private List<Sprite> sheetList = new List<Sprite>();
    private int page_info = 1;

    void Start()
    {
        StartCoroutine(PostReq("http://202.31.202.9:80/sheet_recent", "musicSheet"));
    }

    public IEnumerator PostReq(string url, string data)
    {
        JObject req = new JObject();
        req["message"] = data;
        string json = req.ToString();
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("요청");
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
                    JObject res = JObject.Parse(webRequest.downloadHandler.text);
                    Debug.Log(res.ToString());
                    JArray sheets = (JArray)res["sheets"];
                    if (sheets != null)
                    {
                        foreach (var jToken in sheets)
                        {
                            string token = jToken.Value<string>();
                            JObject imgData = JObject.Parse(token);
                            JObject sheetImg = (JObject)imgData["sheet_img"];
                            byte[] binaryData = (byte[])sheetImg["$binary"];
                            
                            Debug.Log("Convert Bytes: " + binaryData);
                            File.WriteAllBytes("C:\\test\\test.img",binaryData);
                            sheetList.Add(LoadImageFromBytes(binaryData));
                        }
                    }
                    leftSheet.sprite = sheetList[0];
                    rightSheet.sprite = sheetList[1];
                }
                else
                {
                    Debug.Log("수신 실패");
                }
            }
        }
    }

    public Sprite LoadImageFromBytes(byte[] imageData)
    {
        Debug.Log("Load Image ..");
        // 이미지 데이터가 null이거나 비어있으면 null 반환
        if (imageData == null || imageData.Length == 0)
        {
            Debug.LogError("이미지 데이터가 유효하지 않습니다.");
            return null;
        }

        try
        {
            // 이미지 데이터를 Texture2D로 변환
            Texture2D texture = new Texture2D(600, 1000);
            texture.LoadImage(imageData); // 바이트 배열을 이미지로 로드
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            return sprite;
        }
        catch (Exception ex)
        {
            Debug.LogError("이미지 로드 중 오류 발생: " + ex.Message);
            return null;
        }
    }

    public void NextToPage()
    {
        page_info += 1;
        
    }

    public void PrevToPage()
    {
        page_info -= 1;
    }
}