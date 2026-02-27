using System.Collections;
using UnityEngine;

public class Ore_mvt : MonoBehaviour
{
    private GameObject player;
    bool is_lerping = false;
    public Asteroid_shard_spawn main;
    public GameObject particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        StartCoroutine(start_lerping());
        float random_z = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, random_z);
    }

    // Update is called once per frame
    void Update()
    {
        if (is_lerping)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 9 * Time.deltaTime);
        }
        float dist = Vector2.Distance(transform.position, player.transform.position);
        if(dist < 0.3f)
        {
            main.play_ore();
            Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    IEnumerator start_lerping()
    {
        yield return new WaitForSeconds(2);
        is_lerping = true;
    }
}
