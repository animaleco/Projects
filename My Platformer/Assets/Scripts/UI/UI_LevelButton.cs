using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNumberText;

    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI fruitText;


    private int levelIndex;
    private string sceneName;


    public void SetupButton(int newLevelIndex)
    {
        this.levelIndex = newLevelIndex;

        levelNumberText.text = "Level " + levelIndex;
        sceneName = "Level_" + levelIndex;

        bestTimeText.text = TimerInfoText();
        fruitText.text = FruitsInfoText();
    }
    public void LoadLevel()
    {
        AudioManager.instance.PlaySFX(4);

        int difficultyIndex = ((int)DifficultyManager.instance.difficulty);
        PlayerPrefs.SetInt("GameDifficulty", difficultyIndex);
        SceneManager.LoadScene(sceneName);
    }

    private string FruitsInfoText()
    {
        int totalFruits = PlayerPrefs.GetInt("Level" + levelIndex + "TotalFruits", 0);
        string totalFruitsText =  totalFruits == 0 ? "?" : totalFruits.ToString();


       int fruitsCollected = PlayerPrefs.GetInt("Level" + levelIndex + "FruitsCollected");

        return "Fruits: " + fruitsCollected + " / " + totalFruitsText;
    }

    private string TimerInfoText()
    {
       float timerValue = PlayerPrefs.GetFloat("Level" + levelIndex + "BestTime", 99);

        return "Best Time: " + timerValue.ToString("00");
    }
}
