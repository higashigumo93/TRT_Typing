using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class returnTitleButton : MonoBehaviour
{   // 始まった時に実行する関数	
    void Start () {
        // ボタンが押された時、ReturnTitle関数を実行 
        gameObject.GetComponent<Button>().onClick.AddListener(ReturnTitle);
}

// ReturnTitle関数 
void ReturnTitle() {
    // GameSceneをロード 
    SceneManager.LoadScene("TitleScene"); }
}