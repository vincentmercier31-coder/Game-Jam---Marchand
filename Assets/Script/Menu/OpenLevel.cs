using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenLevel : MonoBehaviour
{
    public void OpLevel(string levelToOpen)
    {
        SceneManager.LoadScene(levelToOpen);
    }
}
