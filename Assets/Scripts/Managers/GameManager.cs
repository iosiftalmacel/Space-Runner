using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    [HideInInspector]
    public int lastScore;
    [HideInInspector]
    public int highestScore;
    public Transform initialSpawns;

    void Awake () {
        Instance = this;
    }

    void SpawnInitialPickups()
    {
        foreach (Transform item in initialSpawns)
        {
            Transform current = Instantiate(item);
            current.parent = SpawnManager.Instance.spawnsParent;
            current.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        SpawnManager.Instance.Toggle(true);
        SpawnManager.Instance.ClearSpawns();
        InputsManager.Instance.Toggle(true);
        PlayerManager.Instance.State = PlayerManager.PlayerState.Enter;
        PlayerManager.Instance.OverrideSpeed = 0;
        SpawnManager.Instance.OverrideExtraWindow = 0;
        PlayerManager.Instance.ClearPlayerPickups();
        RepositionManager.Instance.Reposition();
        UIManager.Instance.GoToPage("Page_InGame");
        SpawnInitialPickups();
    }

    public void StopGame()
    {
        SpawnManager.Instance.Toggle(true);
        InputsManager.Instance.Toggle(true);
        PlayerManager.Instance.State = PlayerManager.PlayerState.Idle;
        PlayerManager.Instance.Progress = 0;
    }

    internal void OnPlayerDestoyed()
    {
        lastScore = (int)PlayerManager.Instance.Progress;
        highestScore = PlayerPrefs.GetInt("highesScore", lastScore);
        highestScore = Mathf.Max(highestScore, lastScore);

        PlayerPrefs.SetInt("highesScore", highestScore);
        PlayerPrefs.Save();

        StopGame();
        UIManager.Instance.GoToPage("Page_Results");
    }
}
