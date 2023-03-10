using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rollSpeed;
    float startSpeed;
    public float groundDrag;

    [Header("Grounde Check")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private bool _grounded;
    public LayerMask ground;


    public Transform orientation;

    private float _horizontalInput, _verticalInput;
    private Vector3 pointToLook, lookAt;


    Rigidbody myRb;
    Camera _mainCamera;

    private Animator _myAnim;
    [SerializeField] private string _xAxisName;
    [SerializeField] private string _zAxisName;
    [SerializeField] private AnimationClip _shootAnimation;

    public GunController gun;

    [Header("Audio")]
    [SerializeField] private AudioClip _gunSFX;
    [SerializeField] private AudioClip _reloadSFX;
    [SerializeField] private AudioClip _noAmmoSFX;
    private AudioSource _myAudioSource;

    public HealthManager pHM;
    bool val = true;
    bool isRolling = false;
    bool isReloading = false;

    public float rollCounter;
    [SerializeField] private float _rollDelay;


    [SerializeField] private int _currentAmmo;
    [SerializeField] private int _maxAmmo;
    private float _shotDelay;

    public Pausa pause;
    [SerializeField] private bool _gameIsPaused;

    public Vector3 getLookAt()
    {
        return lookAt;
    }

    public float getRollDelay()
    {
        return _rollDelay;
    }
    
    public float getRollCounter()
    {
        return rollCounter;
    }

    public void BuffGun(float val, float timer)
    {
        gun.BuffWeapon(val, timer);
    }

    private void Start()
    {
        myRb = GetComponent<Rigidbody>();
        myRb.freezeRotation = true;
        _mainCamera = FindObjectOfType<Camera>();
        _myAnim = GetComponentInChildren<Animator>();
        startSpeed = _moveSpeed;
        _myAudioSource = GetComponent<AudioSource>();
        pHM = GetComponent<HealthManager>();
        _maxAmmo = gun.getMaxAmmo();
        _currentAmmo = _maxAmmo;
        _shotDelay = gun.GetShotDelay();
    }

    private void Update()
    {
        val = pHM.getDmg();


        _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, ground);
        if (_grounded)
            myRb.drag = groundDrag;
        else
            myRb.drag = 0;

        if (!_gameIsPaused) 
        { 

            if (!isRolling)
                LookAround();

            if (!isReloading)
                Shoot();

            if (!gun.getIsFiring())
                Roll();

            MyInput();
            SpeedControl();
        }

        _gameIsPaused = pause.getPausa();
        


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo")
        {
            var ammoBox = other.GetComponentInParent<Reload>();
            ammoBox.ActiveUI();

            isReloading = true;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Ammo")
        {

            var ammoBox = other.GetComponentInParent<Reload>();
            if (ammoBox.GetFill() >= 1)
            {
                gun.setAmmo(_maxAmmo);
                OnReload();
                isReloading = false;
                ammoBox.DeactiveUI();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ammo")
        {
            other.GetComponentInParent<Reload>().DeactiveUI();
            isReloading = false;
        }

    }



    private void FixedUpdate()
    {
        if (!gun.getIsFiring())
        {
            MovePlayer();
            _myAnim.SetFloat(_xAxisName, _horizontalInput);
            _myAnim.SetFloat(_zAxisName, _verticalInput);
        }

        if (isRolling == true)
            myRb.AddForce(transform.forward * _rollSpeed, ForceMode.Impulse);

    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        //moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        Vector3 moveInput = new Vector3(_horizontalInput, 0f, _verticalInput).normalized;

        myRb.AddForce(moveInput * _moveSpeed * 10f, ForceMode.Force);

    }

    private void LookAround()
    {
        Ray cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, gun.transform.position);
        float raylenght;

        if (groundPlane.Raycast(cameraRay, out raylenght))
        {
            pointToLook = cameraRay.GetPoint(raylenght);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);

            lookAt = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);

            transform.LookAt(lookAt);
            //Debug.DrawLine(this.transform.position, lookAt, Color.red);
        }
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && gun.getAmmo() > 0 && !gun.getIsFiring())
        {
            gun.setIsFiring(true);
            StartCoroutine(crOnShoot()); 
            gun.setAmmo(gun.getAmmo() - 1);
        }
        else if (Input.GetMouseButtonDown(0) && gun.getAmmo() <= 0)
        {
            OnNoAmmo();
        }

       if (Input.GetMouseButtonUp(0))
        {
            gun.setIsFiring(false);
        }
    }

    private void Roll()
    {
        rollCounter -= Time.deltaTime;
        if (rollCounter <= 0 && Input.GetMouseButtonDown(1))

        {
            rollCounter = _rollDelay;
            StartCoroutine(crOnRoll());
        }
        else
        {
            rollCounter -= Time.deltaTime;
        }
    }


    private void SpeedControl()
    {

        Vector3 flatVel = new Vector3(myRb.velocity.x, 0f, myRb.velocity.z);

        if (flatVel.magnitude > _moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _moveSpeed;
            myRb.velocity = new Vector3(limitedVel.x, myRb.velocity.y, limitedVel.z);
        }

    }

    public void OnShoot()
    {
        _myAudioSource.clip = _gunSFX;
        if (!_myAudioSource.isPlaying && _gameIsPaused == false) //y si el juego no esta en pausa
        {
            _myAudioSource.Play();
        }

    }

    public void OnNoAmmo()
    {
        _myAudioSource.clip = _noAmmoSFX;
        if (!_myAudioSource.isPlaying)
        {
            _myAudioSource.Play();
        }

    }
    public void OnReload()
    {
        _myAudioSource.clip = _reloadSFX;
        if (!_myAudioSource.isPlaying)
        {
            _myAudioSource.Play();
        }

    }

    private IEnumerator crOnShoot()
    {
        _moveSpeed = 0;
        _myAnim.SetTrigger("onShoot");
        OnShoot();
        yield return new WaitForSeconds(0.25f);
        _myAnim.SetTrigger("onEndShoot");
        _moveSpeed = startSpeed;
    }

    private IEnumerator crOnRoll()
    {
        _moveSpeed = 0;
        _myAnim.SetTrigger("onRoll");
        isRolling = true;
        pHM.setDmg(val);
        yield return new WaitForSeconds(.8f);
        _myAnim.SetTrigger("onEndRoll");
        isRolling = false;
        pHM.setDmg(val);
        _moveSpeed = startSpeed;
    }
}


