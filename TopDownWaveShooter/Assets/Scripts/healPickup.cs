using UnityEngine;

public class healPickup : MonoBehaviour
{
    [SerializeField] int healAmount = 10;
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

        if (player != null)
        {
            player.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
