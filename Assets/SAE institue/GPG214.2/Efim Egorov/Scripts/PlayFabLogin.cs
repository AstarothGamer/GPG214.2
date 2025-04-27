using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    public static Action OnLoginSuccess;
    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "67487";
        }
        var request = new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, 
            result =>
            {
                Debug.Log("PlayFab login successful!");
                OnLoginSuccess?.Invoke();
            },   
            error => Debug.Log("PlayFab login failed: " + error.GenerateErrorReport()));
    }
}
