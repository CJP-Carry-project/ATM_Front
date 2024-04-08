using UnityEngine;
using UnityEngine.SceneManagement;

public class CamHover : MonoBehaviour
{
    public Camera main;
    private bool isActive = false;
    private float activeDistance = 10f;
    public GameObject interactiveUI;

    void Start()
    {
        main = GetComponent<Camera>();
        interactiveUI.SetActive(isActive);
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
            }else if (target.CompareTag("MusicSheet"))
            {
                isActive = (distance <= activeDistance);
                type = 2;
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
        }
    }
}