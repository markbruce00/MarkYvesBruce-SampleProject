using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseDebug : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Checking Firebase dependencies...");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase dependencies check failed: " + task.Exception);
                return;
            }

            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies available, initializing app...");
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase initialized with name: " + app.Name);
                Debug.Log("Firebase options: " + app.Options.ToString());
            }
            else
            {
                Debug.LogError($"Firebase dependencies are not available: {dependencyStatus}");
            }
        });
    }
}
