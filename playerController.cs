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
    int HPOrgi;
    float shootTimer;
    Vector3 moveDir;
    Vector3 playervel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      HPOrgi = HP;
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
      shootTimer += shootTimer.deltaTime;
      Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
      if(controller.isGrounded)
        {
          jumpCount = 0;
          playervel = Vector3.zero;
        }
        //moveDir = new Vector3(Input.GetAxis("Horizontal") 0, Input.GetAxis("Vertical"));
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.move(moveDir * speed * shootTimer.deltaTime);
        jump();
        controller.Move(playerVel * shootTimer.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        shoot();
    }
    void jump()
    {
      if(Imput.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
          playervel.y = jumpSpeed;
          jumpCount++;
        }
    }
    void sprint()
    {
      if(Input.GetButtonDown("Sprint"))
        {
          speed *= sprintMod;
        }
      else if(Input.GetButtonUp("Sprint"))
        {
          speed /= sprintMod;
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
      HP -= amount;
      updatePlayerUI();
      startCoroutine(flashScreen());
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
      gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
}
