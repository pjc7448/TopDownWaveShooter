using UnityEngine;

public class IFramePowerup : MonoBehaviour
{

    [SerializeField] int duration = 1;
    [SerializeField] float rateReduction = 1f;
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
            player.addIFrame(duration, rateReduction);
            Destroy(gameObject);
        }
    }
}
