using UnityEngine;
using UnityEngine.Experimental.Rendering;


public class PlayerController : MonoBehaviour
{
    [Header("-----Components-----")]

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("-----Stats-----")]

    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int speed;

    [Header("-----Guns-----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] int shootRate;
    [SerializeField] float shootCooldown = 0.2f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform FirePoint;

    int HPOriginal;
    float shootTimer;
    float dashSpeed = 20f;
    float dashDuration = 0.2f;
    float dashCooldown = 1f;
    bool isDashing;
    bool canDash = true;
    bool isInvulnerable;
    float dashTimeLeft;
    float dashTimer;

    Vector3 moveDir;
    Vector3 dashDirection;

    void Start()
    {
        HPOriginal = HP;
    }

    // Update is called once per frame
    void Update()
    {
        HandleDashInput();
        HandleDashTimer();
        HandleCooldown();
        movement();
        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootCooldown)
        {
            shoot();
        }
    }

    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartDash();
        }
    }
    void HandleDashTimer()
    {
        if (!isDashing) return;

        dashTimeLeft -= Time.deltaTime;

        if (dashTimeLeft <= 0)
        {
            EndDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        isInvulnerable = true;
        canDash = false;
        dashTimeLeft = dashDuration;

        dashDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }
    }
    void EndDash()
    {
        isDashing = false;
        isInvulnerable = false;
        dashTimer = dashCooldown;
    }

    void HandleCooldown()
    {

        if (!canDash)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                canDash = true;
            }
        }
    }

    void movement()
    {
        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            return;
        }

        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(moveDir.normalized * speed * Time.deltaTime);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
    }

    void shoot()
    {
        shootTimer = 0;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = (target - FirePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, FirePoint.position, Quaternion.LookRotation(direction));

            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = shootDamage;
            }
        }
    }
}
