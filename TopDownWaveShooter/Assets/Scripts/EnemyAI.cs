using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int meleeDamage = 10;
    [SerializeField] float meleeRate = 1f;
    [SerializeField] int FaceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] int RoamDist;
    [SerializeField] int RoamPauseTime;

    Color colorOrg;
    Vector3 startingPos;
    float roamTimer;

    bool canDamage = true;

    bool PlayerInTrigger;

    Vector3 PlayerDir;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrg = model.material.color;
        startingPos = transform.position;

        agent.stoppingDistance = 1f;

        gamemanager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (gamemanager.instance.player != null)
        {
            Vector3 playerPos = gamemanager.instance.player.transform.position;

            agent.SetDestination(playerPos);

            FacePlayer(playerPos);
        }
        else
        {
            Roam();
        }
    }

    void FacePlayer(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * FaceTargetSpeed);
        }
    }

    void CheckRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= RoamPauseTime)
        {
            Roam();
        }
    }

    void Roam()
    {

        if (agent.remainingDistance < 0.1f)
        {
            roamTimer += Time.deltaTime;
            if (roamTimer >= RoamPauseTime)
            {
                Vector3 randomPos = startingPos + Random.insideUnitSphere * RoamDist;
                randomPos.y = transform.position.y;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPos, out hit, RoamDist, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }

                roamTimer = 0f;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canDamage)
        {
            IDamage dmg = other.GetComponent<IDamage>();
            if (dmg != null)
            {
                StartCoroutine(DealMeleeDamage(dmg));
            }
        }
    }

    IEnumerator DealMeleeDamage(IDamage dmg)
    {
        canDamage = false;
        dmg.takeDamage(meleeDamage);
        yield return new WaitForSeconds(meleeRate);
        canDamage = true;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrg;
    }
}