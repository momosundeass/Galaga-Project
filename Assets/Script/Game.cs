using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {
    // Singleton for Game
    public static Game i;

    #region Game Parameter
    public static Setting parameter = new Setting();
    #endregion

    [Header("Prefab")]
    public GameObject PlayerBulletPrefab;
    public GameObject EnemyBlue;
    public GameObject EnemyGreen;
    public GameObject EnemyBeam;
    public GameObject EnemyRed;
    public GameObject EnemyRocket;
    public GameObject Explosion;
    public GameObject CurveAnchon;

    [Header("Scene Object")]
    public GameObject State;
    public EnemyController EnemyControl;
    public UIController UIControl;
    public Player CurrentPlayer;
    public Transform PlayerBulletTranform;

    #region Game Status
    [HideInInspector]
    public int spareLive;
    // Current playing score
    private int score = 0;
    // Current state of the game
    private bool isGameRunning = true;
    #endregion

    private void Awake() {
        i = this;
        spareLive = parameter.StartingLive - 1;
    }

    #region Function
    public void AddScore(int adder) {
        score += adder;
        UIControl.UpdateScore(score);
    }

    /// <summary>
    /// Remove live from Game Data
    /// If there have no live to remove will return [false].
    /// </summary>
    /// <returns> If there have no live to remove will return [false]. </returns>
    public bool RemoveLive() {
        var canRemove = true;
        if (spareLive > 0) {
            spareLive--;
        } else {
            canRemove = false;
            GameOver();
        }
        UIControl.UpdateLiveShip(spareLive);
        return canRemove;
    }

    public void MissionComplete() {
        EnemyControl.GameComplete();
        CurrentPlayer.GameComplete();
        UIControl.GameComplete();
    }

    public void GameOver() {
        EnemyControl.GameOver();
        CurrentPlayer.GameOver();
        UIControl.GameOver();
        isGameRunning = false;
    }

    public void GameReset() {
        // load scene to reset
        SceneManager.LoadScene(1);
    }
    #endregion

    #region Keyboard Control
    private void Update() {
        // Force Mission Complete
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isGameRunning) {
            GameReset();
        }
        // Debug : Force win game.
        if (Input.GetKeyDown(KeyCode.F10)) {
            foreach (IDamageAble child in EnemyControl.transform.GetComponentsInChildren<IDamageAble>()) {
                child.Damage();
            }
        }
    }
    #endregion
}
