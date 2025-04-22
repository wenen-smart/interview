 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    private Cinemachine.CinemachineCollisionImpulseSource MyInpulse;

    public Image en;  
    public float energy=500;
    public float energyUp = 500;
    public GameObject gun;
    //  public  delegate void delegategun();
    // Start is called before the first frame update
    public PlayerShooting playerShooting;
    float timer;
    public float fireRate;
    public float fireRatepuls;
    public int spreadAmount;
    private void Start()
    {
        Getsj();
        MyInpulse = GetComponent<Cinemachine.CinemachineCollisionImpulseSource>();
    }
    public  void Getsj()
    {
        playerShooting = gun.GetComponent<PlayerShooting>();
        fireRate = playerShooting.fireRate;
        fireRatepuls = playerShooting.fireRatepuls;
        spreadAmount = playerShooting.spreadAmount;
        Debug.Log("fuck!");
    }
    // public delegategun playGun;
    // Update is called once per frame
  private   void Update()
    {
        en.fillAmount = energy / energyUp;
        timer += Time.deltaTime;
        if (Input.GetButton("Fire1")&&timer >= fireRate&& energy > 0)
        {
           playerShooting.shootLeft();
            MyInpulse.GenerateImpulse();
            timer = 0f;
            energy--;
        }
        else if (Input.GetButton("Fire2") && timer >= fireRatepuls && energy >= 10)
        { if (gun.gameObject.name != "shoot(L)") {
                MyInpulse.GenerateImpulse();
                playerShooting.shootRight();
                timer = 0f;
                energy -= 10;
            }
            else
            {
                playerShooting.shootRight();
                MyInpulse.GenerateImpulse();
                timer = 0f;
                energy -= 20;
            }
        }
    }
    public void getGun()
    {
        playerShooting = gun.GetComponent<PlayerShooting>();
    }
}
