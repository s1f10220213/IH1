using UnityEngine;

public class TypingManager : MonoBehaviour
{
    [Header("KeySettings")]
    [SerializeField] KeyCode key1;
    [SerializeField] KeyCode key2;
    [SerializeField] KeyCode key3;
    [SerializeField] KeyCode key4;
    [SerializeField] KeyCode enterKey;

    [Header("TypingSettings")]
    [SerializeField] int typeCount; //　タイプ成功回数
    [SerializeField] int missCount; //　ミス回数
    [SerializeField] float typingTime; //　作業時間
    
    [Header("ScoreSettings")]
    [SerializeField] int maxScore; //　最大スコア
    [SerializeField] int maxScoreTypeCount; //　この回数タイプ成功でマックススコア
    [SerializeField] float maxTypingTime; //　この時間を超過したら強制で朝に
    [Space(10)]
    [SerializeField] int gameOverMissCount; //　この回数以上のミスでゲームオーバー

    ////////////////////////////////////////////////////////////////////////////

    private bool isCanType;
    private bool isTimer;
    [SerializeField] private int[] keyCodes = new int[5];
    private int currentKeyCode;

    ////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        for (int i=0; i<5; i++)
        {
           keyCodes[i] = Random.Range(0, 4);
        }
        currentKeyCode = keyCodes[0];

        TypingStart();
    }

    void Update()
    {
        if (isTimer)
        {
            typingTime += Time.deltaTime;
        }

        if (isCanType)
        {
            if (Input.GetKeyDown(enterKey))
            {
                TypingStop();
            }
            else if (Input.GetKeyDown(key1))
            {
                JudgeKey(0);
            }
            else if (Input.GetKeyDown(key2))
            {
                JudgeKey(1);
            }
            else if (Input.GetKeyDown(key3))
            {
                JudgeKey(2);
            }
            else if (Input.GetKeyDown(key4))
            {
                JudgeKey(3);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////
     
    void JudgeKey(int keyID) //　キーの正誤判定
    {
        if (currentKeyCode == keyID)
        {
            SuccessType();
        }
        else
        {
            FailType();
        }
    }

    ////////////////////////////////////////////////////////////////////////////

    void SuccessType() //　正解
    {
        typeCount += 1;
        NextKey();
    }

    void FailType() //　間違い
    {
        missCount += 1;
    }

    ////////////////////////////////////////////////////////////////////////////
    
    void NextKey()
    {
        currentKeyCode = keyCodes[1];
        for (int i=0; i<4; i++)
        {
            keyCodes[i] = keyCodes[i+1];
        }
        GenerateNextKey();
    }

    void GenerateNextKey()
    {
        keyCodes[4] = Random.Range(0, 4);
    }

    ////////////////////////////////////////////////////////////////////////////
    
    void TimerStart()
    {
        typingTime = 0;
        isTimer = true;
    }

    void TimerStop()
    {
        isTimer = false;
    }

    ////////////////////////////////////////////////////////////////////////////
     
    void TypingStart()
    {
        isCanType = true;
        TimerStart();
    }

    void TypingStop()
    {
        isCanType = false;
        isTimer = false;
    }
}
