using FirstGearGames.SmoothCameraShaker;
using UnityEngine;

public class Asteroid_shard_spawn : MonoBehaviour
{
    public GameObject[] ores;
    private AudioSource ore_point;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ore_point = GetComponent<AudioSource>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null && rb.tag == "SHARD")
            {
                Vector2 direction = (rb.transform.position - transform.position).normalized;
                rb.AddForce(direction * 7, ForceMode2D.Impulse);
            }
        }

    }


    public void play_ore()
    {
        ore_point.Play();
    }
    public void reveal_ore(int num)
    {
        for(int i = 0; i < num; i++)
        {
            ores[i].SetActive(true);
        }
    }
}
