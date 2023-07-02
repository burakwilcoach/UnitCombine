using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public float acceleration = 10f; // The acceleration of the unit, adjust this as needed
    public float maxSpeed = 5f; // The maximum speed of the unit, adjust this as needed

    public int hitpoints; // Unit's health points
    public int collisionDamage; // Collision damage
    public string enemyTag; // Tag of the enemy to target

    private GameObject target; // Target to move towards

    public float damageDelay = 1.0f; // Delay in seconds between damage applications
    private bool canTakeDamage = true;

    private float lastCollisionTime = -1f;
    public float collisionCooldown = 0.1f;  // Set this as needed

    private void Update()
    {
        // If the target is null or if it has been destroyed (e.g., in a collision), find a new one
        if (target == null || target.GetComponent<Unit>()?.hitpoints <= 0 || target.GetComponent<Base>()?.hitpoints <= 0)
        {
            target = EnemyFinder.FindClosestEnemy(gameObject, enemyTag); // Find closest enemy
        }

        // Move towards target using acceleration and maximum speed
        if (target != null)
        {
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // Apply force in the direction of the target
            rb.AddForce(direction * acceleration);

            // Limit the speed to the maximum speed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
        else
        {
            // If there is no target, stop moving
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < lastCollisionTime + collisionCooldown) return;

        Unit enemyUnit = collision.gameObject.GetComponent<Unit>();
        Base enemyBase = collision.gameObject.GetComponent<Base>();

        if (collision.gameObject.tag != gameObject.tag)
        {
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(collisionDamage);

                // Add force for the bounce
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.AddForce((transform.position - collision.transform.position) * 5, ForceMode2D.Impulse);

                // Start a coroutine to apply damping
                StartCoroutine(ApplyDamping(rb));
            }
            else if (enemyBase != null)
            {
                if ((gameObject.tag == "PlayerUnit" && enemyBase.gameObject.tag == "EnemyBase") ||
                    (gameObject.tag == "EnemyUnit" && enemyBase.gameObject.tag == "PlayerBase"))
                {
                    enemyBase.TakeDamage(collisionDamage);
                }
            }
        }

        lastCollisionTime = Time.time;
    }

    IEnumerator DamageDelay()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageDelay);
        canTakeDamage = true;
    }

    IEnumerator ApplyDamping(Rigidbody2D rb)
    {
        float dampingAmount = 1f; // Adjust to your needs
        while (rb.velocity.magnitude > 0.1f) // W hile the unit is still moving
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, dampingAmount * Time.deltaTime);
            yield return null;
        }
    }

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;

        if (hitpoints <= 0)
        {
            Destroy(gameObject); // Unit is destroyed
        }
    }
}