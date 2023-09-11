using Firebase;
using UnityEngine;

namespace Script.Utility
{
    public class FirebaseTrigger : MonoBehaviour
    {
        private FirebaseApp _app;
        private void Awake()
        {
#if UNITY_ANDROID
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
            }
            else
            {
                UnityEngine.Debug.LogError(
                    System.String.Format("Could not resolve all firebase{0}", 
                        dependencyStatus));
            }
        }); 
#endif
        }

    }
}
