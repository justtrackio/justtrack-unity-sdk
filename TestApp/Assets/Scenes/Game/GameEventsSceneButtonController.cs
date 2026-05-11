using JustTrack;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEventsButtonController : MonoBehaviour
{
    private int level = 1;
    private bool levelIsFinished = false;

    public void OnClickJtLevelStart()
    {
        if (levelIsFinished)
        {
            level += 1;
            levelIsFinished = false;
        }
        var levelStartEvent = new JtProgressionEvent("start", LevelName());
        JustTrackSDK.Track(EventWithDefaultDimensions(levelStartEvent));
    }

    public void OnClickJtLevelFail()
    {
        var levelFailEvent = new JtProgressionEvent("fail", LevelName());
        JustTrackSDK.Track(EventWithDefaultDimensions(levelFailEvent));
    }

    public void OnClickJtLevelFinish()
    {
        levelIsFinished = true;
        var levelFinishEvent = new JtProgressionEvent("complete", LevelName());
        JustTrackSDK.Track(EventWithDefaultDimensions(levelFinishEvent));
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }

    private string LevelName()
    {
        return $"level_{level}";
    }

    private AppEvent EventWithDefaultDimensions(AppEvent userEvent)
    {
        return userEvent
            .AddDimension("scene", "PredefinedEvents")
            .AddDimension("platform", Application.platform.ToString());
    }
}
