using UnityEngine;
using UnityEditor;

public class HeatmapEditorWindow : EditorWindow
{
    private enum VisualizationMode { Points, Gradient }
    private VisualizationMode currentMode = VisualizationMode.Points;

    private HeatmapDataFetcher heatmapFetcher; // For point-based visualization
    private HeatmapDataFetcherGrid heatmapFetcherGrid; // For gradient-based visualization

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
        DeactivateAllGameObjects();
    }

    private void OnDisable()
    {
        DeactivateAllGameObjects();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("Heatmap Editor", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        });

        GUILayout.Space(15);
        DrawModeSelection();

        GUILayout.Space(15);

        if (currentMode == VisualizationMode.Points)
        {
            ShowPointSettings();
        }
        else if (currentMode == VisualizationMode.Gradient)
        {
            ShowGradientSettings();
        }

        GUILayout.Space(20);
        DrawApplySettingsButton();
    }

    private void DrawModeSelection()
    {
        EditorGUILayout.LabelField("Select Visualization Mode", EditorStyles.boldLabel);
        VisualizationMode newMode = (VisualizationMode)EditorGUILayout.EnumPopup(new GUIContent("Mode:", "Choose how the heatmap is visualized"), currentMode);

        heatmapFetcher = (HeatmapDataFetcher)EditorGUILayout.ObjectField(new GUIContent("Point Heatmap Data Fetcher:", "Assign a data fetcher for point-based visualization"), heatmapFetcher, typeof(HeatmapDataFetcher), true);
        heatmapFetcherGrid = (HeatmapDataFetcherGrid)EditorGUILayout.ObjectField(new GUIContent("Gradient Heatmap Data Fetcher:", "Assign a data fetcher for gradient-based visualization"), heatmapFetcherGrid, typeof(HeatmapDataFetcherGrid), true);

        if (newMode != currentMode)
        {
            currentMode = newMode;
            ClearSceneObjects();
            HandleGameObjectActivation();
        }
    }

    private void ShowPointSettings()
    {
        showPointSettings = EditorGUILayout.Foldout(showPointSettings, "Point-Based Settings", true);
        if (showPointSettings && heatmapFetcher != null)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Configure Point-Based Heatmap", EditorStyles.boldLabel);

            heatmapFetcher.attackColor = EditorGUILayout.ColorField(new GUIContent("Attack Color", "Color for attack points"), heatmapFetcher.attackColor);
            heatmapFetcher.interactionColor = EditorGUILayout.ColorField(new GUIContent("Interaction Color", "Color for interaction points"), heatmapFetcher.interactionColor);
            heatmapFetcher.pathColor = EditorGUILayout.ColorField(new GUIContent("Path Color", "Color for path points"), heatmapFetcher.pathColor);
            heatmapFetcher.damageColor = EditorGUILayout.ColorField(new GUIContent("Damage Color", "Color for damage points"), heatmapFetcher.damageColor);
            heatmapFetcher.deathColor = EditorGUILayout.ColorField(new GUIContent("Death Color", "Color for death points"), heatmapFetcher.deathColor);
            heatmapFetcher.pauseColor = EditorGUILayout.ColorField(new GUIContent("Pause Color", "Color for pause points"), heatmapFetcher.pauseColor);

            GUILayout.Space(10);
            GUILayout.Label("Point Size Settings", EditorStyles.boldLabel);
            heatmapFetcher.attackPointSize = EditorGUILayout.Slider("Attack Point Size", heatmapFetcher.attackPointSize, 0.1f, 5f);
            heatmapFetcher.interactionPointSize = EditorGUILayout.Slider("Interaction Point Size", heatmapFetcher.interactionPointSize, 0.1f, 5f);
            heatmapFetcher.pathPointSize = EditorGUILayout.Slider("Path Point Size", heatmapFetcher.pathPointSize, 0.1f, 5f);
            heatmapFetcher.damagePointSize = EditorGUILayout.Slider("Damage Point Size", heatmapFetcher.damagePointSize, 0.1f, 5f);
            heatmapFetcher.deathPointSize = EditorGUILayout.Slider("Death Point Size", heatmapFetcher.deathPointSize, 0.1f, 5f);
            heatmapFetcher.pausePointSize = EditorGUILayout.Slider("Pause Point Size", heatmapFetcher.pausePointSize, 0.1f, 5f);

            EditorGUILayout.HelpBox("Adjust point size and colors for the selected heatmap data.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
    }

    private void ShowGradientSettings()
    {
        showGradientSettings = EditorGUILayout.Foldout(showGradientSettings, "Gradient-Based Settings", true);
        if (showGradientSettings && heatmapFetcherGrid != null)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Configure Gradient-Based Heatmap", EditorStyles.boldLabel);

            heatmapFetcherGrid.colorGradient = EditorGUILayout.GradientField(new GUIContent("Gradient Colors", "Color gradient for the heatmap"), heatmapFetcherGrid.colorGradient);
            heatmapFetcherGrid.cellSize = EditorGUILayout.Slider("Cell Size", heatmapFetcherGrid.cellSize, 0.1f, 10f);
            heatmapFetcherGrid.gridSize = EditorGUILayout.IntSlider("Grid Size", heatmapFetcherGrid.gridSize, 10, 100);

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Adjust the gradient settings for smoother heatmap visualization.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawApplySettingsButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Apply Settings", "Apply the changes to the selected heatmap data"), GUILayout.Width(200), GUILayout.Height(30)))
        {
            ApplySettingsToFetchers();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void ApplySettingsToFetchers()
    {
        if (currentMode == VisualizationMode.Points && heatmapFetcher != null)
        {
            Debug.Log("Point-based settings applied to HeatmapDataFetcher.");
        }

        if (currentMode == VisualizationMode.Gradient && heatmapFetcherGrid != null)
        {
            Debug.Log("Gradient-based settings applied to HeatmapDataFetcherGrid.");
        }

        HandleGameObjectActivation();
    }

    private void HandleGameObjectActivation()
    {
        DeactivateAllGameObjects();

        if (currentMode == VisualizationMode.Points && heatmapFetcher != null && heatmapFetcher.gameObject != null)
        {
            heatmapFetcher.gameObject.SetActive(true);
            Debug.Log("Activated Points Heatmap.");
        }

        if (currentMode == VisualizationMode.Gradient && heatmapFetcherGrid != null && heatmapFetcherGrid.gameObject != null)
        {
            heatmapFetcherGrid.gameObject.SetActive(true);
            Debug.Log("Activated Gradient Heatmap.");
        }
    }

    private void DeactivateAllGameObjects()
    {
        if (heatmapFetcher != null && heatmapFetcher.gameObject != null)
        {
            heatmapFetcher.gameObject.SetActive(false);
            Debug.Log("Deactivating HeatmapDataFetcher GameObject.");
        }

        if (heatmapFetcherGrid != null && heatmapFetcherGrid.gameObject != null)
        {
            heatmapFetcherGrid.gameObject.SetActive(false);
            Debug.Log("Deactivating HeatmapDataFetcherGrid GameObject.");
        }
    }

    private void ClearSceneObjects()
    {
        Debug.Log("Clearing all heatmap-related objects from the scene.");
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("HeatmapPoint") || obj.name.Contains("HeatmapGrid"))
            {
                DestroyImmediate(obj);
                Debug.Log($"Destroyed object: {obj.name}");
            }
        }
    }
}
