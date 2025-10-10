using UnityEngine;

public class PlayerAwernessController : MonoBehaviour
{
    public bool AwareOfPlayer {  get; private set; }
    public Vector2 DirectionToPlayer {  get; private set; }

    [SerializeField]
    private float playerAwarenessDistance;
    private Transform player;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 enemyToPlayerVector = player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;

        if (enemyToPlayerVector.magnitude <= playerAwarenessDistance)
        {
            AwareOfPlayer = true;
        }
        else
        {
            AwareOfPlayer = false;
        }
    }
}
