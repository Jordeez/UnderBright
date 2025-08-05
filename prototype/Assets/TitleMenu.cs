using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        // Loads the scene with build index 1
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        // Quits the application
        Debug.Log("Exit button pressed"); // Works in editor
        Application.Quit();
    }
}
