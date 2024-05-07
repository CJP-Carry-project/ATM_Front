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
    [SerializeField] private Texture2D endImg;
    private List<Sprite> sheetList = new List<Sprite>();
    private int leftIndex = 0;
    private int rightIndex = 1;

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
                            File.WriteAllBytes("C:\\test\\test.img", binaryData);
                            sheetList.Add(LoadImageFromBytes(binaryData));
                        }
                    }

                    ShowSheet(leftIndex, rightIndex);
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

    // 페이지 1일 때 0,1
    // 페이지 2일 때 2,3
    // 페이지 3일 때 4,5
    public void NextToPage()
    {
        int length = sheetList.Count;
        leftIndex += 2;
        rightIndex += 2;
        if (length % 2 == 0 && leftIndex > length - 1) //짝수이면서 왼쪽이 OutOfBoundary인 경우
        {
            leftIndex = length - 2;
            rightIndex = length - 1;
        }

        if (length % 2 != 0 && rightIndex > length - 1) //홀수이면서 오른쪽이 OutOfBoundary인 경우
        {
            leftIndex = length - 1;
            rightIndex = -1;
        }

        ShowSheet(leftIndex, rightIndex);
    }

    public void PrevToPage()
    {
        if (rightIndex == -1) //End를 찍은 경우
        {
            rightIndex = sheetList.Count;
        }
        leftIndex -= 2;
        rightIndex -= 2;
        if (leftIndex < 0)
        {
            leftIndex = 0;
            rightIndex = 1;
        }

        ShowSheet(leftIndex, rightIndex);
    }

    public void ShowSheet(int left, int right)
    {
        leftSheet.sprite = sheetList[left];
        if (right == -1)
        {
            rightSheet.sprite = Sprite.Create(endImg, new Rect(0, 0, endImg.width, endImg.height), Vector2.one * 0.5f);
        }
        else
        {
            rightSheet.sprite = sheetList[right];
        }
    }
}