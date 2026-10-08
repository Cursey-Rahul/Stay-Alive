using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 10f;
    public float range = 5f;

    [SerializeField] private LayerMask zombieLayer;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage();
            Destroy(gameObject);
        }
    }
}