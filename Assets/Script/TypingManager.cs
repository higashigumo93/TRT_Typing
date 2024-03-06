using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using System.Linq;
using System;

// 現状
// ・ひらがな入力に対応（基本的なタイピング方法(lyuやxyuなど)には対応できているはず）
// ・「！」入力にも対応

public class TypingManager : MonoBehaviour
{
    public GameObject StartAnim_Panel;
    public GameObject CharacterAndTable;

    // 問題管理関係の宣言

    int _qCount = 0; // 正解数のカウント
    public static int RariCount = 0;  // 入力回数カウント
    public static int LuCount = 0;

    private int MaxQuizNum = 10; // 「武田羅梨沙多胡」の入力回数の設定


    // 時間管理
    DateTime RariStart = new DateTime(); 
    DateTime LuStart = new DateTime();
    DateTime RariEnd = new DateTime();
    DateTime LuEnd = new DateTime();

    public static double RariLogTime = 0;   // らり入力時間集計
    public static double LuLogTime = 0;     // ルゥ入力時間集計
    

    // 残り問題数管理
    [SerializeField] TextMeshProUGUI RemainingNum_TMP;


    // 文字入力関係の宣言
    // 画面にあるテキストを持ってくる
    [SerializeField] Text fText; // ふりがな用のテキスト
    [SerializeField] Text qText; // 問題用のテキスト
    [SerializeField] Text aText; // 答え用のテキスト

    // テキストデータを読み込む
    [SerializeField] TextAsset _furigana;
    [SerializeField] TextAsset _question;

    // テキストデータを格納するためのリスト
    private List<string> f_List = new List<string>();
    private List<string> q_List = new List<string>();

    // 何番目か指定するためのstring
    private string _fString;
    private string _qString;
    private string _aString;

    // 何番目の問題か
    private int _qNum;

    // 問題の何文字めか
    private int _aNum;

    // 問題のリストを作成
    List<int> _qList = new List<int>();


    // あっているかどうかの判断
    bool isCorrect;
    bool isShift = false;

    // ChangeDictionaryの読み込みあたりよくわからない
    private ChangeDictionary cd;

    private List<string> _romSliceList = new List<string>();
    private List<int> _furiCountList = new List<int>(); //何文字目を入力しているか知るためのリスト
    private List<int> _romNumList = new List<int>(); //ひらがな1文字のローマ字何文字目を入力しているか知るためのリスト


    // ゲームを始めたときに1度だけ呼ばれる関数 // Start is called before the first frame update
    void Start()
    {
        // ChangeDictionaryの読み込みあたりよくわからない
        cd = GetComponent<ChangeDictionary>();

        // テキストデータの読み込み
        SetList();

        // 問題を生成（ランダムな配列を作成(0 or 1)）
        System.Random rnd = new System.Random();
        while (true)
        {
            _qList.Clear();

            for (int i = 0; i <10; i++)
            {
                _qList.Add(rnd.Next(0, 2));
            }
            // 0が4～6の範囲に収まるようにする
            if(_qList.Count(item => item ==0) >= (int)(MaxQuizNum*0.4) && _qList.Count(item => item == 0) <= (int)(MaxQuizNum * 0.6))
            {
                break;
            }
        }

        // 残り問題数を設定
        RemainingNum_TMP.text = MaxQuizNum.ToString();


        // StartAnim開始処理
        StartCoroutine(StartAnim());
    }

