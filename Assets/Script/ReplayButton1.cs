using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class replayButton : MonoBehaviour
{   // 始まった時に実行する関数	
    void Start()
    {
        // ボタンが押された時、ReturnTitle関数を実行 
        gameObject.GetComponent<Button>().onClick.AddListener(Replay);
    }

    // Replay関数 
    void Replay()
    {
        // GameSceneをロード 
        SceneManager.LoadScene("GameScene");
    }
}