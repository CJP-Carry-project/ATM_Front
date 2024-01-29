using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static GameManager _instance;
    // 다른 스크립트에서 GameManager에 접근할 수 있는 프로퍼티
    public static GameManager Instance
    {
        get
        {
            // 인스턴스가 없으면 생성
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                // Scene에 GameManager가 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    // GameManager의 기타 변수 및 메서드들을 여기에 추가하세요.

    private void Awake()
    {
        // 인스턴스가 이미 존재하면 새로 생성된 인스턴스를 파괴
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        // 인스턴스를 유지하도록 함
        DontDestroyOnLoad(this.gameObject);
    }

    // 게임 초기화 또는 기타 GameManager의 기능을 구현하는 메서드들을 추가하세요.
}