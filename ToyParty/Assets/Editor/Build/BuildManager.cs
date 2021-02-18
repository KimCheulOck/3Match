using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildManager
{
    public static void Build_AOS()
    {
        string[] scenes = new string[] { "Assets/Scenes/GameScene.unity" };
        BuildPipeline.BuildPlayer(scenes, "Build.apk", BuildTarget.Android, BuildOptions.None);
    }
}
