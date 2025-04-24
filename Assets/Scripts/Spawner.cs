using UnityEngine;

public class Spawner : MonoBehaviour{
    public GameObject prefab;
    public float minTime = 1f;
    public float maxTime = 2f;

    private void Start(){
        Spawn();
    }

    private void Spawn(){
        Instantiate(prefab, transform.position, Quaternion.identity);
        Invoke(nameof(Spawn), 3f);
    }
}
