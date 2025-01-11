using UnityEngine;
using UnityEditor;

public class HeatmapEditorWindow : EditorWindow
{
    private enum VisualizationMode { Points, Gradient }
    private VisualizationMode currentMode = VisualizationMode.Points;

    private HeatmapDataFetcher heatmapFetcher;  // For point-based visualization
    private HeatmapDataFetcherGrid heatmapFetcherGrid;  // For gradient-based visualization

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
        AssignGameObjects();
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.update += CheckForDataTypeChange;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.update -= CheckForDataTypeChange;
    }

    private void AssignGameObjects()
    {
        if (heatmapFetcher == null)
        {
            GameObject fetcherObject = GameObject.Find("HeatmapDataFetcher");
            if (fetcherObject != null)
            {
                heatmapFetcher = fetcherObject.GetComponent<HeatmapDataFetcher>();
                Debug.Log("HeatmapDataFetcher GameObject assigned automatically.");
            }
        }

        if (heatmapFetcherGrid == null)
        {
            GameObject gridObject = GameObject.Find("HeatmapDataFetcherGrid");
            if (gridObject != null)
            {
                heatmapFetcherGrid = gridObject.GetComponent<HeatmapDataFetcherGrid>();
                Debug.Log("HeatmapDataFetcherGrid GameObject assigned automatically.");
            }
        }
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            AssignGameObjects();
        }
    }

    private void CheckForDataTypeChange()
    {
        if (heatmapFetcher != null && currentMode == VisualizationMode.Points)
        {
            Repaint();  // Refreshes the window when the data type changes
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("🔥 Heatmap Editor 🔥", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter
        });

        GUILayout.Space(15);
        DrawModeSelection();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);  // Separator line

        if (currentMode == VisualizationMode.Points)
        {
            ShowPointSettings();
        }
        else if (currentMode == VisualizationMode.Gradient)
        {
            ShowGradientSettings();
        }
    }

    private void DrawModeSelection()
    {
        EditorGUILayout.LabelField("Select Visualization Mode", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        VisualizationMode newMode = (VisualizationMode)EditorGUILayout.EnumPopup(new GUIContent("Mode:", "Choose how the heatmap is visualized"), currentMode, GUILayout.MaxWidth(250));

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
        {
            AssignGameObjects();
        }
        GUILayout.EndHorizontal();

        if (newMode != currentMode)
        {
            currentMode = newMode;
            ClearSceneObjects();  // Remove previous objects when switching modes
            HandleGameObjectActivation();  // Activate the correct GameObject
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

    private void HandleGameObjectActivation()
    {
        DeactivateAllGameObjects();

        if (currentMode == VisualizationMode.Points && heatmapFetcher != null)
        {
            heatmapFetcher.gameObject.SetActive(true);
            Debug.Log("Activated Points Heatmap.");
        }

        if (currentMode == VisualizationMode.Gradient && heatmapFetcherGrid != null)
        {
            heatmapFetcherGrid.gameObject.SetActive(true);
            Debug.Log("Activated Gradient Heatmap.");
        }
    }

    private void DeactivateAllGameObjects()
    {
        if (heatmapFetcher != null)
        {
            heatmapFetcher.gameObject.SetActive(false);
            Debug.Log("Deactivating HeatmapDataFetcher GameObject.");
        }

        if (heatmapFetcherGrid != null)
        {
            heatmapFetcherGrid.gameObject.SetActive(false);
            Debug.Log("Deactivating HeatmapDataFetcherGrid GameObject.");
        }
    }

    private void ShowPointSettings()
    {
        showPointSettings = EditorGUILayout.Foldout(showPointSettings, "🎯 Point-Based Settings", true);
        if (showPointSettings && heatmapFetcher != null)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("🔧 Configure Point-Based Heatmap", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Adjust the point settings below to change visualization behavior.", MessageType.Info);

            switch (heatmapFetcher.currentDataType)
            {
                case HeatmapDataFetcher.DataType.Attack:
                    ShowAttackSettings();
                    break;
                case HeatmapDataFetcher.DataType.Interaction:
                    ShowInteractionSettings();
                    break;
                case HeatmapDataFetcher.DataType.Path:
                    ShowPathSettings();
                    break;
                case HeatmapDataFetcher.DataType.Damage:
                    ShowDamageSettings();
                    break;
                case HeatmapDataFetcher.DataType.Death:
                    ShowDeathSettings();
                    break;
                case HeatmapDataFetcher.DataType.Pause:
                    ShowPauseSettings();
                    break;
                default:
                    EditorGUILayout.HelpBox("No data type selected. Press the corresponding key to display the settings.", MessageType.Warning);
                    break;
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void ShowAttackSettings()
    {
        EditorGUILayout.LabelField("🛡️ Attack Settings", EditorStyles.boldLabel);
        DrawPrefabField("Attack Prefab", ref heatmapFetcher.attackPrefab);
        heatmapFetcher.attackColor = EditorGUILayout.ColorField("Attack Color", heatmapFetcher.attackColor);
        heatmapFetcher.attackPointSize = EditorGUILayout.Slider("Attack Point Size", heatmapFetcher.attackPointSize, 0.1f, 5f);
    }

    private void ShowInteractionSettings()
    {
        GUILayout.Label("🛠️ Interaction Settings", EditorStyles.boldLabel);
        heatmapFetcher.interactionPrefab = (GameObject)EditorGUILayout.ObjectField("Interaction Prefab", heatmapFetcher.interactionPrefab, typeof(GameObject), false);
        heatmapFetcher.interactionColor = EditorGUILayout.ColorField("Interaction Color", heatmapFetcher.interactionColor);
        heatmapFetcher.interactionPointSize = EditorGUILayout.Slider("Interaction Point Size", heatmapFetcher.interactionPointSize, 0.1f, 5f);
    }

    private void ShowPathSettings()
    {
        GUILayout.Label("🛤️ Path Settings", EditorStyles.boldLabel);
        heatmapFetcher.pathPrefab = (GameObject)EditorGUILayout.ObjectField("Path Prefab", heatmapFetcher.pathPrefab, typeof(GameObject), false);
        heatmapFetcher.pathColor = EditorGUILayout.ColorField("Path Color", heatmapFetcher.pathColor);
        heatmapFetcher.pathPointSize = EditorGUILayout.Slider("Path Point Size", heatmapFetcher.pathPointSize, 0.1f, 5f);
    }

    private void ShowDamageSettings()
    {
        GUILayout.Label("💥 Damage Settings", EditorStyles.boldLabel);
        heatmapFetcher.damagePrefab = (GameObject)EditorGUILayout.ObjectField("Damage Prefab", heatmapFetcher.damagePrefab, typeof(GameObject), false);
        heatmapFetcher.damageColor = EditorGUILayout.ColorField("Damage Color", heatmapFetcher.damageColor);
        heatmapFetcher.damagePointSize = EditorGUILayout.Slider("Damage Point Size", heatmapFetcher.damagePointSize, 0.1f, 5f);
    }

    private void ShowDeathSettings()
    {
        GUILayout.Label("☠️ Death Settings", EditorStyles.boldLabel);
        heatmapFetcher.deathPrefab = (GameObject)EditorGUILayout.ObjectField("Death Prefab", heatmapFetcher.deathPrefab, typeof(GameObject), false);
        heatmapFetcher.deathColor = EditorGUILayout.ColorField("Death Color", heatmapFetcher.deathColor);
        heatmapFetcher.deathPointSize = EditorGUILayout.Slider("Death Point Size", heatmapFetcher.deathPointSize, 0.1f, 5f);
    }

    private void ShowPauseSettings()
    {
        GUILayout.Label("⏸️ Pause Settings", EditorStyles.boldLabel);
        heatmapFetcher.pausePrefab = (GameObject)EditorGUILayout.ObjectField("Pause Prefab", heatmapFetcher.pausePrefab, typeof(GameObject), false);
        heatmapFetcher.pauseColor = EditorGUILayout.ColorField("Pause Color", heatmapFetcher.pauseColor);
        heatmapFetcher.pausePointSize = EditorGUILayout.Slider("Pause Point Size", heatmapFetcher.pausePointSize, 0.1f, 5f);
    }

    private void ShowGradientSettings()
    {
        showGradientSettings = EditorGUILayout.Foldout(showGradientSettings, "🌈 Gradient-Based Settings", true);
        if (showGradientSettings && heatmapFetcherGrid != null)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("🎨 Configure Gradient-Based Heatmap", EditorStyles.boldLabel);

            heatmapFetcherGrid.colorGradient = EditorGUILayout.GradientField("Gradient Colors", heatmapFetcherGrid.colorGradient);
            heatmapFetcherGrid.cellSize = EditorGUILayout.Slider("Cell Size", heatmapFetcherGrid.cellSize, 0.1f, 10f);
            heatmapFetcherGrid.gridSize = EditorGUILayout.IntSlider("Grid Size", heatmapFetcherGrid.gridSize, 10, 100);

            EditorGUILayout.HelpBox("Adjust the gradient settings for smoother heatmap visualization.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawPrefabField(string label, ref GameObject prefab)
    {
        prefab = (GameObject)EditorGUILayout.ObjectField(label, prefab, typeof(GameObject), false);
    }
}
