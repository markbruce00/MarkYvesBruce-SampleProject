using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Title("UI Elements")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;

    [Title("Card Database")]
    [SerializeField] private CardDatabase cardDatabase;

    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>(); // Track flipped cards

    private int difficultyLevel;
    private int movesCount;
    private float timer;
    private float maxTime;
    private bool gameStarted = false;
    private bool isFlippingCards = false; // Prevent flipping new cards while others are flipping

    private int totalPairs;
    private int matchedPairs; // Track the number of matched pairs

    [Title("Events")]
    [FoldoutGroup("Events")] public UnityEvent OnRestart;
    [FoldoutGroup("Events")] public UnityEvent OnPairMatched;

    [Title("Manager reference")]
    [SerializeField] private FinishScreenManager finishScreenManager;


    Coroutine coroutine;
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        CardEvent.onCardFlip += CardClicked;
    }

    private void OnDisable()
    {
        CardEvent.onCardFlip -= CardClicked;
    }

    private void Start()
    {
        difficultyLevel = PlayerPrefs.GetInt("DifficultyLevel", 1); // Default to level 1 if not set
        coroutine = null;

        PoolingManager.Instance.CreatePool("Cards",cardPrefab,30,cardParent);
        GenerateCards();
    }

    private void GenerateCards()
    {
        foreach (Card child in cards)
        {
            PoolingManager.Instance.ReturnToPool("Cards",child.gameObject);
        }
        cards.Clear();

        int pairs = difficultyLevel + 1;
        totalPairs = pairs; // Set total pairs for later comparison
        int totalCards = pairs * 2;
        timer = pairs * 5; // Adjust timer based on pairs
        maxTime = timer;
        timerText.text = $"{Mathf.Max(0, Mathf.RoundToInt(timer))}s";

        List<CardData> selectedCards = cardDatabase.GetRandomCards(pairs);

        List<CardData> cardsToSpawn = new List<CardData>();
        foreach (CardData cardData in selectedCards)
        {
            cardsToSpawn.Add(cardData);
            cardsToSpawn.Add(cardData); // Duplicate cards for pairs
        }

        cardsToSpawn = cardsToSpawn.OrderBy(x => Random.value).ToList();

        foreach (CardData cardData in cardsToSpawn)
        {
            GameObject cardObj = PoolingManager.Instance.SpawnFromPool("Cards",Vector3.zero,Quaternion.Euler(0,0,0));
            cardObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
            Card cardComponent = cardObj.GetComponent<Card>();
            cardComponent.ResetCard();
            cardComponent.Initialize(cardData);
            cards.Add(cardComponent);
        }

        
        movesCount = 0;
        matchedPairs = 0; // Reset matched pairs

        UpdateMovesUI();
       
    }

    private void UpdateMovesUI()
    {
        movesText.text = movesCount.ToString();
    }

    private IEnumerator TimerCoroutine()
    {
        gameStarted = true;// Start Game

        while (timer > 0 && matchedPairs < totalPairs)
        {
            timer -= Time.deltaTime;
            timerText.text = $"{Mathf.Max(0, Mathf.RoundToInt(timer))}s";
            yield return null; // Wait until next frame
        }

        // If all pairs are matched, stop the timer and show finish screen
        if (isAllPairsMatched())
        {
            StopCoroutine(coroutine);
        }
        GameOver();
    }

    public void CardClicked(Card clickedCard)
    {
        if (flippedCards.Count >= 2 || !gameStarted || isFlippingCards) return; // Ignore if already flipping or game not started

        movesCount++;
        UpdateMovesUI();
        flippedCards.Add(clickedCard);

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatchCoroutine());
        }
    }

    private IEnumerator CheckMatchCoroutine()
    {
        isFlippingCards = true;  // Start blocking further card flips

        yield return new WaitForSeconds(0.3f); // Allow flip animation to complete

        // Check if the cards match
        if (flippedCards[0].CardID == flippedCards[1].CardID)
        {
            flippedCards[0].DisableCard();
            flippedCards[1].DisableCard();
            matchedPairs++; // Increase matched pairs
            flippedCards[0].PlayMatchedAnimation();
            flippedCards[1].PlayMatchedAnimation();

            OnPairMatched?.Invoke();
        }
        else
        {
            flippedCards[0].FlipCard();
            flippedCards[1].FlipCard();
        }

        // Clear the flipped cards list and unlock card flips
        flippedCards.Clear();
        isFlippingCards = false;  // Allow new card flips

        // If all pairs are matched, stop the timer and show finish screen
        if (isAllPairsMatched())
        {
            //timerText.text = "Time: 0s"; // Ensure time shows 0
            GameOver();
        }
    }

    private void GameOver()
    {
        gameStarted = false;
        if (finishScreenManager != null)
        {
            finishScreenManager.ShowFinishScreen(movesCount, isAllPairsMatched());
            Invoke("SaveData", 0.1f);
        }

    }

    private void SaveData() {
        if (isAllPairsMatched()) FirebaseManager.Instance.SaveUserData(PlayerPrefs.GetString("userID"), difficultyLevel.ToString(), (maxTime - timer).ToString("F2"), movesCount);
        CancelInvoke("SaveData");
    }
    private bool isAllPairsMatched() { return matchedPairs >= totalPairs; }


    public void RestartGame()
    {
        StopCoroutine(coroutine);
        gameStarted = false;
        flippedCards = new List<Card>();
        isFlippingCards = false;
        GenerateCards();
        OnRestart?.Invoke();
    }
    public void ReturntoMainMenu () { SceneManager.LoadScene("Main Menu  Scene"); }
    public void StartGame() => coroutine = StartCoroutine(TimerCoroutine());
    public bool IsFlippingCards => isFlippingCards;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space)) FirebaseManager.Instance.SaveUserData(PlayerPrefs.GetString("Name",PlayerPrefs.GetString("userID"),Random.Range(2,8).ToString(),Random.Range(1,20).ToString(),Random.Range(1,5));
    //    if (Input.GetKeyDown(KeyCode.LeftShift)) FirebaseManager.Instance.LoadLeaderboard(10);
    //}
}
