using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    private DatabaseReference _databaseReference;

    private Dictionary<string, UserSessionData> leaderboardList;
    private void Awake()
    {
        // Singleton pattern to ensure only one instance of FirebaseManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeFirebase();
    }

    /// <summary>
    /// Initialize Firebase and get the database reference.
    /// </summary>
    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Initialized successfully.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    /// <summary>
    /// Save user session data to Firebase.
    /// </summary>
    public void SaveUserData(string userName, string difficulty, string finishedTime, int score)
    {
        if (_databaseReference == null)
        {
            Debug.LogError("Firebase not initialized!");
            return;
        }

        string userId = userName.ToLower();
        UserSessionData sessionData = new UserSessionData
        {
            difficulty = difficulty,
            moves = score,
            finishedTime = finishedTime,
            timestamp = GetCurrentTimestamp()
        };

        string json = JsonUtility.ToJson(sessionData);

        // Save data under each user ID
        _databaseReference.Child("users").Child(userId).Push().SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Data saved for user: {userName}");
            }
            else
            {
                Debug.LogError("Failed to save data: " + task.Exception);
            }
        });
    }

    /// <summary>
    /// Load leaderboard data for a specific difficulty and return the top players based on low moves.
    /// </summary>
    /// <param name="topN">Number of top players to retrieve</param>
    public void LoadLeaderboard(int topN)
    {
        if (_databaseReference == null)
        {
            Debug.LogError("Firebase not initialized!");
            return;
        }

        _databaseReference.Child("users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving leaderboard: " + task.Exception);
                return;
            }

            if (task.IsCompleted && task.Result.Exists)
            {
                List<(string userId, UserSessionData sessionData)> leaderboard = new List<(string, UserSessionData)>();
                HashSet<string> addedUserIds = new HashSet<string>(); // Track added user IDs

                // Traverse all users and their sessions
                foreach (DataSnapshot userSnapshot in task.Result.Children)
                {
                    string userId = userSnapshot.Key; // Store user ID

                    // If this user has already been added, skip this user
                    if (addedUserIds.Contains(userId))
                    {
                        continue;
                    }

                    // Traverse each user's session data
                    foreach (DataSnapshot sessionSnapshot in userSnapshot.Children)
                    {
                        UserSessionData sessionData = JsonUtility.FromJson<UserSessionData>(sessionSnapshot.GetRawJsonValue());

                        // Only add valid session data to the leaderboard
                        if (sessionData != null)
                        {
                            leaderboard.Add((userId, sessionData));
                            addedUserIds.Add(userId); // Mark this user as added
                            break; // Stop once we've added this user (to avoid duplicate user entries)
                        }
                    }
                }

                // Sort the leaderboard:
                // 1. Highest difficulty first
                // 2. If tied, fastest finishedTime
                // 3. If still tied, fewest moves
                leaderboard.Sort((a, b) =>
                {
                    // Compare by difficulty (highest first)
                    int difficultyComparison = b.sessionData.difficulty.CompareTo(a.sessionData.difficulty);
                    if (difficultyComparison != 0)
                    {
                        return difficultyComparison;
                    }

                    // Compare by finishedTime (fastest first)
                    float timeA = ParseFinishedTime(a.sessionData.finishedTime);
                    float timeB = ParseFinishedTime(b.sessionData.finishedTime);
                    int timeComparison = timeA.CompareTo(timeB);
                    if (timeComparison != 0)
                    {
                        return timeComparison;
                    }

                    // If difficulty and finishedTime are the same, compare by moves (fewest moves first)
                    return a.sessionData.moves.CompareTo(b.sessionData.moves);
                });

                Debug.Log($"Top {topN} Players:");

                leaderboardList = new Dictionary<string, UserSessionData>();
                // Print the top N players
                for (int i = 0; i < Mathf.Min(topN, leaderboard.Count); i++)
                {
                    var entry = leaderboard[i];
                    leaderboardList.Add(entry.userId, entry.sessionData);
                    // Debug.Log($"{i + 1}. UserID: {entry.userId} - Difficulty: {entry.sessionData.difficulty} - Finished Time: {entry.sessionData.finishedTime} - Moves: {entry.sessionData.moves}");
                }
            }
            else
            {
                Debug.Log("No leaderboard data found.");
            }
        });
    }






    // Helper method to parse finishedTime string (e.g., "10.05" -> 10.05f)
    private float ParseFinishedTime(string finishedTime)
    {
        if (float.TryParse(finishedTime, out float result))
        {
            return result;
        }
        return float.MaxValue; // If parsing fails, return a high value (invalid time)
    }

    public Dictionary<string, UserSessionData> GetLeaderboard() => leaderboardList;


    /// <summary>
    /// Utility to get the current timestamp.
    /// </summary>
    private long GetCurrentTimestamp()
    {
        return new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeSeconds();
    }
}

[System.Serializable]
public class UserSessionData
{
    public string difficulty;
    public int moves;
    public string finishedTime;
    public long timestamp;
}
