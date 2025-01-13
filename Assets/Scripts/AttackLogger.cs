using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;
using System.Collections.Generic;

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
            // Debug.Log("Session ID Response: " + www.downloadHandler.text);
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
                HashSet<Gamekit3D.Damageable> hitEnemies = new HashSet<Gamekit3D.Damageable>(); // Track unique enemies
                Vector3 attackPosition = Vector3.zero; // Position of the attack

                foreach (var attackPoint in meleeWeapon.attackPoints)
                {
                    attackPosition = attackPoint.attackRoot.position; // Store attack position
                    Collider[] hitColliders = Physics.OverlapSphere(attackPoint.attackRoot.position + attackPoint.attackRoot.TransformVector(attackPoint.offset), attackPoint.radius);

                    foreach (var hitCollider in hitColliders)
                    {
                        var enemy = hitCollider.GetComponent<Gamekit3D.Damageable>();
                        if (enemy != null && enemy.gameObject != meleeWeapon.gameObject && !hitEnemies.Contains(enemy))
                        {
                            LogAttack(enemy, attackPoint.attackRoot.position); // Log attack only once per unique enemy
                            hitEnemies.Add(enemy); // Mark enemy as hit
                        }
                    }
                }

                // If no enemies were hit, log as a missed attack
                if (hitEnemies.Count == 0)
                {
                    LogMissedAttack(attackPosition);
                }

                lastAttackTime = Time.time; // Prevent immediate next attack
            }
        }
    }

    private void LogAttack(Gamekit3D.Damageable enemy, Vector3 attackPosition)
    {
        StartCoroutine(SendAttackData(enemy, attackPosition));
    }

    private void LogMissedAttack(Vector3 attackPosition)
    {
        StartCoroutine(SendAttackData(null, attackPosition)); // Pass attack position for missed attacks
    }

    private IEnumerator SendAttackData(Gamekit3D.Damageable enemy, Vector3 attackPosition)
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
        form.AddField("damage_amount", enemy == null ? "0" : meleeWeapon.damage.ToString());

        form.AddField("position_x", attackPosition.x.ToString("F6", CultureInfo.InvariantCulture));
        form.AddField("position_y", attackPosition.y.ToString("F6", CultureInfo.InvariantCulture));
        form.AddField("position_z", attackPosition.z.ToString("F6", CultureInfo.InvariantCulture));

        UnityWebRequest www = UnityWebRequest.Post(attackLogUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("Attack data logged successfully!");
        }
        else
        {
            Debug.LogError("Failed to log attack data: " + www.error);
        }
    }

}
