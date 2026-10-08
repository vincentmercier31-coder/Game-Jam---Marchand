using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 10f;

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
    }
}
