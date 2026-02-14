using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingManager : MonoBehaviour
{
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

    [Header("UI")]
    [SerializeField] UIImageBase imageBase;
    [SerializeField] Sprite spriteW;
    [SerializeField] Sprite spriteA;
    [SerializeField] Sprite spriteS;
    [SerializeField] Sprite spriteD;

    [Header("StartScene")]
    [SerializeField] UIImageBase startUI;
    [SerializeField] UIImageBase ui3;
    [SerializeField] UIImageBase ui2;
    [SerializeField] UIImageBase ui1;
    [SerializeField] UIImageBase uiStart;

    ////////////////////////////////////////////////////////////////////////////

    private bool isCanType;
    private bool isTimer;
    private bool isStart;
    [SerializeField] private int[] keyCodes = new int[5];
    private UIImageBase[] images = new UIImageBase[6];
    private int currentKeyCode;

    private int currentImage;

    ////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        foreach(UIImageBase ui in images)
        {
            ui.SetAlpha(0);
        }
    }
    
    private IEnumerator GameStart()
    {
        float time = 0;
        startUI.FadeOutImage(0.3f);
        
        for (int i=0; i<5; i++)
        {
           keyCodes[i] = Random.Range(0, 4);
        }
        currentKeyCode = keyCodes[0];

        currentImage = 0;
        SetUI();
        
        foreach (UIImageBase ui in images)
        {
            ui.FadeInImage(0.35f);
        }

        yield return new WaitForSeconds(3);
        TypingStart();
    }

    private void SetUI()
    {
        for (int i=0; i<5; i++)
        {
            if (keyCodes[i] == 0)
            {
                images[i].SetSprite(spriteW);
            }
            else if (keyCodes[i] == 1)
            {
                images[i].SetSprite(spriteA);
            }
            else if (keyCodes[i] == 2)
            {
                images[i].SetSprite(spriteS);
            }
            else if (keyCodes[i] == 3)
            {
                images[i].SetSprite(spriteD);
            }
        }
    }

    private void MoveUI()
    {
        for (int i=0; i<6; i++)
        {
            if (i == currentImage) //　下に落ちる・フェードアウト・縮小
            {
                images[i].MoveDirection(-Vector2.up, 500, 0.04f, UIImageBase.EasingType.EaseOut);
                //images[i].ScaleToScaleImage(, 0.04);
            }
            else if (i == (currentImage+1)%6) //　右からスライドイン
            {
                //images[i].MoveFromTo();
            }
            else if (i == (currentImage+5)%6) //　右へ一つ・拡大
            {
                images[i].MoveDirection(-Vector2.right, 50, 0.04f, UIImageBase.EasingType.EaseOut);
                //images[i].ScaleToScaleImage(, 0.04);
            }
            else //　右へ一つずらす
            {
                images[i].MoveDirection(-Vector2.right, 50, 0.04f, UIImageBase.EasingType.EaseOut);
            }
        }
        currentImage += 1;
        currentImage %= 6;
    }

    /// ///////////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {
        if (!isStart)
        {
            var keyboard = Keyboard.current;
            if (keyboard.anyKey.wasPressedThisFrame)
            {
                foreach (var key in keyboard.allKeys)
                {
                    if(key.wasPressedThisFrame && key.displayName == "Enter")
                    {
                        StartCoroutine(GameStart());
                    }
                }
            }
        }
        if (isTimer)
        {
            typingTime += Time.deltaTime;

            if (typingTime >= maxTypingTime)
            {
                TypingStop();
            }
        }

        if (isCanType)
        {
            var keyboard = Keyboard.current;
            if (keyboard.anyKey.wasPressedThisFrame)
            {
                foreach (var key in keyboard.allKeys)
                {
                    if (key.wasPressedThisFrame && key.displayName == "W")
                    {
                        JudgeKey(0);
                    }
                    else if(key.wasPressedThisFrame && key.displayName == "A")
                    {
                        JudgeKey(1);
                    } 
                    else if(key.wasPressedThisFrame && key.displayName == "S")
                    {
                        JudgeKey(2);
                    } 
                    else if(key.wasPressedThisFrame && key.displayName == "D")
                    {
                        JudgeKey(3);
                    } 
                    else if(key.wasPressedThisFrame && key.displayName == "Enter")
                    {
                        TypingStop();
                    }
                }
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
        MoveUI();
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
        if (keyCodes[4] == 0)
        {
            images[(currentKeyCode+5)%6].SetSprite(spriteW);
        }
        else if (keyCodes[4] == 1)
        {
            images[(currentKeyCode+5)%6].SetSprite(spriteA);
        }
        else if (keyCodes[4] == 2)
        {
            images[(currentKeyCode+5)%6].SetSprite(spriteS);
        }
        else if (keyCodes[4] == 3)
        {
            images[(currentKeyCode+5)%6].SetSprite(spriteD);
        }
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
