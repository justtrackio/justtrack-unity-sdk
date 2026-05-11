using UnityEngine;
using UnityEngine.SceneManagement;

public class SdkInfoSceneButtonController : MonoBehaviour
{
    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }
}
