using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI;
using FirstGearGames.SmoothCameraShaker;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class ShuttleMvt : MonoBehaviour
{
    public float movement_force = 5f;
    public float max_speed = 10f;
    public float rotationspeed = 200f;
    public float max_ang_velocity = 100;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    public float ray_length = 1f;
    public LayerMask asteroid_mask;
    public GameObject landing_gear;

    private bool landed = false;
    private bool connected_to_asteroid = false;
    float connection_time = 0;


    public Image fuel_main;
    //public Image fuel_background;
    public Canvas fuel_canvas;
    public Canvas dashboard_canvas;
    public Canvas main_menu_canvas;
    private float max_fuel = 100;
    private float current_fuel = 0;
    private float consumption_rate = 14;
    public Color NORM_fuel_color;
    public Color low_fuel_color;

    public float size_diff = 1.05f;
    private Vector2 original_size;

    public TextMeshProUGUI speedometer;
    public TextMeshProUGUI Out_of_bounds_time;
    public float o_o_b_time = 5f;
    private bool counting_down = false;

    public TextMeshProUGUI Score_text;
    public int score = 0;
    public ScoreCount score_count;

    public ShakeData Thrust_shake;
    public ParticleSystem[] thrust_particles;

    public AudioSource thrust;
    public AudioSource landing_sound;
    public AudioSource shuttle_impact;
    public AudioSource FUEL_RECHARGE;
    public AudioSource Speed_warning;
    public AudioSource Bounds_beep_main;
    public AudioSource bounds_return;

    public AudioSource[] speech;

    public bool crashed = false;

    public ParticleSystem landing_particles;
    public ShakeData land_shake;
    public Volume volume;


    public AudioSource lost_gps;

    public GameObject shuttle_explosion;

    public ShakeData death_shake;

    public bool game_started = false;
    public GameLossCanvas loss_canv;

    public Animator anim;
    bool playing_fast;
    bool playing_slow;

    public TrailRenderer speed_trail;
    public GameObject gameplay_ui;
    public ShuttleMvt me;
    public GameObject damage_flash;

    public GameObject gps;
    public ParticleSystem drill_particles;
    public AudioSource drill_sound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        current_fuel = max_fuel;
        original_size = fuel_canvas.gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        volume.profile.TryGet<ChromaticAberration>(out ChromaticAberration chr_abr);
        if (chr_abr.intensity.value > 0.23f)
        {
            chr_abr.intensity.value -= 0.2f * Time.deltaTime;
        }
        if (!crashed && game_started)
        {
            fuel_main.fillAmount = Mathf.Clamp(current_fuel / max_fuel, 0, 1);
            FUEL_RECHARGE.pitch = Mathf.Clamp(FUEL_RECHARGE.pitch, 1, 6);
            fuel_canvas.transform.position = transform.position;
            //fuel_main.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position);
            //fuel_background.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position);
            if (current_fuel > 25)
            {
                fuel_main.color = NORM_fuel_color;

            }
            else
            {
                fuel_main.color = low_fuel_color;
            }
            if(current_fuel <= 0)
            {
                StartCoroutine(out_of_gas());
            }



            //print(rb.linearVelocity.magnitude * 10);
            speedometer.text = rb.linearVelocity.magnitude.ToString("0.0.0");
            if (rb.linearVelocity.magnitude >= 1.99)
            {
                Speed_warning.volume = 1;
                anim.Play("spd_speeding");
                speed_trail.emitting = true;
            }
            else
            {
                Speed_warning.volume = 0;
                anim.Play("spd_norm");
                speed_trail.emitting = false;
            }


            if (counting_down && o_o_b_time >= 0 && crashed == false)
            {
                Out_of_bounds_time.text = o_o_b_time.ToString("0.00");
                o_o_b_time -= Time.deltaTime;
                Bounds_beep_main.volume = 1;
            }
            else 
            {
                Bounds_beep_main.volume = 0;

            }
            if(o_o_b_time <= 0)
            {
                if (lost_gps.isPlaying == false)
                {
                    lost_gps.Play();
                    Bounds_beep_main.volume = 0;
                    crashed = true;
                    StartCoroutine(lost_ship());
                }
            }

            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            landing_gear.transform.rotation = transform.rotation;

            Vector2 raydirection = transform.up;
            Vector2 rayorigin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(rayorigin, raydirection, ray_length, asteroid_mask);
            Debug.DrawRay(rayorigin, raydirection * ray_length, Color.red);

            if (hit.collider != null)
            {
                landed = true;
                //print("hit " + hit.collider.name);
            }
            else
            {
                landed = false;
            }

            if (connected_to_asteroid)
            {
                connection_time += Time.deltaTime;
                current_fuel = Mathf.Lerp(current_fuel, max_fuel, 0.005f);
                FUEL_RECHARGE.pitch += Time.deltaTime * 2;
                FUEL_RECHARGE.volume = 1;
               // print(connection_time);
                fuel_canvas.transform.localScale = original_size * size_diff;
                if (connection_time >= 3)
                {
                    FixedJoint2D joint = gameObject.GetComponent<FixedJoint2D>();
                    if (joint != null)
                    {
                        Destroy(joint);
                        FUEL_RECHARGE.volume = 0;
                        FUEL_RECHARGE.pitch = 1;
                        current_fuel = max_fuel;
                        fuel_canvas.transform.localScale = original_size;
                        if (hit.collider != null)
                        {
                            hit.collider.gameObject.TryGetComponent<Asteroid>(out Asteroid asteroid);
                            asteroid.explode();
                            volume.profile.TryGet<ChromaticAberration>(out ChromaticAberration chr_abr2);
                            drill_particles.Stop();
                            drill_sound.Stop();
                            if (chr_abr2)
                            {
                                chr_abr2.intensity.value = 1;
                            }
                            

                            //current_fuel = max_fuel;
                        }
                        connection_time = 0;
                        connected_to_asteroid = false;
                        int rand_speech = Random.Range(0, speech.Length);
                        speech[rand_speech].Play();
                    }
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (!crashed && game_started)
        {
            if (moveInput != Vector2.zero && current_fuel > 0)
            {
                Vector2 force2add = moveInput * movement_force;
                rb.AddForce(force2add, ForceMode2D.Impulse);
                current_fuel -= consumption_rate * Time.fixedDeltaTime;
                CameraShakerHandler.Shake(Thrust_shake);

            }
            if (moveInput.y != 0 && current_fuel > 0 || moveInput.x != 0 && current_fuel > 0)
            {
                thrust.volume = 1;
            }
            else
            {
                thrust.volume = Mathf.Lerp(thrust.volume, 0, 0.1f);
            }

            if (moveInput.y > 0 && current_fuel > 0)
            {
                thrust_particles[0].Play();


            }
            else
            {
                thrust_particles[0].Stop();

            }
            if (moveInput.y < 0 && current_fuel > 0)
            {
                thrust_particles[1].Play();

            }
            else
            {
                thrust_particles[1].Stop();

            }
            if (moveInput.x < 0 && current_fuel > 0)
            {
                thrust_particles[3].Play();


            }
            else
            {
                thrust_particles[3].Stop();

            }
            if (moveInput.x > 0 && current_fuel > 0)
            {
                thrust_particles[2].Play();

            }
            else
            {
                thrust_particles[2].Stop();

            }

            if (Input.GetKey(KeyCode.Space))
            {
                //rb.linearVelocity.magnitude = Mathf.Lerp(rb.linearVelocity.magnitude, 0, 0.01f);
            }
            if (rb.linearVelocity.magnitude > max_speed)
            {
                rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, max_speed);
            }
            if (Input.GetMouseButton(0) && current_fuel > 0)
            {
                rb.AddTorque(rotationspeed);
                current_fuel -= consumption_rate * Time.fixedDeltaTime;
                CameraShakerHandler.Shake(Thrust_shake);
                thrust_particles[4].Play();


            }
            else
            {
                thrust_particles[4].Stop();

            }
            if (Input.GetMouseButton(1) && current_fuel > 0)
            {
                rb.AddTorque(-rotationspeed);
                current_fuel -= consumption_rate * Time.fixedDeltaTime;
                CameraShakerHandler.Shake(Thrust_shake);
                thrust_particles[5].Play();
            }
            else
            {
                thrust_particles[5].Stop();

            }
            rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -max_ang_velocity, max_ang_velocity);
            if (Input.GetMouseButton(1) && current_fuel > 0 || Input.GetMouseButton(0) && current_fuel > 0)
            {
                thrust.volume = 1;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print(collision.relativeVelocity.magnitude);
        if (landed)
        {
            if(collision.relativeVelocity.magnitude > 2)
            {
                shuttle_impact.pitch = UnityEngine.Random.RandomRange(1f, 1.25f);
                shuttle_impact.Play();
                crashed = true;
                print("CRASHED");
                StartCoroutine(destroy_ship());
                damage_flash.SetActive(true);
            }
            else
            {
                print("Good_landing");
                FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
                Rigidbody2D other_rb = collision.collider.GetComponent<Rigidbody2D>();
                if(other_rb != null)
                {
                    joint.connectedBody = other_rb;
                    connected_to_asteroid = true;
                    landing_sound.Play();
                    print("CONNECTED");
                    landing_particles.Play();
                    drill_particles.Play();
                    drill_sound.Play();
                    CameraShakerHandler.Shake(land_shake);
                }
            }
        }
        else
        {
            StartCoroutine(destroy_ship());
            print("CRASHED");
            damage_flash.SetActive(true);
            crashed = true;
            shuttle_impact.pitch = UnityEngine.Random.RandomRange(1f, 1.25f);
            shuttle_impact.Play();
        }
        if(collision.transform.tag == "OBSTACLE")
        {
            crashed = true;
            damage_flash.SetActive(true);
            print("obstacle hit");
            StartCoroutine(destroy_ship());
        }

    }

    IEnumerator destroy_ship()
    {
        yield return new WaitForSeconds(1);
        Instantiate(shuttle_explosion, transform.position, Quaternion.identity);
        CameraShakerHandler.Shake(death_shake);
        fuel_canvas.gameObject.SetActive(false);
        foreach(ParticleSystem f in thrust_particles)
        {
            f.gameObject.SetActive(false);
        }
        loss_canv.start_appearence();
        loss_canv.display_causes(0);
        loss_canv.score = score;
        gameObject.SetActive(false);
        gameplay_ui.SetActive(false);
    }

    IEnumerator lost_ship()
    {
        yield return new WaitForSeconds(1);
        lost_gps.Play();
        loss_canv.start_appearence();
        loss_canv.display_causes(2);
        loss_canv.score = score;
        gameObject.SetActive(false);
        fuel_canvas.gameObject.SetActive(false);
        gameplay_ui.SetActive(false);
    }

    IEnumerator out_of_gas()
    {
        yield return new WaitForSeconds(1);
        crashed = true;
        lost_gps.Play();
        loss_canv.start_appearence();
        loss_canv.display_causes(1);
        loss_canv.score = score;
        //gameObject.SetActive(false);
        //fuel_canvas.gameObject.SetActive(false);
        gameplay_ui.SetActive(false);
        this.enabled = false;
        me.enabled = false;
        lost_gps.Stop();
    }
    private void OnBecameInvisible()
    {
        if (!crashed)
        {
            print("WARNING : IN DEEP SPACE");
            Out_of_bounds_time.gameObject.SetActive(true);
            counting_down = true;
            gps.SetActive(true);
        }
        
    }

    private void OnBecameVisible()
    {
        print("BACK IN MISSION ZONE");
        Out_of_bounds_time.gameObject.SetActive(false);
        counting_down = false;
        o_o_b_time = 5;
        bounds_return.Play();
        gps.SetActive(false);
    }

    public void add_score(int score_num)
    {
        score_count._value = score;
        score += score_num;
        //Score_text.text = score.ToString("0000");
        score_count.UpdateText(score);
    }

    public void start_game()
    {
        game_started = true;
        fuel_canvas.gameObject.SetActive(true);
        dashboard_canvas.gameObject.SetActive(true);
        main_menu_canvas.gameObject.SetActive(false);
    }

}
