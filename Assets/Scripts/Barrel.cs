using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Barrel : MonoBehaviour{
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private Collider2D[] results;
    private float[] rand_num = new float[8];

    private Vector2 direction;
    private bool canFall;
    private bool isGrounded;
    public float threshold = 0.5f;
    public float moveSpeed = 5f;

    private void Awake(){
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        for(int i=0;i<8;i++){
            rand_num[i] = Random.Range(0f, 1f);
        }
    }

    private Collider2D checkCollisions(){
        isGrounded = false;
        canFall = false;

        Collider2D groundCollider = null;
        Vector2 size = collider.bounds.size;
        results = Physics2D.OverlapBoxAll(transform.position, size, 0f);

        for(int i=0;i<results.Length;i++){
            GameObject hit = results[i].gameObject;

            if(hit.layer == LayerMask.NameToLayer("Ground")){
                groundCollider = results[i];
                isGrounded = true;
            }

            if(hit.layer == LayerMask.NameToLayer("FallCollider_0") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[0] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_0") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);

            if(hit.layer == LayerMask.NameToLayer("FallCollider_1") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[1] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_1") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);

            if(hit.layer == LayerMask.NameToLayer("FallCollider_2") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[2] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_2") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);

            if(hit.layer == LayerMask.NameToLayer("FallCollider_3") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[3] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_3") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);

            if(hit.layer == LayerMask.NameToLayer("FallCollider_4") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[4] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_4") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);

            if(hit.layer == LayerMask.NameToLayer("FallCollider_5") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[5] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_5") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);

            if(hit.layer == LayerMask.NameToLayer("FallCollider_6") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[6] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_6") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);

            if(hit.layer == LayerMask.NameToLayer("FallCollider_7") && (results[i].transform.right.x * direction.x) > 0){
                canFall = (rand_num[7] > threshold);
                if(!canFall)
                    Physics2D.IgnoreCollision(collider, results[i], true);
            }
            if(hit.layer == LayerMask.NameToLayer("FallCollider_7") && (results[i].transform.right.x * direction.x) < 0)
                Physics2D.IgnoreCollision(collider, results[i], true);
        }
        return groundCollider;
    }

    private void FixedUpdate(){
        Collider2D groundCollider = checkCollisions();

        if(canFall){
            direction.x = 0;
            direction.y = -moveSpeed;
            if(groundCollider != null)
                Physics2D.IgnoreCollision(collider, groundCollider, true);
        }
        else if(isGrounded){
            direction.x = groundCollider.transform.right.x * moveSpeed;
        }
        else{
            direction.y = -moveSpeed;
        }

        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }
}