using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    [Header("Typing")]
    [SerializeField] float typingTime;
    [SerializeField] float typingAcuracy;
    [SerializeField] float typingNumber;

    // WakeUpManagerから参照するためのプロパティ
    public float TypingTime
    {
        get { return typingTime; }
        set { typingTime = value; }
    }

    [Header("WakeUp")]
    public float WakeUpPushNumber;

    // ★追加: 成功か失敗かを保持するBool変数（Resultシーンなどで参照可能）
    public bool IsWakeUpSuccess;

    [Header("Score")]
    [SerializeField] float score;
    public float Score
    {
        get { return score; }
        set { score = value; }
    }

    void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //　Typing　/////////////////////////////////////////////////////////

    public void TypingResult(float type)
    {
        score = type * 3;
        SceneManager.LoadScene("WakeUp");
    }


    //　WakeUp　/////////////////////////////////////////////////////////

    // ★変更: 成功・失敗を引数(bool)で受け取る形に統合
    public void WakeUpResult(bool isSuccess)
    {
        // 結果を保存
        IsWakeUpSuccess = isSuccess;

        // 結果シーンへ遷移
        SceneManager.LoadScene("Result");
    }
}