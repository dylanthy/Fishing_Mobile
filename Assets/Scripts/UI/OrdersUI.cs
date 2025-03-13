using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class OrdersUI : MonoBehaviour
{
    private static OrdersUI instance;
    public static OrdersUI Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }
    public TextMeshProUGUI dayCounterText;
    public RectTransform dayTimerNeedle;
    public RectTransform dayTimerBackground;
    public RectTransform dayTimerForeground;
    public TextMeshProUGUI isOpenText;

    [Header("Order1")]
    public TextMeshProUGUI orderNumber_1;
    public Image item1_1;
    public Image item2_1;
    public GameObject plus_1;
    public RectTransform timerFill_1;
    public RectTransform timerBackground_1;
    public RectTransform order1Panel_1;
    [Header("Order2")]
    public TextMeshProUGUI orderNumber_2;
    public Image item1_2;
    public Image item2_2;
    public GameObject plus_2;
    public RectTransform timerFill_2;
    public RectTransform timerBackground_2;
    public RectTransform order1Panel_2;
    [Header("Order3")]
    public TextMeshProUGUI orderNumber_3;
    public Image item1_3;
    public Image item2_3;
    public GameObject plus_3;
    public RectTransform timerFill_3;
    public RectTransform timerBackground_3;
    public RectTransform order1Panel_3;
    public int day = 1;

    void Start()
    {
        dayCounterText.text = $"Day: {day}";
        float openTime = OrderManager.Instance.openTime;
        float closeTime = OrderManager.Instance.closeTime;
        float totalDuration = OrderManager.Instance.dayDuration;
        dayTimerForeground.offsetMin = new Vector2(dayTimerBackground.rect.width * (openTime / totalDuration), dayTimerForeground.offsetMin.y);
        dayTimerForeground.offsetMax = new Vector2(-dayTimerBackground.rect.width * (1 - closeTime / totalDuration), dayTimerForeground.offsetMax.y);
    }

    public void Update()
    {
        dayCounterText.text = $"Day: {day}";
        UpdateDayTimer();
    }


    void UpdateDayTimer()
    {
        if(OrderManager.Instance.isOpen)
        {
            isOpenText.text = "Open";
            isOpenText.color = Color.green;
        }
        else
        {
            isOpenText.text = "Closed";
            isOpenText.color = Color.red;
        }
        float normalizedTime = OrderManager.Instance.currentTime / OrderManager.Instance.dayDuration;
        float leftBound = dayTimerBackground.rect.xMin;
        float rightBound = dayTimerBackground.rect.xMax;
        dayTimerNeedle.anchoredPosition = new Vector2(Mathf.Lerp(leftBound, rightBound, normalizedTime), dayTimerNeedle.anchoredPosition.y);
    }

    public void DisplayOrder(Order order)
    {
        if (order.orderAnyFish)
        {
            if (order.orderFishNumber == 1)
            {
                Debug.Log("Customer wants 1 fish");
            }
            else
            {
                Debug.Log($"Customer wants {order.orderFishNumber} fish");
            }
        }
        else
        {
            Debug.Log($"Customer wants a specific fish: {order.orderFishNumber}");
        }
    }

}
