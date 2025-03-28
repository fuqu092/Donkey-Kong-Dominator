using Unity.VisualScripting;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] results1;
    private Collider2D[] results2;
    public Sprite[] runSprites;
    public Sprite[] climbSprites;

    private Vector2 direction;

    private int runSpriteIndex;
    private int climbSpriteIndex;
    public bool isGrounded;
    public bool canClimb;
    public float moveSpeed = 2f;
    public float climbSpeed = 3f;
    public float jumpStrength = 4f;

    public float moveX;
    public float moveY;
    public int jump;

    private void getCollision()
    {
        Vector2 size = collider.bounds.size;

        size.y += 0.1f;
        size.x /= 2f;
        results1 = Physics2D.OverlapBoxAll(transform.position, size, 0f);

        size = collider.bounds.size;
        size.y += 0.1f;
        size.x *= 2f;
        results2 = Physics2D.OverlapBoxAll(transform.position, size, 0f);
    }

    private void checkCollisions()
    {
        isGrounded = false;
        canClimb = false;

        for (int i = 0; i < results1.Length; i++)
        {
            GameObject hit = results1[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                isGrounded = hit.transform.position.y < (transform.position.y - 0.5f);
                Physics2D.IgnoreCollision(collider, results1[i], !isGrounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                canClimb = true;
            }
        }

        for (int i = 0; i < results2.Length; i++)
        {
            GameObject hit = results2[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("FallCollider_0"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if (hit.layer == LayerMask.NameToLayer("FallCollider_1"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if (hit.layer == LayerMask.NameToLayer("FallCollider_2"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if (hit.layer == LayerMask.NameToLayer("FallCollider_3"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if (hit.layer == LayerMask.NameToLayer("FallCollider_4"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if (hit.layer == LayerMask.NameToLayer("FallCollider_5"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if (hit.layer == LayerMask.NameToLayer("FallCollider_6"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if (hit.layer == LayerMask.NameToLayer("FallCollider_7"))
            {
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
        }
    }

    private void MoveAgent()
    {
        getCollision();
        checkCollisions();

        if (canClimb)
        {
            direction.y = moveY * climbSpeed;
        }
        else if (jump == 1 && isGrounded)
        {
            direction = Vector2.up * jumpStrength;
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = moveX * moveSpeed;

        if (isGrounded)
            direction.y = Mathf.Max(direction.y, -1f);

        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void animateSprites()
    {
        if (canClimb && !isGrounded && direction.y != 0f)
        {
            climbSpriteIndex++;

            if (climbSpriteIndex >= climbSprites.Length)
                climbSpriteIndex = 0;

            spriteRenderer.sprite = climbSprites[climbSpriteIndex];
        }
        else if (direction.x != 0)
        {
            runSpriteIndex++;

            if (runSpriteIndex >= runSprites.Length)
                runSpriteIndex = 0;

            spriteRenderer.sprite = runSprites[runSpriteIndex];
        }
        else if (direction.x == 0)
        {
            runSpriteIndex = 0;
            spriteRenderer.sprite = runSprites[runSpriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            SetReward(+2f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    [SerializeField] private Transform objectiveTransform;
    Vector3 objectivePosition = new Vector3(-1f, 6.9f, 0f);

    public override void Initialize()
    {
        // base.Initialize();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        InvokeRepeating(nameof(animateSprites), 1f / 12f, 1f / 12f);
    }

    public override void OnEpisodeBegin()
    {
        transform.position = new Vector3(-5.25f, -5.1f, 0f);
        rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continousActions[0] = Input.GetAxis("Horizontal");
        continousActions[1] = Input.GetAxis("Vertical");
        discreteActions[0] = Input.GetButton("Jump") ? 1 : 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(objectivePosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Recieves 2 continous values and 1 discrete value
        // continous[0] -> Horizontal, continous[1] -> Vertical, discrete[0] -> Jump/Not

        moveX = actions.ContinuousActions[0];
        moveY = actions.ContinuousActions[1];
        jump = actions.DiscreteActions[0];

        MoveAgent();
    }
}