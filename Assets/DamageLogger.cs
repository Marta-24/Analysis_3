using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Gamekit3D;  // For Damageable
using Gamekit3D.Message;  // For IMessageReceiver
using System.Globalization;

public class PlayerDamageLogger : MonoBehaviour, IMessageReceiver
{
    private string damageLogUrl = "https://citmalumnes.upc.es/~albertcf5/D3/save_player_damage.php";  // URL to save damage data
    private string sessionUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_last_session.php";  // URL to fetch the last session ID
    private int sessionID = -1;  // Default invalid session ID
    private string playerId = "Player1";  // Replace with actual player ID if needed

    [System.Serializable]
    public class SessionResponse
    {
        public int session_id;
    }

    void Start()
    {
        StartCoroutine(GetSessionID());

        // Register this script to receive damage messages
        var damageable = GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.onDamageMessageReceivers.Add(this);
        }
        else
        {
            Debug.LogError("Damageable component not found on the player.");
        }
    }

    private IEnumerator GetSessionID()
    {
        UnityWebRequest www = UnityWebRequest.Get(sessionUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Raw server response: {www.downloadHandler.text}");
            SessionResponse response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);
            sessionID = response.session_id;
            Debug.Log($"Parsed session ID for player damage: {sessionID}");
        }
        else
        {
            Debug.LogError("Error fetching session ID: " + www.error);
        }
    }

    public void OnReceiveMessage(MessageType type, object sender, object data)
    {
        if (type == MessageType.DAMAGED)
        {
            Damageable.DamageMessage damageMessage = (Damageable.DamageMessage)data;
            LogDamage(damageMessage);
        }
    }

    private void LogDamage(Damageable.DamageMessage damageMessage)
    {
        StartCoroutine(SendDamageData(damageMessage));
    }

    private IEnumerator SendDamageData(Damageable.DamageMessage damageMessage)
    {
        if (sessionID == -1)
        {
            Debug.LogError("No valid session ID. Cannot log player damage.");
            yield break;
        }

        string damageTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        float posX = transform.position.x;
        float posY = transform.position.y;
        float posZ = transform.position.z;

        // Force the use of '.' as decimal separator
        string posXFormatted = posX.ToString("F4", CultureInfo.InvariantCulture);
        string posYFormatted = posY.ToString("F4", CultureInfo.InvariantCulture);
        string posZFormatted = posZ.ToString("F4", CultureInfo.InvariantCulture);

        WWWForm form = new WWWForm();
        form.AddField("session_id", sessionID);
        form.AddField("player_id", playerId);
        form.AddField("damage_time", damageTime);
        form.AddField("damage_amount", damageMessage.amount);
        form.AddField("position_x", posXFormatted);
        form.AddField("position_y", posYFormatted);
        form.AddField("position_z", posZFormatted);

        Debug.Log($"Sending damage data: \n" +
                  $"Session ID: {sessionID}\n" +
                  $"Player ID: {playerId}\n" +
                  $"Damage Time: {damageTime}\n" +
                  $"Damage Amount: {damageMessage.amount}\n" +
                  $"Position: (X: {posXFormatted}, Y: {posYFormatted}, Z: {posZFormatted})");

        UnityWebRequest www = UnityWebRequest.Post(damageLogUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Player damage data logged successfully!");
        }
        else
        {
            Debug.LogError("Failed to log player damage data: " + www.error);
        }
    }

}
