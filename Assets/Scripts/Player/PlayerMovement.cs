using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 12f;
    public float speed = 12f;
    private float jumpPower = 19.6f;
    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float _inputX;
    private SpriteRenderer SpriteRenderer;

    public PhysicsMaterial2D highFrictionMaterial;
    public PhysicsMaterial2D zeroFrictionMaterial;
    private Collider2D playerCollider;
    private PhysicsMaterial2D lastAppliedMaterial;

    [Header("CheckGround")]
    [SerializeField] private Transform _groudChecker;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] bool _isGrounded;

    [Header("Dash")]
    private float dashPower = 2.5f;
    private float dashCoolTime = 1f;
    private bool candash = true;
    private bool isDashing = false;

    [Header("Footstep Sound")]
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.3f;
    public AudioSource footstepAudioSource; // Inspector에서 할당
    private float lastFootstepTime;

    private Vector2 lastPosition;
    private float minMoveDistance = 0.01f;

    private void Awake()
    {
        speed = baseSpeed;
    }

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        lastAppliedMaterial = null;

        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
            footstepAudioSource.playOnAwake = false;
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        if (isDashing) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && _isGrounded)
            Jump();

        if (Keyboard.current.shiftKey.wasPressedThisFrame && candash)
            StartCoroutine(Dash());

        ChangeMousePointPosition();
        Animator.SetFloat("Speed", Mathf.Abs(_inputX));

        PlayFootsteps();
        lastPosition = transform.position;
    }

    private IEnumerator Dash()
    {
        candash = false;
        isDashing = true;
        float dashDirection = _inputX != 0 ? Mathf.Sign(_inputX) : (SpriteRenderer.flipX ? -1 : 1);
        float originalGravity = Rigidbody2D.gravityScale;
        gameObject.layer = LayerMask.NameToLayer("OnlyGroundCollision");
        Rigidbody2D.gravityScale = 0f;
        Rigidbody2D.linearVelocity = new Vector2(transform.localScale.x * dashPower * dashDirection, 0);
        Animator.SetBool("Slide", true);
        yield return new WaitForSeconds(0.17f);
        gameObject.layer = LayerMask.NameToLayer("Player");
        Rigidbody2D.gravityScale = originalGravity;
        isDashing = false;
        Animator.SetBool("Slide", false);
        yield return new WaitForSeconds(dashCoolTime);
        candash = true;
    }

    private void Jump()
    {
        Rigidbody2D.linearVelocityY += jumpPower;
    }

    public void FacePosition(Vector2 targetPosition)
    {
        SpriteRenderer.flipX = targetPosition.x < transform.position.x;
    }

    private void FixedUpdate()
    {
        _isGrounded = CheckGround();
        Animator.SetBool("IsGround", _isGrounded);

        if (_isGrounded)
        {
            if (playerCollider.sharedMaterial != highFrictionMaterial)
            {
                playerCollider.sharedMaterial = highFrictionMaterial;
                lastAppliedMaterial = highFrictionMaterial;
            }
        }
        else
        {
            if (playerCollider.sharedMaterial != zeroFrictionMaterial)
            {
                playerCollider.sharedMaterial = zeroFrictionMaterial;
                lastAppliedMaterial = zeroFrictionMaterial;
            }
        }

        if (!isDashing)
        {
            Rigidbody2D.linearVelocityX = _inputX * speed;
        }
    }

    public bool CheckGround()
    {
        Collider2D collider = Physics2D.OverlapBox(_groudChecker.position, _groundCheckSize, 0, _whatIsGround);
        return collider;
    }

    private void ChangeMousePointPosition()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        FacePosition(mouseWorldPos);
    }

    private void OnDrawGizmos()
    {
        if (_groudChecker != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_groudChecker.position, _groundCheckSize);
        }
    }

    public void OnMove(InputValue value)
    {
        _inputX = value.Get<Vector2>().x;
    }

    private void PlayFootsteps()
    {
        float distanceMoved = Vector2.Distance(lastPosition, transform.position);

        if (_isGrounded && distanceMoved > minMoveDistance && !isDashing)
        {
            if (Time.time - lastFootstepTime > footstepInterval)
            {
                PlayRandomFootstep();
                lastFootstepTime = Time.time;
            }
        }
    }

    private void PlayRandomFootstep()
    {
        if (footstepSounds.Length == 0 || footstepAudioSource == null) return;
        int index = Random.Range(0, footstepSounds.Length);
        AudioClip clip = footstepSounds[index];
        if (clip != null)
        {
            footstepAudioSource.PlayOneShot(clip);
        }
    }
}
