using UnityEngine;

public class Time_Image_Change : MonoBehaviour
{
    public static Time_Image_Change instance;

    [SerializeField] GameObject[] coffee;
    [SerializeField] GameObject background;
    [SerializeField] GameObject clock;

    [SerializeField] GameObject[] pc_image;

    private int maxTypingTime;

    private float ratio;
    private float clock_rot;
    private float Bground_rot;

    private int nature;


    private int pc_int;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
         if (instance == null)
        {
            // ���g���C���X�^���X�Ƃ���
            instance = this;
        }
        else
        {
            // �C���X�^���X���������݂��Ȃ��悤�ɁA���ɑ��݂��Ă����玩�g����������
            Destroy(gameObject);
        }
         nature = 0;
        pc_int = 0;
    }

    public void Timeset(float time)
    {
        maxTypingTime = (int)time;
    }

    public void Clock_Bground_rot(float time)
    {
        //���v -150~-540
        //�w�i 320~510

        ratio = time / 13;
        
        clock_rot = 390 * ratio;
        Bground_rot = 190 * ratio;

        clock.transform.rotation = Quaternion.Euler(0f, 0f, (-150f - clock_rot));
        background.transform.rotation = Quaternion.Euler(0f, 0f, (320f+Bground_rot));

        if((int)time > nature)
        {
            foreach(GameObject coffee in coffee)
            {
                coffee.SetActive(false);
            }
            coffee[nature].SetActive(true);
        }
    }

    public void typing_pc()
    {
        foreach (GameObject pc in pc_image)
        {
            pc.SetActive(false);
        }
        pc_image[pc_int].SetActive(true);
        pc_int++;
        if (pc_image.Length <= pc_int)
        {
            pc_int = 0;
        }
    }
}
