using UnityEngine;

public class Time_Image_Change : MonoBehaviour
{
    public static Time_Image_Change instance;

    [SerializeField] GameObject[] coffee;
    [SerializeField] GameObject background;
    [SerializeField] GameObject clock;

    private int maxTypingTime;

    private float ratio;
    private float clock_rot;
    private float Bground_rot;

    private int nature;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         if (instance == null)
        {
            // 自身をインスタンスとする
            instance = this;
        }
        else
        {
            // インスタンスが複数存在しないように、既に存在していたら自身を消去する
            Destroy(gameObject);
        }
         nature = 0;
    }

    public void Timeset(int time)
    {
        maxTypingTime = time;
    }

    public void Clock_Bground_rot(float time)
    {
        //時計 -150~-540
        //背景 320~510

        ratio = time / maxTypingTime;
        
        clock_rot = 390 / ratio;
        Bground_rot = 190 / ratio;

        clock.transform.rotation = Quaternion.Euler(0f, 0f, (-150f - clock_rot));
        background.transform.rotation = Quaternion.Euler(0f, 0f, (320f+Bground_rot));]

        if((int)time > nature)
        {
            foreach(GameObject coffee in coffee)
            {
                coffee.SetActive(false);
            }
            coffee[nature].SetActive(true);
        }
    }

    
}
