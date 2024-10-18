using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] public float speed = 10.0f;
    public float laneDistance = 2.0f;
    public float laneSwitchSpeed = 5.0f;

    [Header("Jump Settings")] public float jumpSpeed = 7.0f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Roll Settings")] public float rollDuration = 0.5f;
    public float rollSpeedMultiplier = 1.5f;

    [Header("Ground Check")] public float groundCheckDistance = 0.1f;

    [Header("Animation")] private Animator _animator;
    private Animator _mosterAnimator;
    private int currentLane = 0;
    private bool isJumping = false;
    private bool isDead = false;
    private bool isRolling = false;
    private float rollTimer = 0.0f;
    private float targetX;
    [Header("EndGame")] [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameOverController gameOverController;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _mosterAnimator = transform.GetChild(0).GetComponent<Animator>();
        // originalScale = transform.localScale;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
        gameOverController = FindObjectOfType<GameOverController>();
    }

    private void Update()
    {
        if (!isDead)
        {
            HandleLaneSwitching();
            HandleJumpAndRoll();
        }


    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyBetterJumpPhysics();
    }

    void HandleLaneSwitching()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > -1)
        {
            currentLane--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 1)
        {
            currentLane++;
        }

        targetX = currentLane * laneDistance;
    }

    void HandleJumpAndRoll()
    {
        // Debug.Log("handle");
        if (Input.GetKeyDown(KeyCode.UpArrow) && IsGrounded())
        {
            Jump();
            Debug.Log("JUmp");
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !isJumping && IsGrounded())
        {
            StartRoll();
            Debug.Log("Roll");
        }

        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0)
            {
                EndRoll();
            }
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse); // 使用 AddForce 跳跃
        isJumping = true;
        _animator.SetBool("IsJumping", true);
        _mosterAnimator.SetBool("IsJumping", true);
    }

    void ApplyBetterJumpPhysics()
    {
        if (rb.velocity.y < 0) // 下落时加速
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            if (IsGrounded())
            {
                isJumping = false;
                _animator.SetBool("IsJumping", false);
                _mosterAnimator.SetBool("IsJumping", false);
            }
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.UpArrow)) // 小跳
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void StartRoll()
    {
        isRolling = true;
        _animator.SetTrigger("IsRolling");
        rollTimer = rollDuration;
    }

    void EndRoll()
    {
        isRolling = false;

    }

    void ApplyMovement()
    {
        // Forward movement (Always moving forward)
        float currentSpeed = isRolling ? speed * rollSpeedMultiplier : speed;
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.fixedDeltaTime;

        // Lane switching (smoothly transition between lanes)
        float newX = Mathf.MoveTowards(rb.position.x, targetX, laneSwitchSpeed * Time.fixedDeltaTime);
        Vector3 newPosition = new Vector3(newX, rb.position.y, rb.position.z) + forwardMovement;

        rb.MovePosition(newPosition); // 通过MovePosition进行平滑移动
    }

    bool IsGrounded()
    {

        // Ground check to see if the player is touching the ground
        return !isRolling && Physics.Raycast(transform.position + Vector3.up, Vector3.down, groundCheckDistance);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over");
            isDead = true;
            _animator.SetBool("IsDead", true);
            timelineDirector.Play();
            audioSource.Play();
            speed = 0;
            GameManager.Instance.EndGame();
        }
    }
}
