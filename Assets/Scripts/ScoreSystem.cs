using UnityEngine;


public class ScoreSystem : MonoBehaviour
{
    public int score = 0; // Score Storage 

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"Points Added: +{amount} → Total Score: {score}");
    }
}
