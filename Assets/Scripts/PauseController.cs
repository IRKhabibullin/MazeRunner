using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool gameIsPaused;
    public Animator pauseAnimator;

    public GameObject pauseMenu;
    public GameObject inGameUI;

    public void PauseGame()
    {
        inGameUI.SetActive(false);
        pauseMenu.SetActive(true);
        pauseAnimator.SetBool("GameIsPaused", true);
        gameIsPaused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        pauseAnimator.SetBool("GameIsPaused", false);
    }
    public void OnUnpauseFinished()
    {
        inGameUI.SetActive(true);
        pauseMenu.SetActive(false);
        gameIsPaused = false;
        Time.timeScale = 1;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
