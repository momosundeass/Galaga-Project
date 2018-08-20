using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    #region Slider
    [SerializeField]
    private Slider EnemySpeed_Slider;
    [SerializeField]
    private Slider EnemyBullet_Slider;
    [SerializeField]
    private Slider StartLive_Slider;
    #endregion



    private void Start() {
        EnemySpeed_Slider.value = Game.parameter.EnemySpeed;
        UpdateText(EnemySpeed_Slider);
        EnemyBullet_Slider.value = Game.parameter.EnemyBulletCount;
        UpdateText(EnemyBullet_Slider);
        StartLive_Slider.value = Game.parameter.StartingLive;
        UpdateText(StartLive_Slider);
    }

    void UpdateText(Slider slider) {
        slider.transform.parent.Find("Value").GetComponent<Text>().text = slider.value.ToString();
    }


    #region Slider Function
    public void EnemySpeedConfig() {
        Game.parameter.EnemySpeed = EnemySpeed_Slider.value;
        UpdateText(EnemySpeed_Slider);
    }
    public void EnemyBulletConfig() {
        Game.parameter.EnemyBulletCount = Mathf.RoundToInt(EnemyBullet_Slider.value);
        UpdateText(EnemyBullet_Slider);
    }
    public void LiveCountConfig() {
        Game.parameter.StartingLive = Mathf.RoundToInt(StartLive_Slider.value);
        UpdateText(StartLive_Slider);
    }
    #endregion

    #region Button Function
    public void Play() {
        SceneManager.LoadScene(1);
    }
    public void Exit() {
        Application.Quit();
    }
    #endregion
}
