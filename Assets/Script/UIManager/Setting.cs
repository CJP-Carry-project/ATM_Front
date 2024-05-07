using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour, HttpRequest
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject player;
    public void TryLogOut()
    {
        StartCoroutine(PostReq("http://202.31.202.9:80/leave", "leave"));
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

    void OnEnable()
    {
        _slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }
    private void OnSliderValueChanged()
    {
        // Slider의 현재 값은 slider.value를 통해 얻을 수 있습니다.
        float sliderValue = _slider.value;

        camera.GetComponent<CamRotate>().rotSpeed = sliderValue;
        player.GetComponent<PlayerRotate>().rotSpeed = sliderValue;
    }
}
