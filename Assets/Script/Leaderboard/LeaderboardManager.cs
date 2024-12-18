using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject leaderboardEntryPrefab; // Prefab for each leaderboard entry
    [SerializeField] private Transform contentTransform; // Parent object to hold entries
    [SerializeField] private GameObject fetchingText;

    private List<GameObject> entryList = new List<GameObject>();

    private void Start()
    {
        PoolingManager.Instance.CreatePool("leaderboardEntry", leaderboardEntryPrefab, 12, contentTransform);
    }
    /// <summary>
    /// Populate leaderboard UI with data from Firebase.
    /// </summary>
    private IEnumerator PopulateLeaderboard()
    {
        // Clear existing entries
        foreach (GameObject entry in entryList)
        {
            PoolingManager.Instance.ReturnToPool("leaderboardEntry",entry);
        }
        entryList.Clear();

        // Retrieve leaderboard data from FirebaseManager
        FirebaseManager.Instance.LoadLeaderboard(10);
        fetchingText.SetActive(true);
        yield return new WaitForSeconds(1);
        fetchingText.SetActive(false);
        Dictionary<string, UserSessionData> leaderboardEntries = FirebaseManager.Instance.GetLeaderboard();

        // Populate the UI with the leaderboard entries
        int rank = 1;
        foreach (var entry in leaderboardEntries)
        {
            // Instantiate a new leaderboard entry from the prefab
            GameObject newEntry = PoolingManager.Instance.SpawnFromPool("leaderboardEntry", Vector3.zero, Quaternion.Euler(0, 0, 0));
            entryList.Add(newEntry);

            // Set up the leaderboard entry using the prefab's script
            LeaderboardEntry entryScript = newEntry.GetComponent<LeaderboardEntry>();
            if (entryScript != null)
            {
                entryScript.SetupEntry(
                    rank: rank,
                    userId: entry.Key,
                    difficulty: entry.Value.difficulty,
                    finishedTime: entry.Value.finishedTime,
                    moves: entry.Value.moves
                );
            }

            // Adjust the sibling index to match the rank
            newEntry.transform.SetSiblingIndex(rank - 1);

            yield return null;
            rank++;
        }
        yield return null;
    }
    [Button]
    public void PopulateLeaderboardData() {
        StartCoroutine(PopulateLeaderboard());
    }


}
