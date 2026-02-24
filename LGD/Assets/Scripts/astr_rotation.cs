using UnityEngine;

public class astr_rotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(50, 50, 0) * Time.fixedDeltaTime);
    }
}
