using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{
    private int lives;
    private int score;

    private void Start(){
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    private void NewGame(){
        lives = 3;
        score = 0;

        // Load Level-1
        LoadLevel(1);
    }

    private void LoadLevel(int index){
        Camera camera = Camera.main;

        if(camera != null){
            camera.cullingMask = 0;
        }

        Invoke(nameof(LoadScene), 1f);

        // SceneManager.LoadScene(index);
    }

    private void LoadScene(){
        SceneManager.LoadScene(1);
    }

    public void LevelComplete(){
        score += 1000;

        // Load next level
        LoadLevel(1);
    }

    public void LevelFailed(){
        lives--;
        
        if(lives <= 0){
            NewGame();
        }
        else{
            // Reload current level
            LoadLevel(1);
        }
    }
}