#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class XcodeProjectModifier
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        ModifyInfoPlist(buildPath);
    }

    private static void ModifyInfoPlist(string buildPath)
    {
        string infoPlistPath = buildPath + "/Info.plist";
        PlistDocument infoPlist = new PlistDocument();
        infoPlist.ReadFromFile(infoPlistPath);
        
        PlistElementDict rootDict = infoPlist.root;
        PlistElementArray skAdNetworkItemsArray = rootDict.CreateArray("SKAdNetworkItems");
        PlistElementDict skAdNetworkItemDict = skAdNetworkItemsArray.AddDict();
        skAdNetworkItemDict.SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork");

        infoPlist.WriteToFile(infoPlistPath);
    }
}
#endif