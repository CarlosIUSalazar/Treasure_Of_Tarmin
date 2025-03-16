using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button veryHardButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button easyButton;

    void Start()
    {
        //titleText.text = "Treasure of Tarmin";
        veryHardButton.onClick.AddListener(() => StartGame(DifficultyLevel.VeryHard));
        hardButton.onClick.AddListener(() => StartGame(DifficultyLevel.Hard));
        normalButton.onClick.AddListener(() => StartGame(DifficultyLevel.Normal));
        easyButton.onClick.AddListener(() => StartGame(DifficultyLevel.Easy));
    }

    void StartGame(DifficultyLevel difficulty)
    {
        // Store the chosen difficulty in a static class or PlayerPrefs
        GameSettings.SelectedDifficulty = difficulty;
        SceneManager.LoadScene("GameScene"); // Load GameScene
    }
}

