using UnityEngine;
using TMPro;


public class PlayerScript : MonoBehaviour
{
    public GameObject scoreTextPrefab ;
    
    GameObject arrow;
    GameObject torus;
    
    // public Color textColor;
    public static float scoreHeight = 1.0f;
    
    public bool active = false;
    public bool playing = false;
    float score = 0.0f;
    // this is the parameter that defines the orientation of the arrow..
    int orientation_code;
    int color_code;

    public static Color[] possibleArrowColors = new Color[] {
        new Color(0.0000f, 0.5020f, 0.5020f), // teal
        new Color(1.0000f, 0.7529f, 0.0000f), // gold
        new Color(0.3725f, 0.6196f, 0.6275f), // bluish-green
        new Color(0.9412f, 0.5020f, 0.5020f), // salmon
        new Color(0.6667f, 0.4314f, 0.6863f), // lavender
        new Color(0.8353f, 0.3686f, 0.0000f), // brown
        new Color(0.9922f, 0.6706f, 0.3882f), // peach
        new Color(0.6431f, 0.8039f, 0.8824f), // light blue
        new Color(0.7098f, 0.4235f, 0.0000f), // rust
        new Color(1.0000f, 0.4000f, 0.8000f), // pink
        new Color(0.6510f, 0.3373f, 0.1569f), // dark brown
        new Color(0.8627f, 0.0784f, 0.2353f), // crimson
        new Color(0.5098f, 0.6824f, 0.3412f), // olive green
        new Color(0.4392f, 0.1765f, 0.4980f), // dark purple
        new Color(0.9020f, 0.7451f, 0.5176f), // beige
        new Color(0.0000f, 0.0000f, 0.8039f), // dark blue
        new Color(0.9333f, 0.5098f, 0.9333f), // violet
        new Color(0.9608f, 0.8706f, 0.7020f), // light beige
        new Color(0.6039f, 0.8039f, 0.1961f), // lime
        new Color(0.8784f, 0.8784f, 0.8784f), // light gray
        new Color(0.4784f, 0.0627f, 0.8941f), // indigo
        new Color(0.3333f, 0.3294f, 0.4314f), // dark gray
        new Color(0.1569f, 0.4471f, 0.6353f), // deep sky blue
        new Color(0.9373f, 0.1608f, 0.6314f) // magenta
    };

    Color[] arrowColors;
    int arrowShots = 0;

    

    public static GameObject CreatePlayer(GameObject playerPrefab,GameObject floatingScorePrefab,Vector3 position,Color Coloring) {
        GameObject player;

        if(playerPrefab)
            player = Instantiate(playerPrefab,position,Quaternion.identity);
        else{
            player = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // player.AddComponent<PlayerScript>();
        }
        
        // player.transform.position = position;
        
        // Get the Renderer component from the new player
        var playerRenderer = player.GetComponent<MeshRenderer>();
        // Call SetColor using the shader property name "_Color" and setting the color to the wanted color
        playerRenderer.material.SetFloat("_Mode",3);
        playerRenderer.material.SetColor("_Color", Coloring);

        PlayerScript.CreateFloatingText(floatingScorePrefab,player.transform.position,Coloring).transform.SetParent(player.transform);

        return player;
    }

    private static GameObject CreateFloatingText(GameObject scoreTextPrefab,Vector3 position,Color textColor) {
        
        GameObject FloatingText = Instantiate(scoreTextPrefab,position+new Vector3(0f,scoreHeight,0f),Quaternion.identity);
        FloatingText.GetComponent<TMP_Text>().color = textColor;
        FloatingText.GetComponent<TMP_Text>().text = "0";
        
        FloatingText.transform.rotation = Quaternion.LookRotation(FloatingText.transform.position - Camera.main.transform.position  , Vector3.up);
        // facingObject.transform.rotation = Quaternion.LookRotation(facingObject.transform.position - camTransform.position  , Vector3.up);
        return FloatingText;
    }

    private void Start() {
    }
    
    

