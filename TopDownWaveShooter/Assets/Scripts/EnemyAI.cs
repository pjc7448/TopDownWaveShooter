using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent Agent;

    [SerializeField] int HP;
    [SerializeField] int FaceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int RoamDist;
    [SerializeField] int RoamPauseTime;

    [SerializeField] GameObject bullet;
    [SerializeField] float ShootRate;

    [SerializeField] int GunRotateSpeed;
    [SerializeField] Transform ShootPos;
    [SerializeField] Transform GunPivot;

    Color colorOrg;

    float ShootTimer;
    float RoamTimer;
    float AngleToPlayer;
    float StoppingDistOrig;

    bool PlayerInTrigger;

    Vector3 PlayerDir;
    Vector3 StartingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrg = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        StoppingDistOrig = Agent.stoppingDistance;
        StartingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ShootTimer += Time.deltaTime;

        if (Agent.remainingDistance < 0.01f)
            RoamTimer += Time.deltaTime;

        if (PlayerInTrigger && !CanSeePlayer())
        {
            CheckRoam();
        }
        else if (!PlayerInTrigger)
        {
            CheckRoam();
        }
    }

    void CheckRoam()
    {
        if (Agent.remainingDistance < 0.01f && RoamTimer >= RoamPauseTime)
        {
            Roam();
        }
    }

    void Roam()
    {
        RoamTimer = 0;
        Agent.stoppingDistance = 0;

        Vector3 RanPos = Random.insideUnitSphere * RoamDist;
        RanPos += StartingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(RanPos, out hit, RoamDist, 1);
        Agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        PlayerDir = gamemanager.instance.player.transform.position - transform.position;
        AngleToPlayer = Vector3.Angle(PlayerDir, transform.forward);

        Debug.DrawRay(transform.position, PlayerDir);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, PlayerDir, out hit))
        {
            if (AngleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                Agent.SetDestination(gamemanager.instance.player.transform.position);

                if (Agent.remainingDistance < Agent.stoppingDistance)
                    FaceTarget();

                if (ShootTimer >= ShootRate)
                {
                    Shoot();
                }

                GunRotate();

                Agent.stoppingDistance = StoppingDistOrig;
                return true;
            }
        }
        Agent.stoppingDistance = 0;
        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(PlayerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * FaceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInTrigger = false;
            Agent.stoppingDistance = 0;
        }
    }

    void Shoot()
    {
        ShootTimer = 0;
        Instantiate(bullet, ShootPos.position, GunPivot.rotation);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        Agent.SetDestination(gamemanager.instance.player.transform.position);

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
        model.material.color = colorOrg;
    }

    void GunRotate()
    {
        Quaternion rot = Quaternion.LookRotation(PlayerDir);
        GunPivot.rotation = Quaternion.LerpUnclamped(GunPivot.rotation, rot, Time.deltaTime * GunRotateSpeed);
    }

}