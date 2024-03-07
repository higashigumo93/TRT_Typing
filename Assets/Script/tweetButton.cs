using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tweet_Button : MonoBehaviour
{
    public void onClick()
    {
        double RariLogTime = TypingManager.RariLogTime;

        Application.OpenURL("https://twitter.com/intent/tweet?text=%E3%81%82%E3%81%AA%E3%81%9F%E3%81%AE%E3%82%BF%E3%82%A4%E3%83%94%E3%83%B3%E3%82%B0%E3%81%AF1%E3%82%89%E3%82%8A+%EF%BC%9D+"
            + RariLogTime.ToString("F2")
            + "%E3%83%AB%E3%82%A5%E3%81%A7%E3%81%97%E3%81%9F%EF%BC%81&url=https://higashigumo93.github.io/TRT_Typing/&hashtags=%E8%A8%80%E3%81%86%E3%81%BB%E3%81%A9%E9%95%B7%E3%81%84%E3%81%8B%E3%82%BF%E3%82%A4%E3%83%94%E3%83%B3%E3%82%B0");//""の中には開きたいWebページのURLを入力します
    }
}
