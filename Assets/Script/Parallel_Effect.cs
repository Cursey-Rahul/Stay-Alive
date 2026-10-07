using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private GameObject[] Layer1;
    [SerializeField] private GameObject[] Layer2;
    [SerializeField] private GameObject[] Layer3;

    [SerializeField] private Transform player;

    [SerializeField] private float layer1Speed = 0.1f;
    [SerializeField] private float layer2Speed = 0.4f;
    [SerializeField] private float layer3Speed = 0.7f;

    private float lastPlayerX;

    private void Start()
    {
        if (player != null)
        {
            lastPlayerX = 0;
        }
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        float playerMovement = player.position.x - lastPlayerX;

        MoveLayer(Layer1, playerMovement, layer1Speed);
        MoveLayer(Layer2, playerMovement, layer2Speed);
        MoveLayer(Layer3, playerMovement, layer3Speed);

        lastPlayerX = player.position.x;
    }

    private void MoveLayer(
        GameObject[] layer,
        float playerMovement,
        float parallaxSpeed)
    {
        foreach (GameObject item in layer)
        {
            if (item == null)
                continue;

            item.transform.position += new Vector3(
                -playerMovement * parallaxSpeed,
                0f,
                0f
            );
        }
    }
}