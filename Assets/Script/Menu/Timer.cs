using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timeRemaining;
    [SerializeField] GameObject endMenu;
    [SerializeField] GameObject timer;
    [SerializeField] GameObject score;
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (timeRemaining <= 0 )
        {
            endMenu.SetActive(true);
            timer.SetActive(false);
        }
    }
}
