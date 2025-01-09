using UnityEngine;
using UnityEditor;

public class HeatmapEditorWindow : EditorWindow
{
    private HeatmapDataFetcher heatmapFetcher;
    private string[] dataTypes = { "Attack", "Interaction", "Path", "Damage", "Death", "Pause" };
    private int selectedDataType = 0;

    private Color pointColor = Color.red;
    private GameObject pointPrefab;

    [MenuItem("Window/Heatmap Editor")]
    public static void ShowWindow()
    {
        HeatmapEditorWindow window = GetWindow<HeatmapEditorWindow>("Heatmap Editor");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Heatmap Editor Settings", EditorStyles.boldLabel);

        heatmapFetcher = (HeatmapDataFetcher)EditorGUILayout.ObjectField("Heatmap Data Fetcher", heatmapFetcher, typeof(HeatmapDataFetcher), true);
        if (heatmapFetcher == null)
        {
            EditorGUILayout.HelpBox("Select a HeatmapDataFetcher to customize.", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);
        GUILayout.Label("Point Settings", EditorStyles.boldLabel);

        selectedDataType = EditorGUILayout.Popup("Data Type", selectedDataType, dataTypes);

        pointColor = EditorGUILayout.ColorField("Point Color", pointColor);

        pointPrefab = (GameObject)EditorGUILayout.ObjectField("Point Prefab", pointPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Apply Changes"))
        {
            ApplySettingsToDataType(selectedDataType);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Preview Heatmap"))
        {
            PreviewHeatmap(selectedDataType);
        }
    }

    private void ApplySettingsToDataType(int dataTypeIndex)
    {
        if (heatmapFetcher == null) return;

        switch (dataTypes[dataTypeIndex])
        {
            case "Attack":
                heatmapFetcher.attackColor = pointColor;
                heatmapFetcher.heatmapPointPrefab = pointPrefab ?? heatmapFetcher.heatmapPointPrefab;
                break;

            case "Interaction":
                heatmapFetcher.interactionColor = pointColor;
                break;

            case "Path":
                heatmapFetcher.pathColor = pointColor;
                break;

            case "Damage":
                heatmapFetcher.damageColor = pointColor;
                break;

            case "Death":
                heatmapFetcher.deathColor = pointColor;
                break;

            case "Pause":
                heatmapFetcher.pauseColor = pointColor;
                break;
        }

        EditorUtility.SetDirty(heatmapFetcher);
    }

    private void PreviewHeatmap(int dataTypeIndex)
    {
        if (heatmapFetcher == null) return;

        switch (dataTypes[dataTypeIndex])
        {
            case "Attack":
                heatmapFetcher.StartCoroutine(heatmapFetcher.FetchAttackData());
                break;

            case "Interaction":
                heatmapFetcher.StartCoroutine(heatmapFetcher.FetchInteractionData());
                break;

            case "Path":
                heatmapFetcher.StartCoroutine(heatmapFetcher.FetchPathData());
                break;

            case "Damage":
                heatmapFetcher.StartCoroutine(heatmapFetcher.FetchDamageData());
                break;

            case "Death":
                heatmapFetcher.StartCoroutine(heatmapFetcher.FetchDeathData());
                break;

            case "Pause":
                heatmapFetcher.StartCoroutine(heatmapFetcher.FetchPauseData());
                break;
        }
    }
}
