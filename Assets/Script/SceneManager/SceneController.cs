using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void goToMusicRoom()
    {
        Debug.Log("메인 화면으로 이동합니다");
        SceneManager.LoadScene("MusicRoom");
    }

    public void goToMusicSheet()
    {
        Debug.Log("악보 UI로 이동");
        SceneManager.LoadScene("MusicSheetUI");
    }

    public void goToSettingUI()
    {
        Debug.Log("설정 화면으로 이동");
    }

    public void goToInputYoutubeLink()
    {
        Debug.Log("음원 링크 입력 화면으로 이동");
        SceneManager.LoadScene("InputYoutubeLink");
    }
}
