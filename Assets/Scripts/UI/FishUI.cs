using TMPro;
using UnityEngine;

public class FishUI : MonoBehaviour
{
    public TextMeshProUGUI fishCounter;
    private int fishCount = 0;
    public TextMeshProUGUI BallCounter;

    public void UpdateFish()
    {
        fishCount++;
        fishCounter.text = $"Fish: {fishCount}";
    }
    public void UpdateBall(int totalBalls)
    {
        BallCounter.text = $"Ball: {totalBalls}";
    }
}
