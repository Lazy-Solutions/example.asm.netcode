static class Example
{

    [InitializeOnLoadMethod]
    static void OnLoad()
    {
        SceneView.duringSceneGui += SceneView_duringSceneGui;
    }

    static void SceneView_duringSceneGui(SceneView view)
    {

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        var c = GUI.color;
        GUI.color = new Color(0.8f, 0.8f, 0.8f, 1);
        GUILayout.BeginVertical(GUI.skin.window);
        GUI.color = c;


        var f = EditorStyles.label.fontSize;
        EditorStyles.label.fontSize = 16;

        var f2 = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 16;

        var c1 = new GUIContent($"Welcome to {Application.productName} example!");
        var c2 = new GUIContent("Advanced Scene Manager must be installed in order for this example to be functional.");
        var c3 = new GUIContent("Due to how Netcode works, we strongly recommend you read the readme file.");
        EditorGUILayout.LabelField(c1, GUILayout.Width(EditorStyles.label.CalcSize(c1).x));
        EditorGUILayout.LabelField(c2, GUILayout.Width(EditorStyles.label.CalcSize(c2).x));
        EditorGUILayout.LabelField(c3, GUILayout.Width(EditorStyles.label.CalcSize(c3).x));

        EditorGUILayout.Space(22);

        if (GUILayout.Button("Open package manager", GUILayout.Height(42)))
            UnityEditor.PackageManager.UI.Window.Open("");

        if (GUILayout.Button("Open asset store", GUILayout.Height(42)))
            Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/advanced-scene-manager-174152");

        GUI.skin.button.fontSize = f2;
        EditorStyles.label.fontSize = f;
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();

    }

}
