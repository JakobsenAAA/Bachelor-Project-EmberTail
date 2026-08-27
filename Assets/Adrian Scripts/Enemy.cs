using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Death")]
    [SerializeField] private float deathDelay = 0f;
    [SerializeField] private GameObject deathEffect;

    [Header("Drops")]
    [SerializeField] private GameObject cinderPrefab;
    [SerializeField] private int cinderDropAmount = 5;
    [SerializeField] private float dropSpreadRadius = 0.75f;
    [SerializeField] private float dropHeight = 0.5f;

    public bool IsDead { get; private set; }

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Collider[] enemyColliders;
    private Renderer[] enemyRenderers;

    private void Awake()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        enemyColliders = GetComponentsInChildren<Collider>(true);
        enemyRenderers = GetComponentsInChildren<Renderer>(true);
    }

    public void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        DropCinders();

        if (deathDelay > 0f)
        {
            Invoke(nameof(HideEnemy), deathDelay);
        }
        else
        {
            HideEnemy();
        }
    }

    public void ResetEnemy()
    {
        CancelInvoke();

        transform.SetPositionAndRotation(startingPosition, startingRotation);

        IsDead = false;

        SetEnemyVisible(true);
        SetEnemyColliders(true);
    }

    private void HideEnemy()
    {
        SetEnemyColliders(false);
        SetEnemyVisible(false);
    }

    private void SetEnemyColliders(bool enabled)
    {
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            if (enemyColliders[i] != null)
            {
                enemyColliders[i].enabled = enabled;
            }
        }
    }

    private void SetEnemyVisible(bool visible)
    {
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null)
            {
                enemyRenderers[i].enabled = visible;
            }
        }
    }

    private void DropCinders()
    {
        if (cinderPrefab == null)
        {
            return;
        }

        for (int i = 0; i < cinderDropAmount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * dropSpreadRadius;

            Vector3 spawnPosition =
                transform.position +
                new Vector3(randomCircle.x, dropHeight, randomCircle.y);

            Instantiate(cinderPrefab, spawnPosition, Quaternion.identity);
        }
    }
}