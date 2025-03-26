using Unity.VisualScripting;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class PlayerAgent : Agent{
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] results1;
    private Collider2D[] results2;
    public Sprite[] runSprites;
    public Sprite[] climbSprites;

    private Vector2 direction;

    public bool isClimbing = false;
    private int runSpriteIndex;
    private int climbSpriteIndex;
    private bool isGrounded;
    private bool canClimb;
    public float moveSpeed = 2f;
    public float climbSpeed = 3f;
    public float jumpStrength = 4f;

    public GameManager gameManager;

    public float moveX;
    public float moveY;
    public int jump;

    protected override void OnEnable(){
        InvokeRepeating(nameof(animateSprites), 1f/12f, 1f/12f);
    }

    protected override void OnDisable(){
        CancelInvoke();
    }

    private void Start(){
        gameManager = FindObjectOfType<GameManager>();
    }

    protected override void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    private void getCollision(){
        Vector2 size = collider.bounds.size;

        size.y += 0.1f;
        size.x /= 2f;
        results1 = Physics2D.OverlapBoxAll(transform.position, size, 0f);

        size = collider.bounds.size;
        size.y += 0.1f;
        size.x *= 2f;
        results2 = Physics2D.OverlapBoxAll(transform.position, size, 0f);
    }

    private void checkCollisions(){
        isGrounded = false;
        canClimb = false;

        for(int i=0;i<results1.Length;i++){
            GameObject hit = results1[i].gameObject;
            
            if(hit.layer == LayerMask.NameToLayer("Ground")){
                isGrounded = hit.transform.position.y < (transform.position.y - 0.5f);
                Physics2D.IgnoreCollision(collider, results1[i], !isGrounded);
            }
            else if(hit.layer == LayerMask.NameToLayer("Ladder")){
                canClimb = true;
            }
        }

        for(int i=0;i<results2.Length;i++){
            GameObject hit = results2[i].gameObject;
            
            if(hit.layer == LayerMask.NameToLayer("FallCollider_0")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_1")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_2")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_3")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_4")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_5")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_6")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_7")){
                Physics2D.IgnoreCollision(collider, results2[i], true);
            }
        }
    }

    private void Update(){
        getCollision();
        checkCollisions();

        if(canClimb){
            direction.y = moveY * climbSpeed;
        }
        else if(jump == 1 && isGrounded){
            direction = Vector2.up * jumpStrength;
        }
        else{
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = moveX * moveSpeed;

        if(isGrounded)
            direction.y = Mathf.Max(direction.y, -1f);
        
        if(direction.x > 0f){
            transform.eulerAngles = Vector3.zero;
        }
        else if(direction.x < 0f){
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate(){
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void animateSprites(){
        if(canClimb && !isGrounded && direction.y != 0f){
            climbSpriteIndex++;

            if(climbSpriteIndex >= climbSprites.Length)
                climbSpriteIndex = 0;

            spriteRenderer.sprite = climbSprites[climbSpriteIndex];
        }
        else if(direction.x != 0){
            runSpriteIndex++;

            if(runSpriteIndex >= runSprites.Length)
                runSpriteIndex = 0;
            
            spriteRenderer.sprite = runSprites[runSpriteIndex];
        }
        else if(direction.x == 0){
            runSpriteIndex = 0;
            spriteRenderer.sprite = runSprites[runSpriteIndex];
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continousActions[0] = Input.GetAxis("Horizontal");
        continousActions[1] = Input.GetAxis("Vertical");
        if(Input.GetButtonDown("Jump"))
            discreteActions[0] = 1;
        else
            discreteActions[0] = 0;
        Debug.Log($"Heuristic - Horizontal: {continousActions[0]}, Vertical: {continousActions[1]}, Jump: {discreteActions[0]}");
    }

    public override void OnEpisodeBegin(){
        transform.position = new Vector3(-5.25f, -5.1f, 0f);
        rigidbody.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Objective")){
            SetReward(+2f);
            EndEpisode();
        }
        else if(collision.gameObject.CompareTag("Obstacle")){
            SetReward(-1f);
            EndEpisode();
        }
    }

    [SerializeField] private Transform objectiveTransform;
    public override void CollectObservations(VectorSensor sensor){
        Debug.Log(objectiveTransform.position);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(objectiveTransform);
    }

    public override void OnActionReceived(ActionBuffers actions){
        // Recieves 2 continous values and 1 discrete value
        // continous[0] -> Horizontal, continous[1] -> Vertical, discrete[0] -> Jump/Not
        // Debug.Log(actions.ContinuousActions[0]);
        // Debug.Log(actions.ContinuousActions[1]);
        // Debug.Log(actions.DiscreteActions[0]);

        moveX = actions.ContinuousActions[0];
        moveY = actions.ContinuousActions[1];
        jump = actions.DiscreteActions[0];
    }
}