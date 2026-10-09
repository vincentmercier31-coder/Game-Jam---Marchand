using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timeRemaining;
    [SerializeField] GameObject endMenu;
    [SerializeField] GameObject timer;
    [SerializeField] GameObject score;
    [SerializeField] Vector3 scorePos;
    [SerializeField] AudioSource Jingle;
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (timeRemaining <= 0 )
        {
            Jingle.Play();
            endMenu.SetActive(true);
            timer.SetActive(false);
            score.transform.SetPositionAndRotation(scorePos, transform.rotation);
        }
    }
}
