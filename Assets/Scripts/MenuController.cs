using UnityEngine;

public class MenuController : MonoBehaviour
{
    public MazeController maze;
    public PlayerController player;

    public static bool gameIsPaused;
    public Animator menuAnimator;

    public GameObject pauseMenu;
    public GameObject inGameUI;

    public void PauseGame()
    {
        inGameUI.SetActive(false);
        pauseMenu.SetActive(true);
        menuAnimator.SetBool("GameIsPaused", true);
        gameIsPaused = true;
        Time.timeScale = 0;
    }

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

    public void RebuildMaze()
    {
        player.ResetPlayer();
        StartCoroutine(maze.RebuildMaze());
        menuAnimator.SetTrigger("StartNewGame");
    }

    public void StartGame()
    {
        player.StartMoving();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
