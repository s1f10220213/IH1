using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    [Header("Typing")]
    [SerializeField] float typingTime;
    [SerializeField] float typingAcuracy;
    [SerializeField] float typingNumber;

    [Header("WakeUp")]
    public float WakeUpPushNumber;

    [Header("Score")]
    [SerializeField] float score;
    public float Score
    {
        get { return score; }
        set { score = value; }
    }

    void Awake()
    {
        gameManager = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    //　Typing　/////////////////////////////////////////////////////////

    public void TypingResult()
    {

    }


    //　WakeUp　/////////////////////////////////////////////////////////

    public void WakeUpSuccessfull()
    {

    }

    public void WakeUpMiss()
    {

    }
}
