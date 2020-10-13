using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Player player;

    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public bool stageFailed = false;
    public GameObject gameOverUI;
    public GameObject retryUI;

    [Space]
    public string levelNoPrefix = "LEVEL ";
    public Text txtLevelNo;

    [Space]
    public GameObject sawPath_Sprite;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverUI.SetActive(true);
    }

    public void StageFailed()
    {
        stageFailed = true;
        retryUI.SetActive(true);
    }

    public void Restart()
    {
        gameOver = false;
        gameOverUI.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Retry()
    {
        stageFailed = false;
        retryUI.SetActive(false);

        LevelManager.instance.RetryStage();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateLevelUI(int levelNo, int stageNo)
    {
        txtLevelNo.text = levelNoPrefix + levelNo + " : " + stageNo;
    }
}