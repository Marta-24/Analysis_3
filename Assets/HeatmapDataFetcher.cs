using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeatmapDataFetcher : MonoBehaviour
{
    private string url = "https://citmalumnes.upc.es/~albertcf5/D3/get_attack_data.php";
    public GameObject heatmapPointPrefab;
    public Gradient heatmapGradient;
    public float maxDamage = 100f;
    public float pointSize = 0.5f;

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
    public class AttackDataList
    {
        public List<AttackData> attacks;
    }

    void Start()
    {
        StartCoroutine(FetchHeatmapData());
    }

    private IEnumerator FetchHeatmapData()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            AttackDataList attackDataList = JsonUtility.FromJson<AttackDataList>("{\"attacks\":" + json + "}");
            DisplayHeatmap(attackDataList.attacks);
        }
        else
        {
            Debug.LogError("Error fetching attack data: " + www.error);
        }
    }

    private void DisplayHeatmap(List<AttackData> attacks)
    {
        foreach (AttackData attack in attacks)
        {
            Vector3 position = new Vector3(attack.position_x, attack.position_y, attack.position_z);

            // Instantiate a point for the heatmap
            GameObject point = Instantiate(heatmapPointPrefab, position, Quaternion.identity);
            point.transform.localScale = Vector3.one * pointSize;

            // Color the point based on damage intensity
            float normalizedDamage = Mathf.Clamp01(attack.damage_amount / maxDamage);
            Color color = heatmapGradient.Evaluate(normalizedDamage);
            point.GetComponent<Renderer>().material.color = color;
        }
    }
}
