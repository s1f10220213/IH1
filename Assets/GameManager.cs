using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    
    [Header("Typing")]
    [SerializeField] float typingTime;
    [SerializeField] float typingAcuracy;
    [SerializeField] float typingNumber;

    [Header("WakeUp")]
    [SerializeField] float WakeUpPushNumber;

    [Header("Score")]
    [SerializeField] float score;

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
