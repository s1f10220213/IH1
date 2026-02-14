using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Settings")]
    [SerializeField] private float borderScore = 60.0f; // これ以上なら「採択」、未満なら「不採択」
    [SerializeField] private float stepDelay = 1.0f;    // 各表示の間の待ち時間
    [SerializeField] private float stampTargetScale = 0.1f; // ★追加: スタンプの目標サイズ

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

    private IEnumerator ResultSequence()
    {
        // 1. スコアの取得（GameManagerが存在しない場合はテスト用に0を入れる）
        float finalScore = (GameManager.gameManager != null) ? GameManager.gameManager.Score : 0f;

        // 評価判定 (true: 採択, false: 不採択)
        bool isAccepted = (finalScore >= borderScore);

        yield return new WaitForSeconds(0.5f);

        // --- Step 1: スコアテキスト表示 ---
        if (scoreText != null)
        {
            scoreText.text = $"{finalScore:F0} 点";
        }
        yield return new WaitForSeconds(stepDelay);

        // --- Step 2: 上司の一言（CSVからランダム取得） ---
        if (bossCommentText != null)
        {
            // 判定結果に応じてCSVファイルを選択
            TextAsset targetCsv = isAccepted ? csvAccepted : csvRejected;

            string randomComment = GetRandomCommentFromCSV(targetCsv);
            bossCommentText.text = $"{randomComment}";
        }
        yield return new WaitForSeconds(stepDelay * 1.5f);

        // --- Step 3: スタンプ画像表示 ---
        if (stampImage != null)
        {
            // 判定結果に応じて画像を選択
            Sprite selectedSprite = isAccepted ? stampAccepted : stampRejected;

            if (selectedSprite != null)
            {
                stampImage.sprite = selectedSprite;
                stampImage.gameObject.SetActive(true);

                // ポンッと出すアニメーション（スケール変化）
                // stampImage.transform.localScale = Vector3.zero;

                // float t = 0;
                // while (t < 1.0f)
                // {
                //     t += Time.deltaTime * 5.0f; // アニメーション速度

                //     // 0 から 目標サイズ(0.1) まで補間する
                //     float scale = Mathf.Lerp(0, stampTargetScale, t);

                //     stampImage.transform.localScale = Vector3.one * scale;
                //     yield return null;
                // }

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