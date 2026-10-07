using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 10f;
    public float range = 5f;
    private Vector3 startPosition;

    public void Start()
    {
        startPosition = transform.position;
    }
    public void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D (Collision2D collision)
    {
        
            Destroy(collision.gameObject);
            Destroy(gameObject);
        
    }
}
