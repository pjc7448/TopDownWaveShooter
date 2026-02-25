using UnityEngine;

public class regenPowerup : MonoBehaviour
{

    [SerializeField] int regenAmount = 2;
    [SerializeField] float regenRate = 1f;
    [SerializeField] float rotateSpeed = 100f;

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
            player.addRegen(regenAmount, regenRate);
            Destroy(gameObject);
        }
    }
}
