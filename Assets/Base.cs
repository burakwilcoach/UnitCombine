using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour
{
    public GameObject unitPrefab; // The unit prefab to spawn
    public int hitpoints; // Base's health points
    public float spawnRate; // Time between each spawn

    public float spawnRateIncrease = 0.1f; // The amount to increase the spawn rate each time
    public float increaseInterval = 10f; // How often to increase the spawn rate (in seconds)
    public float maxSpawnRate = 5f; // The maximum spawn rate

    private void Start()
    {
        // Start the spawn and spawn rate increase coroutines
        StartCoroutine(SpawnUnits());
        StartCoroutine(IncreaseSpawnRate());
    }

    IEnumerator SpawnUnits()
    {
        while (hitpoints > 0)
        {
            // Generate a random offset
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

            // Spawn a unit at the base position plus the offset
            Instantiate(unitPrefab, transform.position + offset, Quaternion.identity);

            // Wait for the next spawn
            yield return new WaitForSeconds(1f / spawnRate);
        }
    }

    IEnumerator IncreaseSpawnRate()
    {
        while (spawnRate < maxSpawnRate)
        {
            yield return new WaitForSeconds(increaseInterval);

            // Increase the spawn rate, up to the maximum
            spawnRate = Mathf.Min(spawnRate + spawnRateIncrease, maxSpawnRate);
        }
    }

    private void SpawnUnit()
    {
        // Spawn a unit around the base within a radius
        Vector2 spawnPosition = (Random.insideUnitCircle.normalized * 2) + (Vector2)transform.position;
        Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
    }

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;

        if (hitpoints <= 0)
        {
            Destroy(gameObject); // Base is destroyed
        }
    }
}