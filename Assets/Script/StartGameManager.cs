using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI difficultyValue;
    [SerializeField] private Button leftDifBtn;
    [SerializeField] private Button rightDifBtn;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI errorText;

    private int difficultyIndex = 1;
    private const int MinDifficulty = 1;
    private const int MaxDifficulty = 7;

    [FoldoutGroup("Events")] public UnityEvent OnSceneStart;

    private void Start()
    {
        OnSceneStart?.Invoke();
    }
    private void OnEnable()
    {
        // Attach button click listeners
        leftDifBtn.onClick.AddListener(DecreaseDifficulty);
        rightDifBtn.onClick.AddListener(IncreaseDifficulty);

        // Initialize UI
        Initialize();
    }

    private void OnDisable()
    {
        // Clean up button click listeners to avoid memory leaks
        leftDifBtn.onClick.RemoveListener(DecreaseDifficulty);
        rightDifBtn.onClick.RemoveListener(IncreaseDifficulty);
    }

    private void Initialize()
    {
        // Set initial difficulty text
        UpdateDifficultyUIText();
    }

    /// <summary>
    /// Increase the difficulty index within bounds
    /// </summary>
    private void IncreaseDifficulty()
    {
        if (difficultyIndex < MaxDifficulty)
        {
            difficultyIndex++;
            UpdateDifficultyUIText();
        }
    }

    /// <summary>
    /// Decrease the difficulty index within bounds
    /// </summary>
    private void DecreaseDifficulty()
    {
        if (difficultyIndex > MinDifficulty)
        {
            difficultyIndex--;
            UpdateDifficultyUIText();
        }
    }

    /// <summary>
    /// Update the difficulty text based on the current index
    /// </summary>
    private void UpdateDifficultyUIText()
    {
        difficultyText.text = $"Difficulty: {difficultyIndex}";
        difficultyValue.text = difficultyIndex.ToString(); // Update additional text if needed
    }

    public void StartGame() {
        string playerName = nameInputField.text.Trim();

        // Validate player name input
        if (string.IsNullOrEmpty(playerName))
        {
            if (errorText != null)
            {
                errorText.gameObject.SetActive(true);
                errorText.text = "Please enter your name!";
                errorText.color = Color.red; // Optional: Change text color to red for visibility
            }
            Debug.LogWarning("Player name is required to start the game.");
            return;
        }

        // Save difficulty and player name using PlayerPrefs
        PlayerPrefs.SetInt("DifficultyLevel", difficultyIndex);
        PlayerPrefs.SetString("userID", playerName);

        // Optionally, you can also save other preferences or load a new scene
        Debug.Log($"Game started with Difficulty: {difficultyIndex}, Player Name: {playerName}");

        // Load the next scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Game Scene");
    }
}
