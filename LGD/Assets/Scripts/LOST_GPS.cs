using UnityEngine;

public class LOST_GPS : MonoBehaviour
{
    public Transform ship;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (ship.transform.position - transform.position).normalized;
        direction.z = transform.position.z;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetrot = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = targetrot;
    }

    private void FixedUpdate()
    {
        

    }
}
