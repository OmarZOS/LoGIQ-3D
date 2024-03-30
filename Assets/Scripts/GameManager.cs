using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using static System.Math;

public class GameManager : MonoBehaviour
{


    public GameObject playerPrefab;
    public GameObject floatingScorePrefab;
    public const int PLAYERS_NUMBER = 4;
    GameObject[] players = new GameObject[PLAYERS_NUMBER];
    int currentPlayerIndex = 0;

    // TODO: you can do better than this man..
    Color[] colorValues = new Color[] {Color.red,Color.green,Color.black,Color.blue,Color.cyan,Color.gray,Color.magenta,Color.white,Color.yellow};

    public Material fadingShader;
    public GameObject shardPrefab;
    public GameObject torusPrefab;
    
    public GameObject arrowPrefab;
    public int ARROW_SHOTS = 10;
    int arrowCount;
    int losingPlayerIndex=0;
    float losingPlayerScore=0;

    public Vector3 arrowPosition = new Vector3(7.3f,2f,0f);
    public int count=0;
    bool endGame = false;

    //this sounds like a better data structure for making rounds, it's alright for keeping it simple.. 
    List<int> playerTurn = new List<int>(PLAYERS_NUMBER);


    public GameObject menuPanel;

    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }


    public void ShowMenu()
    {
        menuPanel.SetActive(true);
    }

    void instantiatePlayers(){
        for (int i = 0; i < PLAYERS_NUMBER; i++)
        {
            players[i] = PlayerScript.CreatePlayer(playerPrefab,floatingScorePrefab,SphericalToCartesian(2.7f,i*360/PLAYERS_NUMBER,90),colorValues[i%colorValues.Length]);   
            playerTurn.Add(i);
        }
    }

    void Start()
    {
        menuPanel.SetActive(false);
        // Starting by 4 players: red, green, black and blue
        instantiatePlayers();
        Camera.main.GetComponent<CameraManager>().rotateDegrees = 360.0f/PLAYERS_NUMBER;
        StartRound();
    }

    void StartTurn() {
        players[playerTurn[currentPlayerIndex]].GetComponent<PlayerScript>().StartTurn();
    }

    void FixSpotLight(){
        bool newRound = false;
        if(currentPlayerIndex==0)
            newRound = true;
        float degrees= ( 
             (playerTurn[currentPlayerIndex])*360/PLAYERS_NUMBER +
                (Camera.main.transform.eulerAngles.y-270.0f)
            );
        // Debug.Log("currentIndex: "+currentPlayerIndex);
        // Debug.Log("PlayerTurn: "+playerTurn[currentPlayerIndex]);
        // Debug.Log("Player: "+(playerTurn[currentPlayerIndex]*360/PLAYERS_NUMBER));
        // Debug.Log("Camera: "+-(Camera.main.transform.eulerAngles.y-270.0f));
        // Debug.Log("Rotating: "+degrees);
        // degrees = Mathf.Round(Mathf.Abs((degrees)/180)  / 90) * 90;
        Camera.main.GetComponent<CameraManager>().AutoRotate(PLAYERS_NUMBER,degrees);
        
        if (endGame)
            Debug.Log("They told me that the end was worth it..");
        
        else
        {   
            if (newRound) 
                StartRound();
            else 
                StartTurn();
        }
    }

    public void EndTurn() {
        //updating the losing player index
        float score = players[playerTurn[currentPlayerIndex]].GetComponent<PlayerScript>().GetScore();
        if(losingPlayerScore > score || playerTurn[currentPlayerIndex] == losingPlayerIndex ){
            losingPlayerScore = score;
            // repetitive, but simple enough..
            losingPlayerIndex = playerTurn[currentPlayerIndex];
        }

        if( ( (currentPlayerIndex+1)%playerTurn.Count) == 0 )
            EndRound();
            
        currentPlayerIndex = (currentPlayerIndex+1)%playerTurn.Count;
        FixSpotLight();
    }

    void StartRound() {
        StartTurn();
    }

    void EndRound(){

        int losingIndex = getLosingPlayerIndex(); 
        // Debug.Log("loser index: "+losingIndex);
        players[losingIndex]
            .GetComponent<PlayerScript>().kill(shardPrefab,fadingShader);
        // Debug.Log("Removing "+losingIndex);
        playerTurn.RemoveAt(playerTurn.IndexOf(losingIndex));
        
        if (playerTurn.Count<2)
            endGame = true;
        else
            //needed this to fix turn's array indexing
            currentPlayerIndex=(currentPlayerIndex + playerTurn.Count -1)%playerTurn.Count;

    }
    int getLosingPlayerIndex(){
        // makign sure it will be updated next time..
        losingPlayerScore = 200000.0f;
        return losingPlayerIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7)){
            HideMenu();
        }
        if (Input.GetKeyDown(KeyCode.Keypad9)){
            ShowMenu();
        }
    }

    public Vector3 SphericalToCartesian(float radius, float polar, float elevation)
    {
        // Convert degrees to radians
        polar = polar * Mathf.Deg2Rad;
        elevation = elevation * Mathf.Deg2Rad;

        return new Vector3(radius * Mathf.Sin(elevation) * Mathf.Cos(polar),
            radius * Mathf.Sin(elevation)
            ,radius * Mathf.Sin(elevation) * Mathf.Sin(polar));
    }
}
