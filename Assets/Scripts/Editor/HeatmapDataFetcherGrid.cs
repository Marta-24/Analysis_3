using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class HeatmapDataFetcherGrid : MonoBehaviour
{
    public string attackDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_attack_data.php";
    public string interactionDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_interaction_data.php";
    public string pathDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_player_path.php";
    public string damageDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_damage_data.php";
    public string deathDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_death_data.php";
    public string pauseDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_pause_data.php";

    public float cellSize = 10f;
    public int gridSize = 50;
    public Gradient colorGradient;

    private List<GameObject> spawnedCubes = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) StartCoroutine(FetchAttackData());
        if (Input.GetKeyDown(KeyCode.F2)) StartCoroutine(FetchInteractionData());
        if (Input.GetKeyDown(KeyCode.F3)) StartCoroutine(FetchPathData());
        if (Input.GetKeyDown(KeyCode.F4)) StartCoroutine(FetchDamageData());
        if (Input.GetKeyDown(KeyCode.F5)) StartCoroutine(FetchDeathData());
        if (Input.GetKeyDown(KeyCode.F6)) StartCoroutine(FetchPauseData());
    }

    private void DisplayGridHeatmap(List<Vector3> positions)
    {
        ClearGrid();
        Dictionary<Vector2Int, int> cellCounts = new Dictionary<Vector2Int, int>();

        foreach (Vector3 position in positions)
        {
            Vector2Int cellIndex = new Vector2Int(Mathf.FloorToInt(position.x / cellSize), Mathf.FloorToInt(position.z / cellSize));
            if (cellCounts.ContainsKey(cellIndex))
                cellCounts[cellIndex]++;
            else
                cellCounts[cellIndex] = 1;
        }

        int maxCount = cellCounts.Values.Max();
        foreach (var cell in cellCounts)
        {
            Vector3 centerPosition = new Vector3(cell.Key.x * cellSize, 1.0f, cell.Key.y * cellSize);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = centerPosition;
            cube.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);

            float normalizedValue = (float)cell.Value / maxCount;
            Color heatColor = colorGradient.Evaluate(normalizedValue);
            cube.GetComponent<Renderer>().material.color = heatColor;

            spawnedCubes.Add(cube);
        }
    }

    private void ClearGrid()
    {
        foreach (GameObject cube in spawnedCubes)
        {
            Destroy(cube);
        }
        spawnedCubes.Clear();
    }

    private IEnumerator FetchAttackData()
    {
        UnityWebRequest www = UnityWebRequest.Get(attackDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            AttackDataList dataList = JsonUtility.FromJson<AttackDataList>("{\"attacks\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (AttackData attack in dataList.attacks)
            {
                positions.Add(new Vector3(attack.position_x, attack.position_y, attack.position_z));
            }
            DisplayGridHeatmap(positions);
        }
        else
        {
            Debug.LogError("Error fetching attack data: " + www.error);
        }
    }

    private IEnumerator FetchInteractionData()
    {
        UnityWebRequest www = UnityWebRequest.Get(interactionDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            InteractionDataList dataList = JsonUtility.FromJson<InteractionDataList>("{\"interactions\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (InteractionData interaction in dataList.interactions)
            {
                positions.Add(new Vector3(interaction.position_x, interaction.position_y, interaction.position_z));
            }
            DisplayGridHeatmap(positions);
        }
        else
        {
            Debug.LogError("Error fetching interaction data: " + www.error);
        }
    }

    private IEnumerator FetchPathData()
    {
        UnityWebRequest www = UnityWebRequest.Get(pathDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            PathDataList dataList = JsonUtility.FromJson<PathDataList>("{\"paths\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (PathData path in dataList.paths)
            {
                positions.Add(new Vector3(path.position_x, path.position_y, path.position_z));
            }
            DisplayGridHeatmap(positions);
        }
        else
        {
            Debug.LogError("Error fetching path data: " + www.error);
        }
    }

    private IEnumerator FetchDamageData()
    {
        UnityWebRequest www = UnityWebRequest.Get(damageDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            DamageDataList dataList = JsonUtility.FromJson<DamageDataList>("{\"damages\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (DamageData damage in dataList.damages)
            {
                positions.Add(new Vector3(damage.position_x, damage.position_y, damage.position_z));
            }
            DisplayGridHeatmap(positions);
        }
        else
        {
            Debug.LogError("Error fetching damage data: " + www.error);
        }
    }

    private IEnumerator FetchDeathData()
    {
        UnityWebRequest www = UnityWebRequest.Get(deathDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            DeathDataList dataList = JsonUtility.FromJson<DeathDataList>("{\"deaths\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (DeathData death in dataList.deaths)
            {
                positions.Add(new Vector3(death.x, death.y, death.z));
            }
            DisplayGridHeatmap(positions);
        }
        else
        {
            Debug.LogError("Error fetching death data: " + www.error);
        }
    }

    private IEnumerator FetchPauseData()
    {
        UnityWebRequest www = UnityWebRequest.Get(pauseDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            PauseDataList dataList = JsonUtility.FromJson<PauseDataList>("{\"pauses\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (PauseData pause in dataList.pauses)
            {
                positions.Add(new Vector3(pause.position_x, pause.position_y, pause.position_z));
            }
            DisplayGridHeatmap(positions);
        }
        else
        {
            Debug.LogError("Error fetching pause data: " + www.error);
        }
    }

    // Data classes
    [System.Serializable] public class AttackData { public float position_x, position_y, position_z; }
    [System.Serializable] public class InteractionData { public float position_x, position_y, position_z; }
    [System.Serializable] public class PathData { public float position_x, position_y, position_z; }
    [System.Serializable] public class DamageData { public float position_x, position_y, position_z; }
    [System.Serializable] public class DeathData { public float x, y, z; }
    [System.Serializable] public class PauseData { public float position_x, position_y, position_z; }

    [System.Serializable] public class AttackDataList { public List<AttackData> attacks; }
    [System.Serializable] public class InteractionDataList { public List<InteractionData> interactions; }
    [System.Serializable] public class PathDataList { public List<PathData> paths; }
    [System.Serializable] public class DamageDataList { public List<DamageData> damages; }
    [System.Serializable] public class DeathDataList { public List<DeathData> deaths; }
    [System.Serializable] public class PauseDataList { public List<PauseData> pauses; }
}
