using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

public class AttackLogger : MonoBehaviour
{
    private string attackLogUrl = "https://citmalumnes.upc.es/~albertcf5/D3/save_attack.php";
    private string sessionUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_last_session.php";
    private int sessionID = -1;
    private string playerId = "Player1";
    private Gamekit3D.MeleeWeapon meleeWeapon;

    private float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    [System.Serializable]
    public class SessionResponse
    {
        public int session_id;
    }

    void Start()
    {
        StartCoroutine(GetSessionID());
        meleeWeapon = GetComponentInChildren<Gamekit3D.MeleeWeapon>();
        if (meleeWeapon == null)
        {
            Debug.LogError("MeleeWeapon not found. Please ensure it is attached to the player.");
        }
    }

    private IEnumerator GetSessionID()
    {
        UnityWebRequest www = UnityWebRequest.Get(sessionUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Session ID Response: " + www.downloadHandler.text);
            SessionResponse response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);
            sessionID = response.session_id;
        }
        else
        {
            Debug.LogError("Error fetching session ID: " + www.error);
        }
    }

    void Update()
    {
        if (sessionID == -1)
        {
            return;
        }

        if (meleeWeapon != null && meleeWeapon.throwingHit && meleeWeapon.enabled)
        {
            if (Time.time - lastAttackTime > attackCooldown)
            {
                foreach (var attackPoint in meleeWeapon.attackPoints)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(attackPoint.attackRoot.position + attackPoint.attackRoot.TransformVector(attackPoint.offset), attackPoint.radius);
                    foreach (var hitCollider in hitColliders)
                    {
                        var enemy = hitCollider.GetComponent<Gamekit3D.Damageable>();
                        if (enemy != null && enemy.gameObject != meleeWeapon.gameObject)
                        {
                            LogAttack(enemy);
                            lastAttackTime = Time.time;
                        }
                    }
                }
            }
        }
    }

    private void LogAttack(Gamekit3D.Damageable enemy)
    {
        StartCoroutine(SendAttackData(enemy));
    }

    private IEnumerator SendAttackData(Gamekit3D.Damageable enemy)
    {
        if (sessionID == -1)
        {
            Debug.LogError("No valid session ID. Cannot log attack.");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("session_id", sessionID);
        form.AddField("player_id", playerId);
        form.AddField("attack_time", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("damage_amount", meleeWeapon.damage.ToString());

        form.AddField("position_x", enemy.transform.position.x.ToString("F6", CultureInfo.InvariantCulture));
        form.AddField("position_y", enemy.transform.position.y.ToString("F6", CultureInfo.InvariantCulture));
        form.AddField("position_z", enemy.transform.position.z.ToString("F6", CultureInfo.InvariantCulture));

        Debug.Log($"Sending attack data: Session ID: {sessionID}, Player ID: {playerId}, Position: ({enemy.transform.position.x.ToString("F6", CultureInfo.InvariantCulture)}, {enemy.transform.position.y.ToString("F6", CultureInfo.InvariantCulture)}, {enemy.transform.position.z.ToString("F6", CultureInfo.InvariantCulture)})");

        UnityWebRequest www = UnityWebRequest.Post(attackLogUrl, form);
        yield return www.SendWebRequest();

        Debug.Log("PHP Response: " + www.downloadHandler.text);

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Attack data logged successfully!");
        }
        else
        {
            Debug.LogError("Failed to log attack data: " + www.error);
        }
    }
}
