using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Spawn_manager : MonoBehaviour
{
    public Transform[] spawn_positions;
    public Transform[] left_side_spawn_positions;
    public Transform[] right_side_spawn_positions;
    public Transform Player;
    private float current_asteroid_force = 5000;

    public Transform[] meteor_spawns;
    private float time_between_spawns = 10;
    public GameObject meteor;
    private bool started_spawning_meteors = false;

    public GameObject asteroid;
    private int spawn_amount = 0;
    private int target_spawn_amount = 1;

    private int Asteroids_drilled = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //spawn_asteroid();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.E))
        {
            spawn_moving_asteroid_right();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(spawn_meteor());
        }*/
    }

    public void asteroid_down()
    {
        Asteroids_drilled += 1;
        //spawn_asteroid();
        if (Asteroids_drilled < 4)
        {
            spawn_asteroid();
        }
        else
        {
            int num1 = Random.Range(0, 2);
            if(num1 == 0)
            {
                spawn_asteroid();
            }
            else
            {
                int num2 = Random.Range(0, 2);
                if (num2 == 0)
                {
                    spawn_moving_asteroid_left();
                }
                else
                {
                    spawn_moving_asteroid_right();
                }
            }

        }

        if(Asteroids_drilled > 10 && started_spawning_meteors == false)
        {
            StartCoroutine(spawn_meteor());
            started_spawning_meteors = true;
        }
        if(Asteroids_drilled >= 15)
        {
            time_between_spawns = 6;
            current_asteroid_force = 7000;
        }
        if(Asteroids_drilled >= 20)
        {
            time_between_spawns = 4;
            current_asteroid_force = 9000;
        }
        if (Asteroids_drilled >= 25)
        {
            time_between_spawns = 2;
            current_asteroid_force = 10000;
        }
    }

    public void spawn_asteroid()
    {
        List<Transform> validPoints = new List<Transform>();

        foreach(Transform pt in spawn_positions)
        {
            if(Vector2.Distance(pt.position, Player.position) >= 7)
            {
                validPoints.Add(pt);
            }
        }
        if(validPoints.Count > 0)
        {
            Transform selectedpoint = validPoints[Random.Range(0, validPoints.Count)];
            GameObject ast =Instantiate(asteroid, selectedpoint.position, Quaternion.identity);
            if(Asteroids_drilled > 7)
            {
                ast.TryGetComponent<Asteroid>(out Asteroid asteroid);
                if (asteroid)
                {
                    asteroid.enable_meteor(90);
                    asteroid.is_15 = true;

                    
                }
            }
            else
            {
                ast.TryGetComponent<Asteroid>(out Asteroid asteroid);
                if (asteroid)
                {
                    //asteroid.enable_meteor(90);
                    asteroid.is_10 = true;


                }
            }

        }
        else
        {
            Debug.LogError("NOTHIN CLOSE BY BRAH");
        }
    }

    public void spawn_moving_asteroid_left()
    {
        List<Transform> validPoints = new List<Transform>();

        foreach (Transform pt in left_side_spawn_positions)
        {
            if (Vector2.Distance(pt.position, Player.position) >= 5)
            {
                validPoints.Add(pt);
            }
        }
        if (validPoints.Count > 0)
        {
            Transform selectedpoint = validPoints[Random.Range(0, validPoints.Count)];
            GameObject astr = Instantiate(asteroid, selectedpoint.position, Quaternion.identity);
            astr.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d);
            if (rb2d)
            {
                rb2d.AddForce(Vector2.left * current_asteroid_force);
                print("force_applied");
            }
            if (Asteroids_drilled > 7)
            {
                astr.TryGetComponent<Asteroid>(out Asteroid asteroid_go);
                if (asteroid_go)
                {
                    asteroid_go.enable_meteor(90);
                    asteroid_go.is_15 = false;
                    asteroid_go.is_10 = false;
                    asteroid_go.is_20 = true;
                }
            }
            else
            {
                astr.TryGetComponent<Asteroid>(out Asteroid asteroid_go);
                if (asteroid_go)
                {
                   // asteroid_go.enable_meteor(90);
                    asteroid_go.is_15 = true;
                    asteroid_go.is_10 = false;
                    asteroid_go.is_20 = false;
                }
            }

        }
        else
        {
            Debug.LogError("NOTHIN CLOSE BY BRAH");
            spawn_moving_asteroid_right();
        }
    }

    public void spawn_moving_asteroid_right()
    {
        List<Transform> validPoints = new List<Transform>();

        foreach (Transform pt in right_side_spawn_positions)
        {
            if (Vector2.Distance(pt.position, Player.position) >= 5)
            {
                validPoints.Add(pt);
            }
        }
        if (validPoints.Count > 0)
        {
            Transform selectedpoint = validPoints[Random.Range(0, validPoints.Count)];
            GameObject astr = Instantiate(asteroid, selectedpoint.position, Quaternion.identity);
            astr.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d);
            if (rb2d)
            {
                rb2d.AddForce(Vector2.left * -current_asteroid_force);
                print("force_applied");
            }
            if(Asteroids_drilled > 7)
            {
                astr.TryGetComponent<Asteroid>(out Asteroid asteroid_go);
                if (asteroid_go)
                {
                    asteroid_go.enable_meteor(90);
                    asteroid_go.is_15 = false;
                    asteroid_go.is_10 = false;
                    asteroid_go.is_20 = true;
                }
            }
            else
            {
                astr.TryGetComponent<Asteroid>(out Asteroid asteroid_go);
                if (asteroid_go)
                {
                    //asteroid_go.enable_meteor(90);
                    asteroid_go.is_15 = true;
                    asteroid_go.is_10 = false;
                    asteroid_go.is_20 = false;
                }
            }


        }
        else
        {
            Debug.LogError("NOTHIN CLOSE BY BRAH");
            spawn_moving_asteroid_left();
        }
    }

    public IEnumerator spawn_meteor()
    {

        int random_m_pos = Random.Range(0, meteor_spawns.Length);
        GameObject met = Instantiate(meteor, meteor_spawns[random_m_pos].position, Quaternion.identity);
        met.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d);
        if (rb2d)
        {
            Vector2 direction = Player.transform.position - met.transform.position  ;
            rb2d.AddForce(direction * 10f);
            print("spawned meteor");
        }

        yield return new WaitForSeconds(time_between_spawns);
        StartCoroutine(spawn_meteor());
    }

    public void main_menu()
    {
        SceneManager.LoadScene(0);
    }

}
