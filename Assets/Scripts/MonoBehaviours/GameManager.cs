using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    BattleDataDefinition _currentBattelDefinition;

    void OnBattleInitiated(BattleDataDefinition battleDataDefinition)
    {
        _currentBattelDefinition = battleDataDefinition;

        StartCoroutine(LoadBattle());
    }

    IEnumerator LoadBattle()
    {
        yield return FindObjectOfType<UIRoomsCanvas>().FadeIn();

        // var asyncOperation = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
        // asyncOperation.completed += OnBattleSceneLoadOperationCompleted;

        // RoomsSceneManager.Instance.DisableSceneObjects();
    }

    void OnBattleSceneLoadOperationCompleted(AsyncOperation asyncOperation)
    {
        // initiate battle controller with battle definition
    }

    void OnBattleEnded()
    {
        _currentBattelDefinition = null;
        RoomsSceneManager.Instance.ActivateScene();
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        EnvironmentEvents.BattleInitiated += OnBattleInitiated;    
        // BattleEvents.BattleEnded += OnBattleEnded;    
    }

    void OnDestroy()
    {
        EnvironmentEvents.BattleInitiated -= OnBattleInitiated;
        // BattleEvents.BattleEnded -= OnBattleEnded;    
    }
}
