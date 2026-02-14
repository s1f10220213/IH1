using System.Collections;
using System.Collections.Generic; // リストを使うために必要
using UnityEngine;
using UnityEngine.UI;

public class WakeUpManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float startCountDownTime = 3.0f;
    [SerializeField] private float timeLimit = 5.0f;
    [SerializeField] private int baseRequiredPushes = 20;

    [Header("Visuals (Sprite List)")]
    [Tooltip("0番目が寝ている画像、最後が起きている画像になるように登録してください")]
    [SerializeField] private List<Sprite> wakeUpSprites = new List<Sprite>(); // ★変更: リスト化
    [SerializeField] private Image characterImage;

    [Header("Shake Settings")]
    [SerializeField] private float shakeMagnitude = 20.0f;
    [SerializeField] private float shakeRecoverySpeed = 20.0f;

    [Header("UI References")]
    [SerializeField] private Text timerText;
    [SerializeField] private Text counterText;
    [SerializeField] private Text messageText;

    // 内部変数
    private float currentTime;
    private int currentPushCount = 0;
    private int targetPushCount;
    private bool isGameActive = false;

    // 揺れ処理用
    private RectTransform imageRectTransform;
    private Vector2 initialPos;
    private Vector2 currentShakeOffset;

    void Start()
    {
        if (characterImage != null)
        {
            imageRectTransform = characterImage.GetComponent<RectTransform>();
            if (imageRectTransform != null)
            {
                initialPos = imageRectTransform.anchoredPosition;
            }

            // ★変更: リストの最初の画像（寝ている状態）をセット
            if (wakeUpSprites.Count > 0 && wakeUpSprites[0] != null)
            {
                characterImage.sprite = wakeUpSprites[0];
            }
        }

        StartCoroutine(GameSequence());
    }

    void Update()
    {
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;
        UpdateUI();

        if (currentTime <= 0)
        {
            OnWakeUpFailed();
            return;
        }

        UpdateShakePosition();

        // 入力処理 (New Input System / Legacy Input 両対応)
        if ((UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            || Input.GetKeyDown(KeyCode.Space))
        {
            currentPushCount++;
            UpdateUI();

            ApplyShakeImpulse();

            // ★変更: 進捗に応じてリストから画像を選択
            UpdateCharacterSprite();

            if (currentPushCount >= targetPushCount)
            {
                OnWakeUpSuccess();
            }
        }
    }

    // ★変更: リストの個数に合わせてインデックスを計算する関数
    private void UpdateCharacterSprite()
    {
        if (characterImage == null) return;
        if (wakeUpSprites.Count == 0) return; // 画像が登録されていなければ何もしない
        if (targetPushCount == 0) return;

        // 現在の進捗率 (0.0 〜 1.0)
        float progress = (float)currentPushCount / targetPushCount;

        // 進捗率 × 画像の枚数 でインデックスを決定
        // 例: 3枚の場合、0.33 * 3 = 0.99 -> floorして 0番目
        //               0.40 * 3 = 1.20 -> floorして 1番目
        int spriteIndex = Mathf.FloorToInt(progress * wakeUpSprites.Count);

        // インデックスが範囲外に出ないように制限 (進捗100%のとき対策)
        spriteIndex = Mathf.Clamp(spriteIndex, 0, wakeUpSprites.Count - 1);

        // 画像を適用
        Sprite nextSprite = wakeUpSprites[spriteIndex];
        if (nextSprite != null && characterImage.sprite != nextSprite)
        {
            characterImage.sprite = nextSprite;
        }
    }

    private void ApplyShakeImpulse()
    {
        float x = Random.Range(-1f, 1f) * shakeMagnitude;
        float y = Random.Range(-1f, 1f) * shakeMagnitude;
        currentShakeOffset = new Vector2(x, y);
    }

    private void UpdateShakePosition()
    {
        if (imageRectTransform == null) return;

        if (currentShakeOffset != Vector2.zero)
        {
            currentShakeOffset = Vector2.Lerp(currentShakeOffset, Vector2.zero, Time.deltaTime * shakeRecoverySpeed);
            if (currentShakeOffset.magnitude < 0.1f) currentShakeOffset = Vector2.zero;
            imageRectTransform.anchoredPosition = initialPos + currentShakeOffset;
        }
    }

    private void ResetImagePosition()
    {
        currentShakeOffset = Vector2.zero;
        if (imageRectTransform != null) imageRectTransform.anchoredPosition = initialPos;
    }

    private IEnumerator GameSequence()
    {
        SetupDifficulty();
        isGameActive = false;
        currentTime = timeLimit;

        if (messageText != null) messageText.text = "Ready...";

        // カウントダウン中もリストの0番目（寝ている画像）を表示
        if (characterImage != null && wakeUpSprites.Count > 0)
            characterImage.sprite = wakeUpSprites[0];

        float count = startCountDownTime;
        while (count > 0)
        {
            if (timerText != null) timerText.text = $"開始まで: {count:F0}";
            yield return new WaitForSeconds(1.0f);
            count--;
        }

        if (messageText != null) messageText.text = "起きろ!!";
        isGameActive = true;
    }

    private void SetupDifficulty()
    {
        currentPushCount = 0;
        if (GameManager.gameManager != null)
        {
            targetPushCount = baseRequiredPushes;
            GameManager.gameManager.WakeUpPushNumber = targetPushCount;
        }
        else
        {
            targetPushCount = baseRequiredPushes;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText != null) timerText.text = $"残り時間: {Mathf.Max(0, currentTime):F1}";
        if (counterText != null) counterText.text = $"連打: {currentPushCount} / {targetPushCount}";
    }

    private void OnWakeUpSuccess()
    {
        isGameActive = false;
        ResetImagePosition();
        if (messageText != null) messageText.text = "成功！";
        if (GameManager.gameManager != null) GameManager.gameManager.WakeUpSuccessfull();
    }

    private void OnWakeUpFailed()
    {
        isGameActive = false;
        ResetImagePosition();
        if (messageText != null) messageText.text = "失敗...";
        if (GameManager.gameManager != null) GameManager.gameManager.WakeUpMiss();
    }
}