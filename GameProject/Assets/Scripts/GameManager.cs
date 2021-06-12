using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    Start,
    inGame,
    gameOver_Win,
    gameOver_Lose
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState currentGameState = GameState.inGame;

    void Awake(){
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    void SetGameState(GameState newGameState)
    {
        if(newGameState == GameState.Start)
        {
            SceneLoader.instance.load("Start");
        }
        else if(newGameState == GameState.inGame)
        {
            SceneLoader.instance.load("InGame");
        }
        else if(newGameState == GameState.gameOver_Win)
        {
            SceneLoader.instance.load("Win");
        }
        else if(newGameState == GameState.gameOver_Lose)
        {
            SceneLoader.instance.load("Lose");
        }
        currentGameState = newGameState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        SetGameState (GameState.Start);
    }
    public void StartinGame()
    {
        SetGameState(GameState.inGame);
    }

    public void GameOver_Win()
    {
        SetGameState(GameState.gameOver_Win);
    }
    public void GameOver_Lose()
    {
        SetGameState(GameState.gameOver_Lose);
    }

}
