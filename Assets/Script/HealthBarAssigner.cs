
using UnityEngine;

public class HealthBarAssigner : MonoBehaviour
{
    [SerializeField] private WorldHealthBar healthBarPrefab;

    private void Start()
    {
        Entity entity = GetComponent<Entity>();

        if (entity == null || healthBarPrefab == null)
        {
            Debug.LogError(
                "Entity or Health Bar Prefab is missing!",
                this
            );
            return;
        }

        WorldHealthBar healthBar = Instantiate(
            healthBarPrefab,
            entity.transform.position ,
            Quaternion.identity
        );

        healthBar.SetTarget(entity);
    }
}