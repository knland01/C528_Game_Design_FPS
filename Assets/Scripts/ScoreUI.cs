using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public ScoreSystem scoreSystem;
    public TextMeshProUGUI scoreText;

    void Update()
    {
        scoreText.text = "Score: " + scoreSystem.score;
    }
}
