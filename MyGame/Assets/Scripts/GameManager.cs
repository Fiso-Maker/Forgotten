using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    inGame,
    gameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState currentGameState = GameState.inGame;

    void Awake(){
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    void SetGameState(GameState newGameState)
    {
        if(newGameState == GameState.inGame)
        {}
        else if(newGameState == GameState.gameOver)
        {}
        currentGameState = newGameState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        SetGameState (GameState.inGame);
    }

    public void GameOver()
    {

    }

}
