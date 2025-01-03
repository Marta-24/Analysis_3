using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class GameSession
{
    public int id;
    public string player_id;
    public string start_time;
}

public class FetchGameSessions : MonoBehaviour
{
    private string url = "https://citmalumnes.upc.es/~albertcf5/D3/get_game_sessions.php";

    void Start()
    {
        StartCoroutine(FetchSessions());
    }

    private IEnumerator FetchSessions()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            GameSession[] sessions = JsonHelper.FromJson<GameSession>(jsonResponse);

            foreach (var session in sessions)
            {
                Debug.Log($"ID: {session.id}, Player ID: {session.player_id}, Start Time: {session.start_time}");
            }
        }
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
