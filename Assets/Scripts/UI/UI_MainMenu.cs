using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("LevelOne");

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}