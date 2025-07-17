using TMPro;
using UnityEngine.UI;
using UnityEngine;
using NUnit.Framework.Internal;
using System.Collections;

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
    public RawImage item1_1;
    public RawImage item2_1;
    public GameObject plus_1;
    public TextMeshProUGUI anyFish_1;
    public RectTransform timerFill_1;
    public RectTransform timerBackground_1;
    public RectTransform orderPanel_1;
    [Header("Order2")]
    public TextMeshProUGUI orderNumber_2;
    public RawImage item1_2;
    public RawImage item2_2;
    public GameObject plus_2;
    public TextMeshProUGUI anyFish_2;
    public RectTransform timerFill_2;
    public RectTransform timerBackground_2;
    public RectTransform orderPanel_2;
    [Header("Order3")]
    public TextMeshProUGUI orderNumber_3;
    public RawImage item1_3;
    public RawImage item2_3;
    public GameObject plus_3;
    public TextMeshProUGUI anyFish_3;
    public RectTransform timerFill_3;
    public RectTransform timerBackground_3;
    public RectTransform orderPanel_3;
    public int day = 1;
    public Texture[] fishSprites;
    [Header("Restaurant Score")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreIncreaseText;
    public RectTransform scoreTextTransform1;
    public RectTransform scoreTextTransform2;
    public RectTransform scoreTextTransform3;
    public TextMeshProUGUI scoreDecreaseText;
    public float scoreChangeDuration = 0.5f;
    public int scoreChangeHeight = 50;

    void Start()
    {
        scoreText.text = $"{OrderManager.Instance.storeScore}/ 5";
        dayCounterText.text = $"Days Open: {day}\n${OrderManager.Instance.currentBalance}";
        float openTime = OrderManager.Instance.openTime;
        float closeTime = OrderManager.Instance.closeTime;
        float totalDuration = OrderManager.Instance.dayDuration;
        dayTimerForeground.offsetMin = new Vector2(dayTimerBackground.rect.width * (openTime / totalDuration), dayTimerForeground.offsetMin.y);
        dayTimerForeground.offsetMax = new Vector2(-dayTimerBackground.rect.width * (1 - closeTime / totalDuration), dayTimerForeground.offsetMax.y);
        ResetOrderPanel(1);
        ResetOrderPanel(2);
        ResetOrderPanel(3);
    }

    public void Update()
    {
        UpdateDayTimer();
    }


    void UpdateDayTimer()
    {
        if (OrderManager.Instance.isOpen)
        {
            dayCounterText.text = $"Days Open: {day}\n${OrderManager.Instance.currentBalance}";
            isOpenText.text = "Open";
            isOpenText.color = Color.green;
            dayTimerBackground.gameObject.SetActive(true);
            dayTimerNeedle.gameObject.SetActive(true);
            dayTimerForeground.gameObject.SetActive(true);
        }
        else
        {
            dayCounterText.text = $"\n${OrderManager.Instance.currentBalance}";
            isOpenText.text = "Closed";
            isOpenText.color = Color.red;
            dayTimerBackground.gameObject.SetActive(false);
            dayTimerNeedle.gameObject.SetActive(false);
            dayTimerForeground.gameObject.SetActive(false);
        }
        float normalizedTime = OrderManager.Instance.currentTime / OrderManager.Instance.dayDuration;
        float leftBound = dayTimerBackground.rect.xMin;
        float rightBound = dayTimerBackground.rect.xMax;
        dayTimerNeedle.anchoredPosition = new Vector2(Mathf.Lerp(leftBound, rightBound, normalizedTime), dayTimerNeedle.anchoredPosition.y);
    }

    public void DisplayOrder(Order order, int orderSpot)
    {
        TextMeshProUGUI orderNumber = null;
        TextMeshProUGUI anyFish = null;
        GameObject item1 = null;
        GameObject item2 = null;
        GameObject plus = null;
        RectTransform timerFill = null;
        RectTransform timerBackground = null;
        RectTransform orderPanel = null;
        switch (orderSpot)
        {
            case 1:
                orderNumber = orderNumber_1;
                item1 = item1_1.gameObject;
                item2 = item2_1.gameObject;
                plus = plus_1;
                timerFill = timerFill_1;
                anyFish = anyFish_1;
                timerBackground = timerBackground_1;
                orderPanel = orderPanel_1;
                break;
            case 2:
                orderNumber = orderNumber_2;
                item1 = item1_2.gameObject;
                item2 = item2_2.gameObject;
                plus = plus_2;
                timerFill = timerFill_2;
                anyFish = anyFish_2;
                timerBackground = timerBackground_2;
                orderPanel = orderPanel_2;
                break;
            case 3:
                orderNumber = orderNumber_3;
                item1 = item1_3.gameObject;
                item2 = item2_3.gameObject;
                plus = plus_3;
                timerFill = timerFill_3;
                anyFish = anyFish_3;
                timerBackground = timerBackground_3;
                orderPanel = orderPanel_3;
                break;
        }
        orderNumber.text = $"#{order.myTicketNumber}";
        if(order.orderAnyFish)
        {
            plus.SetActive(false);
            anyFish.text = "Any 1 Fish";
            if(order.orderFishNumber > 1)
            {
                anyFish.text = "Any 2 Fish";
            }
        }
        else
        {
            item1.GetComponent<RawImage>().texture = fishSprites[order.orderFishNumber];
        }
        item1.gameObject.SetActive(!order.orderAnyFish);
        item2.gameObject.SetActive(false);
        anyFish.gameObject.SetActive(order.orderAnyFish);
        plus.SetActive(false);
        timerFill.sizeDelta = new Vector2(timerBackground.rect.width, timerBackground.rect.height);
        orderPanel.gameObject.SetActive(true);
    }
    public void ResetOrderPanel(int orderSpot)
    {
        RectTransform orderPanel = null;
        switch (orderSpot)
        {
            case 1:
                orderPanel = orderPanel_1;
                break;
            case 2:
                orderPanel = orderPanel_2;
                break;
            case 3:
                orderPanel = orderPanel_3;
                break;
        }
        orderPanel.gameObject.SetActive(false);
        scoreText.text = $"{OrderManager.Instance.storeScore}/ 5";
    }

    public void UpdateOrderTimer(int orderSpot, float timeTotal)
    {
        RectTransform timerFill = null;
        RectTransform timerBackground = null;
        switch (orderSpot)
        {
            case 1:
                timerFill = timerFill_1;
                timerBackground = timerBackground_1;
                break;
            case 2:
                timerFill = timerFill_2;
                timerBackground = timerBackground_2;
                break;
            case 3:
                timerFill = timerFill_3;
                timerBackground = timerBackground_3;
                break;
        }
        float fullWidth = timerBackground.rect.width;
        timerFill.sizeDelta = new Vector2(fullWidth * timeTotal, timerBackground.rect.height);
        timerFill.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, timeTotal);
    }
    public void SpawnScoreChangeText(float scoreChange, int orderSpot)
    {
        TextMeshProUGUI scoreChangeText = scoreChange > 0 ? scoreIncreaseText : scoreDecreaseText;
        RectTransform targetTransform = null;
        switch (orderSpot)
        {
            case 1:
                targetTransform = scoreTextTransform1;
                break;
            case 2:
                targetTransform = scoreTextTransform2;
                break;
            case 3:
                targetTransform = scoreTextTransform3;
                break;
        }
        Debug.Log($"Target transform: {targetTransform}");
        if (targetTransform != null)
        {
            scoreChangeText.color = scoreChange > 0 ? Color.green : Color.red;
            scoreChangeText.rectTransform.position = targetTransform.position;
            StartCoroutine(AnimateScoreText(scoreChangeText, targetTransform));
        }
    }

    private IEnumerator AnimateScoreText(TextMeshProUGUI textObject, RectTransform targetTransform)
    {
        TextMeshProUGUI spawnedText = Instantiate(textObject, targetTransform.position, Quaternion.identity, transform);
        spawnedText.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Vector2 startPos = spawnedText.rectTransform.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, scoreChangeHeight);
        float duration = scoreChangeDuration;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            spawnedText.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            // Fade out
            Color textColor = spawnedText.color;
            textColor.a = 1 - t;
            spawnedText.color = textColor;
            yield return null;
        }
        Destroy(spawnedText.gameObject);
    }
}
