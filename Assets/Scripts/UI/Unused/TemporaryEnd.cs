using UnityEngine;
using UnityEngine.SceneManagement;

public class TemporaryEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) => SceneManager.LoadScene("EndScreen");
}