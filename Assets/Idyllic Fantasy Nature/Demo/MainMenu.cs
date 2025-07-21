using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main Scene"); // غيّر الاسم حسب اسم الـ Scene اللي فيها اللعبه
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed"); // يشتغل بس في الـ Build
    }
}

