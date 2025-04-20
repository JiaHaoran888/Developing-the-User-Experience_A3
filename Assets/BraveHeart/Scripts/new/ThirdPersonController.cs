using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Combat")]
    // 
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;

    [Header("References")]
    public Camera cam; //
    private Animator animator;

    private Vector3 moveDirection;
    private bool isGrounded;
    // 
    private bool isAttacking;
    private bool wasMovingBeforeAttack = false;

    //UI
    public CanvasGroup tipPanelobj;
    public Text tiptext;
    public SimpleSettings settingPanel;
    //smallmap
    public Transform smallmapTrans;
    private Camera smallmapCamera;

    public enum PlayerState
    {
        Idle,
        Walking,
        Hit,
        Attacking
    }

    public PlayerState currentPlayerState = PlayerState.Idle;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        smallmapCamera = smallmapTrans.GetComponent<Camera>();
        animator = GetComponent<Animator>();
       
        GameMgr.instance.canmove = false;
        switch (GameMgr.instance.difficulity)
        {
            case 1:
                smallmapCamera.orthographicSize = 20f;
                smallmapTrans.position = new Vector3(17.72f, 31.94f, 18.18f);
                break;
            case 2:
                smallmapCamera.orthographicSize = 30.6f;
                smallmapTrans.position = new Vector3(27.4f, 31.9f, 29);
                break;
            case 3:
                smallmapCamera.orthographicSize = 40.3f;
                smallmapTrans.position = new Vector3(38.3f, 31.9f, 37.4f);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);

        // Handle PlayerState transitions
        HandlePlayerStateTransitions();
        UpdatePlayerState();

        if (!GameMgr.instance.canmove)
        {
            currentPlayerState = PlayerState.Idle;
            if (animator != null)
            {
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsHit", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.canvasGroup.alpha = 1;
            Cursor.lockState = CursorLockMode.None; // 
        }
    }

    void HandlePlayerStateTransitions()
    {
       
        if (GameMgr.instance.canmove)
        {
            // Check for attack input
            if (Input.GetButtonDown("Fire1") && !isAttacking)
            {
                isAttacking = true;
                var previousState = currentPlayerState;
                currentPlayerState = PlayerState.Attacking;
                wasMovingBeforeAttack = previousState == PlayerState.Walking;
            }

            // Check for movement input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool isMoving = horizontal != 0 || vertical != 0;

            // Only change state if not in Hit or Attacking state
            if (currentPlayerState != PlayerState.Hit && currentPlayerState != PlayerState.Attacking)
            {
                if (isMoving)
                {
                    currentPlayerState = PlayerState.Walking;
                }
                else
                {
                    currentPlayerState = PlayerState.Idle;
                }
            }
        }
    }

    void UpdatePlayerState()
    {
        switch (currentPlayerState)
        {
            case PlayerState.Idle:
                UpdateIdlePlayerState();
                break;
            case PlayerState.Walking:
               
                if (GameMgr.instance.canmove)
                {
                    UpdateWalkingPlayerState();
                }
                break;
            case PlayerState.Attacking:
               
                if (GameMgr.instance.canmove)
                {
                    UpdateAttackingPlayerState();
                }
                break;
            case PlayerState.Hit:
                UpdateHitPlayerState();
                break;
        }
    }

    void UpdateIdlePlayerState()
    {
        // Play idle animation
        if (animator != null)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsHit", false);
        }
    }

    void UpdateWalkingPlayerState()
    {
        // Play walking animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsHit", false);
        }

        // Handle movement based on camera direction
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        if (move.magnitude >= 0.1f && cam != null && GameMgr.instance.canmove)
        {
            // Get the camera's forward direction and make it horizontal
            Vector3 cameraForward = cam.transform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            // Calculate the target direction based on camera forward and input
            Vector3 targetDirection = (cameraForward * vertical + cam.transform.right * horizontal).normalized;

            // Move character
            transform.position += targetDirection * moveSpeed * Time.deltaTime;

            // Rotate character to face movement direction
            if (targetDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(targetDirection);
            }
        }
    }

  

    void UpdateAttackingPlayerState()
    {
        // Play attack animation
        if (animator != null && Cursor.lockState == CursorLockMode.Locked)
        {
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsWalking", wasMovingBeforeAttack); // Continue walking if was moving before attack
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsHit", false);
        }

        // Perform attack

        PerformAttack();
        // Check if attack animation is finished
        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("IsAttacking"))
        {
            isAttacking = false;
            if (wasMovingBeforeAttack)
            {
                currentPlayerState = PlayerState.Walking;
            }
            else
            {
                currentPlayerState = PlayerState.Idle;
            }
        }
    }

    void UpdateHitPlayerState()
    {
        // Play hit animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsHit", true);
        }

        // Check if hit animation is finished
        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("IsHit"))
        {
            currentPlayerState = PlayerState.Idle;
        }
    }

    void PerformAttack()
    {
        // Check for enemies in attack range
        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitColliders)
        {
            if (enemy.CompareTag("Dragon") && this.currentPlayerState == PlayerState.Attacking)
            {
                GameMgr.instance.SetSfxSound(GameMgr.instance.hit);
                BossController bossController = enemy.GetComponentInParent<BossController>();
                bossController.TakeDamage();
               
                if (bossController.hitCount >= 2)
                {
                    bossController.UpdateDeadState();
                    string[] message = { "Congratulations", "You have defeated the dragon and saved the people of the whole country" };
                    showTip(message, () =>
                    {
                        SceneManager.UnloadSceneAsync(3);
                        Cursor.lockState = CursorLockMode.None; // 
                    });
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wizard"))
        {
            string[] messages = { "Warrior, this shield is for you", "Go and slay the dragon", "May God bless you" };
            showTip(messages, () =>
            {
                SceneManager.UnloadSceneAsync(1);
                SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
            });
        }
        else if (other.gameObject.CompareTag("King"))
        {
            string[] messages = { "Warrior, this dragon-slaying sword and clothes is for you", "Go and defeat the dragon", "You are the only hope for the whole country" };
            showTip(messages, () =>
            {
                SceneManager.UnloadSceneAsync(2);
                GameMgr.instance.SetGloabMusic(GameMgr.instance.fight);
                SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
            });
        }
    }

    void showTip(string[] messages, System.Action oncomplete)
    {
        GameMgr.instance.canmove = false;
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append(tipPanelobj.DOFade(1, 1)).OnComplete(() =>
        {
        });
        foreach (string message in messages)
        {
            sequence.Append(tiptext.DOText("", 1f)); // clear last message
            sequence.Append(tiptext.DOText(message, 3));
        }

        sequence.OnComplete(() =>
        {
            GameMgr.instance.canmove = true;
            oncomplete?.Invoke();
        });
    }

    void OnDrawGizmosSelected()
    {
        // Draw attack range
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}