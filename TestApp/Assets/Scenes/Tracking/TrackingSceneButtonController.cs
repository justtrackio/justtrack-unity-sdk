using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackingSceneButtonController : MonoBehaviour
{
    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }
}
