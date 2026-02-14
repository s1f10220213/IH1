using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] private bool isCanType;
    private bool isTimer;
    [SerializeField] private int[] keyCodes = new int[5];
    private int currentKeyCode;

    [SerializeField] InputSystem_Actions action;

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

        var keyboard = Keyboard.current;
        if (keyboard.anyKey.wasPressedThisFrame)
        {
            // 3. どのキーが押されたか特定したい場合
            foreach (var key in keyboard.allKeys)
            {
                if (key.wasPressedThisFrame && key.displayName == "W")
                {
                    InputW();
                }
                else if(key.wasPressedThisFrame && key.displayName == "A")
                {
                    InputA();
                } 
                else if(key.wasPressedThisFrame && key.displayName == "S")
                {
                    InputS();
                } 
                else if(key.wasPressedThisFrame && key.displayName == "D")
                {
                    InputD();
                } 
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    public void InputW()
    {
        if (!isCanType) return;
        JudgeKey(0);
    }
    public void InputA()
    {
        if (!isCanType) return;
        JudgeKey(1);
    }
    public void InputS()
    {
        if (!isCanType) return;
        JudgeKey(2);
    }
    public void InputD()
    {
        if (!isCanType) return;
        JudgeKey(3);
    }

    public void InputEnter()
    {
        if (!isCanType) return;
        TypingStop();
    }

    void JudgeKey(int keyID) //　キーの正誤判定
    {
        Debug.Log("判定集");
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
