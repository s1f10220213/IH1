using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private List<Sprite> wakeUpSprites = new List<Sprite>();
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
    private int currentSpriteIndex = 0; // ★追加: 現在表示している画像の番号を記録

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

            if (wakeUpSprites.Count > 0 && wakeUpSprites[0] != null)
            {
                characterImage.sprite = wakeUpSprites[0];
                currentSpriteIndex = 0; // 初期化
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

        if ((UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            || Input.GetKeyDown(KeyCode.Space))
        {
            currentPushCount++;

            // UI更新（連打数表示のため）
            if (counterText != null) counterText.text = $"連打: {currentPushCount} / {targetPushCount}";

            ApplyShakeImpulse();
            UpdateCharacterSprite();

            if (currentPushCount >= targetPushCount)
            {
                OnWakeUpSuccess();
            }
        }
    }

    private void UpdateCharacterSprite()
    {
        if (characterImage == null) return;
        if (wakeUpSprites.Count == 0) return;
        if (targetPushCount == 0) return;

        float progress = (float)currentPushCount / targetPushCount;
        int newIndex = Mathf.FloorToInt(progress * wakeUpSprites.Count);
        newIndex = Mathf.Clamp(newIndex, 0, wakeUpSprites.Count - 1);

        // ★変更: インデックスが変わった時だけ画像を更新し、変数を記録
        if (newIndex != currentSpriteIndex)
        {
            currentSpriteIndex = newIndex;
            if (characterImage.sprite != wakeUpSprites[newIndex])
            {
                characterImage.sprite = wakeUpSprites[newIndex];
            }
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

        if (characterImage != null && wakeUpSprites.Count > 0)
        {
            characterImage.sprite = wakeUpSprites[0];
            currentSpriteIndex = 0;
        }

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

    // ★追加: 失敗時に画像を順番に戻すアニメーション
    private IEnumerator FailAnimationSequence()
    {
        // 戻るのにかける合計時間
        float totalDuration = 1.0f;
        int startVal = currentSpriteIndex;

        // すでに0番目（完全に寝ている）でなければアニメーションする
        if (startVal > 0)
        {
            // 1ステップあたりの待機時間
            float stepTime = totalDuration / startVal;

            // 現在のインデックスの一つ前から 0 まで順番に表示
            for (int i = startVal - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(stepTime);

                if (characterImage != null && i < wakeUpSprites.Count)
                {
                    characterImage.sprite = wakeUpSprites[i];
                }
            }
        }
        else
        {
            // 最初から寝ていた場合は念のため0番目をセットして少し待つ
            if (characterImage != null && wakeUpSprites.Count > 0)
            {
                characterImage.sprite = wakeUpSprites[0];
            }
            yield return new WaitForSeconds(0.5f);
        }

        // アニメーションが終わってからゲームオーバー通知
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.WakeUpMiss();
        }
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
        // 連打数の更新は入力時に行うようにしたのでここはタイマーのみでも良いが、念のため残す
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
        isGameActive = false; // 操作を受け付けなくする
        ResetImagePosition(); // 揺れを止める

        if (messageText != null) messageText.text = "失敗...";

        // ★変更: ここでGameManagerには通知せず、アニメーションを開始する
        StartCoroutine(FailAnimationSequence());
    }
}