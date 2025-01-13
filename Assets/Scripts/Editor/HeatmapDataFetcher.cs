using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeatmapDataFetcher : MonoBehaviour
{
    public enum DataType { None, Attack, Interaction, Path, Damage, Death, Pause }
    public DataType currentDataType = DataType.None;

    public GameObject attackPrefab;
    public GameObject interactionPrefab;
    public GameObject pathPrefab;
    public GameObject damagePrefab;
    public GameObject deathPrefab;
    public GameObject pausePrefab;

    public Color attackColor = Color.red;
    public Color interactionColor = Color.yellow;
    public Color pathColor = Color.blue;
    public Color damageColor = Color.red;
    public Color deathColor = Color.black;
    public Color pauseColor = Color.green;

    public float attackPointSize = 2f;
    public float interactionPointSize = 2f;
    public float pathPointSize = 2f;
    public float damagePointSize = 2f;
    public float deathPointSize = 2f;
    public float pausePointSize = 2f;

    public string attackDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_attack_data.php";
    public string interactionDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_interaction_data.php";
    public string pathDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_player_path.php";
    public string damageDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_damage_data.php";
    public string deathDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_death_data.php";
    public string pauseDataUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_pause_data.php";

    private List<GameObject> spawnedPoints = new List<GameObject>();

    public bool showOnlyDamagingAttacks = true;

    public enum AttackFilterType { All, WithDamage, NoDamage }
    public AttackFilterType attackFilterType = AttackFilterType.All;
    public enum InteractionType { All, Switch, Piston, Health }
    public InteractionType interactionFilterType = InteractionType.All;

    public List<int> availableSessions = new List<int>();
    public int selectedSessionID = -1; 


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            currentDataType = DataType.Attack;
            StartCoroutine(FetchAttackData());
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            currentDataType = DataType.Interaction;
            StartCoroutine(FetchInteractionData());
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            currentDataType = DataType.Path;
            StartCoroutine(FetchPathData());
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            currentDataType = DataType.Damage;
            StartCoroutine(FetchDamageData());
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            currentDataType = DataType.Death;
            StartCoroutine(FetchDeathData());
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            currentDataType = DataType.Pause;
            StartCoroutine(FetchPauseData());
        }
    }

    private void DisplayHeatmap(List<Vector3> positions, GameObject prefab, Color color, float pointSize)
    {
        ClearExistingPoints();
        foreach (Vector3 position in positions)
        {
            GameObject point = Instantiate(prefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;
            point.GetComponent<Renderer>().material.color = color;
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

    public IEnumerator FetchAttackData()
    {
        UnityWebRequest www = UnityWebRequest.Get(attackDataUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            AttackDataList attackDataList = JsonUtility.FromJson<AttackDataList>("{\"attacks\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();

            foreach (AttackData attack in attackDataList.attacks)
            {
                switch (attackFilterType)
                {
                    case AttackFilterType.WithDamage:
                        if (attack.damage_amount <= 0) continue;
                        break;
                    case AttackFilterType.NoDamage:
                        if (attack.damage_amount > 0) continue;
                        break;
                    case AttackFilterType.All:
                        break;
                }

                positions.Add(new Vector3(attack.position_x, attack.position_y, attack.position_z));
            }

            DisplayHeatmap(positions, attackPrefab, attackColor, attackPointSize);
        }
        else
        {
            Debug.LogError("Error fetching attack data: " + www.error);
        }
    }
    public IEnumerator FetchInteractionData()
    {
        UnityWebRequest www = UnityWebRequest.Get(interactionDataUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            InteractionDataList interactionDataList = JsonUtility.FromJson<InteractionDataList>("{\"interactions\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();

            foreach (InteractionData interaction in interactionDataList.interactions)
            {
                switch (interactionFilterType)
                {
                    case InteractionType.Switch:
                        if (!interaction.interaction_type.Trim().Equals("Switch", System.StringComparison.OrdinalIgnoreCase)) continue;
                        break;
                    case InteractionType.Piston:
                        if (!interaction.interaction_type.Trim().Equals("Piston", System.StringComparison.OrdinalIgnoreCase)) continue;
                        break;
                    case InteractionType.Health:
                        if (!interaction.interaction_type.Trim().Equals("Health", System.StringComparison.OrdinalIgnoreCase)) continue;
                        break;
                }


                positions.Add(new Vector3(interaction.position_x, interaction.position_y, interaction.position_z));
            }

            DisplayHeatmap(positions, interactionPrefab, interactionColor, interactionPointSize);
        }
        else
        {
            Debug.LogError("Error fetching interaction data: " + www.error);
        }
    }


    public IEnumerator FetchPathData()
    {
        UnityWebRequest www = UnityWebRequest.Get(pathDataUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            PathDataList pathDataList = JsonUtility.FromJson<PathDataList>("{\"paths\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (PathData path in pathDataList.paths)
            {
                positions.Add(new Vector3(path.position_x, path.position_y, path.position_z));
            }
            DisplayHeatmap(positions, pathPrefab, pathColor, pathPointSize);
        }
        else
        {
            Debug.LogError("Error fetching path data: " + www.error);
        }
    }

    public IEnumerator FetchDamageData()
    {
        UnityWebRequest www = UnityWebRequest.Get(damageDataUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            DamageDataList damageDataList = JsonUtility.FromJson<DamageDataList>("{\"damages\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (DamageData damage in damageDataList.damages)
            {
                positions.Add(new Vector3(damage.position_x, damage.position_y, damage.position_z));
            }
            DisplayHeatmap(positions, damagePrefab, damageColor, damagePointSize);
        }
        else
        {
            Debug.LogError("Error fetching damage data: " + www.error);
        }
    }

    public IEnumerator FetchDeathData()
    {
        UnityWebRequest www = UnityWebRequest.Get(deathDataUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            DeathDataList deathDataList = JsonUtility.FromJson<DeathDataList>("{\"deaths\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (DeathData death in deathDataList.deaths)
            {
                positions.Add(new Vector3(death.x, death.y, death.z));
            }
            DisplayHeatmap(positions, deathPrefab, deathColor, deathPointSize);
        }
        else
        {
            Debug.LogError("Error fetching death data: " + www.error);
        }
    }

    public IEnumerator FetchPauseData()
    {
        UnityWebRequest www = UnityWebRequest.Get(pauseDataUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            PauseDataList pauseDataList = JsonUtility.FromJson<PauseDataList>("{\"pauses\":" + json + "}");
            List<Vector3> positions = new List<Vector3>();
            foreach (PauseData pause in pauseDataList.pauses)
            {
                positions.Add(new Vector3(pause.position_x, pause.position_y, pause.position_z));
            }
            DisplayHeatmap(positions, pausePrefab, pauseColor, pausePointSize);
        }
        else
        {
            Debug.LogError("Error fetching pause data: " + www.error);
        }
    }

    [System.Serializable] public class AttackData { public int session_id; public string player_id; public string attack_time; public float damage_amount; public float position_x; public float position_y; public float position_z; }
    [System.Serializable] public class InteractionData { public int session_id; public string player_id; public string interaction_type; public string interaction_time; public float position_x; public float position_y; public float position_z; }
    [System.Serializable] public class PathData { public int session_id; public string player_name; public string timestamp; public float position_x; public float position_y; public float position_z; }
    [System.Serializable] public class DamageData { public int session_id; public string player_name; public string timestamp; public float damage_amount; public float position_x; public float position_y; public float position_z; }
    [System.Serializable] public class DeathData { public int session_id; public string player_id; public string death_time; public float x; public float y; public float z; }
    [System.Serializable] public class PauseData { public int session_id; public string player_id; public string pause_time; public float position_x; public float position_y; public float position_z; }

    [System.Serializable] public class AttackDataList { public List<AttackData> attacks; }
    [System.Serializable] public class InteractionDataList { public List<InteractionData> interactions; }
    [System.Serializable] public class PathDataList { public List<PathData> paths; }
    [System.Serializable] public class DamageDataList { public List<DamageData> damages; }
    [System.Serializable] public class DeathDataList { public List<DeathData> deaths; }
    [System.Serializable] public class PauseDataList { public List<PauseData> pauses; }
}
