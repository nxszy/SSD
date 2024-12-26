using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI; 

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && !pauseUI.activeSelf){
            pauseUI.SetActive(true);
        }
    }

    public void OnResumePress() {
        pauseUI.SetActive(false);
    }

    public void onExitPress() {
        SceneManager.LoadScene("MainMenu");
    }
}
