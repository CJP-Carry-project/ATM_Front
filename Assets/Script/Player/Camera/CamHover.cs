using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class CamHover : MonoBehaviour, HttpRequest
{
    public Camera main;
    private bool isActive = false;
    private float activeDistance = 10f;
    public GameObject interactiveUI;

    private AudioClip audioClip; // 받은 WAV 오디오 데이터를 저장할 AudioClip
    private AudioSource audioSource;

    void Start()
    {
        main = GetComponent<Camera>();
        interactiveUI.SetActive(isActive);
        audioSource = transform.GetComponent<AudioSource>();
    }

    void Update()
    {
        // 카메라 정 중앙 Ray 설정
        Ray ray = main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hitInfo;
        int type = 0; //0: 기본 1: 악보 채보 2: 악보 결과
        // 레이가 어떤 물체에 닿았는지 확인
        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject target = hitInfo.collider.gameObject;
            float distance = Vector3.Distance(transform.position, target.transform.position);

            // 'Music' 또는 'MusicSheet' 태그를 가진 오브젝트와의 거리를 확인하여 UI 활성화 여부 설정
            if (target.CompareTag("Music"))
            {
                isActive = (distance <= activeDistance);
                type = 1;
            }
            else if (target.CompareTag("MusicSheet"))
            {
                isActive = (distance <= activeDistance);
                type = 2;
            }
            else if (target.CompareTag("PlayMidi"))
            {
                isActive = (distance <= activeDistance);
                type = 3;
            }
            else
            {
                isActive = false;
            }
        }
        else
        {
            isActive = false;
        }

        // UI 활성화 상태에 따라 UI를 활성화 또는 비활성화
        interactiveUI.SetActive(isActive);

        // 'E' 키를 눌렀을 때 다음 화면으로 이동
        if (isActive && Input.GetKeyDown(KeyCode.E))
        {
            if (type == 1) //악보 채보로
            {
                SceneManager.LoadScene("InputYoutubeLink");
            }

            if (type == 2) //악보 결과로
            {
                SceneManager.LoadScene("MusicSheetUI");
            }

            if (type == 3) //midi
            {
                StartCoroutine(PostReq("http://202.31.202.9:80/midi_recent", "midi plz"));
            }
        }
    }

    public IEnumerator PostReq(string url, string data)
    {
        Debug.Log("Request");
        // JSON 데이터 준비
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
                    string byteCode = (string)res["midi_wav"];
                    Debug.Log(byteCode);
                    audioClip = LoadWavFromBase64String(byteCode);
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                else
                {
                    Debug.Log("수신 실패");
                }
            }
        }
    }
    
    public static AudioClip LoadWavFromBase64String(string base64WavData)
    {
        // Base64 디코딩을 사용하여 문자열을 바이트 배열로 변환
        byte[] wavBytes = Convert.FromBase64String(base64WavData);

        // 헤더 정보 파싱
        int sampleRate = BitConverter.ToInt32(wavBytes, 24);
        int numChannels = BitConverter.ToInt16(wavBytes, 22);
        int headerSize = 44;

        // 데이터 부분만 추출
        byte[] audioData = new byte[wavBytes.Length - headerSize];
        Array.Copy(wavBytes, headerSize, audioData, 0, audioData.Length);

        // AudioClip 생성
        AudioClip audioClip = AudioClip.Create("AudioClip", audioData.Length / 2, numChannels, sampleRate, false);

        // 데이터 삽입
        audioClip.SetData(Convert16BitByteArrayToFloatArray(audioData), 0);

        return audioClip;
    }

    // 16비트 바이트 배열을 float 배열로 변환하는 메서드
    private static float[] Convert16BitByteArrayToFloatArray(byte[] source)
    {
        float[] converted = new float[source.Length / 2];
        for (int i = 0; i < converted.Length; i++)
        {
            // 16비트 바이트 배열에서 short으로 변환 후 -1.0f ~ 1.0f 사이의 범위로 정규화
            short value = (short)((source[i * 2 + 1] << 8) | source[i * 2]);
            converted[i] = value / 32768.0f;
        }

        return converted;
    }
}