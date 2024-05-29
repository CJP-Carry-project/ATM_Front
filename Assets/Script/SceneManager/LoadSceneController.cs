using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneController : MonoBehaviour
{
    public static string nextScene;

    [SerializeField] private Image progressLoad;
    public static void LoadScene(string scene)
    {
        nextScene = scene;
        SceneManager.LoadScene("LoadingScene");
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);//비동기 방식
        op.allowSceneActivation = false; //allowSceneActivation의 기능 => 완료 시 자동으로 넘어감 [true인 경우]

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null; //유니티 엔진에 제어권을 넘겨준다.
            if (op.progress < 0.9)
            {
                progressLoad.fillAmount = op.progress;
            }
            else
            {
                timer = Time.unscaledDeltaTime;
                progressLoad.fillAmount = Mathf.Lerp(0.9f, 3f, timer); // 3초에 걸쳐서 나머지 채우게끔
                if (progressLoad.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break; //코루틴 종료
                }
            }
        }
    }
}
