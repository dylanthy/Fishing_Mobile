using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrdersUI : MonoBehaviour
{
    public Button isOpenToggle;
    public RectTransform orderTimer;
    public RectTransform happiness;
    public TextMeshProUGUI money;

    private OrderManager myOrderManager;
    private float initialWidth; // Stores the original width of orderTimer

    void Start()
    {
        myOrderManager = FindFirstObjectByType<OrderManager>();
        OpenToggle();

        // Store the initial width of the orderTimer UI element
        initialWidth = orderTimer.sizeDelta.x;
    }

    void Update()
    {
        UpdateOrderTimerWidth();
        UpdateHappinessWidth();
        UpdateCurrency();
    }

    public void OpenToggle()
    {
        TextMeshProUGUI buttonText = isOpenToggle.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = myOrderManager.isOpen ? "Currently Open" : "Currently Closed";
        }
    }

    private void UpdateOrderTimerWidth()
    {
        if (orderTimer != null)
        {
            // Get the percentage of time remaining
            float percentage = Mathf.Clamp01(myOrderManager.remainingTimeBetweenCustomers / myOrderManager.timeBetweenCustomers);

            // Update the width of the orderTimer based on the percentage
            orderTimer.sizeDelta = new Vector2(initialWidth * percentage, orderTimer.sizeDelta.y);
        }
    }
    private void UpdateHappinessWidth()
    {
        if (happiness != null)
        {
            // Get the percentage of time remaining
            float percentage = Mathf.Clamp01(myOrderManager.remainingHappiness / myOrderManager.totalHappiness);

            // Update the width of the orderTimer based on the percentage
            happiness.sizeDelta = new Vector2(initialWidth * percentage, happiness.sizeDelta.y);
        }
    }

    void UpdateCurrency()
    {
        money.text = $"${myOrderManager.currency}";
    }
}
