using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour{
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

    private void OnEnable(){
        InvokeRepeating(nameof(animateSprites), 1f/12f, 1f/12f);
    }

    private void OnDisable(){
        CancelInvoke();
    }

    private void Start(){
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Awake(){
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
            direction.y = Input.GetAxis("Vertical") * climbSpeed;
        }
        else if(Input.GetButtonDown("Jump") && isGrounded){
            direction = Vector2.up * jumpStrength;
        }
        else{
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = Input.GetAxis("Horizontal") * moveSpeed;

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

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Objective")){
            enabled = false;
            gameManager.LevelComplete();
        }
        else if(collision.gameObject.CompareTag("Obstacle")){
            enabled = false;
            gameManager.LevelFailed();
        }
    }
}