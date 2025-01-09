using UnityEngine;
using UnityEditor;

public class HeatmapEditorWindow : EditorWindow
{
    private enum VisualizationMode { Points, Gradient }
    private VisualizationMode currentMode = VisualizationMode.Points;

    private HeatmapDataFetcher heatmapFetcher;
    private string[] dataTypes = { "None", "Attack", "Interaction", "Path", "Damage", "Death", "Pause" };

    private Color pointColor;
    private GameObject pointPrefab;
    private float pointSize;
    private string dataUrl;

    private Gradient gradientColors = new Gradient();
    private float gradientIntensity = 1f;
    private int gradientResolution = 256;

    private HeatmapDataFetcher.DataType lastDataType = HeatmapDataFetcher.DataType.None;

    private bool showPointSettings = true;
    private bool showGradientSettings = true;

    [MenuItem("Window/Heatmap Editor")]
    public static void ShowWindow()
    {
        HeatmapEditorWindow window = GetWindow<HeatmapEditorWindow>("Heatmap Editor");
        window.Show();
    }

    private void OnEnable()
    {
        AssignHeatmapFetcher();
        EditorApplication.update += AutoRefresh;
    }

    private void OnDisable()
    {
        EditorApplication.update -= AutoRefresh;
    }

    private void AssignHeatmapFetcher()
    {
        if (heatmapFetcher == null)
        {
            if (Selection.activeGameObject != null)
            {
                heatmapFetcher = Selection.activeGameObject.GetComponent<HeatmapDataFetcher>();
            }

            if (heatmapFetcher == null)
            {
                heatmapFetcher = FindObjectOfType<HeatmapDataFetcher>();
            }
        }
    }

    private void AutoRefresh()
    {
        if (heatmapFetcher == null)
        {
            AssignHeatmapFetcher();
            if (heatmapFetcher == null) return;
        }

        if (lastDataType != heatmapFetcher.currentDataType)
        {
            lastDataType = heatmapFetcher.currentDataType;
            LoadSettingsForCurrentDataType();
            Repaint();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Heatmap Editor", EditorStyles.boldLabel);
        GUILayout.Space(10);

        heatmapFetcher = (HeatmapDataFetcher)EditorGUILayout.ObjectField("Heatmap Data Fetcher", heatmapFetcher, typeof(HeatmapDataFetcher), true);
        if (heatmapFetcher == null)
        {
            EditorGUILayout.HelpBox("Select a HeatmapDataFetcher to customize or press play mode to automatically assign.", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);

        int selectedDataTypeIndex = (int)heatmapFetcher.currentDataType;
        EditorGUILayout.LabelField("Current Data Type:", dataTypes[selectedDataTypeIndex]);

        if (heatmapFetcher.currentDataType == HeatmapDataFetcher.DataType.None)
        {
            EditorGUILayout.HelpBox("No dataset selected. Press F1-F6 to display a dataset.", MessageType.Info);
            return;
        }

        GUILayout.Space(15);

        currentMode = (VisualizationMode)EditorGUILayout.EnumPopup("Visualization Mode", currentMode);
        GUILayout.Space(10);

        switch (currentMode)
        {
            case VisualizationMode.Points:
                ShowPointSettings();
                break;

            case VisualizationMode.Gradient:
                ShowGradientSettings();
                break;
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Apply Changes", GUILayout.Height(30)))
        {
            ApplySettingsToCurrentDataType();
        }
    }

    private void ShowPointSettings()
    {
        showPointSettings = EditorGUILayout.Foldout(showPointSettings, "Point Settings", true);
        if (showPointSettings)
        {
            EditorGUILayout.BeginVertical("box");
            pointColor = EditorGUILayout.ColorField("Point Color", pointColor);
            GUILayout.Space(5);
            pointSize = EditorGUILayout.Slider("Point Size", pointSize, 0.1f, 5f);
            GUILayout.Space(10);
            pointPrefab = (GameObject)EditorGUILayout.ObjectField("Point Prefab", pointPrefab, typeof(GameObject), false);
            EditorGUILayout.HelpBox("Drag and drop a prefab to visualize points in the heatmap.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
    }

    private void ShowGradientSettings()
    {
        showGradientSettings = EditorGUILayout.Foldout(showGradientSettings, "Gradient Settings", true);
        if (showGradientSettings)
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("Gradient Color Map", EditorStyles.boldLabel);
            gradientColors = EditorGUILayout.GradientField("Gradient", gradientColors);

            GUILayout.Space(10);
            gradientIntensity = EditorGUILayout.Slider("Intensity", gradientIntensity, 0.1f, 10f);
            gradientResolution = EditorGUILayout.IntSlider("Resolution", gradientResolution, 64, 1024);

            EditorGUILayout.HelpBox("Adjust the gradient intensity and resolution for smoother visualization.", MessageType.Info);

            if (GUILayout.Button("Preview Gradient Heatmap", GUILayout.Height(30)))
            {
                ApplyGradientHeatmap();
            }

            EditorGUILayout.EndVertical();
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
                dataUrl = heatmapFetcher.attackDataUrl;
                break;
            case HeatmapDataFetcher.DataType.Interaction:
                pointColor = heatmapFetcher.interactionColor;
                pointPrefab = heatmapFetcher.interactionPrefab;
                pointSize = heatmapFetcher.interactionPointSize;
                dataUrl = heatmapFetcher.interactionDataUrl;
                break;
            case HeatmapDataFetcher.DataType.Path:
                pointColor = heatmapFetcher.pathColor;
                pointPrefab = heatmapFetcher.pathPrefab;
                pointSize = heatmapFetcher.pathPointSize;
                dataUrl = heatmapFetcher.pathDataUrl;
                break;
            case HeatmapDataFetcher.DataType.Damage:
                pointColor = heatmapFetcher.damageColor;
                pointPrefab = heatmapFetcher.damagePrefab;
                pointSize = heatmapFetcher.damagePointSize;
                dataUrl = heatmapFetcher.damageDataUrl;
                break;
            case HeatmapDataFetcher.DataType.Death:
                pointColor = heatmapFetcher.deathColor;
                pointPrefab = heatmapFetcher.deathPrefab;
                pointSize = heatmapFetcher.deathPointSize;
                dataUrl = heatmapFetcher.deathDataUrl;
                break;
            case HeatmapDataFetcher.DataType.Pause:
                pointColor = heatmapFetcher.pauseColor;
                pointPrefab = heatmapFetcher.pausePrefab;
                pointSize = heatmapFetcher.pausePointSize;
                dataUrl = heatmapFetcher.pauseDataUrl;
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

        EditorUtility.SetDirty(heatmapFetcher);
    }

    private void ApplyGradientHeatmap()
    {
        if (heatmapFetcher == null) return;

        Debug.Log("Applying Gradient Heatmap with selected settings...");
    }
}
