using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Animator animator;
    public NavMeshAgent agent;
    public Transform player;
    public Transform checker;
    public Transform bulletSpawnPos;
    public Transform boundsChecker, wallChecker;
    public LayerMask whatIsGround, whatIsWall, whatIsPlayer;
    public float health;
    public AudioSource sfx;
    public AudioSource aiStateSounds;
    public AudioClip deathSfx, attackSfx, takeDamage, patrolSound, chaseSound;
    bool IsDead;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public Vector3 bounds;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    float attackTimer;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, withinLevel, hitWall, seenPlayer;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        aiStateSounds.loop = true;
    }

    void Update(){
        attackTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if(IsDead) {return;}

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(checker.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(checker.position, attackRange, whatIsPlayer);
        withinLevel = Physics.CheckBox(boundsChecker.position, bounds, Quaternion.identity, whatIsGround);
        hitWall = Physics.CheckBox(boundsChecker.position, bounds, Quaternion.identity, whatIsWall);
        
        if (Physics.Raycast(wallChecker.position, wallChecker.forward, 2f, whatIsWall))
        {
            walkPointSet = false;
            SearchWalkPoint();
        }

        Debug.DrawRay(wallChecker.position, wallChecker.forward, Color.green);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

        if(!withinLevel) SearchWalkPoint();
        if(hitWall) SearchWalkPoint();
    }

    private void Patroling()
    {
        animator.SetBool("inSight", false);
        if (!walkPointSet) SearchWalkPoint();
        aiStateSounds.clip = patrolSound;

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 0.5f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        hitWall = false;

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    
    private void ChasePlayer()
    {
        animator.SetBool("inSight", true);
        aiStateSounds.clip = chaseSound;
        transform.LookAt(player);

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        animator.SetTrigger("attack");
        animator.SetBool("inSight", false);
        seenPlayer = false;
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        if (!alreadyAttacked)
        {
            //Attack code here
            Instantiate(projectile, bulletSpawnPos.position, bulletSpawnPos.rotation);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        if(IsDead) {return;}
        if(health > 0)
        {
            health -= damage;
            animator.SetTrigger("TakeHit");
            sfx.PlayOneShot(takeDamage);
            seenPlayer = true;
        }
        else Invoke(nameof(Die), 0.5f);
    }

    void Die()
    {
        if (!IsDead)
        {
            sfx.PlayOneShot(deathSfx);
            //Logic for the Death function
            animator.SetTrigger("Die");
            animator.SetBool("Dead", true);
            IsDead = true;
            Destroy(gameObject);
        }   
    }

    void OnTriggerEnter(Collider other)
    {
        hitWall = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checker.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checker.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(boundsChecker.position, new Vector3(bounds.x, bounds.y, bounds.z));
    }
}
