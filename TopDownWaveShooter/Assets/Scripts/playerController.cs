using UnityEngine;
using System.Collections;

public class playerscript : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignorelayer;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int jumpSpeed;
    [SerializeField] int sprintMod;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] int shootRate;

    int jumpCount;
    int HPOrig;
    float shootTimer;
    Vector3 moveDir;
    Vector3 playerVel;

    int baseSpeed;
    float slowMultiplier = 1f;
    int maxHP;
    int shieldHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      HPOrig = HP;
      maxHP = HP;
      shieldHP = 0;

      baseSpeed = speed;

      updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
      movement();
      sprint();
    }
    void movement()
    {
      shootTimer += Time.deltaTime;
      Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
      if(controller.isGrounded)
        {
          jumpCount = 0;
          playerVel = Vector3.zero;
        }
        //moveDir = new Vector3(Input.GetAxis("Horizontal") 0, Input.GetAxis("Vertical"));
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * baseSpeed * slowMultiplier * Time.deltaTime);
        jump();
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        shoot();
    }
    void jump()
    {
      if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
          playerVel.y = jumpSpeed;
          jumpCount++;
        }
    }
    void sprint()
    {
      if(Input.GetButtonDown("Sprint"))
        {
          baseSpeed *= sprintMod;
        }
      else if(Input.GetButtonUp("Sprint"))
        {
          baseSpeed /= sprintMod;
        }
    }
    void shoot()
    {
        shootTimer = 0;
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignorelayer))
        {
          Debug.Log(hit.collider.name);
          IDamage dmg = hit.collider.GetComponent<IDamage>();
          if(dmg != null)
            {
              dmg.takeDamage(shootDamage);
            }
        }
    }
    public void takeDamage(int amount)
    {
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
        if (maxHP > 0) return;

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
            gamemanager.instance.playerShield.fillAmount = 0f;
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
        updatePlayerUI();
    }
}