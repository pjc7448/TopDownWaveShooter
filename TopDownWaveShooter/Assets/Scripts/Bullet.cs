using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifeTime = 3f;




    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        IDamage dmg = other.GetComponentInParent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
