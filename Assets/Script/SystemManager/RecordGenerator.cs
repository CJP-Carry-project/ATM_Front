using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RecordGenerator : MonoBehaviour, HttpRequest
{
    [SerializeField] private GameObject recordPrefab;
    [SerializeField] private GameObject recordPos;

    private void Start()
    {
        // StartCoroutine(PostReq("http://202.31.202.9/music_sheet_list", "MusicList"));
        GenerateRecord(5);
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

            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("정상적으로 정보 송신");

                if (webRequest.responseCode == 200)
                {
                    Debug.Log("정상적으로 정보 수신");
                }
            }
        }
    }

    private void GenerateRecord(int count)
    {
        Vector3 rotationAngles = new Vector3(90, 0 ,0);
        for (int i = 0; i < count; i++)
        {
            GameObject record = Instantiate(recordPrefab, recordPos.transform.position, Quaternion.Euler(rotationAngles));
            Debug.Log(recordPos.transform.position);
            record.SetActive(true);
            recordPos.transform.position =
                new Vector3(recordPos.transform.position.x - 5, recordPos.transform.position.y, recordPos.transform.position.z);
        }
    }
}
