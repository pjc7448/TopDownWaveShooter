using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("----Stats----")]

    [SerializeField] float speed = 3f;
    [SerializeField] int health = 3;

    [Header("----Target----")]
    [SerializeField] Transform player;


    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame

    }

    void Update()
    {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position);
        direction.y = 0;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(player);
    }
    void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}
