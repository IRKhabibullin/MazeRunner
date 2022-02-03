using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool gameIsPaused;

    [SerializeField] private MazeController maze;
    [SerializeField] private PlayerController player;
    [SerializeField] private Animator menuAnimator;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject inGameUI;

    public void PauseGame()
    {
        inGameUI.SetActive(false);
        pauseMenu.SetActive(true);
        menuAnimator.SetBool("GameIsPaused", true);
        gameIsPaused = true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// Here we only start unpause animation. Everything else runs in OnUnpauseFinished after animation end
    /// </summary>
    public void UnpauseGame()
    {
        menuAnimator.SetBool("GameIsPaused", false);
    }

    public void OnUnpauseFinished()
    {
        inGameUI.SetActive(true);
        pauseMenu.SetActive(false);
        gameIsPaused = false;
        Time.timeScale = 1;
    }

    public void OnVictory()
    {
        menuAnimator.SetTrigger("Victory");
    }

    /// <summary>
    /// Called after victory animation finished
    /// </summary>
    public void StartNewGame()
    {
        player.ResetPlayer();
        StartCoroutine(maze.RebuildMaze());
        menuAnimator.SetTrigger("StartNewGame");
    }

    /// <summary>
    /// Called after rebuilding maze and black screen fade out finished
    /// </summary>
    public void StartGame()
    {
        player.StartMoving();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
