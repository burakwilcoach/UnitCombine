using UnityEngine;

public static class EnemyFinder
{
    public static GameObject FindClosestEnemy(GameObject self, string enemyTag)
    {
        // Retrieve all enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(self.transform.position, enemy.transform.position);

            if (distance < minDistance)
            {
                closestEnemy = enemy;
                minDistance = distance;
            }
        }

        return closestEnemy; // This could be null if there are no enemies
    }
}

