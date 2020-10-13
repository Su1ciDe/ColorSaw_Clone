using System.Collections;
using UnityEngine;

[System.Serializable]
public class Level
{
    public Stages[] stages;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Level[] levels;
    [HideInInspector] public int curLevelNo;
    [HideInInspector] public int curStageNo;
    private GameObject curStage;

    [HideInInspector] public bool isAdvancing;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        NextLevel();
    }

    public void NextLevel()
    {
        curLevelNo++;
        curStageNo = 0;
        NextStage();
        GameManager.instance.UpdateLevelUI(curLevelNo, curStageNo);
    }

    public void NextStage()
    {
        curStageNo++;
        if (curStage)
            Destroy(curStage);

        GameManager.instance.UpdateLevelUI(curLevelNo, curStageNo);

        StartCoroutine(SpawnNextStage());
    }

    private IEnumerator SpawnNextStage()
    {
        yield return new WaitForSeconds(1f);
        curStage = Instantiate(levels[curLevelNo - 1].stages[curStageNo - 1].stagePrefab);
        isAdvancing = false;
    }

    public void RetryStage()
    {
        if (curStage)
            Destroy(curStage);

        StartCoroutine(SpawnNextStage());
    }

    public void Advance()
    {
        if (isAdvancing)
            return;

        isAdvancing = true;

        if (curStageNo < levels[curLevelNo - 1].stages.Length)
        {
            NextStage();
        }
        else if (curLevelNo < levels.Length)
        {
            NextLevel();
        }
        else if (curLevelNo >= levels.Length)
        {
            GameManager.instance.GameOver();
        }
    }
}