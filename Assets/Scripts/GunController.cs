using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private GameObject _bulletTrail;
    [SerializeField] private float _shotDelay;
    [SerializeField] private float _weaponRange = 25f;
    [SerializeField] private float _weaponDmg = 25f;
    [SerializeField] private Animator _muzzleFlashAnim;
    [SerializeField] private LayerMask _layerMask;
    public GameObject player;

    [SerializeField] private Vector3 shootingRay;

    public PlayerMovement playerMov;

    public float shotCounter;

    [SerializeField] private int _currentAmmo;
    public int maxAmmo;


    private bool isFiring;

    public GameObject buffParticles;


    public float GetShotDelay()
    {
        return _shotDelay;
    }


    public bool getIsFiring()
    {
        return isFiring;
    }

    public void setIsFiring(bool val)
    {
        isFiring = val;
    }
    
    
    public int getAmmo()
    {
        return _currentAmmo;
    }
    
    public int getMaxAmmo()
    {
        return maxAmmo;
    }

    public void setAmmo(int val)
    {
        _currentAmmo = val;
    }

    void Start()
    {
        _currentAmmo = maxAmmo;
        player = GameObject.FindGameObjectWithTag("Player");

    }

    private void Update()
    {
        //Debug.DrawLine(this.transform.position, playerMov.getLookAt(), Color.green);
        shootingRay = new Vector3(playerMov.getLookAt().x, this.transform.position.y, playerMov.getLookAt().z);
        Debug.DrawLine(this.transform.position, shootingRay, Color.red);
        if (isFiring)
        {
            shotCounter -= Time.deltaTime;
            if (shotCounter <= 0)
            {
                shotCounter = _shotDelay;
                RaycastHit hit;
                var trail = Instantiate(
                    _bulletTrail,
                    this.transform.position,
                    this.transform.rotation
                    );

                var trailScript = trail.GetComponent<BulletTrail>();

                trailScript.SetStartingPosition(this.transform.position);

                if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.forward), out hit, _weaponRange, ~_layerMask))
                {
                    Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.forward), Color.yellow, 5f);
                    trailScript.SetTargetPosition(hit.transform.position);
                    Debug.Log("hit");
                    Debug.Log(hit.transform.name);
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.collider.GetComponent<Enemy>().TakeDamage(_weaponDmg);
                    }
                }
                else
                {
                    Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.forward), Color.blue, 3f);
                    Debug.Log("miss");
                    trailScript.SetTargetPosition(this.transform.position + transform.TransformDirection(Vector3.forward) * _weaponRange);
                }



                
                //Physics.Raycast(this.transform.position, playerMov.getLookAt(), 25f);
                //Instantiate(typeOfBullet, pointToShoot.position, pointToShoot.rotation);
            }
        }
        else
        {
            shotCounter -= Time.deltaTime;

        }
        

    }

    public void BuffWeapon(float val, float timer)
    {
        StartCoroutine(BuffWeaponCouroutine(val, timer));
    }


    private IEnumerator BuffWeaponCouroutine(float val, float timer)
    {
        var aux = _weaponDmg;
        var particles = Instantiate(buffParticles, player.gameObject.transform.position, player.gameObject.transform.rotation, player.transform);
        _weaponDmg = val;
        yield return new WaitForSeconds(timer);
        Destroy(particles);
        _weaponDmg = aux;
    }
}

