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

    public void goInitUI()
    {
        Debug.Log("처음 화면으로 돌아갑니다.");
        SceneManager.LoadScene("init");
    }

    public void goToLogin()
    {
        Debug.Log("로그인으로 이동");
        SceneManager.LoadScene("Login");
    }

    public void goToRegister()
    {
        Debug.Log("회원가입으로 이동");
        SceneManager.LoadScene("Register");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
