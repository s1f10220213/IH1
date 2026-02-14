using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void WakeUpSuccessfull()
    {
        SceneManager.LoadScene("Result");
    }

    public void WakeUpMiss()
    {
        SceneManager.LoadScene("Result");
    }
}
