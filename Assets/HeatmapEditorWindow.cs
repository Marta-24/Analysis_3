using UnityEngine;
using UnityEditor;

public class HeatmapEditorWindow : EditorWindow
{
    private HeatmapDataFetcher heatmapFetcher;
    private string[] dataTypes = { "None", "Attack", "Interaction", "Path", "Damage", "Death", "Pause" };

    private Color pointColor;
    private GameObject pointPrefab;
    private float pointSize;

    private HeatmapDataFetcher.DataType lastDataType = HeatmapDataFetcher.DataType.None;

    [MenuItem("Window/Heatmap Editor")]
    public static void ShowWindow()
    {
        HeatmapEditorWindow window = GetWindow<HeatmapEditorWindow>("Heatmap Editor");
        window.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += AutoRefresh;
    }

    private void OnDisable()
    {
        EditorApplication.update -= AutoRefresh;
    }

    private void AutoRefresh()
    {
        if (heatmapFetcher == null) return;

        if (lastDataType != heatmapFetcher.currentDataType)
        {
            lastDataType = heatmapFetcher.currentDataType;
            LoadSettingsForCurrentDataType();
            Repaint();
        }
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

        int selectedDataTypeIndex = (int)heatmapFetcher.currentDataType;
        EditorGUILayout.LabelField("Current Data Type:", dataTypes[selectedDataTypeIndex]);

        if (heatmapFetcher.currentDataType == HeatmapDataFetcher.DataType.None)
        {
            EditorGUILayout.HelpBox("No dataset selected. Press F1-F6 to display a dataset.", MessageType.Info);
            return;
        }

        pointColor = EditorGUILayout.ColorField("Point Color", pointColor);
        pointPrefab = (GameObject)EditorGUILayout.ObjectField("Point Prefab", pointPrefab, typeof(GameObject), false);
        pointSize = EditorGUILayout.FloatField("Point Size", pointSize);

        if (GUILayout.Button("Apply Changes"))
        {
            ApplySettingsToCurrentDataType();
        }
    }

    private void LoadSettingsForCurrentDataType()
    {
        if (heatmapFetcher == null) return;

        switch (heatmapFetcher.currentDataType)
        {
            case HeatmapDataFetcher.DataType.Attack:
                pointColor = heatmapFetcher.attackColor;
                pointPrefab = heatmapFetcher.attackPrefab;
                pointSize = heatmapFetcher.attackPointSize;
                break;
            case HeatmapDataFetcher.DataType.Interaction:
                pointColor = heatmapFetcher.interactionColor;
                pointPrefab = heatmapFetcher.interactionPrefab;
                pointSize = heatmapFetcher.interactionPointSize;
                break;
            case HeatmapDataFetcher.DataType.Path:
                pointColor = heatmapFetcher.pathColor;
                pointPrefab = heatmapFetcher.pathPrefab;
                pointSize = heatmapFetcher.pathPointSize;
                break;
            case HeatmapDataFetcher.DataType.Damage:
                pointColor = heatmapFetcher.damageColor;
                pointPrefab = heatmapFetcher.damagePrefab;
                pointSize = heatmapFetcher.damagePointSize;
                break;
            case HeatmapDataFetcher.DataType.Death:
                pointColor = heatmapFetcher.deathColor;
                pointPrefab = heatmapFetcher.deathPrefab;
                pointSize = heatmapFetcher.deathPointSize;
                break;
            case HeatmapDataFetcher.DataType.Pause:
                pointColor = heatmapFetcher.pauseColor;
                pointPrefab = heatmapFetcher.pausePrefab;
                pointSize = heatmapFetcher.pausePointSize;
                break;
        }
    }

    private void ApplySettingsToCurrentDataType()
    {
        if (heatmapFetcher == null) return;

        switch (heatmapFetcher.currentDataType)
        {
            case HeatmapDataFetcher.DataType.Attack:
                heatmapFetcher.attackColor = pointColor;
                heatmapFetcher.attackPrefab = pointPrefab;
                heatmapFetcher.attackPointSize = pointSize;
                break;
            case HeatmapDataFetcher.DataType.Interaction:
                heatmapFetcher.interactionColor = pointColor;
                heatmapFetcher.interactionPrefab = pointPrefab;
                heatmapFetcher.interactionPointSize = pointSize;
                break;
            case HeatmapDataFetcher.DataType.Path:
                heatmapFetcher.pathColor = pointColor;
                heatmapFetcher.pathPrefab = pointPrefab;
                heatmapFetcher.pathPointSize = pointSize;
                break;
            case HeatmapDataFetcher.DataType.Damage:
                heatmapFetcher.damageColor = pointColor;
                heatmapFetcher.damagePrefab = pointPrefab;
                heatmapFetcher.damagePointSize = pointSize;
                break;
            case HeatmapDataFetcher.DataType.Death:
                heatmapFetcher.deathColor = pointColor;
                heatmapFetcher.deathPrefab = pointPrefab;
                heatmapFetcher.deathPointSize = pointSize;
                break;
            case HeatmapDataFetcher.DataType.Pause:
                heatmapFetcher.pauseColor = pointColor;
                heatmapFetcher.pausePrefab = pointPrefab;
                heatmapFetcher.pausePointSize = pointSize;
                break;
        }

        EditorUtility.SetDirty(heatmapFetcher);  // Mark the data fetcher as changed
    }
}
