using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    private Enemy[] enemies;

    private void Awake()
    {
        enemies = FindObjectsByType<Enemy>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
    }

    public void ResetAllEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].ResetEnemy();
            }
        }
    }
}