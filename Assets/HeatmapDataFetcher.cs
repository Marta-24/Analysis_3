using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeatmapDataFetcher : MonoBehaviour
{
    private string attackDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_attack_data.php";
    private string interactionDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_interaction_data.php";
    private string pathDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_player_path.php";
    private string damageDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_damage_data.php";
    private string deathDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_death_data.php";
    private string pauseDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_pause_data.php";  // Nueva URL para datos de pausa

    public GameObject heatmapPointPrefab;
    public Gradient heatmapGradient;
    public float maxDamage = 100f;
    public float pointSize = 0.5f;

    private List<GameObject> spawnedPoints = new List<GameObject>();

    [System.Serializable]
    public class AttackData
    {
        public int session_id;
        public string player_id;
        public string attack_time;
        public float damage_amount;
        public float position_x;
        public float position_y;
        public float position_z;
    }

    [System.Serializable]
    public class InteractionData
    {
        public int session_id;
        public string player_id;
        public string interaction_type;
        public string interaction_time;
        public float position_x;
        public float position_y;
        public float position_z;
    }

    [System.Serializable]
    public class PathData
    {
        public int session_id;
        public string player_name;
        public string timestamp;
        public float position_x;
        public float position_y;
        public float position_z;
    }

    [System.Serializable]
    public class DamageData
    {
        public int session_id;
        public string player_name;
        public string timestamp;
        public float damage_amount;
        public float position_x;
        public float position_y;
        public float position_z;
    }

    [System.Serializable]
    public class DeathData
    {
        public int session_id;
        public string player_id;
        public string death_time;
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class PauseData
    {
        public int session_id;
        public string player_id;
        public string pause_time;
        public float position_x;
        public float position_y;
        public float position_z;
    }

    [System.Serializable]
    public class AttackDataList
    {
        public List<AttackData> attacks;
    }

    [System.Serializable]
    public class InteractionDataList
    {
        public List<InteractionData> interactions;
    }

    [System.Serializable]
    public class PathDataList
    {
        public List<PathData> paths;
    }

    [System.Serializable]
    public class DamageDataList
    {
        public List<DamageData> damages;
    }

    [System.Serializable]
    public class DeathDataList
    {
        public List<DeathData> deaths;
    }

    [System.Serializable]
    public class PauseDataList
    {
        public List<PauseData> pauses;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Fetching attack data...");
            StartCoroutine(FetchAttackData());
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Fetching interaction data...");
            StartCoroutine(FetchInteractionData());
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("Fetching player path data...");
            StartCoroutine(FetchPathData());
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("Fetching player damage data...");
            StartCoroutine(FetchDamageData());
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("Fetching player death data...");
            StartCoroutine(FetchDeathData());
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("Fetching player pause data...");
            StartCoroutine(FetchPauseData());
        }
    }

    // Fetch y mostrar ataques
    private IEnumerator FetchAttackData()
    {
        UnityWebRequest www = UnityWebRequest.Get(attackDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            AttackDataList attackDataList = JsonUtility.FromJson<AttackDataList>("{\"attacks\":" + json + "}");
            DisplayAttackHeatmap(attackDataList.attacks);
        }
        else
        {
            Debug.LogError("Error fetching attack data: " + www.error);
        }
    }

    private void DisplayAttackHeatmap(List<AttackData> attacks)
    {
        ClearExistingPoints();

        foreach (AttackData attack in attacks)
        {
            Vector3 position = new Vector3(attack.position_x, attack.position_y, attack.position_z);
            GameObject point = Instantiate(heatmapPointPrefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;

            float normalizedDamage = Mathf.Clamp01(attack.damage_amount / maxDamage);
            Color color = heatmapGradient.Evaluate(normalizedDamage);
            point.GetComponent<Renderer>().material.color = color;

            spawnedPoints.Add(point);
        }
    }

    // Fetch y mostrar interacciones
    private IEnumerator FetchInteractionData()
    {
        UnityWebRequest www = UnityWebRequest.Get(interactionDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            InteractionDataList interactionDataList = JsonUtility.FromJson<InteractionDataList>("{\"interactions\":" + json + "}");
            DisplayInteractionHeatmap(interactionDataList.interactions);
        }
        else
        {
            Debug.LogError("Error fetching interaction data: " + www.error);
        }
    }

    private void DisplayInteractionHeatmap(List<InteractionData> interactions)
    {
        ClearExistingPoints();

        foreach (InteractionData interaction in interactions)
        {
            Vector3 position = new Vector3(interaction.position_x, interaction.position_y, interaction.position_z);
            GameObject point = Instantiate(heatmapPointPrefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;

            point.GetComponent<Renderer>().material.color = Color.yellow;  // Interacciones en amarillo

            spawnedPoints.Add(point);
        }
    }

    // Fetch y mostrar posiciones del jugador
    private IEnumerator FetchPathData()
    {
        UnityWebRequest www = UnityWebRequest.Get(pathDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            PathDataList pathDataList = JsonUtility.FromJson<PathDataList>("{\"paths\":" + json + "}");
            DisplayPathHeatmap(pathDataList.paths);
        }
        else
        {
            Debug.LogError("Error fetching player path data: " + www.error);
        }
    }

    private void DisplayPathHeatmap(List<PathData> paths)
    {
        ClearExistingPoints();

        foreach (PathData path in paths)
        {
            Vector3 position = new Vector3(path.position_x, path.position_y, path.position_z);
            GameObject point = Instantiate(heatmapPointPrefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;

            point.GetComponent<Renderer>().material.color = Color.blue;  // Puntos de ruta en azul

            spawnedPoints.Add(point);
        }
    }

    // Fetch y mostrar daño recibido
    private IEnumerator FetchDamageData()
    {
        UnityWebRequest www = UnityWebRequest.Get(damageDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            DamageDataList damageDataList = JsonUtility.FromJson<DamageDataList>("{\"damages\":" + json + "}");
            DisplayDamageHeatmap(damageDataList.damages);
        }
        else
        {
            Debug.LogError("Error fetching player damage data: " + www.error);
        }
    }

    private void DisplayDamageHeatmap(List<DamageData> damages)
    {
        ClearExistingPoints();

        foreach (DamageData damage in damages)
        {
            Vector3 position = new Vector3(damage.position_x, damage.position_y, damage.position_z);
            GameObject point = Instantiate(heatmapPointPrefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;

            point.GetComponent<Renderer>().material.color = Color.red;  // Daño recibido en rojo

            spawnedPoints.Add(point);
        }
    }

    // Fetch y mostrar muertes
    private IEnumerator FetchDeathData()
    {
        UnityWebRequest www = UnityWebRequest.Get(deathDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            DeathDataList deathDataList = JsonUtility.FromJson<DeathDataList>("{\"deaths\":" + json + "}");
            DisplayDeathHeatmap(deathDataList.deaths);
        }
        else
        {
            Debug.LogError("Error fetching player death data: " + www.error);
        }
    }

    private void DisplayDeathHeatmap(List<DeathData> deaths)
    {
        ClearExistingPoints();

        foreach (DeathData death in deaths)
        {
            Vector3 position = new Vector3(death.x, death.y, death.z);
            GameObject point = Instantiate(heatmapPointPrefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;

            point.GetComponent<Renderer>().material.color = Color.black;  // Muertes en negro

            spawnedPoints.Add(point);
        }
    }

    // Fetch y mostrar pausas
    private IEnumerator FetchPauseData()
    {
        UnityWebRequest www = UnityWebRequest.Get(pauseDataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            PauseDataList pauseDataList = JsonUtility.FromJson<PauseDataList>("{\"pauses\":" + json + "}");
            DisplayPauseHeatmap(pauseDataList.pauses);
        }
        else
        {
            Debug.LogError("Error fetching pause data: " + www.error);
        }
    }

    private void DisplayPauseHeatmap(List<PauseData> pauses)
    {
        ClearExistingPoints();

        foreach (PauseData pause in pauses)
        {
            Vector3 position = new Vector3(pause.position_x, pause.position_y, pause.position_z);
            GameObject point = Instantiate(heatmapPointPrefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;

            point.GetComponent<Renderer>().material.color = Color.green;  // Pausas en verde

            spawnedPoints.Add(point);
        }
    }

    private void ClearExistingPoints()
    {
        foreach (GameObject point in spawnedPoints)
        {
            Destroy(point);
        }
        spawnedPoints.Clear();
    }
}
