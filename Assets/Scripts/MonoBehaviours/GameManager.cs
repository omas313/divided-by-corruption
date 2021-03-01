using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    BattleDataDefinition _currentBattelDefinition;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadEndScene()
    {
        SceneManager.LoadScene("End");
    }

    public void LoadRooms()
    {
        SceneManager.LoadScene("Rooms");
    }

    void OnBattleInitiated(BattleDataDefinition battleDataDefinition)
    {
        _currentBattelDefinition = battleDataDefinition;

        StartCoroutine(LoadBattle());
    }

    IEnumerator LoadBattle()
    {
        yield return FindObjectOfType<UIRoomsCanvas>().FadeIn();

        var asyncOperation = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
        asyncOperation.completed += OnBattleSceneLoadOperationCompleted;

        RoomsSceneManager.Instance.DisableSceneObjects();
    }

    void OnBattleSceneLoadOperationCompleted(AsyncOperation asyncOperation)
    {
        FindObjectOfType<BattleController>().InitBattleAndStart(_currentBattelDefinition.PlayerParty, _currentBattelDefinition.Enemies);
    }

    void OnBattleEnded(bool hasWon)
    {
        if (hasWon)
        {
            _currentBattelDefinition = null;
            var asyncOperation = SceneManager.UnloadSceneAsync("Battle");
            asyncOperation.completed += op => RoomsSceneManager.Instance.ActivateScene();
        }
        else
            ReloadBattle();
    }

    void ReloadBattle()
    {
        var asyncOperation = SceneManager.UnloadSceneAsync("Battle");
        asyncOperation.completed += ReloadBattleScene;
    }

    void ReloadBattleScene(AsyncOperation asyncOperation)
    {
        var loadOperation = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
        loadOperation.completed += OnBattleSceneLoadOperationCompleted;
    }

    private void OnPlayerOpenedMainDoor()
    {
        LoadEndScene();
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        EnvironmentEvents.BattleInitiated += OnBattleInitiated;    
        BattleEvents.BattleEnded += OnBattleEnded;    
        EnvironmentEvents.PlayerOpenedMainDoor += OnPlayerOpenedMainDoor;    
        
    }

    void OnDestroy()
    {
        EnvironmentEvents.BattleInitiated -= OnBattleInitiated;
        BattleEvents.BattleEnded -= OnBattleEnded;    
        EnvironmentEvents.PlayerOpenedMainDoor -= OnPlayerOpenedMainDoor;    
    }
}
