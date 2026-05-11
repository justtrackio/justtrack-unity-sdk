#if !UNITY_WEBGL
using System.Collections.Generic;
using System.Text;
using JustTrack;
using UnityEngine;

public class RemoteConfigSceneController : MonoBehaviour
{
    public void OnClickFetch()
    {
        JustTrackSDK.GetRemoteConfig().Fetch(
            () =>
            {
                Log("Fetch RemoteConfig (Success)");
            },
            (message) =>
            {
                Log("Fetch RemoteConfig (Failure) " + message);
            });
    }

    public void OnClickActivate()
    {
        Assignment[] assignments = JustTrackSDK.GetRemoteConfig().GetAll();
        Log("GetAll RemoteConfig (Success)");
        List<string> experimentIds = new List<string>();
        if (assignments != null)
        {
            for (int i = 0; i < assignments.Length; i++)
            {
                string experimentId = assignments[i].ExperimentId;
                if (!string.IsNullOrEmpty(experimentId))
                {
                    Log("GetAll RemoteConfig expId: " + experimentId + ", configKey: " + assignments[i].ConfigKey + ", configValue: " + assignments[i].ConfigValue);

                    experimentIds.Add(experimentId);
                }
            }
        }

        if (experimentIds.Count == 0)
        {
            Log("No RemoteConfig experiments to activate");
            return;
        }

        JustTrackSDK.GetRemoteConfig().Activate(
            experimentIds.ToArray(),
            () =>
            {
                Log("Activate RemoteConfig (Success)");
            },
            (activateMessage) =>
            {
                Log("Activate RemoteConfig (Failure) " + activateMessage);
            });
    }

    public void OnClickFetchAndActivate()
    {
        JustTrackSDK.GetRemoteConfig().FetchAndActivate(
            () =>
            {
                Log("Fetch and Activate RemoteConfig (Success)");
                Assignment[] assignments = JustTrackSDK.GetRemoteConfig().GetAll();
                if (assignments == null || assignments.Length == 0)
                {
                    Log("No RemoteConfig experiments to activate");
                    return;
                }

                for (int i = 0; i < assignments.Length; i++)
                {
                    if (!string.IsNullOrEmpty(assignments[i].ExperimentId))
                    {
                        return;
                    }
                }

                Log("No RemoteConfig experiments to activate");
            },
            (message) =>
            {
                Log("Fetch and Activate RemoteConfig (Failure) " + message);
            });
    }

    public void OnClickSetConfig()
    {
        JustTrackSDK.GetRemoteConfig().SetConfig(new JusttrackRemoteConfigSettings(60));
        Log("Set RemoteConfig settings (1 minute)");
    }

    public void onClickFetchAllValueType()
    {
        Assignment[] assignments = JustTrackSDK.GetRemoteConfig().GetAll();
        if (assignments == null || assignments.Length == 0)
        {
            Log("No RemoteConfig assignments to read");
            return;
        }

        for (int i = 0; i < assignments.Length; i++)
        {
            string configKey = assignments[i].ConfigKey;
            string configValue = assignments[i].ConfigValue;
            StringBuilder builder = new StringBuilder();
            builder.Append("Key [").Append(configKey).Append("], Value [").Append(configValue).Append("]\n");
            builder.Append("getInt(): [").Append(JustTrackSDK.GetRemoteConfig().GetInt(configKey)?.ToString() ?? "null").Append("]\n");
            builder.Append("getBoolean(): [").Append(JustTrackSDK.GetRemoteConfig().GetBoolean(configKey)?.ToString() ?? "null").Append("]\n");
            builder.Append("getDouble(): [").Append(JustTrackSDK.GetRemoteConfig().GetDouble(configKey)?.ToString() ?? "null").Append("]\n");
            builder.Append("getLong(): [").Append(JustTrackSDK.GetRemoteConfig().GetLong(configKey)?.ToString() ?? "null").Append("]\n");
            builder.Append("getString(): [").Append(JustTrackSDK.GetRemoteConfig().GetString(configKey) ?? "null").Append("]\n");
            builder.Append("---------------------------------------------");
            Log(builder.ToString());
        }
    }

    private void Log(string message)
    {
        Debug.Log(">RemoteConfig< " + message);
    }
}
#endif