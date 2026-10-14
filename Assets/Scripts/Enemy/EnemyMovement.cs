using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private Transform player;
    private NavMeshAgent navMeshAgent;

    private float initSpeed;

    void Start()
    {
        initSpeed = speed;

        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        if(Player == null)
        {  
            Debug.LogError("Player must have Player tag");
            return;
        }

        player = Player.transform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent)
        {
            navMeshAgent.speed = speed;
        }
    }

    void Update()
    {
        if(player == null) return;

        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}
