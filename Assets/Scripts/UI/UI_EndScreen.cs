using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_EndScreen : MonoBehaviour
{
    public void Retry() => SceneManager.LoadScene("LevelOne");

    public void BackToMenu() => SceneManager.LoadScene("MainMenu");
}