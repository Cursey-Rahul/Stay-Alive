using UnityEngine;

public class NPCPrefabSpawner : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    [SerializeField] private GameObject[] prefabsToSpawn;

    private NPC npc;
    private bool hasSpawned;

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }

    private void Update()
    {
        if (npc == null || !npc.permitted || hasSpawned)
            return;

        SpawnPrefabs();
    }

    private void SpawnPrefabs()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to NPCPrefabSpawner.", this);
            return;
        }

        foreach (GameObject prefab in prefabsToSpawn)
        {
            if (prefab == null)
                continue;

            Instantiate(
                prefab,
                transform.position - new Vector3(2f, 2f, 0f),
                Quaternion.identity
            );
        }

        hasSpawned = true;
    }
}