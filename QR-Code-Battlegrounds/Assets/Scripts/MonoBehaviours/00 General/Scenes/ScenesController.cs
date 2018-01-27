using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum ScenesEnum
{
    mainmenu,
    battlescene,
    map,
    scanner,
    backpack,
    store,
    information,
    login,
    registration,
    qrcodecustom
}

public class ScenesController : MonoBehaviour
{
    private static string previousSceneName = "MainMenu"; // MainMenu is default
    private readonly string[] sceneNames = new string[] { "MainMenu", "BattleScene", "Map", "QR Scanner", "Backpack", "Store", "Information", "LoginScene", "RegistrationScene", "QRCodeCustom" };

    // Called from inspector
    public void LoadScene(GetScenesEnumFromInspector getScenesEnumFromInspector)
    {
        string name = sceneNames[(int)getScenesEnumFromInspector.scenesEnum];
        StartCoroutine(LoadingSceneModeSingle(name));
    }

    // Called in files
    public void LoadScene(ScenesEnum scenesEnum)
    {
        string name = sceneNames[(int)scenesEnum];
        StartCoroutine(LoadingSceneModeSingle(name));
    }

    private IEnumerator LoadingSceneModeSingle(string sceneName)
    {
        previousSceneName = SceneManager.GetActiveScene().name;
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    public void LoadPreviousScene()
    {
        StartCoroutine(LoadingSceneModeSingle(previousSceneName));
    }
}
