using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class MusicSheet : MonoBehaviour, HttpRequest
{
    [SerializeField] private Image leftSheet; //왼쪽
    [SerializeField] private Image rightSheet; //오른쪽
    private List<Texture2D> sheetList;
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
                    //TODO 악보의 개수와 악보 리스트를 받아야함.
                    //TODO 악보의 개수만큼 반복해서 UTF 인코딩을 실행해서 바이트로 나눈다 -> 바이트 배열로 나온 것들을 전부 텍스처로 변환하여 리스트에 하나씩 넣어준다.
                    //TODO 넣어준 리스트의 0~n번째까지 있을텐데 page처리하도록 한다.
                }
                else
                {
                    Debug.Log("수신 실패");
                }
            }
        }
    }

    public Texture2D LoadImageFromBytes(byte[] imageData)
    {
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
            return texture;
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
        if (sheetList.Count < page_info)//최대 리스트 개수
        {
            page_info = sheetList.Count;
        }
    }

    public void PrevToPage()
    {
        page_info -= 1;
        if (page_info < 0) //최소 1페이지
        {
            page_info = 1;
        }
    }
}