    // txtファイルからリスト形式で読み込み
    void SetList()
    {
        string[] _fArray = _furigana.text.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });    // 改行コード"\n"だけでSplitしようとしたら失敗
        f_List.AddRange(_fArray);

        string[] _qArray = _question.text.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
        q_List.AddRange(_qArray);

    }

    // Update is called once per frame
    void Update()
    {

        // 入力されたときに判断
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                return;
            }

            isCorrect = false;
            int furiCount = _furiCountList[_aNum]; //furiCount：何文字目のひらがなかを表す整数

            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    //Debug.Log(code);
                    break;
                }
            }


            // 完全にあっていたら正解
            if (Input.GetKeyDown(_aString[_aNum].ToString()) || GetKeyReturn() == _aString[_aNum].ToString())    //　n文字目を取り出すとcharになるから、stringに戻す
            {
                // trueにする
                isCorrect = true;

                // 正解
                Correct();

                // 最後の文字に正解したら
                //if (_aNum >= _aString.Length -1)
                if (_aNum >= _aString.Length)
                {
                    // 次の問題に変える
                    CorrectLastChar();
                }
            }
            else if (Input.GetKeyDown("n") && furiCount > 0 && _romSliceList[furiCount - 1] == "n") //前の文字がnで、かつ、nが入力されたとき
            {
                //nnにしたい
                _romSliceList[furiCount - 1] = "nn"; //例：["si","n","bu","n"]のfuriCount=0をrom=["shi"]に変える
                _aString = string.Join("", GetRomSliceListWithoutSkip());

                ReCreatList(_romSliceList); // "si"を"shi"に変換すると、何番目の文字かが変わってしまうため、再定義

                // trueにする
                isCorrect = true;

                // 正解
                Correct();

                // 最後の文字に正解したら
                //if (_aNum >= _aString.Length -1)
                if (_aNum >= _aString.Length)
                {
                    // 次の問題に変える
                    CorrectLastChar();
                }
            }
            else  //柔軟な入力があるかどうか
            {
                // いまどのふりがなを打たないといけないのかを取得する
                string currentFuri = _fString[furiCount].ToString();

                if (furiCount < _fString.Length - 1)
                {
                    // 2文字を考慮した候補検索「しゃ」
                    string addNextMoji = _fString[furiCount].ToString() + _fString[furiCount + 1].ToString();
                    CheckIrregularType(addNextMoji, furiCount, false);
                }

                if (!isCorrect)
                {
                    // 今まで通りの候補検索「し」「ゃ」
                    string moji = _fString[furiCount].ToString();
                    CheckIrregularType(moji, furiCount, true);
                }

            }


            // 正解じゃなかったら
            if (!isCorrect)
            {
                //失敗
                Miss();
            }
        }
        
        
    }

    // ノーマルじゃない入力を受け付けた場合の処理をする関数 
    void CheckIrregularType(string currentFuri, int furiCount, bool addSmallMoji)
    {
        if (cd.dic.ContainsKey(currentFuri))
        {
            List<string> stringList = cd.dic[currentFuri];    // stringListには"し"の非デフォルト入力である"ci"と"shi"が入っている
            //Debug.Log(string.Join(",", stringList));

            //stringList[0]
            for (int i = 0; i < stringList.Count; i++)
            {
                string rom = stringList[i];
                int romNum = _romNumList[_aNum];

                // 「ju」を「jixltu」と入力したときに、表示が「zixlyu」となる問題を回避
                bool preCheck = true;
                for (int j = 0; j < romNum; j++)
                {
                    if (rom[j] != _romSliceList[furiCount][j])
                    {
                        preCheck = false;
                    }
                }

                if (preCheck && Input.GetKeyDown(rom[romNum].ToString()))    //　n文字目を取り出すとcharになるから、stringに戻す
                {
                    _romSliceList[furiCount] = rom; //例：["si","n","bu","n"]のfuriCount=0をrom=["shi"]に変える
                    _aString = string.Join("", GetRomSliceListWithoutSkip());

                    ReCreatList(_romSliceList); // "si"を"shi"に変換すると、何番目の文字かが変わってしまうため、再定義

                    // trueにする
                    isCorrect = true;

                    if (addSmallMoji)
                    {
                        AddSmallMoji();
                    }

                    // 正解
                    Correct();

                    // 最後の文字に正解したら
                    //if (_aNum >= _aString.Length -1)
                    if (_aNum >= _aString.Length)
                    {
                        // 次の問題に変える
                        CorrectLastChar();
                    }

                    break;
                }
            }
        }
    }

    // 問題を出すための関数
    void Output()
    {
        //_aNumを0にする
        _aNum = 0;

        _qNum = _qList[_qCount];
        
        if (_qNum == 0)
        {
            Debug.Log("らり出題");
            RariStart = DateTime.Now;

        }
        else
        {
            Debug.Log("ルゥ出題");
            LuStart = DateTime.Now;
        }

        _fString = f_List[_qNum];
        _qString = q_List[_qNum];
        

        CreatRomSliceList(_fString);
        _aString = string.Join("", GetRomSliceListWithoutSkip());

        // 文字を変更する
        fText.text = _fString;
        qText.text = _qString;
        aText.text = _aString;

        //Debug.Log(string.Join("", _romSliceList));
    }

    // 正解用の関数
    void Correct()
    {
        //正解時の処理
        _aNum++;
        aText.text = "<color=#6A6A6A>" + _aString.Substring(0, _aNum) + "</color>" + _aString.Substring(_aNum);
        //Debug.Log(_aNum);
        //Debug.Log("正解したよ！");
    }

    //間違い用の関数
    void Miss()
    {
        // 間違えた時の処理
        aText.text = "<color=#6A6A6A>" + _aString.Substring(0, _aNum) + "</color>" + "<color=#ff0000>" + _aString.Substring(_aNum, 1) + "</color>" + _aString.Substring(_aNum + 1);
        //Debug.Log("間違えたよ！");
    }


