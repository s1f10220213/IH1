using UnityEngine;
using UnityEngine.UI; // UIを操作するために必要

public class WakeUpManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 5.0f; // 制限時間（5秒）
    [SerializeField] private int baseRequiredPushes = 40; // 基本の必要連打数（仮の固定値）

    [Header("UI References")]
    [SerializeField] private Text timerText; // 残り時間を表示するテキスト
    [SerializeField] private Text counterText; // 連打数を表示するテキスト
    [SerializeField] private Text messageText; // 成功・失敗を表示するテキスト（オプション）

    // 内部変数
    private float currentTime;
    private int currentPushCount = 0;
    private int targetPushCount;
    private bool isGameActive = false;

    void Start()
    {
        InitializeWakeUpPhase();
    }

    void Update()
    {
        if (!isGameActive) return;

        // タイマー処理
        currentTime -= Time.deltaTime;
        UpdateUI();

        // タイムオーバー判定（失敗）
        if (currentTime <= 0)
        {
            OnWakeUpFailed();
            return;
        }

        // 連打入力処理 (スペースキー)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentPushCount++;

            // クリア判定（成功）
            if (currentPushCount >= targetPushCount)
            {
                OnWakeUpSuccess();
            }
        }
    }

    /// <summary>
    /// 朝フェーズの初期化処理
    /// </summary>
    private void InitializeWakeUpPhase()
    {
        currentTime = timeLimit;
        currentPushCount = 0;
        isGameActive = true;

        // --- 難易度の決定ロジック ---
        // GameManagerが存在する場合、夜フェーズのデータ（typingTime）を取得して難易度に反映可能
        // 現在は固定値を使用していますが、以下のように計算式を入れることで連携できます。

        if (GameManager.gameManager != null)
        {
            // 例: 夜更かし時間(typingTime)が長いほど、必要連打数が増える
            // targetPushCount = baseRequiredPushes + (int)GameManager.gameManager.typingTime; 

            // 今回は「固定値」とのことなので、一旦ベース値を代入します
            targetPushCount = baseRequiredPushes;

            // GameManager側の変数にも今の目標値をセット（デバッグや表示用）
            // GameManager.gameManager.WakeUpPushNumber = targetPushCount;
        }
        else
        {
            // GameManagerがない場合（単体テスト用）
            targetPushCount = baseRequiredPushes;
            Debug.LogWarning("GameManagerが見つかりません。単体テストモードで動作します。");
        }

        Debug.Log($"朝フェーズ開始！ 目標連打数: {targetPushCount}, 制限時間: {timeLimit}秒");
    }

    private void UpdateUI()
    {
        // UIが設定されている場合のみ更新
        if (timerText != null)
        {
            timerText.text = $"残り時間: {currentTime:F1}"; // 小数点第1位まで表示
        }

        if (counterText != null)
        {
            counterText.text = $"連打: {currentPushCount} / {targetPushCount}";
        }
    }

    private void OnWakeUpSuccess()
    {
        isGameActive = false;
        currentTime = 0;
        UpdateUI();

        if (messageText != null) messageText.text = "おはよう！(成功)";
        Debug.Log("起立成功！");

        // GameManagerに成功を通知
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.WakeUpSuccessfull();
        }
    }

    private void OnWakeUpFailed()
    {
        isGameActive = false;
        currentTime = 0;
        UpdateUI();

        if (messageText != null) messageText.text = "二度寝... (失敗)";
        Debug.Log("起立失敗...");

        // GameManagerに失敗を通知
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.WakeUpMiss();
        }
    }
}