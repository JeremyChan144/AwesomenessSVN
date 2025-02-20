using System.Collections;
using System.Collections.Generic;
using TMPro;  // Add this to use TextMeshPro
using UnityEngine;

public class ScoreUpdate : MonoBehaviour
{
    // Reference to the TextMeshPro component
    public TextMeshProUGUI scoreText; // Make sure the TextMeshProUGUI component is attached in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Update the score display initially (optional)
        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Function to increase the score
    public void IncreaseScore(int amount)
    {
        GameManager.Instance.currentScore += amount; // Increase the score by the specified amount
        UpdateScoreText(); // Call the function to update the TextMeshPro text
    }

    // Function to update the displayed score on the screen
    private void UpdateScoreText()
    {
        // Set the text of the TextMeshProUGUI component to the current score
        if (scoreText != null)
        {
           scoreText.text = "Score: " + GameManager.Instance.currentScore.ToString(); // Update the score text
        }
    }
}
