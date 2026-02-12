using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int RoamDist;
    [SerializeField] int RoamPauseTime;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform ShootPos;

    Color colorOrig;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    bool playerInTrigger;

    Vector3 playerDir;
    Vector3 startingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer = Time.deltaTime;

        if(agent.remainingDistance < 0.01f)
            roamTimer = Time.deltaTime;
        if(playerInTrigger && !canSeePlayer())
        {
            checkRoam();
        }
        else if(!playerInTrigger)
        {
            checkRoam();
        }
    }

    void checkRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= RoamPauseTime)
        {
            Roam();
        }
    }

    void Roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 RanPos = Random.insideUnitSphere * RoamDist;
        RanPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(RanPos, out hit, RoamDist, 1);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(transform.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                    FaceTarget();

                if (shootTimer >= shootRate)
                {
                    Shoot();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            agent.stoppingDistance = 0;
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, ShootPos.position, transform.rotation);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

}
