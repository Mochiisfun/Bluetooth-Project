using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void OpenDate()
    {
        // Make sure the game isn't paused.
        Time.timeScale = 1f;

        // Load the dating scene.
        SceneManager.LoadScene("date");
    }

    public void OpenShop()
    {
        // Make sure the game isn't paused.
        Time.timeScale = 1f;

        // Load the shopping scene.
        SceneManager.LoadScene("shop");
    }

    public void OpenMainHub()
    {
        // Make sure the game isn't paused.
        Time.timeScale = 1f;

        // Load the MainHub scene.
        SceneManager.LoadScene("mainHub");
    }
}