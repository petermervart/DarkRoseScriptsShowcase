using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level Manager", menuName = "Level Manager")]
public class LevelManager : ScriptableObject
{
    public List<GameScene> levels = new List<GameScene>();
    public GameScene mainMenu;
    public GameScene gameOverScene;
    public GameScene survivedScene;

    public int CurrentLevelIndex = 1;

    public void LoadLevelWithIndex(int index)
    {
        if (index <= levels.Count)
        {
            SceneManager.LoadSceneAsync("Level" + index.ToString());
        }
        else CurrentLevelIndex = 1;
    }

    public void NextLevel()
    {
        CurrentLevelIndex++;
        LoadLevelWithIndex(CurrentLevelIndex);
    }

    public void RestartLevel()
    {
        LoadLevelWithIndex(CurrentLevelIndex);
    }

    public void NewGame()
    {
        LoadLevelWithIndex(1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenu.sceneName);
    }

    public void LoadSurvived()
    {
        SceneManager.LoadSceneAsync(survivedScene.sceneName);
    }

    public void LoadGameOver()
    {
        SceneManager.LoadSceneAsync(gameOverScene.sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}