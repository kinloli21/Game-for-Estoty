using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyLogic : MonoBehaviour
{
    public float primaryRadius;
    [Range(0, 360)]
    public float primaryAngle;

    public float damageRadius;
    [Range(0, 360)]
    public float damageAngle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    private bool canSeePlayer;
    private bool isInDamageArea;

    public float moveSpeed = 3f;

    // Add an Audio Source component to the enemy GameObject
    private AudioSource audioSource;

    public AudioClip detectionSound; // Assign the detection sound effect in the Unity Editor

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Get the Audio Source component
        StartCoroutine(EnemyRoutine());
    }

    private IEnumerator EnemyRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;

            EnemyLogicCheck();

            // If player is seen, move towards the player and play the detection sound
            if (canSeePlayer)
            {
                MoveTowardsPlayer();
                PlayDetectionSound();
            }

            // If player is in damage area, trigger game over
            if (isInDamageArea)
            {
                GameOver();
            }
        }
    }

    private void EnemyLogicCheck()
    {
        // Primary field of view detection
        Collider[] primaryRangeChecks = Physics.OverlapSphere(transform.position, primaryRadius, targetMask);

        if (primaryRangeChecks.Length != 0)
        {
            Transform target = primaryRangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < primaryAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;

        // Secondary field of view (damage area) detection
        Collider[] damageRangeChecks = Physics.OverlapSphere(transform.position, damageRadius, targetMask);

        if (damageRangeChecks.Length != 0)
        {
            Transform target = damageRangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < damageAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    isInDamageArea = true;
                else
                    isInDamageArea = false;
            }
            else
                isInDamageArea = false;
        }
        else if (isInDamageArea)
            isInDamageArea = false;
    }

    private void MoveTowardsPlayer()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, primaryRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToPlayer = (target.position - transform.position).normalized;

            // Move the object towards the player
            transform.Translate(directionToPlayer * Time.deltaTime * moveSpeed, Space.World);
        }
    }

    private void PlayDetectionSound()
    {
        // Play the detection sound effect only when the player is detected
        if (canSeePlayer && detectionSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = detectionSound; // Set the clip
            audioSource.Play(); // Use Play() instead of PlayOneShot
            Debug.Log("Detection sound played.");
        }
        else
        {
            Debug.Log("Detection sound not played. Condition not met.");
        }
    }

    private void GameOver()
    {
        // Implement your game over logic here
        // For example: Load the "Game Over" scene
        SceneManager.LoadScene("GameOver");
    }

    private void OnDrawGizmosSelected()
    {
        // Primary field of view visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, primaryRadius);

        Vector3 viewAngleA = DirFromAngle(-primaryAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(primaryAngle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * primaryRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * primaryRadius);

        if (canSeePlayer)
        {
            Gizmos.color = Color.red;
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, primaryRadius, targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }

        // Secondary field of view (damage area) visualization
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);

        Vector3 damageViewAngleA = DirFromAngle(-damageAngle / 2, false);
        Vector3 damageViewAngleB = DirFromAngle(damageAngle / 2, false);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + damageViewAngleA * damageRadius);
        Gizmos.DrawLine(transform.position, transform.position + damageViewAngleB * damageRadius);

        if (isInDamageArea)
        {
            Gizmos.color = Color.green;
            Collider[] damageRangeChecks = Physics.OverlapSphere(transform.position, damageRadius, targetMask);

            if (damageRangeChecks.Length != 0)
            {
                Transform target = damageRangeChecks[0].transform;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }


    // Helper method to calculate a direction from an angle
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a GameObject with a specific tag
        if (other.CompareTag("Player"))
        {
            // Check if the specified scene is in the build settings
            if (SceneUtility.GetBuildIndexByScenePath(sceneToLoad) != -1)
            {
                // Load the specified scene
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("Scene not found in build settings: " + sceneToLoad);
            }
        }
    }
}
