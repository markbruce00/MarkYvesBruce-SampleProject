using UnityEngine;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI userIdText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI finishedTimeText;
    [SerializeField] private TextMeshProUGUI movesText;

    /// <summary>
    /// Set up the leaderboard entry with provided data.
    /// </summary>
    public void SetupEntry(int rank, string userId, string difficulty, string finishedTime, int moves)
    {
        if (rankText != null) rankText.text = rank.ToString();
        if (userIdText != null) userIdText.text = userId;
        if (difficultyText != null) difficultyText.text = difficulty;
        if (finishedTimeText != null) finishedTimeText.text =finishedTime;
        if (movesText != null) movesText.text = moves.ToString();
        
    }

}