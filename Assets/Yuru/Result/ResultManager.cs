using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text scoreText;       // スコア表示用テキスト
    [SerializeField] private Image stampImage;     // スタンプ表示用Image
    [SerializeField] private Text bossCommentText; // 上司の一言用テキスト

    [Header("Stamp Resources")]
    [SerializeField] private Sprite stampAccepted; // 「採択」画像
    [SerializeField] private Sprite stampRejected; // 「不採択」画像

    [Header("CSV Resources (Boss Comments)")]
    [Tooltip("一行に一言ずつ記述したテキストファイルをアタッチしてください")]
    [SerializeField] private TextAsset csvAccepted; // 「採択」時のコメント集
    [SerializeField] private TextAsset csvRejected; // 「不採択」時のコメント集
    [SerializeField] private TextAsset csvEvaluationImpossible; // ★追加: 「評価不可（失敗）」時のコメント集

    [Header("Settings")]
    [SerializeField] private float borderScore = 60.0f; // これ以上なら「採択」、未満なら「不採択」
    [SerializeField] private float stepDelay = 1.0f;    // 各表示の間の待ち時間
    [SerializeField] private float stampTargetScale = 0.1f; // スタンプの目標サイズ

    private void Start()
    {
        // 初期化：表示を隠しておく
        if (scoreText != null) scoreText.text = "";
        if (stampImage != null)
        {
            stampImage.gameObject.SetActive(false);
            stampImage.color = Color.white;
        }
        if (bossCommentText != null) bossCommentText.text = "";

        // リザルト演出開始
        StartCoroutine(ResultSequence());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("TypingScene");
        }
    }

    private IEnumerator ResultSequence()
    {
        // 1. 各種パラメータの取得
        bool isWakeUpSuccess = true;
        float finalScore = 0f;

        if (GameManager.gameManager != null)
        {
            isWakeUpSuccess = GameManager.gameManager.IsWakeUpSuccess;
            finalScore = GameManager.gameManager.Score;
        }

        // 2. 状態の判定
        //  - wakeUpSuccess: 起床成功かどうか
        //  - isAccepted: 点数がボーダーを超えているか（起床失敗時は強制false）
        bool isAccepted = false;

        if (isWakeUpSuccess)
        {
            // 起床成功時のみ点数判定を行う
            isAccepted = (finalScore >= borderScore);
        }
        else
        {
            // 起床失敗時は強制的に不採択扱い
            isAccepted = false;
        }


        yield return new WaitForSeconds(0.5f);

        // --- Step 1: スコアテキスト表示 ---
        if (scoreText != null)
        {
            if (isWakeUpSuccess)
            {
                // 成功時は点数を表示
                scoreText.text = $"{finalScore:F0} 点";
            }
            else
            {
                // ★変更: 失敗時は「評価不可」を表示
                scoreText.text = "評価不可";
            }
        }
        yield return new WaitForSeconds(stepDelay);

        // --- Step 2: 上司の一言（CSVからランダム取得） ---
        if (bossCommentText != null)
        {
            TextAsset targetCsv;

            if (!isWakeUpSuccess)
            {
                // ★変更: 起床失敗時は「評価不可」用CSVを使用
                targetCsv = csvEvaluationImpossible;
            }
            else
            {
                // 起床成功時は点数に応じてCSVを選択
                targetCsv = isAccepted ? csvAccepted : csvRejected;
            }

            string randomComment = GetRandomCommentFromCSV(targetCsv);
            bossCommentText.text = $"{randomComment}";
        }
        yield return new WaitForSeconds(stepDelay * 1.5f);

        // --- Step 3: スタンプ画像表示 ---
        if (stampImage != null)
        {
            Sprite selectedSprite;

            if (!isWakeUpSuccess)
            {
                // ★変更: 起床失敗時は「不採択」スタンプを使用
                selectedSprite = stampRejected;
            }
            else
            {
                // 起床成功時は判定結果に応じて選択
                selectedSprite = isAccepted ? stampAccepted : stampRejected;
            }

            if (selectedSprite != null)
            {
                stampImage.sprite = selectedSprite;
                stampImage.gameObject.SetActive(true);
                stampImage.transform.localScale = Vector3.one * stampTargetScale;
            }
        }
    }

    /// <summary>
    /// CSV(TextAsset)を改行区切りで読み込み、ランダムな1行を返す
    /// </summary>
    private string GetRandomCommentFromCSV(TextAsset csvFile)
    {
        if (csvFile == null) return "（コメントデータが見つかりません）";

        // 改行コードで分割して配列にする
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0) return "……";

        int randomIndex = Random.Range(0, lines.Length);
        return lines[randomIndex];
    }
}