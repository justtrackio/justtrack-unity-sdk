using JustTrack;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiscellaneousEventsButtonController : MonoBehaviour
{
    public void OnClickPlainEvent()
    {
        var userEvent = new AppEvent("plain_event")
            .AddDimension("plain_event_dimension_0", "value_0")
            .AddDimension("plain_event_dimension_1", "value_1");
        JustTrackSDK.Track(userEvent);
    }

    public void OnClickCountEvent()
    {
        var userEvent = new AppEvent("count_event", 41.1, Unit.Count)
            .AddDimension("count_event_dimension_0", "value_0")
            .AddDimension("count_event_dimension_1", "value_1");
        JustTrackSDK.Track(userEvent);
    }

    public void OnClickMillisecondsEvent()
    {
        var userEvent = new AppEvent("milliseconds_event", 41.2, Unit.Milliseconds)
            .AddDimension("milliseconds_event_dimension_0", "value_0")
            .AddDimension("milliseconds_event_dimension_1", "value_1");
        JustTrackSDK.Track(userEvent);
    }

    public void OnClickSecondsEvent()
    {
        var userEvent = new AppEvent("seconds_event", 41.3, Unit.Seconds)
            .AddDimension("seconds_event_dimension_0", "value_0")
            .AddDimension("seconds_event_dimension_1", "value_1");
        JustTrackSDK.Track(userEvent);
    }

    public void OnClickMoneyEvent()
    {
        var userEvent = new AppEvent("money_event", new Money(100.56, "EUR"));
        JustTrackSDK.Track(userEvent);
    }

    public void OnClickTestVariants()
    {
#if !UNITY_WEBGL
        StartCoroutine(TestVariantsWithDelay());
#endif
    }

#if !UNITY_WEBGL
    private IEnumerator TestVariantsWithDelay()
    {
        Debug.Log("Starting experiment variants test sequence...");
        JustTrackSDK.SetExperimentVariant(
            "button_color_test",
            "red_button",
            new string[] { "ui", "color" },
            null,
            () => Debug.Log("Successfully set Button Color Test variant"),
            (error) => Debug.LogError($"Failed to set Button Color Test variant: {error}")
        );
        yield return new WaitForSeconds(3f);

        JustTrackSDK.SetExperimentVariant(
            "checkout_flow_test",
            "control",
            new string[] { },
            null,
            () => Debug.Log("Successfully set Checkout Flow Test variant"),
            (error) => Debug.LogError($"Failed to set Checkout Flow Test variant: {error}")
        );
        yield return new WaitForSeconds(3f);

        JustTrackSDK.SetExperimentVariant(
            "premium_features_test",
            "variant_a",
            new string[] { "premium", "features", "test_a" },
            null,
            () => Debug.Log("Successfully set Premium Feature Test variant"),
            (error) => Debug.LogError($"Failed to set Premium Feature Test variant: {error}")
        );
        yield return new WaitForSeconds(3f);

        JustTrackSDK.SetExperimentVariant(
            "retention_an_our_ago_test",
            "week_2",
            new string[] { "retention" },
            System.DateTime.Now.AddHours(-1),
            () => Debug.Log("Successfully set Custom Date Test variant"),
            (error) => Debug.LogError($"Failed to set Custom Date Test variant: {error}")
        );
        yield return new WaitForSeconds(3f);

        JustTrackSDK.SetExperimentVariant(
            "ein_test_der_prüft_dass_es_im_experiment_ein_zeichen_gibt_das_kein_ascii_zeichen_ist",
            "variante",
            null,
            null,
            () => Debug.Log("[Unexpectedly] Successfully set ein_test_der_prüft_dass_es_im_experiment_ein_zeichen_gibt_das_kein_ascii_zeichen_ist"),
            (error) => Debug.LogError($"Failed to set ein_test_der_prüft_dass_es_im_experiment_ein_zeichen_gibt_das_kein_ascii_zeichen_ist [as expected]: {error}")
        );
        yield return new WaitForSeconds(3f);

        JustTrackSDK.SetExperimentVariant(
            "test_that_checks_that_there_is_a_variant_with_a_symbol_that_is_not_an_ascii_symbol",
            "eine_variante_die_ein_zeichen_enthält_dass_kein_ascii_zeichen_ist",
            null,
            null,
            () => Debug.Log("[Unexpectedly] Successfully set test_that_checks_that_there_is_a_variant_with_a_symbol_that_is_not_an_ascii_symbol"),
            (error) => Debug.LogError($"Failed to set test_that_checks_that_there_is_a_variant_with_a_symbol_that_is_not_an_ascii_symbol [as expected]: {error}")
        );
        yield return new WaitForSeconds(3f);

        JustTrackSDK.SetExperimentVariant(
            "exceeding_max_tag_number_test",
            "week_2",
            new string[] { "one", "two", "three", "four", "five", "six" },
            null,
            () => Debug.Log("[Unexpectedly] Successfully set exceeding_max_tag_number_test"),
            (error) => Debug.LogError($"Failed to set exceeding_max_tag_number_test [as expected]: {error}")
        );
        yield return new WaitForSeconds(3f);

        JustTrackSDK.SetExperimentVariant(
            "not_ascii_tag_test",
            "week_2",
            new string[] { "eins", "zwei", "drei", "vier", "fünf" },
            null,
            () => Debug.Log("[Unexpectedly] Successfully set not_ascii_tag_test"),
            (error) => Debug.LogError($"Failed to set not_ascii_tag_test [as expected]: {error}")
        );

        Debug.Log("Experiment variants test sequence completed!");
    }
#endif

    public void OnClickPredefined()
    {
        JustTrackSDK.Track(new JtProgressionEvent("test-pJtAction", "test-pJtProgression1", "test-pJtProgression2", "test-pJtProgression3"));
        JustTrackSDK.Track(new JtProgressionEvent("test-pJtAction", 1.0, TimeUnitGroup.Milliseconds, "test-pJtProgression1", "test-pJtProgression2", "test-pJtProgression3"));
        JustTrackSDK.Track(new JtResourceEvent("test-pJtAction", "test-pJtItemType", "test-pJtItemName", "test-pJtItemId"));
        JustTrackSDK.Track(new JtResourceEvent("test-pJtAction", 1.0, "test-pJtItemType", "test-pJtItemName", "test-pJtItemId"));
        JustTrackSDK.Track(new JtPurchaseEvent("test-pJtAction", "test-pJtProductId", "test-pJtProductType", "test-pJtToken"));
        JustTrackSDK.Track(new JtPurchaseEvent("test-pJtAction", "test-pJtProductId", "test-pJtProductType", 1.0, "test-pJtToken"));
        JustTrackSDK.Track(new JtPurchaseEvent(JtPurchaseEvent.Action.VIEW, "test-pJtProductId", "test-pJtProductType", "test-pJtToken"));
        JustTrackSDK.Track(new JtPurchaseEvent(JtPurchaseEvent.Action.CLICK, "test-pJtProductId", "test-pJtProductType", 1.0, "test-pJtToken"));
        JustTrackSDK.Track(new JtAdEvent("test-pJtAction", "test-pJtAdBundleId", "test-pJtAdInstanceName", "test-pJtAdNetwork", "test-pJtAdPlacement", "test-pJtAdSdk", "test-pJtAdSegment", "test-pJtAdUnit", "test-pJtAdTestGroup"));
        JustTrackSDK.Track(new JtAdEvent("test-pJtAction", 1.0, TimeUnitGroup.Milliseconds, "test-pJtAdBundleId", "test-pJtAdInstanceName", "test-pJtAdNetwork", "test-pJtAdPlacement", "test-pJtAdSdk", "test-pJtAdSegment", "test-pJtAdUnit", "test-pJtAdTestGroup"));
        JustTrackSDK.Track(new JtLoginEvent("test-pJtAction", "test-pJtMethod"));
    }

    public void OnClickCustom()
    {
        var userEvent = new AppEvent("unique_event")
            .AddDimension(JustTrack.Dimension.JT_CATEGORY, "category")
            .AddDimension(Dimension.JT_CONTEXT, "context")
            .AddDimension(Dimension.JT_DETAIL, "detail")
            .AddDimension(Dimension.JT_LOCATION, "location")
            .AddDimension(Dimension.JT_STATE, "state")
            .AddDimension(Dimension.JT_TRIGGER, "trigger");
        JustTrackSDK.Track(userEvent);
    }

    public void OnClickFirebase()
    {
#if !UNITY_WEBGL
        JustTrackSDK.SetFirebaseAppInstanceId(TestAppCredentials.FirebaseAppInstanceId);
#endif
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }
}