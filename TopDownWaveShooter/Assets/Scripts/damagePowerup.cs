using UnityEngine;

public class damagePowerup : MonoBehaviour
{

    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] int damageInc = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        playerscript player = other.GetComponent<playerscript>();
        if (player != null )
        {
            player.addDamage(damageInc);
            Destroy(gameObject);
        }
    }
}
