using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;

public class PlayerAgent : Agent
{
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] results1;
    private Collider2D[] results2;
    public Sprite[] runSprites;
    public Sprite[] climbSprites;
    GameObject[] barrels;

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

    float maxY = -5.25f;

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

        if (transform.position.y > maxY && isGrounded)
        {
            AddReward((transform.position.y - maxY) * 2);
            maxY = transform.position.y;
        }
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
            AddReward(+15f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-5f);
            EndEpisode();
        }
    }

    private void getBarrelPosition()
    {
        barrels = GameObject.FindGameObjectsWithTag("Obstacle");
        barrels = barrels.OrderBy(g => Vector3.Distance(transform.position, g.transform.position)).ToArray();
    }


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
        barrels = GameObject.FindGameObjectsWithTag("Obstacle");
        for (int i = 0; i < barrels.Length; i++)
        {
            if (barrels[i].layer != LayerMask.NameToLayer("Kong"))
                Destroy(barrels[i]);
        }
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
        getBarrelPosition();
        getCollision();
        checkCollisions();

        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(objectivePosition.x);
        sensor.AddObservation(objectivePosition.y);
        sensor.AddObservation(canClimb);
        sensor.AddObservation(isGrounded);

        if (barrels.Length >= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                sensor.AddObservation(barrels[i].transform.position.x);
                sensor.AddObservation(barrels[i].transform.position.y);
            }
            for (int i = 0; i < 5; i++)
            {
                sensor.AddObservation(barrels[i].GetComponent<Rigidbody2D>().linearVelocity);
            }
        }
        else
        {
            for (int i = 0; i < barrels.Length; i++)
            {
                sensor.AddObservation(barrels[i].transform.position.x);
                sensor.AddObservation(barrels[i].transform.position.y);
            }
            for (int i = 0; i < 5 - barrels.Length; i++)
            {
                sensor.AddObservation(10);
                sensor.AddObservation(10);
            }

            for (int i = 0; i < barrels.Length; i++)
            {
                sensor.AddObservation(barrels[i].GetComponent<Rigidbody2D>().linearVelocity);
            }
            for (int i = 0; i < 5 - barrels.Length; i++)
            {
                sensor.AddObservation(0);
                sensor.AddObservation(0);
            }

        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Recieves 2 continous values and 1 discrete value
        // continous[0] -> Horizontal, continous[1] -> Vertical, discrete[0] -> Jump/Not

        moveX = actions.ContinuousActions[0];
        moveY = actions.ContinuousActions[1];
        jump = actions.DiscreteActions[0];

        AddReward(-1f / MaxStep);
        MoveAgent();
    }
}