using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Mathematics;


public class Gui : MonoBehaviour
{ public  GameObject image;
    public GameObject Lose;
    public Transform menu;
    public Button pauseButton;
    public Button continueButton;
    public Button homeButton;

    public Text Timegame;
    public Text Money;
    public static CanvasGroup canvasGroupLose;
    public static CanvasGroup canvasGroupBoold;
    public GameObject text;
    static Gui instanceGUI;//单例
  public  static  bool ISOGUI = false;
    public static Unity.Mathematics.Random random;
    public static float timer = 0.4f;
    private float times = 0f;
    public int gameTime =1;
    public static bool isOpenMenu;
    public static bool isOpenSmallMap;
    public Transform smallMapPanel;
    public static CanvasGroup menuCanvasGroup;
    public static CanvasGroup smallMapCanvasGroup;
    [SerializeField]
    private const float fadeSpeed = 1.5f;
    // Start is called before the first frame update
    void Awake()
    {
        random = new Unity.Mathematics.Random(15210);
        if (instanceGUI != null && instanceGUI != this)
            Destroy(gameObject);
        else
            instanceGUI = this;
        canvasGroupBoold = image.GetComponent<CanvasGroup>();
        canvasGroupLose = Lose.GetComponent<CanvasGroup>();
        menuCanvasGroup = menu.GetComponent<CanvasGroup>();
        smallMapCanvasGroup = smallMapPanel.GetComponent<CanvasGroup>();
        pauseButton.onClick.AddListener(OnMenu);
        homeButton.onClick.AddListener(OnHomeScene);
        continueButton.onClick.AddListener(OnContinue);
    }

    // Update is called once per frame
    void Update()
    { times += Time.deltaTime;
        if (times >= 1)
        {
            gameTime -=1;
            times -= 1;
        }
       
        timer -= Time.deltaTime;
        if (ISOGUI && timer <= 0)
        {

            OffBlood();

        }
        if (gameTime >= 0)
        {
            Timegame.text = gameTime.ToString();
        }
        if(gameTime == 0)
        {
            Settings.PlayerDied();

        }
       
        
        Money.text =  CollisionGet.money.ToString();
        if (isOpenMenu)
        {
            if (menuCanvasGroup.alpha<1)
            {
                menuCanvasGroup.alpha = math.lerp(menuCanvasGroup.alpha, 1, Time.deltaTime* fadeSpeed);
                if (math.abs(menuCanvasGroup.alpha-1)<=0.05f)
                {
                    menuCanvasGroup.alpha = 1;
                    isOpenMenu = false;
                }
            }
        }
        if (isOpenSmallMap)
        {
             if (smallMapCanvasGroup.alpha<1)
            {
                smallMapCanvasGroup.alpha = math.lerp(smallMapCanvasGroup.alpha, 1, Time.deltaTime * fadeSpeed);
                if (math.abs(smallMapCanvasGroup.alpha-1)<=0.05f)
                {
                    smallMapCanvasGroup.alpha = 1;
                    isOpenSmallMap = false;
                }
            }
        }

    }

  public   static void NoMoney()
    {
        instanceGUI.text.SetActive(true);
    }
 public    static void OffMoney()
    {
        instanceGUI.text.SetActive(false);
    }

    public static void OnBlood()
    {

        timer = random.NextFloat(0.4f,1.2f);
        canvasGroupBoold.alpha = 1;
        canvasGroupBoold.interactable = true;
        canvasGroupBoold.blocksRaycasts = true;
        ISOGUI = true;
        
    }

    private static void  OffBlood()
    {
        canvasGroupBoold.alpha = 0;
        canvasGroupBoold.interactable = false;
        canvasGroupBoold.blocksRaycasts = false ;
       
    }
    public static void OnLose()
    {

        timer = random.NextFloat(0.4f, 1.2f);
        canvasGroupLose.alpha =1;
        canvasGroupLose.interactable = true;
        canvasGroupLose.blocksRaycasts = true;
        ISOGUI = true;

    }
    public static  void OnMenu()
    {
        isOpenMenu = true;
        menuCanvasGroup.blocksRaycasts = true;
    }
    public static void OnContinue()
    {
        isOpenMenu = false;
        menuCanvasGroup.alpha = 0;
        menuCanvasGroup.blocksRaycasts = false;
    }
    public static void OnHomeScene()
    {
        SceneManager.LoadScene("One");
    }
    public static void SetSmallMap()
    {
        smallMapCanvasGroup.gameObject.SetActive(true);
        smallMapCanvasGroup.alpha = 0;
        isOpenSmallMap = true;
    }
}
