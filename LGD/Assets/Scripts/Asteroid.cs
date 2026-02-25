using FirstGearGames.SmoothCameraShaker;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public ShakeData exp_data;
    public GameObject shards;
    public Transform shard_rot;

    public GameObject revolving_meteor;
    private int rot_speed;

    public GameObject score_10;
    public GameObject score_15;
    public GameObject score_20;

    public bool is_10;
    public bool is_15;
    public bool is_20;
    public bool is_main;
    public GameObject astr_to_clr_chnange;

    [SerializeField] LeanTweenType easetype;
    public Color drain_color;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeanTween.scale(gameObject, new Vector3(2, 2, 1), 0.5f).setEase(easetype);
        //drain();
    }

    // Update is called once per frame
    void Update()
    {
        shard_rot.transform.Rotate(0, 0, 90 * Time.deltaTime);
    }

    public void enable_meteor(int speed)
    {
        rot_speed = speed;
        revolving_meteor.SetActive(true);
        //is_10 = false;
        //is_15 = true;
    }

    public void drain()
    {
        if (is_main)
        {
            LeanTween.color(astr_to_clr_chnange, drain_color, 2f);
        }
        
    }

    public void explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach(Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if(rb != null && rb.tag != "Gear")
            {
                Vector2 direction = (rb.transform.position - transform.position).normalized;
                rb.AddForce(direction * 2, ForceMode2D.Impulse);
            }
        }
        CameraShakerHandler.Shake(exp_data);
        Instantiate(shards, transform.position, Quaternion.identity);
        
        GameObject spawner = GameObject.FindGameObjectWithTag("SPAWN");
        if (spawner)
        {
            spawner.TryGetComponent<Spawn_manager>(out Spawn_manager spmg);
            spmg.asteroid_down();
        }
        if (is_10)
        {
            Instantiate(score_10, transform.position, Quaternion.identity);
        }
        if (is_15)
        {
            Instantiate(score_15, transform.position, Quaternion.identity);
        }
        if (is_20)
        {
            Instantiate(score_20, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);

    }
}
