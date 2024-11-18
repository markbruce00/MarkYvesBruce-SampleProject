using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    
    // Time in seconds for the countdown to start
    [SerializeField] private float countdownTime = 3f;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Title("Events")]
    [FoldoutGroup("Events")] public UnityEvent OnCountDownStart;
    [FoldoutGroup("Events")] public UnityEvent OnCountDown;
    [FoldoutGroup("Events")] public UnityEvent OnCountDownEnd;

    public void Start()
    {
        OnCountDownStart?.Invoke();
        countdownTime = 3f; // Always start at 3
        StartCoroutine(StartCountdown());
    }

    // Coroutine to manage the countdown
    private IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            // Update the countdown text
            countdownText.text = Mathf.Ceil(countdownTime).ToString();
            OnCountDown?.Invoke();
            // Wait for 1 second before updating again
            yield return new WaitForSeconds(1f);

            // Decrease countdown time by 1 second
            countdownTime--;
        }

        OnCountDownEnd?.Invoke();
    }
}
