using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private int _enemysToKillInWave;
    [SerializeField] private int _waves;
    [SerializeField] public int _level;
    [SerializeField] AudioClip _doorSFX;

    AudioSource _myAudioSource;
    AdditiveSceneLoader loader;

    //public Transform door;
    public Animation doorAnim;
    bool doorIsOpen = false;

    private int _currentWave = 1;
    [SerializeField] private int currentEnemies;

    HealthManager player;
    CameraController cam;
    private float _health;

    private void Start()
    {
        currentEnemies = _enemysToKillInWave;
        loader = FindObjectOfType<AdditiveSceneLoader>();
        player = FindObjectOfType<HealthManager>();
        cam = FindObjectOfType<CameraController>();
        _myAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (currentEnemies == 0)
        {
            if(_currentWave == _waves)
            {
                OpenDoor();
                doorIsOpen = true;
                loader.AllEnemiesKilled();
            }
            else
            {
                _currentWave++;
                currentEnemies = _enemysToKillInWave;
            }
            
        }


        _health = player.getHealth();
        
        if(_health <= 0)
        {
            SceneManager.LoadScene("Lose");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(_level >= 2)
            {
                SceneManager.LoadScene("Victory");
            }
            Debug.Log("nueva zona");
            GetComponent<BoxCollider>().enabled = false;
            cam.minPos.z = 25f;
            currentEnemies = _enemysToKillInWave;
            _currentWave = 1;
        }
    }

    public void EnemyDown()
    {
        currentEnemies--;
    }


    public int getCurrentWave()
    {
        return _currentWave;
    }

    public int getWaves()
    {
        return _waves;
    }

    public void OpenDoor()
    {
       
        if (doorIsOpen == false)
        {
            doorAnim.Play();
            _myAudioSource.Play();
        }

    }

}
