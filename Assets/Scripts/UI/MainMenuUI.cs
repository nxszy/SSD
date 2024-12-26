using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void onStartPress() {
        SceneManager.LoadScene("SimulationScene");
    }

    public void onExitPress() {
        Application.Quit();
    }
}
