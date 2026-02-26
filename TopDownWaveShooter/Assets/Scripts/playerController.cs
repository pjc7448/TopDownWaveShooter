using UnityEngine;
using System.Collections;
using System;

public class playerscript : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignorelayer;
    [SerializeField] Transform firepoint;

    [SerializeField] int HP;
    [SerializeField] int maxShield = 20;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;

    [SerializeField] float turnSpeed = 10f;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] int shootRate;

    int HPOrig;
    float shootTimer;
    Vector3 moveDir;
    Vector3 playerVel;

    int baseSpeed;
    float slowMultiplier = 1f;
    int maxHP;
    int shieldHP;

    int baseShootDamage;

    int regenAmount = 0;
    float regenRate = 1f;
    bool isRegen = false;

    int iFrameDuration = 1;
    float iFrameRate = 5f;
    bool isInvincible = false;
    bool hasIFrame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      HPOrig = HP;
      maxHP = HP;
      shieldHP = 0;

        baseSpeed = speed;
        baseShootDamage = shootDamage;

      updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        rotateMouse();
        sprint();
        
        shootTimer += Time.deltaTime;
        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
            shoot();
    }
    void movement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 mousePos = ray.GetPoint(distance);
                Vector3 forward = (mousePos - transform.position).normalized;
                forward.y = 0;

                Vector3 right = Vector3.Cross(Vector3.up, forward);

                Vector3 move = forward * input.z + right * input.x;
                moveDir = move.normalized;

                controller.Move(moveDir * baseSpeed * slowMultiplier * Time.deltaTime);
            }
        }
    }
    void rotateMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);

            Vector3 lookDir = point - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude < 0.01f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime);
        }
    }
    void sprint()
    {
      if (Input.GetButtonDown("Sprint"))
        {
          baseSpeed = speed * sprintMod;
        }
      else
        {
          baseSpeed = speed;
        }
    }
    void shoot()
    {
        shootTimer = 0;
        RaycastHit hit;
        Vector3 origin = firepoint ? firepoint.position : transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * shootDist, Color.red, 1f);

        if (Physics.Raycast(origin, direction, out hit, shootDist, ~ignorelayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(baseShootDamage);
            }
        }
    }
    public void takeDamage(int amount)
    {
        if (isInvincible)
        {
            return;
        }

        int remainingDamage = amount;
        if (remainingDamage > 0)
        {
            shieldHP -= remainingDamage;
            if (shieldHP < 0)
            {
                remainingDamage = -shieldHP;
                shieldHP = 0;
            }
            else
            {
                remainingDamage = 0;
            }
        }

        if (remainingDamage > 0)
        {
            HP -= remainingDamage;
        }

      updatePlayerUI();
      StartCoroutine(flashScreen());
      if(HP <= 0)
        {
          gamemanager.instance.youLose();
        }
    }
    IEnumerator flashScreen()
    {
      gamemanager.instance.playerDamageFlash.SetActive(true);
      yield return new WaitForSeconds(0.1f);
      gamemanager.instance.playerDamageFlash.SetActive(false);
    }
    public void updatePlayerUI()
    {
        if (maxHP <= 0) return;

        float hpPercent = (float)HP / maxHP;
        hpPercent = Mathf.Clamp01(hpPercent);

        gamemanager.instance.playerHPBar.fillAmount = hpPercent;

        if(shieldHP > 0)
        {
            float shieldPercent = (float)shieldHP / maxHP;
            shieldPercent = Mathf.Clamp01(shieldPercent);

            gamemanager.instance.playerShield.gameObject.SetActive(true);
            gamemanager.instance.playerShield.fillAmount = shieldPercent;
        }
        else
        {
            gamemanager.instance.playerShield.fillAmount = 0;
            gamemanager.instance.playerShield.gameObject.SetActive(false);
        }
    }

    public void ModifySpeed(float amount)
    {
        slowMultiplier = amount;
    }
    public void ResetSpeed()
    {
        slowMultiplier = 1f;
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > maxHP)
            HP = maxHP;

        updatePlayerUI();
    }

    public void AddShield(int amount)
    {
        shieldHP += amount;
        shieldHP = Math.Clamp(shieldHP, 0, maxShield);
        updatePlayerUI();
    }

    public void addDamage(int amount)
    {
        baseShootDamage += amount;
    }

    public void addMaxHealth(int amount)
    {
        maxHP += amount;
        HP += amount;

        updatePlayerUI();
    }

    public void addRegen(int amount, float rate)
    {
        regenAmount += amount;
        regenRate += rate;

        if(!isRegen)
        {
            StartCoroutine(RegenRoutine());
        }
    }

    IEnumerator RegenRoutine()
    {
        isRegen = true;

        while (true)
        {
            yield return new WaitForSeconds(regenRate);

            if (HP < maxHP)
            {
                Heal(regenAmount);
            }
        }
    }

    public void  addIFrame(int duration, float rate)
    {
        iFrameDuration += duration;
        iFrameRate -= rate;
        if (!hasIFrame)
        {
            StartCoroutine(IFrameRoutine());
        }
    }

    IEnumerator IFrameRoutine()
    {
        hasIFrame = true;

        while (true)
        {
            yield return new WaitForSeconds (iFrameRate);
            isInvincible = true;
            yield return new WaitForSeconds(iFrameDuration);
            isInvincible = false;
        }
    }
}