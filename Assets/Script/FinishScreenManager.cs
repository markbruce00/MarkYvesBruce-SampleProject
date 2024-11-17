using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FinishScreenManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private TextMeshProUGUI winLoseLabelText;
    [SerializeField] private TextMeshProUGUI finishMovesText;
    [SerializeField] private Button restartButton;

    [Header("Animation")]
    [SerializeField] private UIAnimator winAnimation;
    [SerializeField] private UIAnimator loseAnimation;

    [Header("Gradient Text Colors")]
    [SerializeField] private Color winStartColor;
    [SerializeField] private Color winEndColor;
    [SerializeField] private Color loseStartColor;
    [SerializeField] private Color loseEndColor;

    public UnityEvent OnWin;
    public UnityEvent OnLose;
    private void Start()
    {
        finishScreen.SetActive(false);// set finish screen disabled at start

        // Add a listener to the restart button
        restartButton.onClick.AddListener(RestartGame);
    }

    // Method to show the finish screen with the moves count
    public void ShowFinishScreen(int movesCount, bool isWin)
    {
        finishScreen.SetActive(true);
        if (isWin)
        {
            winAnimation.PlayAnimations();
            winLoseLabelText.text = "WIN";
            setWinLoseTextGradient(winStartColor,winEndColor);
            finishMovesText.enabled = true;
            finishMovesText.text = $"You finished with {movesCount} moves!";
            OnWin?.Invoke();
        }
        else {
            loseAnimation.PlayAnimations();
            winLoseLabelText.text = "GAMEOVER";
            setWinLoseTextGradient(loseStartColor, loseEndColor);
            finishMovesText.enabled = false;
            OnLose?.Invoke();
        }
    }

    private void setWinLoseTextGradient(Color startColor, Color endColor) {
        VertexGradient verticalGradient = new VertexGradient(startColor, startColor, endColor, endColor);
        winLoseLabelText.colorGradient = verticalGradient;
    }

    // Method to restart the game
    private void RestartGame()
    {
        finishScreen.SetActive(false);
        GameManager.instance.RestartGame();
    }
}