// アニメーション関係
    // StartAnim
    IEnumerator StartAnim()
    {
        Debug.Log("開始アニメーション入りました");

        enabled = false;

        StartAnim_Panel.SetActive(true);
        var animator = StartAnim_Panel.gameObject.GetComponent<Animator>();
        animator.SetTrigger("trigStartAnim");
        yield return null;

        while (true)
        {
            yield return null;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            //Debug.Log($"Wating finishi {stateInfo.normalizedTime}");
            if (stateInfo.IsName("StartAnim") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
        }

        StartAnim_Panel.SetActive(false);

        // 問題を出す
        Output();
        enabled = true;
    }
    // EndAnim
    IEnumerator EndAnim()
    {

        enabled = false;
        StartAnim_Panel.SetActive(true);

        var animator = StartAnim_Panel.gameObject.GetComponent<Animator>();
        animator.SetTrigger("trigEndAnim");
        yield return null;

        while (true)
        {
            yield return null;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            //Debug.Log($"Wating finishi {stateInfo.normalizedTime}");
            if (stateInfo.IsName("EndAnim") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
        }

        enabled = true;

        SceneManager.LoadScene("ResultScene");
    }

    // RariAnswerAnim
    IEnumerator RariAnswerAnim()
    {
        var animator = CharacterAndTable.gameObject.GetComponent<Animator>();
        animator.SetTrigger("trigRariAnswerAnim");
        yield return null;

        while (true)
        {
            yield return null;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            //Debug.Log($"Wating finishi {stateInfo.normalizedTime}");
            if (stateInfo.IsName("Rari_answer") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
        }
    }

    // LuAnswerAnim
    IEnumerator LuAnswerAnim()
    {
        var animator = CharacterAndTable.gameObject.GetComponent<Animator>();
        animator.SetTrigger("trigLuAnswerAnim");
        yield return null;

        while (true)
        {
            yield return null;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            //Debug.Log($"Wating finishi {stateInfo.normalizedTime}");
            if (stateInfo.IsName("Lu_answer") && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
        }
    }


    // 各問題の最後の1文字を正解したときの関数
    void CorrectLastChar()
    {
        // 正解数+1
        _qCount += 1;
        RemainingNum_TMP.text = (MaxQuizNum - _qCount).ToString();

        if (_qNum == 0) // 正解したらカウントを+1にする
        {
            RariCount += 1;
            Debug.Log(RariCount);

            // 武田羅梨沙多胡入力時間のカウント
            RariEnd = DateTime.Now;
            RariLogTime += (RariEnd - RariStart).TotalMilliseconds / 1000;
            Debug.Log("武田羅梨沙多胡合計入力時間[秒]");
            Debug.Log(RariLogTime);

            // 武田羅梨沙多胡アニメーション
            StartCoroutine(RariAnswerAnim());
            
        }
        else
        {

            LuCount += 1;
            Debug.Log(LuCount);

            // ルゥティン入力時間のカウント
            LuEnd = DateTime.Now;
            LuLogTime += (LuEnd - LuStart).TotalMilliseconds / 1000;
            Debug.Log("ルゥティン入力時間[秒]");
            Debug.Log(LuLogTime);

            // ルゥティンアニメーション
            StartCoroutine(LuAnswerAnim());
        }

        if (_qCount >= MaxQuizNum)
        {
            RariLogTime = RariLogTime / RariCount;
            LuLogTime = LuLogTime / LuCount;
            StartCoroutine(EndAnim());
        }
        else
        {
            Output();
        }
    }
 // 柔軟な入力用
     // しんぶん→"si","n","bu","n"のようなリストをつくる
    void CreatRomSliceList(string moji)
    {
        _romSliceList.Clear();
        _furiCountList.Clear();
        _romNumList.Clear();

        // 「し」→「si」、「ん」→「n」
        for (int i = 0; i < moji.Length; i++)
        {
            string a = cd.dic[moji[i].ToString()][0];  //moji[i]：ひらがなが入っており、dicでそのひらがなをローマ字に変換してaに代入
            if (moji[i].ToString() == "ゃ" | moji[i].ToString() == "ゅ" | moji[i].ToString() == "ょ"|
                moji[i].ToString() == "ぁ"| moji[i].ToString() == "ぃ"| moji[i].ToString() == "ぇ"| moji[i].ToString() == "ぉ")
            {
                a = "SKIP";
            }
            else if (moji[i].ToString() == "っ" && i + 1 < moji.Length)
            {
                a = cd.dic[moji[i + 1].ToString()][0][0].ToString();
            }
            else if (i + 1 < moji.Length)
            {
                // 次の文字を含めて辞書から探す
                string addNectMoji = moji[i].ToString() + moji[i + 1].ToString();
                if (cd.dic.ContainsKey(addNectMoji))
                {
                    a = cd.dic[addNectMoji][0];
                }
            }

            _romSliceList.Add(a);

            if (a == "SKIP")
            {
                continue;
            }

            for (int j = 0; j < a.Length; j++)
            {
                _furiCountList.Add(i);
                _romNumList.Add(j);
            }
        }
        //Debug.Log(string.Join(",", _romSliceList));
    }

    void ReCreatList(List<string> romList)
    {
        _furiCountList.Clear();
        _romNumList.Clear();

        // 「し」→「si」、「ん」→「n」
        for (int i = 0; i < romList.Count; i++)
        {
            string a = romList[i];  //romList[i]：ひらがなをローマ字に変換したものが入っている
            if (a == "SKIP")
            {
                continue;
            }
            for (int j = 0; j < a.Length; j++)
            {
                _furiCountList.Add(i);
                _romNumList.Add(j);
            }
        }
        //Debug.Log(string.Join(",", _romSliceList));
    }

    // 柔軟な入力をしたときに、次の文字が小文字なら小文字を挿入する
    void AddSmallMoji()
    {
        int nextMojiNum = _furiCountList[_aNum] + 1;

        // もし次の文字がなければ処理しない
        if (_fString.Length - 1 < nextMojiNum)
        {
            return;
        }

        string nextMoji = _fString[nextMojiNum].ToString();
        string a = cd.dic[nextMoji][0];

        // もしaの0番目がxでもlでもなければ（小文字でなければ）処理をしない
        if (a[0] != 'x' && a[0] != 'l')
        {
            return;
        }

        // romslicelistに挿入と表示の反映
        _romSliceList.Insert(nextMojiNum, a);
        // SKIPを削除する
        _romSliceList.RemoveAt(nextMojiNum + 1);

        // 変更したリストを再度表示させる
        ReCreatList(_romSliceList);
        _aString = string.Join("", GetRomSliceListWithoutSkip());
    }

    // SKIPをなしで表示させるためのListを作り直す
    List<string> GetRomSliceListWithoutSkip()   //引数なしで_romSliceListが変更されるListがキモい・・・関数とは違うんか？
    {
        List<string> returnList = new List<string>();
        foreach (string rom in _romSliceList)
        {
            if (rom == "SKIP")
            {
                continue;
            }
            returnList.Add(rom);
        }
        return returnList;
    }

    // 「Shift + 1」が入力されたときに「Alpha1」ではなく「!」に変換する
    string GetKeyReturn()
    {
        isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift);
        if (isShift)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                return "!";
            }
        }
        return "";
    }


}
