using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool gameIsPaused;
    public Animator pauseAnimator;

    public void PauseGame()
    {
        pauseAnimator.SetBool("GameIsPaused", true);
        gameIsPaused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        pauseAnimator.SetBool("GameIsPaused", false);
        gameIsPaused = false;
        Time.timeScale = 1;
    }
}
