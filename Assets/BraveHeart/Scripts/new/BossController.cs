using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float chaseRadius = 1f;
    public float roarDistance =0.1f; // 
    public float sleepRadius = 3f; // 

    [Header("Combat")]
    public float attackCooldown = 2f;
    public float attackRange = 3f;
    public int attackDamage = 20;

    [Header("References")]
    private Transform player;
    public Animator animator;
    public GameObject attackPoint;

    private float attackTimer;
    private bool isAttacking;
    private bool isChasing;
    public int hitCount = 0; //

    public enum BossState
    {
        Chase,
        Attack,
        Hit,
        Sleep,
        Dead //
    }

    public BossState currentBossState = BossState.Sleep;

    void Start()
    {
        attackTimer = Time.time;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
       
        HandleStateTransitions();

        
        UpdateState();
    }

    void HandleStateTransitions()
    {
        if (player == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

      
        if (hitCount >= 2)
        {
            currentBossState = BossState.Dead;
            return;
        }

      
        if (isAttacking && animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("IsAttacking"))
        {
            currentBossState = BossState.Chase;
            isAttacking = false;
        }

       
        if (distanceToPlayer <= attackRange && Time.time >= attackTimer)
        {
            currentBossState = BossState.Attack;
            attackTimer = Time.time + attackCooldown;
            isAttacking = true;
        }
       
        else if (distanceToPlayer <= chaseRadius)
        {
            currentBossState = BossState.Chase;
        }
      
        else if (distanceToPlayer > sleepRadius)
        {
            currentBossState = BossState.Sleep;
        }

        //
        if (currentBossState == BossState.Hit && animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit"))
        {
            currentBossState = BossState.Chase;
            if (hitCount < 2)
            {
                currentBossState = BossState.Chase; 
            }
        }
    }

    void UpdateState()
    {
        switch (currentBossState)
        {
            case BossState.Chase:
                UpdateChaseState();
                break;
            case BossState.Attack:
                UpdateAttackState();
                break;
            case BossState.Hit:
                UpdateHitState();
                break;
            case BossState.Sleep:
                UpdateSleepState();
                break;
            case BossState.Dead:
                UpdateDeadState();
                break;
        }
    }

    void UpdateChaseState()
    {
        if (player == null) return;
       

        // 
        RotateToFacePlayer();

        // 
        SetAnimatorBools(false, false, false, true, false);
    }

    void UpdateAttackState()
    {
        if (player == null) return;
        //
        RotateToFacePlayer();

        // 
        SetAnimatorBools(false, true, false, false, false);

        // 
        PerformAttack();
    }

    void UpdateHitState()
    {
        // 
        SetAnimatorBools(false, false, true, false, false);

        // 
        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit"))
        {
            currentBossState = BossState.Chase;
        }
    }

    void UpdateSleepState()
    {
        //
        SetAnimatorBools(true, false, false, false, false);
    }

   public void UpdateDeadState()
    {
        // 
        SetAnimatorBools(false, false, false, false, true);
        // 
        Destroy(gameObject, 2f); // 
    }

    void PerformAttack()
    {
        // 
        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.transform.position, attackRange);
        foreach (Collider target in hitColliders)
        {
            if (target.CompareTag("Player"))
            {
              
            }
        }
    }

    public void TakeDamage()
    {
        if (currentBossState != BossState.Hit && currentBossState != BossState.Dead)
        {
            currentBossState = BossState.Hit;
            hitCount++;
           
               
          
           
        }
    }

   

    void OnDrawGizmosSelected()
    {
        // »æÖÆ×·×Ù°ë¾¶
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // »æÖÆ¹¥»÷·¶Î§
        Gizmos.color = Color.red;
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
        }

        // »æÖÆºð½Ð°ë¾¶
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, roarDistance);

        // »æÖÆË¯Ãß°ë¾¶
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sleepRadius);
    }

    void RotateToFacePlayer()
    {
        if (player == null) return;
        Vector3 direction = player.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    void SetAnimatorBools(bool isSleeping, bool isAttacking, bool isHit, bool isChasing, bool isDead)
    {
        if (animator != null)
        {
            animator.SetBool("IsSleeping", isSleeping);
            animator.SetBool("IsAttacking", isAttacking);
            animator.SetBool("IsHit", isHit);
            animator.SetBool("IsChasing", isChasing);
            animator.SetBool("IsDead", isDead);
        }
    }
}