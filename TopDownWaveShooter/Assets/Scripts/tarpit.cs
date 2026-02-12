using UnityEngine;
using UnityEngine.AI;

public class tarpit : MonoBehaviour
{
    [SerializeField] float slowMultiplier = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        playerscript player = other.GetComponent<playerscript>();
        if (player != null)
        {
            player.ModifySpeed(slowMultiplier);
        }
       
        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed *= slowMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerscript player = other.GetComponent<playerscript>();
        if (player != null)
        {
            player.ResetSpeed();
        }

        NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed /= slowMultiplier;
        }
    }
}
