using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour, IGameState {
    [Header("UI Prefab")]
    [SerializeField]
    private GameObject PlayerShipImagePrefab;

    [Header("Scene Object")]
    [SerializeField]
    private Text score;
    [SerializeField]
    private Transform liveSlot;
    [SerializeField]
    private Text GameReportText;



    private void Start() {
        UpdateLiveShip(Game.i.spareLive);
        StartCoroutine(StartGameSeq());
    }
    IEnumerator StartGameSeq() {
        UpdateGameReportText("3");
        yield return new WaitForSeconds(1);
        UpdateGameReportText("2");
        yield return new WaitForSeconds(1);
        UpdateGameReportText("1");
        yield return new WaitForSeconds(1);
        UpdateGameReportText();
    }

    #region UI Update Function
    public void UpdateScore(int newScore) {
        score.text = newScore.ToString();
    }

    public void UpdateLiveShip(int newSpareLive) {
        if (newSpareLive < liveSlot.childCount) {
            var dif =  liveSlot.childCount - (newSpareLive);
            for (int i = 0; i < dif; i++) {
                Destroy(liveSlot.GetChild(0).gameObject);
            }
        } else {
            var dif = (newSpareLive) - liveSlot.childCount;
            for (int i = 0; i < dif; i++) {
                Instantiate(PlayerShipImagePrefab, liveSlot);
            }
        }
    }

    /// <summary>
    /// Sent on screen Text
    /// Empty parameter = clear Text
    /// </summary>
    /// <param name="newText"></param>
    public void UpdateGameReportText(string newText = "") {
        GameReportText.text = newText.ToString();
    }

    public void ShowKeyHelpingAfterGameEnd() {
        transform.Find("KeyHelp").gameObject.SetActive(true);
    }


    #endregion

    #region interface
    public void GameOver() {
        UpdateGameReportText("GAME OVER!");
        ShowKeyHelpingAfterGameEnd();
    }

    public void GameComplete() {
        StartCoroutine(ResetRoundSeq());
    }

    IEnumerator ResetRoundSeq() {
        UpdateGameReportText("Mission Complete!");
        yield return new WaitForSeconds(1);
        UpdateGameReportText("Another Round");
        yield return new WaitForSeconds(1.5f);
        UpdateGameReportText("GO");
        yield return new WaitForSeconds(1.5f);
        UpdateGameReportText();
    }
    #endregion
}
