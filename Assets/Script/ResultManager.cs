using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ResultManager : MonoBehaviour
{
    // TypingManegerから各変数の読み込み
    int RariCount = TypingManager.RariCount;
    int LuCount = TypingManager.LuCount;
    double RariLogTime = TypingManager.RariLogTime;
    double LuLogTime = TypingManager.LuLogTime;

    // 画面にあるテキストオブジェクトの読み込み
    [SerializeField] TextMeshProUGUI RariMessageText;
    [SerializeField] TextMeshProUGUI LuMessageText;
    [SerializeField] TextMeshProUGUI RariLogTimeText;
    [SerializeField] TextMeshProUGUI LuLogTimeText;
    [SerializeField] TextMeshProUGUI ResultMessageConstText;
    [SerializeField] TextMeshProUGUI ResultMessageText;
    [SerializeField] GameObject ReturnToTitleButton;
    [SerializeField] GameObject ReplayButton;
    [SerializeField] float RouletteSpeed;

    // その他の変数宣言
    double LuPerRari;
    System.Random rnd = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(RariCount);
        Debug.Log(LuCount);
        Debug.Log(RariLogTime);
        Debug.Log(LuLogTime);

        RariMessageText.text = "「武田羅梨沙多胡」\n平均入力時間";
        LuMessageText.text = "「ルゥティン」\n平均入力時間";


    
        if (RariCount != 0){
            LuPerRari = RariLogTime/LuLogTime;
        }
        else{
            LuPerRari = 1;
        }

        StartCoroutine(LogTimeAnim());


    }

    IEnumerator LogTimeAnim()
    {

        RariLogTimeText.gameObject.SetActive(true);
        for (int i = 0; i < (int)(1/RouletteSpeed); i++)
        {
            double RandomLogTime = rnd.NextDouble() * 9.99;
            RariLogTimeText.text = RandomLogTime.ToString("F2");
            Debug.Log(RandomLogTime);
            yield return new WaitForSeconds(RouletteSpeed);
        }
        RariLogTimeText.text = RariLogTime.ToString("F2");

        yield return new WaitForSeconds((float)1);

        LuLogTimeText.gameObject.SetActive(true);
        for (int i = 0; i < (int)(0.8/RouletteSpeed); i++)
        {
            double RandomLogTime = rnd.NextDouble() * 9.99;
            LuLogTimeText.text = RandomLogTime.ToString("F2");
            yield return new WaitForSeconds(RouletteSpeed);
        }
        LuLogTimeText.text = LuLogTime.ToString("F2");
        
        yield return new WaitForSeconds((float)1);

        ResultMessageConstText.gameObject.SetActive(true);

        yield return new WaitForSeconds((float)1.0);

        ResultMessageText.gameObject.SetActive(true);
        for (int i = 0; i < (int)(0.8 / RouletteSpeed); i++)
        {
            double RandomLogTime = rnd.NextDouble() * 9.99;
            ResultMessageText.text = "1武田羅梨沙多胡 = " + RandomLogTime.ToString("F2") + "ルゥティン";
            yield return new WaitForSeconds(RouletteSpeed);
        }
        ResultMessageText.text = "1武田羅梨沙多胡 = " + LuPerRari.ToString("F2") + "ルゥティン";

        yield return new WaitForSeconds((float)1.3);

        ReturnToTitleButton.SetActive(true);
        ReplayButton.SetActive(true);


    }

}
