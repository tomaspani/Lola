using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Transform _pointToShoot;
    [SerializeField] private GameObject _bulletTrail;
    [SerializeField] private float _shotDelay;
    [SerializeField] private float _weaponRange = 25f;
    [SerializeField] private Animator _muzzleFlashAnim;

    
    [SerializeField] private Vector3 shootingRay;

    public PlayerMovement playerMov;

    public float shotCounter;


    private bool isFiring;


    public bool getIsFiring()
    {
        return isFiring;
    }

    public void setIsFiring(bool val)
    {
        isFiring = val;
    }

    void Awake()
    {
    }

    private void Update()
    {
        //Debug.DrawLine(this.transform.position, playerMov.getLookAt(), Color.green);
        shootingRay = new Vector3(playerMov.getLookAt().x, this.transform.position.y, playerMov.getLookAt().z);
        Debug.DrawLine(this.transform.position, shootingRay, Color.red);
        if (isFiring)
        {
            shotCounter -= Time.deltaTime;
            if(shotCounter <= 0)
            {
                shotCounter = _shotDelay;
                RaycastHit hit;
                if(Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.forward), out hit, _weaponRange))
                {
                    Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.forward), Color.yellow, 5f);
                    Debug.Log("s");
                }
                else
                {
                    Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.forward), Color.blue, 3f);
                    Debug.Log("a");
                }

                

                var trail = Instantiate(
                    _bulletTrail,
                    this.transform.position,
                    this.transform.rotation
                    );

                

                var trailScript = trail.GetComponent<BulletTrail>();

                
                trailScript.SetTargetPosition(shootingRay);
                Debug.Log(trailScript.GetTargetPosition());

                //Physics.Raycast(this.transform.position, playerMov.getLookAt(), 25f);
                //Instantiate(typeOfBullet, pointToShoot.position, pointToShoot.rotation);
            }
        }
        else
        {
            shotCounter -= Time.deltaTime;

        }
    }



    
}