    private void createTorus(){

        GameObject torus = Instantiate(GameObject.Find("GameManager")
            .gameObject.GetComponent<GameManager>().torusPrefab
            ,GameObject.Find("Arrow Holder").transform.position
            ,GameObject.Find("Arrow Holder").transform.rotation);

        torus.transform.Rotate(-45, 90, -90, Space.Self);
        // torus.transform.Translate(0, -4f, 0, Space.Self);
        torus.transform.SetParent(GameObject.Find("Arrow Holder").transform);

        torus.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>()
            .material.SetColor("_Color",arrowColors[0]);
        
        torus.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>()
            .material.SetColor("_Color",arrowColors[1]);
        
        torus.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>()
            .material.SetColor("_Color",arrowColors[2]);
        
        torus.transform.GetChild(3).gameObject.GetComponent<MeshRenderer>()
            .material.SetColor("_Color",arrowColors[3]);
        
    }

    private void Update() {
        
        if(active){
            if (playing){
                if (Input.GetKeyDown(KeyCode.Keypad8))
                    updateScore(checkAnswer(0));
                if (Input.GetKeyDown(KeyCode.Keypad6))
                    updateScore(checkAnswer(1));
                if (Input.GetKeyDown(KeyCode.Keypad2))
                    updateScore(checkAnswer(2));
                if (Input.GetKeyDown(KeyCode.Keypad4))
                    updateScore(checkAnswer(3));
            }
            else
                if (Input.GetKeyDown(KeyCode.Keypad5))
                    BeginShooting();
        }   
    }

    // to be used by the game manager..
    public void StartTurn() {
        arrowColors =  SelectTurnColors();
        arrowShots = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().ARROW_SHOTS;
        createTorus();
        active = true;
    }
    public void EndTurn() {
        playing = false;
        GameObject torus = GameObject.Find("torus(Clone)").gameObject;
        Destroy(torus);
        active = false;
        GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().EndTurn();
    }

    float checkAnswer(int number){
        if(number != ((orientation_code+color_code)%4))
            return 0.0f;
        float lifetime = Time.time - arrow.GetComponent<ArrowScript>().creationTime;
        return 10/lifetime;
    }
    void updateScore(float addition){
        score+= addition;
        // getting the floating text and updating it..
        if (this.transform.GetChild(0))
            this.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = score.ToString("#.00");
        Destroy(arrow);
        shootArrow();
    }

    public void BeginShooting(){
        playing = true;
        shootArrow();   
    }

    void shootArrow(){
        if(arrowShots-- > 0){
            orientation_code =Random.Range(0,4);
            CreateArrow(orientation_code);
        }
        else{
            EndTurn();
        }
    }

    void CreateArrow(int orientation){
        Transform holder_transform = GameObject.Find("Arrow Holder").transform;
        GameObject arrow_prefab = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>().arrowPrefab;
        // instantiation in the wanted position, and the rotation aTurn the object only..
        // then fixing our arrow's rotation to face the camera
        arrow = ArrowScript.CreateArrow(arrow_prefab,holder_transform,-orientation_code*90f,ChooseColor());
        // arrow.transform.rotation *= holder_transform.rotation;
        playing= true;
    }
    
    Color ChooseColor(){
        color_code = Random.Range(0, 4);
        return arrowColors[color_code];
    }

    Color[] SelectTurnColors() {
        // Generate four random, distinct indices into the possibleArrowColors array
        int[] randomIndices = new int[4];
        for (int i = 0; i < 4; i++)
        {
            int randomIndex;
            do
                randomIndex = Random.Range(0, possibleArrowColors.Length);
            while (System.Array.IndexOf(randomIndices, randomIndex) != -1);
            randomIndices[i] = randomIndex;
        }

        // Retrieve the four selected colors
        Color[] selectedColors = new Color[4];
        for (int i = 0; i < 4; i++)
            selectedColors[i] = possibleArrowColors[randomIndices[i]];
        return selectedColors;
    }

    public float GetScore() {
        return score;
    }

    public void kill(GameObject shardPrefab,Material fadingShader) {
        killPlayer(shardPrefab,fadingShader);
    }

    private void killPlayer(GameObject shardPrefab,Material fadingShader) {
        GameObject floater = this.transform.GetChild(0).gameObject;
        floater.transform.SetParent(null);
        Destroy(floater);

        shardPrefab.GetComponent<MeshRenderer>().material = this.gameObject.GetComponent<MeshRenderer>().material;
        // shardPrefab.GetComponent<Renderer>().material.SetFloat("_Mode",3);
        shardPrefab.transform.position = this.gameObject.transform.position;
        ShardScript.Fragment(shardPrefab,fadingShader);//shardPrefab
        Destroy(this.gameObject);
    }

}

