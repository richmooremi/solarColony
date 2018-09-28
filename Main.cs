/*
    This is the main class for the game and controls most of the action.

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    #region variables
    //gameObjects containing the new tutorial system
    public GameObject tutorialPanel, tutorialPanel2;

    //variables pulled from the title screen
    public PlayerStats stats;
    public string playerName;       
    public int difficulty;
    public Sprite avatar;

    //variables related to the ID badge panel
    public Image avatarImage, avatarImageMini;
    public Text nameText, nameTextMini;
    public Text badgeIDText;
    public Text difficultyText;
    public GameObject identCard;

    //variables related to the achievement system
    public GameObject venusAchievement, commerceAchievement, exploreAchievement,
        hydrogenAchievement, ironAchievement, sulfurAchievement,
        moneyAchievement, craftAchievement;
    public GameObject achievementPanel;
    public Text achievementText;
    public Dictionary<string, bool> achievements = new Dictionary<string, bool>();      //key: achievement name / value: true when complete
    public bool builtStudio, builtFactory, builtCasino;                                 //true if the player has built these commercial buildings, used for an achievement
    public static bool syphonedVenus;                                                   //true once the player has syphoned sulfur from venus
    public int craftBuilt = 0;                                                          //the number of craft which have been built

    //variables related to building craft
    public GameObject selectedCraft = null;     //holds the selected craft on the map screen
    public GameObject orbitSpherePrefab;        //prefab of the orbit sphere used to position craft
    public Transform manufactureTarget;         //the location where craft will be instantiated
    public GameObject moveTarget;               //prefab of the location where a craft will move to

    //camera related variables  
    public Camera[] cameras;                    //aray of usable cameras
    public GameObject mapCamera;                //the camera associated with the map screen
    public Vector3 mapCameraDefault;            //the default transform of the map camera

    //variables related to finding the current planet
    public static int currentPlanetIndex;       //current working planet index number
    public static GameObject currentPlanet;     //current working planet game object

    //array of planet names, used to switch between gameObject and index number
    public static string[] planets = { "Solar System", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

    //vaiables for generating circular planet menu
    static GameObject menuItemInstance;
    static GameObject menuItem;
    static GameObject[] menuItems;
    static GameObject[] subMenuItems;
    static GameObject subMenuItemInstance;
    static GameObject subMenuItem;
    static private bool menuOpen;               //true when the planet menu is open
    static private bool subMenuOpen;            //true when the build submenu is open
    static private string nameOfSumMenu;        //contains the name of the submenu open
    public GameObject buttonBuild;
    public GameObject buttonMine;
    public GameObject buttonSyphon;
    public GameObject buttonExplore;
    public GameObject buttonCasino;
    public GameObject buttonStudio;
    public GameObject buttonFactory;
    public GameObject buttonIron, buttonSulfur, buttonHydrogen, buttonOxygen,
        buttonSodium, buttonHelium, buttonCarbon, buttonWater, buttonNitrogen,
        buttonSilicon, buttonAluminum, buttonMagnesium, buttonPotassium, buttonArgon,
        buttonMethane;


    int targetResource;
    public ResourcePanel resourcePanel;
    private Touch touch;


    private string clickMode = "normal";        //normal, explore, mine, syphon, casino, factory, studio

    //other objects will set this true to redraw the menu
    public static bool forcedRedrawMenu = false;                        //set in Explorer and read in this.Update()
    public static Planet forcedRedrawMenuPlanet;                        //the planet which will be redrawn if still open
    public static List<string> exploredPlanets = new List<string>();    //list of explored planets, prevents user from launching multiple explorers to the same planet

    //variables for controlling other planet UI elements
    public Text miniMessageText;
    public GameObject miniMessagePanel;
    public ToolTip toolTip;
    
    //variables for mouse control
    private RaycastHit hit;
    private Ray ray;
    private float rayCastLength = 100;
    GameObject target;

    //variables for keeping player stats
    public static string[] resourceName = { "iron", "sulfur", "money", "hydrogen", "oxygen", "sodium", "helium", "carbon dioxide", "water", "nitrogen", "silicon", "aluminum", "magnesium", "potassium", "argon", "methane" };    //names of the resources by index
    public static int[] playerResources = { 0, 0 , 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };                              //resources in the player's posession
    //public static int[] marketResources = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };                               //resources in the marketplace (no longner implemented in Main)


    //array to hold player goals
    public static int[] goals;          //the goals the player must reach set in SetGoals()
    private bool goalsMet = false;      //true when the player has met his goals

    //speed buttons, used to control color
    public Button[] speedButtons;   

    //prefabs
    public GameObject minerPrefab, explorerPrefab, syphonPrefab, casinoPrefab, factoryPrefab, studioPrefab;
    public GameObject clickMarker;

    //variables used to set text
    public Text ironText, sulfurText, moneyText, hydrogenText;    
    public Text ironGoalText, sulfurGoalText, moneyGoalText, hydrogenGoalText;

    //resource cost counters
    public int minerMoney = 25, minerIronCost = 15, minerHydrogenCost = 10;
    public int explorerMoneyCost = 50, explorerIronCost = 25, explorerHydrogenCost = 20;
    public int syphonerMoneyCost = 25, syphonerIronCost = 15, syphonerHydrogenCost = 40;

    //other classes 
    public PlanetPanel planetPanel;
    public AdvisorPanel advisorPanel;
    public GameObject planetDetails;

    //variables related to probe tech level
    private int techLevel = 1;

    //public Vector2 minerTempRange = new Vector2(-5, -1);

    #endregion

    #region start and update
    void Start()
    {
        resourcePanel = GameObject.FindObjectOfType<ResourcePanel>();

        //initialize exploredPlanets list
        exploredPlanets = new List<string>();

        //load stats from statManager
        stats = FindObjectOfType<PlayerStats>();
        playerName = stats.playerName;
        difficulty = stats.playerDifficulty;
        avatar     = stats.playerAvatar;
        achievements = stats.achievements;

        //assign values from statManager to current scene
        avatarImage.sprite = avatar;
        avatarImageMini.sprite = avatar;
        nameText.text = "Admiral " + playerName;
        nameTextMini.text = playerName;
        difficultyText.text = "Difficulty: " + difficulty;
        badgeIDText.text = "Badge# " + stats.badgeID;

        //set the default selected speed to 2 and change button color
        Button defaultSpeed = GameObject.Find("2").GetComponent<Button>();
        defaultSpeed.image.color = Color.blue;

        //define various game objects
        planetPanel = GameObject.Find("PlanetPanel").GetComponent<PlanetPanel>();      //the planet panel to be updated on planet change
        advisorPanel = GameObject.Find("AdvisorPanel").GetComponent<AdvisorPanel>();   //the planet panel to be updated on planet change

        //randomly align planets in their orbits
        AlignPlanets();

        //turn off all cameras, prevents audio listener errors
        for (int i = 1; i < cameras.Length ; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }
        cameras[0].gameObject.SetActive(false);

        //enable the first camera
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }

        //set the map camera and default location and zoom of the camera
        mapCamera = cameras[0].gameObject;
        mapCameraDefault = cameras[0].transform.position;

        //set the current planet index and gameObject to the system
        currentPlanetIndex = 0;
        currentPlanet = GameObject.Find(planets[currentPlanetIndex]);

        //set various text and values
        SetAchievements();
        SetResources();
        SetResourceText();
        SetGoals();
        SetGoalText();

        //launch quick tutorial
        if (stats.firstPlay)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        //update planet panel to create correct text for solar system
        planetPanel.updatePanel();
    }

    void Update()
    {
        //if another object has asked the menu to be redrwan
        if (forcedRedrawMenu)
        {
            //if the current planet is the one which asked for the redraw
            if (currentPlanet == forcedRedrawMenuPlanet.gameObject)
            {
                RedrawMenu();
                clickMode = "normal";
            }

            //reset the redraw flag
            forcedRedrawMenu = false;
        }

        //catch space key for testing various things throughout development
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //create the win condition
            //goalsMet = true;
            //advisorPanel.SetNewMessage("GoalsMet");
            //advisorPanel.ShowLaunch();
            //StartCoroutine(displayMiniMessage("You have met your objective!"));

            Application.CaptureScreenshot("Screenshot.png");
        }

        //check for win condition
        if (CheckGoals() && !goalsMet)
        {
            goalsMet = true;
            advisorPanel.SetNewMessage("GoalsMet");
            advisorPanel.ShowLaunch();
            StartCoroutine(displayMiniMessage("You have met your objective!"));
        }
        
        //check for any new achievements
        CheckAchievements();

        //get keyboard and mouse input
        GetCameraInput();
        GetMouseInput();
        
        //if the planet hasn't been explored, hide the enhanced details
        if (currentPlanetIndex != 0 && !getCurrentPlanet().explored)
        {
            HideDetails();
        }

        //if on the map screen, disable the planet details and force enable the long description
        if (currentPlanetIndex == 0)
        {
            planetDetails.SetActive(false);
            planetPanel.longDesc.enabled = true;
            planetPanel.explorePlanet.enabled = false;
        }

        //if the planet has been explored, show enhanced details
        if (!planetDetails.active)
            if (currentPlanetIndex != 0 && getCurrentPlanet().explored)
                ShowDetails();

        //update resource text
        SetResourceText();
        
    }

    private void AlignPlanets()
    {
        //this method randomly places planets at a
        //point along their orbit so they aren't
        //aligned when the player starts the game

        //make an array of all of the planet systems
        GameObject[] thesePlanets = GameObject.FindGameObjectsWithTag("System");

        foreach (GameObject p in thesePlanets)
        {
            double r = p.transform.position.x;                              //get the radius of the orbit
            double a = Random.Range(0, 359);                                //randomly generated angle
            
            //simple trig
            double x = (r * System.Math.Cos(a * System.Math.PI / 180));
            float y = 0;
            double z = (r * System.Math.Sin(a * System.Math.PI / 180));

            //position planet at new location
            p.transform.position = new Vector3((float)x, y, (float)z);
        }
    }
    #endregion

    #region camera movement

    private void NextCamera()
    {
        //this method is called on the Right Arrow
        //and selects the next active camera
        #region tutorial
        //used in tutorial
        if (advisorPanel.inTutorial && !advisorPanel.newPlanet)
        {
            advisorPanel.SetNewMessage("TutorialNP");
            advisorPanel.newPlanet = true;
        }
        #endregion

        //reset click mode
        clickMode = "normal";

        //increment array counter
        currentPlanetIndex++;

        //close planet menu, assuming that there was one
        menuOpen = false;
        EraseMenu();
        EraseSubMenu();

        //if not at the end of the array, go to the next index
        if (currentPlanetIndex < cameras.Length)
        {
            //assign current planet
            string s;
            s = planets[currentPlanetIndex ];
            currentPlanet = GameObject.Find(s);
            cameras[currentPlanetIndex - 1].gameObject.SetActive(false);
            cameras[currentPlanetIndex].gameObject.SetActive(true);
        }

        //if at the end of the array, reset the index 
        else
        {
            //set index
            cameras[currentPlanetIndex - 1].gameObject.SetActive(false);
            currentPlanetIndex = 0;
            cameras[currentPlanetIndex].gameObject.SetActive(true);

            //assign current planet
            string s;
            s = planets[currentPlanetIndex];
            currentPlanet = GameObject.Find(s);
        }

        //update planet panel to reflect current planet
        planetPanel.updatePanel();
    }

    private void PrevCamera()
    {
        //this method is called on the Left Arrow
        //and selects the previous active camera
        #region tutorial
        //used in tutorial
        if (advisorPanel.inTutorial && !advisorPanel.newPlanet)
        {
            advisorPanel.SetNewMessage("TutorialNP");
            advisorPanel.newPlanet = true;
        }
        #endregion

        //reset click mode
        clickMode = "normal";

        //decrement counter
        currentPlanetIndex--;

        //close planet menu, assuming there was one
        menuOpen = false;
        EraseMenu();
        EraseSubMenu();

        //if not at the beginning of the array, go the the previous index
        if (currentPlanetIndex >= 0)
        {
            //assign current planet
            string s;
            s = planets[currentPlanetIndex];
            currentPlanet = GameObject.Find(s);

            //set index
            cameras[currentPlanetIndex + 1].gameObject.SetActive(false);
            cameras[currentPlanetIndex].gameObject.SetActive(true);
        }


        //if at the beginning of the array, set index to array length
        else
        {
            //set index
            cameras[currentPlanetIndex + 1].gameObject.SetActive(false);
            currentPlanetIndex = 8;
            cameras[currentPlanetIndex].gameObject.SetActive(true);

            //assign current planet
            string s;
            s = planets[currentPlanetIndex];
            currentPlanet = GameObject.Find(s);

        }

        //update planet panel to reflect current planet
        planetPanel.updatePanel();
    }

    private void MainCamera()
    {
        //this method is called on the Down Arrow
        //and selects the first active camera

        //reset click mode
        clickMode = "normal";

        //erase planet menu, assuming one existed
        EraseMenu();
        EraseSubMenu();

        //set index to 0 and move the camera
        cameras[currentPlanetIndex].gameObject.SetActive(false);
        currentPlanetIndex = 0;
        cameras[currentPlanetIndex].gameObject.SetActive(true);

        //assign current planet
        string s;
        s = planets[currentPlanetIndex];
        currentPlanet = GameObject.Find(s);

        //update planet panel to reflect current planet
        planetPanel.updatePanel();
    }

    public void JumpToCamera(int currentNxd, int ndx)
    {
        //this method is called when the player clicks on a planet
        //button in the nav panel and jumps to another camera

        #region tutorial
        //used in tutorial
        if (advisorPanel.inTutorial && !advisorPanel.newPlanet)
        {
            advisorPanel.SetNewMessage("TutorialNP");
            advisorPanel.newPlanet = true;
        }
        #endregion

        //reset click mode
        clickMode = "normal";

        //close planet menu, assuming one existed
        EraseMenu();
        EraseSubMenu();

        //set index to that of the planet button clicked and move camera
        cameras[currentNxd].gameObject.SetActive(false);
        currentPlanetIndex = ndx;
        cameras[currentPlanetIndex].gameObject.SetActive(true);

        //assign current planet
        string s;
        s = planets[currentPlanetIndex];
        currentPlanet = GameObject.Find(s);

        //update planet panel to reflect current planet
        planetPanel.updatePanel();
    }

    private void RotateCamera()
    {
        //this method is called on the Bracket Keys
        //rotates the camera around the current planet
        //Note: this currently misaligns the planet menu buttons

        //if not in the map screen
        if (currentPlanetIndex != 0)
        {
            //rotate cw on left bracket
            if (Input.GetKey(KeyCode.LeftBracket))
                Camera.main.transform.RotateAround(currentPlanet.transform.position, Vector3.up, Time.deltaTime * 100f);

            //rotate ccw on right bracket
            if (Input.GetKey(KeyCode.RightBracket))
                Camera.main.transform.RotateAround(currentPlanet.transform.position, Vector3.up, Time.deltaTime * -100f);
        }
    }

    private void MapCamera()
    {
        //this method is called in Update() and used to move the
        //camera around the map screen and change the diameter of
        //Orbit Sphere when on a planet

        //if on the map screen, move the camera accordingly
        if (currentPlanetIndex == 0)
        {

            if (!stats.useTouchInput)
            {
            if (Input.GetKey(KeyCode.A) && mapCamera.transform.position.x > -20)
                mapCamera.transform.position = new Vector3(mapCamera.transform.position.x - .1f, mapCamera.transform.position.y, mapCamera.transform.position.z);

            if (Input.GetKey(KeyCode.D) && mapCamera.transform.position.x < 38)
                mapCamera.transform.position = new Vector3(mapCamera.transform.position.x + .1f, mapCamera.transform.position.y, mapCamera.transform.position.z);

            if (Input.GetKey(KeyCode.W) && mapCamera.transform.position.y < 23)
                mapCamera.transform.position = new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y, mapCamera.transform.position.z + .1f);

            if (Input.GetKey(KeyCode.S) && mapCamera.transform.position.x > -40)
                mapCamera.transform.position = new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y, mapCamera.transform.position.z - .1f);

            if (Input.GetAxis("Mouse ScrollWheel") != 0 && currentPlanetIndex == 0)
                    if ((Input.GetAxis("Mouse ScrollWheel") < 0 && mapCamera.transform.position.y < 42) || (Input.GetAxis("Mouse ScrollWheel") > 0 && mapCamera.transform.position.y > 3))
                        mapCamera.transform.position = new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y - (Input.GetAxis("Mouse ScrollWheel") * 10), mapCamera.transform.position.z + (Input.GetAxis("Mouse ScrollWheel") * 10));
            }



            else
            {
                touch = Input.touches[0];

                //touch scrolling
                if (touch.phase == TouchPhase.Moved && Input.touchCount == 1)
                {
                    if (((touch.deltaPosition.x > 0 && mapCamera.transform.position.x > -20) || (touch.deltaPosition.x < 0 && mapCamera.transform.position.x < 38)) && ((touch.deltaPosition.y > 0 && mapCamera.transform.position.z > -40) || (touch.deltaPosition.y < 0 && mapCamera.transform.position.z < 23)))
                    { 
                        Vector3 newPos =  new Vector3(mapCamera.transform.position.x - touch.deltaPosition.x, mapCamera.transform.position.y, mapCamera.transform.position.z - touch.deltaPosition.y);
                        mapCamera.transform.position = Vector3.Lerp(mapCamera.transform.position, newPos, 0.5f);
                    }
                }

                //pinch zooming
                if (Input.touchCount == 2)
                {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // ... change the canvas size based on the change in distance between the touches.
                    //canvas.scaleFactor -= deltaMagnitudeDiff * zoomSpeed;
                    if ((deltaMagnitudeDiff > 0 && mapCamera.transform.position.y < 42) || (deltaMagnitudeDiff < 0 && mapCamera.transform.position.y > 3))
                    {
                        Vector3 newPos = new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y + (deltaMagnitudeDiff), mapCamera.transform.position.z);
                        mapCamera.transform.position = Vector3.Lerp(mapCamera.transform.position, newPos, 0.5f);
                    }

                    // Make sure the canvas size never drops below 0.1
                    //canvas.scaleFactor = Mathf.Max(canvas.scaleFactor, 0.1f);
                }
            }
        }

        //when not on the map screen, check for mouse wheel input and adjust any existing Orbit Sphere
        else if (currentPlanetIndex != 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0 && (clickMode == "explore" || clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
            {
                if (GameObject.Find("OrbitSphere"))
                {
                    GameObject sphere = GameObject.Find("OrbitSphere");
                    float newScale = Mathf.Clamp((sphere.transform.localScale.x + (Input.GetAxis("Mouse ScrollWheel") * .1f)), 1.1f, 1.5f);
                    sphere.transform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
        }
    }

    private void ShowDetails()
    {
        //this method displays the planet details panel

        planetDetails.SetActive(true);
        planetPanel.longDesc.enabled = true;
        planetPanel.explorePlanet.enabled = false;
    }

    private void HideDetails()
    {
        //this method hides the planet details panel

        planetDetails.SetActive(false);
        planetPanel.longDesc.enabled = false;
        planetPanel.explorePlanet.enabled = true;
    }
    #endregion
    
    #region Planet Menu System
    public void EraseMenu()
    {
        //this method removes the planet menu from the screen

        //get menu items
        menuItems = GameObject.FindGameObjectsWithTag("MenuItems");

        //destroy menu items
        foreach (GameObject item in menuItems)
            Destroy(item);

        //reset flag
        menuOpen = false;
    }

    public void EraseSubMenu()
    {
        //this method removes the build submenu from the screen

        //get menu items
        subMenuItems = GameObject.FindGameObjectsWithTag("SubMenuItems");

        //destroy menu items
        foreach (GameObject item in subMenuItems)
            Destroy(item);
    }

    public void DrawMenu()
    {
        //this method is called when a planet is clicked in normal
        //mode and draws a circular menu originating from the planet

        //remove the menu if the player clicks on the planet and the menu is still open
        if (Input.GetMouseButtonDown(0) && menuOpen)
        {
            EraseMenu();
            EraseSubMenu();
            return;
        }

        //display the planet menu when the planet is clicked
        if (Input.GetMouseButtonDown(0) && !menuOpen)
        {
            {
                //clear possible menu from array
                if (menuItems != null)
                    menuItems = null;
            }

        //get number of items to display
        List<string> availableActions = currentPlanet.GetComponent<Planet>().availableActions;
        int numItems = availableActions.Count;

        //get items to display
        menuItems = new GameObject[numItems];

        int j = 0;
        foreach(string s in availableActions)
        {
            menuItems[j] = getButton(s);
            j++;
        }

        //find radius of planet by scale
        double r =  currentPlanet.transform.localScale.z * .8;

            for (int i = 1; i <= menuItems.Length; i++)
            {
                //find angle
                double a = 360 / ((double)numItems / (double)i);

                //translate angle to location
                double x = currentPlanet.transform.position.x + (r * System.Math.Cos(a * System.Math.PI / 180));
                double y = currentPlanet.transform.position.y + (r * System.Math.Sin(a * System.Math.PI / 180));

                //create new vector for menu item position
                Vector3 newPos = new Vector3((float)x, (float)y, currentPlanet.transform.position.z);
                menuItemInstance = menuItems[i - 1];

                //spawn item
                menuItem = GameObject.Instantiate(menuItemInstance, newPos, Quaternion.identity) as GameObject;
                menuItem.transform.position = newPos;
                menuItem.transform.SetParent(Camera.main.transform);
                menuItem.transform.localScale = currentPlanet.transform.localScale;
            }

            //set flag to let the system know the menu is open
            menuOpen = true;
        }
    }

    public void RedrawMenu()
    {
        //this method is called when new menu options are
        //available and simply erases and draws a new menu

        EraseMenu();
        DrawMenu();
    }

    private void DrawSubMenu(Transform target, string btn)
    {
        //this method is called when the build button is clicked and creates
        //a circular menu originating from the center of the button


        //close the submenu and return if it is already open
        if (subMenuOpen)
        {
            EraseSubMenu();
            subMenuOpen = false;
            //return;
        }

        if (nameOfSumMenu == btn)
        {
            nameOfSumMenu = null;
            return;
        }

        //set submenu flag
        subMenuOpen = true;

        //clear possible submenu from array
        if (subMenuItems != null)
            subMenuItems = null;

        //get number of submenu items (can use a list for dynamic menus)
        int numSubItems = 0;

        //assign buttons to submenu items
        string[] availableSubItems = null;

        nameOfSumMenu = btn;

        if (btn == "build")
        {
            numSubItems = 3;
            availableSubItems = new string[numSubItems];
            availableSubItems[0] = "Casino";
            availableSubItems[1] = "Studio";
            availableSubItems[2] = "Factory";
        }

        if (btn == "mine")
        {
            numSubItems = currentPlanet.GetComponent<Planet>().numberOfResources;

            if (numSubItems >= 4)
                numSubItems = 4;

            Debug.Log("Draw Menu Called " + numSubItems);

            availableSubItems = new string[numSubItems];


            for (int i = 0; i < numSubItems; i++)
            {
                Debug.Log(i + " element: " + resourceName[currentPlanet.GetComponent<Planet>().topFourResources[i]]);
                availableSubItems[i] = resourceName[currentPlanet.GetComponent<Planet>().topFourResources[i]];
            }
        }

        //get items to display
        subMenuItems = new GameObject[numSubItems];

        int j = 0;
        foreach (string s in availableSubItems)
        {
            subMenuItems[j] = getButton(s);
            j++;
        }

        //find radius of planet by scale
        double r = currentPlanet.transform.localScale.z * .25;

        for (int i = 1; i <= subMenuItems.Length; i++)
        {
            //find angle
            double a = (360 / ((double)numSubItems / (double)i)) +90f;

            //translate angle to location
            double x = target.position.x + (r * System.Math.Cos(a * System.Math.PI / 180));
            double y = target.position.y + (r * System.Math.Sin(a * System.Math.PI / 180));

            //create new vector for menu item position
            Vector3 newPos = new Vector3((float)x, (float)y, target.position.z);

            subMenuItemInstance = subMenuItems[i - 1];

            //spawn item
            subMenuItem = GameObject.Instantiate(subMenuItemInstance, newPos, Quaternion.identity) as GameObject;
            subMenuItem.transform.position = newPos;
            subMenuItem.transform.SetParent(Camera.main.transform);
            subMenuItem.transform.localScale = currentPlanet.transform.localScale * .5f;
        }
    }

    public GameObject getButton(string s)
    {
        //this menu is called in DrawMenu() and DrawSubMenu()
        //and returns a button gameObject based on the name

        switch (s)
        {
            case "Explore":
                return buttonExplore;
            case "Mine":
                return buttonMine;
            case "Syphon":
                return buttonSyphon;
            case "Build":
                return buttonBuild;

            case "Casino":
                return buttonCasino;
            case "Factory":
                return buttonFactory;
            case "Studio":
                return buttonStudio;

            case "hydrogen":
                return buttonHydrogen;
            case "iron":
                return buttonIron;
            case "sulfur":
                return buttonSulfur;
            case "oxygen":
                return buttonOxygen;
            case "sodium":
                return buttonSodium;
            case "helium":
                return buttonHelium;
            case "carbon dioxide":
                return buttonCarbon;
            case "water":
                return buttonWater;
            case "nitrogen":
                return buttonNitrogen;
            case "silicon":
                return buttonSilicon;
            case "aluminum":
                return buttonAluminum;
            case "magnesium":
                return buttonMagnesium;
            case "potassium":
                return buttonPotassium;
            case "argon":
                return buttonArgon;
            case "methane":
                return buttonMethane;

        }

        return null;
    }

    public void sendMiniMessage(string s)
    {
        StartCoroutine(displayMiniMessage(s));
    }

    public IEnumerator displayMiniMessage(string msg)
    {
        //thie method gets display text from ReadyToMine()
        //and displays the panel for 3 seconds

        //if the panel isn't already open or if there is a new message to display
        if (!miniMessagePanel.activeSelf || (miniMessageText.text != msg))
        {
            miniMessageText.text = msg;
            miniMessagePanel.SetActive(true);
            yield return new WaitForSeconds(2);
            miniMessagePanel.SetActive(false);
        }
    }
    #endregion

    #region input
    public IEnumerator DisplayToolTip()
    {
        //tihs coroutine waits for a false condition for 2 seconds
        //then displays the tooltip if it doesn't recieve one

        yield return new WaitForSeconds(2);

        if (toolTip.getStatus())
        {
            toolTip.setFollow(true);
        }
    }

    private void GetMouseInput()
    {
        //toggles the use of touch input for clicking on game objects
        if (stats.useTouchInput)
        {
            touch = Input.GetTouch(0);
            ray = Camera.main.ScreenPointToRay(touch.position);
        }
        else
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        //this method creates a ray from the camera to the object behind the mouse,
        //then performs a function based on hover and click conditions

        //if the raycast doesn't hit anything
        if (!Physics.Raycast(ray, out hit, rayCastLength))
        {
            //move the Click Marker to its default location
            clickMarker.transform.position = new Vector3(-1000, -1000, -1000);

            //cancel any previous tooltip
            StopCoroutine("DisplayToolTip");

            //move tooltip to its default location
            toolTip.setFollow(false);

            //erase the planet menu if the void is clicked on
            if (Input.GetMouseButtonDown(0))
            {
                EraseMenu();
                EraseSubMenu();
            }
        }

        //TOOL TIPS
        if (Physics.Raycast(ray, out hit, rayCastLength) && currentPlanetIndex != 0 )
        {
            target = hit.collider.gameObject;

            //if the cursor is over the sun or an orbit sphere
            if (target == getCurrentPlanet().gameObject || target.name == "Sun" || target.name == "OrbitSphere")
            {
                //cancel any existing tooltip and move the tooltip box to its default location
                StopCoroutine("DisplayToolTip");
                toolTip.setFollow(false);
            }

                
           //mine button
            if (target.name.Contains("ButtonMine"))
            {   
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Mining\nRobot");
                toolTip.costPanel.SetActive(true);
                toolTip.setCosts(minerMoney, Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, getCurrentPlanet().transform.position)), minerIronCost);
                toolTip.setStatus(true);
            }

            //syphon button
            if (target.name.Contains("ButtonSyphon"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.costPanel.SetActive(true);
                toolTip.setCosts(syphonerMoneyCost, Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, getCurrentPlanet().transform.position)), syphonerIronCost);
                toolTip.setStatus(true);

                //if the current planet is Venus, use Venus text
                if (currentPlanet.name == "Venus")
                    toolTip.SendMessage("setMessage", "Syphoning\nRobot ");

                //if the current planet isn't Venus, use the default text
                else
                    toolTip.SendMessage("setMessage", "Syphoning\nRobot");
            }

            //explore button
            if (target.name.Contains("ButtonExplore"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Exploration\nSatellite");
                toolTip.costPanel.SetActive(true);
                toolTip.setCosts(explorerMoneyCost, Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, getCurrentPlanet().transform.position)), explorerIronCost);
                toolTip.setStatus(true);
            }

            //build button
            if (target.name.Contains("ButtonBuild"))
            {
                //start the button tooltip, excluding the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Building\nOptions");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //resort button
            if (target.name.Contains("ButtonCasino"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Resort");
                toolTip.costPanel.SetActive(true);
                toolTip.setCosts(casinoPrefab.GetComponent<Casino>().cost[2], Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, getCurrentPlanet().transform.position) *5), casinoPrefab.GetComponent<Casino>().cost[0]);
                toolTip.setStatus(true);
            }

            //studio button
            if (target.name.Contains("ButtonStudio"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Movie\nStudio");
                toolTip.costPanel.SetActive(true);
                toolTip.setCosts(studioPrefab.GetComponent<Craft>().cost[2], Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, getCurrentPlanet().transform.position) * 5), studioPrefab.GetComponent<Craft>().cost[0]);
                toolTip.setStatus(true);
            }

            //factory button
            if (target.name.Contains("ButtonFactory"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Factory");
                toolTip.costPanel.SetActive(true);
                toolTip.setCosts(factoryPrefab.GetComponent<Craft>().cost[2], Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, getCurrentPlanet().transform.position) * 5), factoryPrefab.GetComponent<Craft>().cost[0]);
                toolTip.setStatus(true);
            }

            //ELEMENTS START HERE

            //iron button
            if (target.name.Contains("ButtonIron"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Iron");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //sulfur button
            if (target.name.Contains("ButtonSulfur"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Sulfur");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //hydrogen button
            if (target.name.Contains("ButtonHydrogen"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Hydrogen");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //oxygen button
            if (target.name.Contains("ButtonOxygen"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Oxygen");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //sodium button
            if (target.name.Contains("ButtonSodium"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Sodium");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //helium button
            if (target.name.Contains("ButtonHelium"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Helium");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //carbon dioxide button
            if (target.name.Contains("ButtonCarbonDioxide"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Carbon\nDioxide");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //water button
            if (target.name.Contains("ButtonWater"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Water");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //nitrogen button
            if (target.name.Contains("ButtonNitrogen"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Nitrogen");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //silicon button
            if (target.name.Contains("ButtonSilicon"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Silicon");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //aluminum button
            if (target.name.Contains("ButtonAluminum"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Aluminum");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //magnesium button
            if (target.name.Contains("ButtonMagnesium"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Magnesium");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //potassium button
            if (target.name.Contains("ButtonPotassium"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Potassium");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //argon button
            if (target.name.Contains("ButtonArgon"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Argon");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

            //methane button
            if (target.name.Contains("ButtonMethane"))
            {
                //start the button tooltip, including the cost panel
                StartCoroutine("DisplayToolTip");
                toolTip.SendMessage("setMessage", "Methane");
                toolTip.costPanel.SetActive(false);
                toolTip.setStatus(true);
            }

        }

        //if in any building mode and pointing at object
        if (Physics.Raycast(ray, out hit, rayCastLength) && currentPlanetIndex != 0 && (clickMode == "mine" || clickMode == "syphon" || clickMode == "explore" || clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
        {
            target = hit.collider.gameObject;

            //if planet and in mine or syphon mode
            if (target == currentPlanet && (clickMode == "mine" || clickMode == "syphon"))
            {
                //move click marker to location
                clickMarker.transform.position = hit.point;
            }

            //if orbit sphere and in any other build mode
            else if (target.name == "OrbitSphere" && (clickMode == "explore" || clickMode == "casino" || clickMode == "factory" || clickMode == "studio") )
            {
                //move click marker to location
                clickMarker.transform.position = hit.point;
            }

            //if pointing at any other object, move click marker to default location
            else
                clickMarker.transform.position = new Vector3(-1000, -1000, -1000);
        }

        //if the player is on the map screen
        if (Physics.Raycast(ray, out hit, rayCastLength) && currentPlanetIndex == 0 && Input.GetMouseButtonDown(0))
        {
            // if planet is clicked, jump to that planet
            if (hit.collider.gameObject.tag == "Planet")
            {
                int ndx = System.Array.IndexOf(planets, hit.collider.gameObject.name);
                JumpToCamera(currentPlanetIndex, ndx);
                return;
            }

            //if craft is clicked, make that gameObject the selected craft
            else if (hit.collider.gameObject.tag == "Craft")
            {
                selectedCraft = hit.collider.gameObject;
                return;
            }
        }

        //if the player has selected a craft, but hasn't clicked the plane yet
        if (Physics.Raycast(ray, out hit, rayCastLength) && currentPlanetIndex == 0 && selectedCraft)
        {
            //if pointing at plane
            if (hit.collider.gameObject.name == "Plane")
            {
                //make destination the point under the mouse
                Vector3 newDest = hit.point;
                newDest = new Vector3(newDest.x, 0, newDest.z);

                //calculate distance between current selected craft position and destination
                float dist = Mathf.Abs(Vector3.Distance(selectedCraft.transform.position, newDest));
                int movementCost = Mathf.FloorToInt(dist) * 5;

                //ADD CODE HERE TO DISPLAY DISTANCE AND COST
            }
        }

        //if the player has selected a craft and right clicks on the plane
        if (Physics.Raycast(ray, out hit, rayCastLength) && currentPlanetIndex == 0 && Input.GetMouseButtonDown(1) && selectedCraft)
        {
            if (hit.collider.gameObject.name == "Plane")
            {
                //find new destination point
                Vector3 newDest = hit.point;
                newDest = new Vector3(newDest.x, 0, newDest.z);

                //get the distance from the ship to the target and calculate movement cost
                float dist = Mathf.Abs(Vector3.Distance(selectedCraft.transform.position, newDest));
                int movementCost = Mathf.FloorToInt(dist) * 5;

                //check if the player has enough fuel
                if (movementCost > playerResources[3])
                {
                    StartCoroutine(displayMiniMessage("You don't have enough hydrogen to move your mothership."));
                    return;
                }

                //deduct cost to move ship
                playerResources[3] -= movementCost;

                //move the selected craft
                selectedCraft.GetComponent<Craft>().target = Instantiate(moveTarget, newDest, Quaternion.identity) as GameObject;
                selectedCraft.GetComponent<Craft>().reachedTarget = false;
            }
        }

        //if a planet or orbit sphere is clicked
        if (Physics.Raycast(ray, out hit, rayCastLength) && currentPlanetIndex != 0 && Input.GetMouseButtonDown(0))
        {
            target = hit.collider.gameObject;

            //if the player clicks an orbit sphere
            if (target.name == "OrbitSphere")
            {
                //after explore button
                if (clickMode == "explore")
                {
                    //launch an explorer
                    BuildExplorer(hit.point);

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        Debug.Log("ORBIT SPHERE");
                    }

                    //erase the menu to prevent player from launching a second explorer
                    EraseMenu();
                }

                //after casino button
                else if (clickMode == "casino")
                {
                    //launch a resort
                    BuildCasino(hit.point);

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                    }

                }

                //after factory button
                else if (clickMode == "factory")
                {
                    //build a factory
                    BuildFactory(hit.point);

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                    }
                }

                //after studio button
                else if (clickMode == "studio")
                {
                    //launch a studio
                    BuildStudio(hit.point);

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                    }
                }
            }


            //if the current planet is clicked
            if (target == currentPlanet)
            {
                //if no previous button clicked
                if (clickMode == "normal")
                {
                    DrawMenu();
                    return;
                }

                //after mine button
                if (clickMode == "mine")
                {
                    #region tutorial
                    //used in tutorial
                    if (advisorPanel.inTutorial)
                    {
                        //if the mining tutorial isn't complete
                        if(!advisorPanel.foundRock)
                        {
                            advisorPanel.SetNewMessage("TutorialMINE2");
                            advisorPanel.foundRock = true;

                            //if the syphon tutorial hasn't been completed
                            if(!advisorPanel.foundGas)
                            {
                                advisorPanel.SetNewMessage("TutorialGAS4");
                                advisorPanel.lookingForGas = true;
                            }

                            else if (advisorPanel.foundGas && advisorPanel.tutRock)
                            {
                                advisorPanel.inTutorial = false;
                                advisorPanel.SetNewMessage("TutorialCASH");
                                advisorPanel.SetNewMessage("TutorialEND");
                            }
                        }
                    }
                    #endregion

                    //launch miner
                    buildMiner(hit.point);
                }

                //after syphon button
                if (clickMode == "syphon")
                {
                    //launch syphone
                    BuildSyphoner(hit.point);
                }
            }

            //if mine button is clicked
            if (target.name.Contains("ButtonMine"))
            {
                {

                    
                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        Debug.Log("ORBIT SPHERE");
                        clickMode = "normal";
                    }

                    //draw the submenu
                    DrawSubMenu(target.transform, "mine");
                    return;
                }

                //used in tutorial
                if (advisorPanel.inTutorial)
                {
                    //if the rock planet tutorial hasn't been completed
                    if (!advisorPanel.clickedMine)
                    {
                        advisorPanel.SetNewMessage("TutorialMINE");
                        advisorPanel.clickedMine = true;
                    }
                }



                clickMode = "normal";
                DrawSubMenu(target.transform, "mine");
                return;

                /*
                if (readyToMine())
                {
                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
                */
            }

            //if explore button is clicked
            if (target.name.Contains("ButtonExplore"))
            {
                //if the current planet is eligable to be explored
                if (!getCurrentPlanet().explored && !exploredPlanets.Contains(getCurrentPlanet().name))
                {
                    #region tutorial
                    //used in tutorial
                    if (advisorPanel.inTutorial)
                    {
                        //if looking for a rock planet and rock planet is clicked
                        if (!advisorPanel.tutRock && getCurrentPlanet().type == "Rock" && !advisorPanel.lookingForGas)
                        {
                            if (getCurrentPlanet().name == "Venus")
                                advisorPanel.SetNewMessage("TutorialVENUS2");

                            else
                            {
                                advisorPanel.SetNewMessage("TutorialROCK");
                                advisorPanel.tutRock = true;
                            }
                        }

                        //if looking for a rock planet and a gas planet is clicked
                        if (getCurrentPlanet().type == "Gas" && !advisorPanel.lookingForGas)
                        {
                            if (!advisorPanel.tutRock)
                            {
                                advisorPanel.SetNewMessage("TutorialGAS1");
                                advisorPanel.lookingForGas = true;
                            }

                            else
                                advisorPanel.SetNewMessage("TutorialGAS5");
                        }

                        //if looking for a gas planet and a gas planet is clicked
                        if (getCurrentPlanet().type == "Gas" && advisorPanel.lookingForGas)
                        {
                            advisorPanel.SetNewMessage("TutorialGAS2");
                            //advisorPanel.tutGas = true;
                        }

                        //if looking for a gas planet and a rock planet is clicked
                        if (getCurrentPlanet().type == "Rock" && advisorPanel.lookingForGas)
                        {
                            if (getCurrentPlanet().name == "Venus")
                                advisorPanel.SetNewMessage("TutorialVENUS");

                            else
                                advisorPanel.SetNewMessage("TutorialROCK2");
                        }
                    }
                    #endregion

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        Debug.Log("ORBIT SPHERE");
                        clickMode = "normal";
                    }

                    //create a new orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.1f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                        clickMode = "explore";
                    }

                    //if the Explored All Planets achievement hasn't been obtained, check for it
                    if (!achievements["ExploredAllPlanets"] == true)
                    {
                        GameObject[] thesePlanets = GameObject.FindGameObjectsWithTag("Planet");

                        bool allExplored = true;

                        foreach (GameObject p in thesePlanets)
                        {
                            if (!p.GetComponent<Planet>().explored)
                                allExplored = false;
                        }

                        //if there are no planets which haven't been explored, award the achievement
                        if (allExplored)
                        {
                            Debug.Log("explored all planets");
                            AddAchievement("Achievement Earned:\n EXPLORED ALL PLANETS", "ExploredAllPlanets");
                        }
                    }
                        return;
                }


                //if the current planet isn't eligable to be explored
                else 
                    StartCoroutine(displayMiniMessage("This planet has already been explored."));
               }

            //if syphon button is clicked
            if (target.name.Contains("ButtonSyphon"))
            {
                EraseSubMenu();

                //remove orbit sphere is there was one
                if (GameObject.Find("OrbitSphere"))
                {
                    Destroy(GameObject.Find("OrbitSphere"));
                    Debug.Log("ORBIT SPHERE");
                }

                //create a new orbit sphere if there wasn't
                else
                {
                    GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                    sphere.GetComponent<SphereCollider>().enabled = false;
                    float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                    sphere.transform.localScale = new Vector3(scale, scale, scale);
                    sphere.transform.SetParent(getCurrentPlanet().transform);
                    sphere.name = "OrbitSphere";
                }

                #region tutorial
                //used in tutorial
                if (advisorPanel.inTutorial)
                {
                    //if the gas tutorial hasn't been completed
                    if (advisorPanel.lookingForGas && !advisorPanel.foundGas)
                    {
                        advisorPanel.SetNewMessage("TutorialGAS3");
                        advisorPanel.foundGas = true;
                        
                        //if the rock tutorial hasn't been completed
                        if(!advisorPanel.foundRock)
                        {
                            advisorPanel.lookingForGas = false;
                            advisorPanel.SetNewMessage("TutorialROCK3");
                        }

                        else if (advisorPanel.foundGas && advisorPanel.tutRock)
                        {
                            advisorPanel.inTutorial = false;
                            advisorPanel.SetNewMessage("TutorialCASH");
                            advisorPanel.SetNewMessage("TutorialEND");
                        }
                    }
                }
                #endregion

                if (readyToSyphon())
                {
                    clickMode = "syphon";
                    return;
                }
            }

            //if build button is clicked
            if (target.name.Contains("ButtonBuild"))
            {
                //remove orbit sphere is there was one
                if (GameObject.Find("OrbitSphere"))
                {
                    Destroy(GameObject.Find("OrbitSphere"));
                    Debug.Log("ORBIT SPHERE");
                    clickMode = "normal";
                }

                //draw the submenu
                DrawSubMenu(target.transform, "build");
                return;
            }

            //if casino button is clicked
            if (target.name.Contains("ButtonCasino"))
            {
                //remove orbit sphere is there was one and changing mode
                if (GameObject.Find("OrbitSphere") && (clickMode == "syphon" || clickMode == "mine"))
                {
                    Destroy(GameObject.Find("OrbitSphere"));
                    clickMode = "normal";
                }

                //change click mode if already in build mode
                else if (GameObject.Find("OrbitSphere"))
                {
                    clickMode = "casino";
                }

                //create a new orbit sphere if there wasn't one
                else
                {
                    GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                    float scale = getCurrentPlanet().transform.localScale.x * 1.1f;
                    sphere.transform.localScale = new Vector3(scale, scale, scale);
                    sphere.transform.SetParent(getCurrentPlanet().transform);
                    sphere.name = "OrbitSphere";
                    clickMode = "casino";
                }
            }

            //if iron button is clicked
            if (target.name.Contains("ButtonIron"))
            {
                if (readyToMine())
                {
                    targetResource = 0;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "mine";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
            }

            //if sulfur button is clicked
            if (target.name.Contains("ButtonSulfur"))
            {
                if (readyToMine())
                {
                    targetResource = 1;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

            //if hydrogen button is clicked
            if (target.name.Contains("ButtonHydrogen"))
            {
                if (readyToMine())
                {
                    targetResource = 3;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

            //if oxygen button is clicked
            if (target.name.Contains("ButtonOxygen"))
            {
                if (readyToMine())
                {
                    targetResource = 4;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

            //if sodium button is clicked
            if (target.name.Contains("ButtonSodium"))
            {
                if (readyToMine())
                {
                    targetResource = 5;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "mine";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
            }

            //if helium button is clicked
            if (target.name.Contains("ButtonHelium"))
            {
                if (readyToMine())
                {
                    targetResource = 6;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

             //if carbon dioxide button is clicked
            if (target.name.Contains("ButtonCarbon"))
            {
                if (readyToMine())
                {
                    targetResource = 7;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

            //if water button is clicked
            if (target.name.Contains("ButtonWater"))
            {
                if (readyToMine())
                {
                    targetResource = 4;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "mine";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
            }

            //if nitrogen button is clicked
            if (target.name.Contains("ButtonNitrogen"))
            {
                if (readyToMine())
                {
                    targetResource = 9;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

            //if silicon button is clicked
            if (target.name.Contains("ButtonSilicon"))
            {
                if (readyToMine())
                {
                    targetResource = 10;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "mine";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
            }

            //if aluminum button is clicked
            if (target.name.Contains("ButtonAluminum"))
            {
                if (readyToMine())
                {
                    targetResource = 11;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "mine";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
            }

            //if magnesium button is clicked
            if (target.name.Contains("ButtonMagnesium"))
            {
                if (readyToMine())
                {
                    targetResource = 12;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "mine";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
            }

            //if potassium button is clicked
            if (target.name.Contains("ButtonPotassium"))
            {
                if (readyToMine())
                {
                    targetResource = 13;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "mine";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "mine";
                    return;
                }
            }

            //if argon button is clicked
            if (target.name.Contains("ButtonArgon"))
            {
                if (readyToMine())
                {
                    targetResource = 14;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

             //if methane button is clicked
            if (target.name.Contains("ButtonMethane"))
            {
                if (readyToMine())
                {
                    targetResource = 15;

                    //remove orbit sphere is there was one
                    if (GameObject.Find("OrbitSphere") && (clickMode == "casino" || clickMode == "factory" || clickMode == "studio"))
                    {
                        Destroy(GameObject.Find("OrbitSphere"));
                        clickMode = "normal";
                    }

                    //change click mode if already in build mode
                    else if (GameObject.Find("OrbitSphere"))
                    {
                        clickMode = "syphon";
                    }

                    //create orbit sphere if there wasn't
                    else
                    {
                        GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                        sphere.GetComponent<SphereCollider>().enabled = false;
                        float scale = getCurrentPlanet().transform.localScale.x * 1.01f;
                        sphere.transform.localScale = new Vector3(scale, scale, scale);
                        sphere.transform.SetParent(getCurrentPlanet().transform);
                        sphere.name = "OrbitSphere";
                    }

                    //change click mode
                    clickMode = "syphon";
                    return;
                }
            }

            //if factory button is clicked
            if (target.name.Contains("ButtonFactory"))
            {
                //remove orbit sphere is there was one, and changing mode
                if (GameObject.Find("OrbitSphere") && (clickMode == "syphon" || clickMode == "mine"))
                {
                    Destroy(GameObject.Find("OrbitSphere"));
                    clickMode = "normal";
                }

                //change click mode if already in build mode
                else if (GameObject.Find("OrbitSphere"))
                {
                    clickMode = "factory";
                }

                //create a new orbit sphere if there wasn't one
                else
                {
                    GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                    float scale = getCurrentPlanet().transform.localScale.x * 1.1f;
                    sphere.transform.localScale = new Vector3(scale, scale, scale);
                    sphere.transform.SetParent(getCurrentPlanet().transform);
                    sphere.name = "OrbitSphere";
                    clickMode = "factory";
                }
            }

            //if studio button is clicked
            if (target.name.Contains("ButtonStudio"))
            {
                //remove orbit sphere is there was one, and changing mode
                if (GameObject.Find("OrbitSphere") && (clickMode == "syphon" || clickMode == "mine"))
                {
                    Destroy(GameObject.Find("OrbitSphere"));
                    clickMode = "normal";
                }

                //change click mode if already in build mode
                else if (GameObject.Find("OrbitSphere"))
                {
                    clickMode = "studio";
                }

                //create a new orbit sphere if there wasn't one
                else
                {
                    GameObject sphere = Instantiate(orbitSpherePrefab, getCurrentPlanet().transform.position, Quaternion.identity) as GameObject;
                    float scale = getCurrentPlanet().transform.localScale.x * 1.1f;
                    sphere.transform.localScale = new Vector3(scale, scale, scale);
                    sphere.transform.SetParent(getCurrentPlanet().transform);
                    sphere.name = "OrbitSphere";
                    clickMode = "studio";
                }
            }
        }
    }

    private void GetCameraInput()
    {
        //this method is called in Update()
        //and controls the camera input

        //get camera controller (keyboard) input
        if (Input.GetKeyDown(KeyCode.RightArrow))
            NextCamera();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            PrevCamera();
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //if not on the map screen, return to the map screen
            if (currentPlanetIndex != 0)
                MainCamera();

            //if on the map screen, reset the map camera to its default position
            else if (currentPlanetIndex == 0)
                mapCamera.transform.position = mapCameraDefault;
        }
        if (Input.GetKey(KeyCode.RightBracket) || Input.GetKey(KeyCode.LeftBracket))
            RotateCamera();

        //if wasd input or scroll wheel, call MapCamera()
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetAxis("Mouse ScrollWheel") != 0f || Input.touchCount > 0)
            MapCamera();
    }

    public void GetSpeed(Button button)
    {
        //this method is called when a speed button is
        //clicked and changes the Time.timeScale of the game

        //change the global time scale to the number of the button / 4
        int scale = int.Parse(button.name);
        Time.timeScale = ((float)scale / 4f);

        //reset each button back to blue
        foreach (Button b in speedButtons)
            b.image.color = new  Color(0, 114, 198);

        //change the color of the selected speed black
        button.image.color = Color.blue;
    }

    public void LaunchButton()
    {
        //this method is called when the launch button is clicked
        //and moves new player values into the stats manager
        //and goes back to the play menu

        //add achievements to stat manager and mark not first play
        stats.achievements = achievements;
        stats.firstPlay = false;

        //add this difficulty to completed difficulties list
        if (!stats.completedDifficulties.Contains(difficulty))
            stats.completedDifficulties.Add(difficulty);

        //load the title screen
        Application.LoadLevel(0);
    }
    #endregion

    #region building
    private void buildMiner(Vector3 site)
    {
        //this method is used to instantiate a mining craft

        //make sure the mining action is available
        if(!readyToMine())
            return;

        //create target object on planet for craft to move toward
        GameObject newTarget = Instantiate(moveTarget, site, Quaternion.identity) as GameObject;
        newTarget.transform.SetParent(Main.currentPlanet.transform);

        //build a miner
        GameObject c = Instantiate(minerPrefab, manufactureTarget.transform.position, Quaternion.identity) as GameObject;

        //set miner target and scale
        Miner m = c.GetComponent<Miner>();
        m.target = newTarget;
        m.transform.localScale = new Vector3(.005f, .005f, .005f);
        m.targetResource = targetResource;

        //deduct resources
        playerResources[2] -= minerMoney;
        playerResources[0] -= minerIronCost;
        playerResources[3] -= Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, site));   //calculate hydrogen cost based on move distance

        //display mini message
        StartCoroutine(displayMiniMessage("Miner Launched"));

        //increment the number of mining rigs on this planet 
        getCurrentPlanet().currentMiners++;
    }

    public void BuildSyphoner(Vector3 site)
    {
        //this method is used to instantiate an explorer craft

        //exit method if not ready to mine
        if (!readyToSyphon())
            return;

        //create target object on planet for craft to move toward
        GameObject newTarget = Instantiate(moveTarget, site, Quaternion.identity) as GameObject;
        newTarget.transform.SetParent(Main.currentPlanet.transform);

        //build a syphoner
        GameObject c = Instantiate(syphonPrefab, manufactureTarget.transform.position, Quaternion.identity) as GameObject;

        //set craft target and scale
        Syphoner s = c.GetComponent<Syphoner>();
        s.target = newTarget;
        s.transform.localScale = new Vector3(.01f, .01f, .01f);
        s.targetResource = targetResource;

        //deduct resources
        playerResources[2] -= syphonerMoneyCost;
        playerResources[0] -= syphonerIronCost;
        playerResources[3] -= Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, site));       //calculate hydrogen cost based on move distance

        //display mini message
        StartCoroutine(displayMiniMessage("Syphoner Launched"));
    }

    public void BuildCasino(Vector3 site)
    {
        //this method is used to instantiate a resort

        //create target object on planet for craft to move toward
        GameObject newTarget = Instantiate(moveTarget, site, Quaternion.identity) as GameObject;
        newTarget.transform.SetParent(Main.currentPlanet.transform);

        //Transform syphonTarget = Main.getCurrentPlanet().syphonTargets[Main.getCurrentPlanet().syphonIndex].transform;
        GameObject s = Instantiate(casinoPrefab, manufactureTarget.transform.position, Quaternion.identity) as GameObject;
        Craft c = s.GetComponent<Craft>();
        c.setDistance(Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, site)));

        //set craft target and scale
        c.target = newTarget;
        c.transform.localScale = new Vector3(.005f, .005f, .005f);
        c.targetPlanet = getCurrentPlanet();

        //Check if player can afford to build
        for (int i =0; i < 4; i++)
        {
            if(c.cost[i] > playerResources[i])
            {
                StartCoroutine(displayMiniMessage("You don't have enough " + resourceName[i] + " to build a resort."));
                Destroy(s);
                return;
            }
        }

        //set achievement flag
        builtCasino = true;

        //deduct resources 
        for (int i = 0; i < 4; i++)
        {
                playerResources[i] -= c.cost[i];
        }

        //display mini message
        StartCoroutine(displayMiniMessage("Resort Launched"));
    }

    public void BuildFactory(Vector3 site)
    {
        //this method is used to instantiate a factory

        //create target object on planet for craft to move toward
        GameObject newTarget = Instantiate(moveTarget, site, Quaternion.identity) as GameObject;
        newTarget.transform.SetParent(Main.currentPlanet.transform);


        //Transform syphonTarget = Main.getCurrentPlanet().syphonTargets[Main.getCurrentPlanet().syphonIndex].transform;
        GameObject s = Instantiate(factoryPrefab, manufactureTarget.transform.position, Quaternion.identity) as GameObject;
        Factory f = s.GetComponent<Factory>();
        f.setDistance(Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, site)));
        f.target = newTarget;
        f.transform.localScale = new Vector3(.005f, .005f, .005f);

        //Check if player can afford to build
        for (int i = 0; i < 4; i++)
        {
            if (f.cost[i] > playerResources[i])
            {
                StartCoroutine(displayMiniMessage("You don't have enough " + resourceName[i] + " to build a factory."));
                Destroy(s);
                return;
            }

        }

        //set achievement flag
        builtFactory = true;

        //deduct resources 
        for (int i = 0; i < 4; i++)
            playerResources[i] -= f.cost[i];
        
        //display mini message
        StartCoroutine(displayMiniMessage("Factory Launched"));
    }

    public void BuildStudio(Vector3 site)
    {
        //this method is used to instantiate a studio

        //create target object on planet for craft to move toward
        GameObject newTarget = Instantiate(moveTarget, site, Quaternion.identity) as GameObject;
        newTarget.transform.SetParent(Main.currentPlanet.transform);

        //Transform syphonTarget = Main.getCurrentPlanet().syphonTargets[Main.getCurrentPlanet().syphonIndex].transform;
        GameObject s = Instantiate(studioPrefab, manufactureTarget.transform.position, Quaternion.identity) as GameObject;
        Studio m = s.GetComponent<Studio>();
        m.setDistance(Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, site)));
        m.target = newTarget;
        m.transform.localScale = new Vector3(.005f, .005f, .005f);

        //Check if player can afford to build
        for (int i = 0; i < 4; i++)
        {
            if (m.cost[i] > playerResources[i])
            {
                StartCoroutine(displayMiniMessage("You don't have enough " + resourceName[i] + " to build a studio."));
                Destroy(s);
                return;
            }
        }

        //set achievement flag
        builtStudio = true;

        //deduct resources 
        for (int i = 0; i < 4; i++)
            playerResources[i] -= m.cost[i];

        //display mini message
        StartCoroutine(displayMiniMessage("Studio Launched"));
    }


    public void BuildExplorer(Vector3 site)
    {
        //this method is used to instantiate an explorer craft

        //make sure the mining action is available
        if (!readyToExplore())
            return;

        //create target object on planet for craft to move toward
        GameObject newTarget = Instantiate(moveTarget, site, Quaternion.identity) as GameObject;
        newTarget.transform.SetParent(Main.currentPlanet.transform);

        //build an explorer
        GameObject c = Instantiate(explorerPrefab, manufactureTarget.transform.position, Quaternion.identity) as GameObject;

        //set explorer target and scale
        Explorer e = c.GetComponent<Explorer>();
        e.target = newTarget;
        e.transform.localScale = new Vector3(.005f, .005f, .005f);
        e.targetPlanet = getCurrentPlanet();

        //deduct resources
        playerResources[2] -= explorerMoneyCost;
        playerResources[0] -= explorerIronCost;
        playerResources[3] -= Mathf.FloorToInt(Vector3.Distance(manufactureTarget.transform.position, site));

        //display mini message
        StartCoroutine(displayMiniMessage("Explorer Launched"));

    }
    #endregion

    #region tests
    public bool readyToMine()
    {
        //this method returns true when all of the criteria
        //are met for the current planet to be mined

        //false if the current planet hasn't been explored
        if (!getCurrentPlanet().explored)
        {
            StartCoroutine(displayMiniMessage("This planet must be further investigated before you can mine."));
            return false;
        }

        /*
        //false if there are already too many miners on the planet
        if (getCurrentPlanet().currentMiners >= (getCurrentPlanet().maxMiners * 100))
        {
            StartCoroutine(displayMiniMessage("This planet can't support any more mining rigs."));
            return false;
        }
        */

        //false if the player doesn't have enough money
        if (playerResources[2] < minerMoney)
        {
            StartCoroutine(displayMiniMessage("You can't afford to build a mining rig."));
            return false;
        }

        //false if the player doesn't have enough iron
        if (playerResources[0] < minerIronCost)
        {
            StartCoroutine(displayMiniMessage("You don't have enough iron to build a mining rig."));
            return false;
        }

        //false if the player doesn't have enough hydrogen
        if (playerResources[3] < minerHydrogenCost)
        {
            StartCoroutine(displayMiniMessage("You don't have enough hdrogen to launch a mining rig."));
            return false;
        }

        //if no problems with mining, return true to continue to mine
        return true;
    }

    public bool readyToExplore()
    {
        //this method returns true when the criteria
        //are met for the current planet to be explored

        //false if the planet has previously been explored
        if (getCurrentPlanet().explored)
        {
            StartCoroutine(displayMiniMessage("This planet has already been explored."));
            return false;
        }

        //false if the player doesn't have enough money
        if (playerResources[2] < explorerMoneyCost)
        {
            StartCoroutine(displayMiniMessage("You can't afford to build an orbiter."));
            return false;
        }

        //false if the player doesn't have enough iron
        if (playerResources[0] < explorerIronCost)
        {
            StartCoroutine(displayMiniMessage("You don't have enough iron to build an orbiter."));
            return false;
        }

        //false if the player doesn't have enough hydrogen
        if (playerResources[3] < explorerHydrogenCost)
        {
            StartCoroutine(displayMiniMessage("You don't have enough hydrogen to launch an oribter."));
            return false;
        }

        //if no problems, return true to continue to explore
        return true;
    }

    public bool readyToSyphon()
    {
        //this method returns true if the criteria
        //are met for the current planet to be syphoned

        //false if the planet hasn't yet been explored
        if (!getCurrentPlanet().explored)
        {
            StartCoroutine(displayMiniMessage("This planet must be further investigated before you can mine."));
            return false;
        }

        //false if the player doesn't have enough money
        if (playerResources[2] < syphonerMoneyCost)
        {
            StartCoroutine(displayMiniMessage("You can't afford to build a syphoning rig."));
            return false;
        }

        //false if the player doesn't have enough iron
        if (playerResources[0] < syphonerIronCost)
        {
            StartCoroutine(displayMiniMessage("You don't have enough iron to build a syphoning rig."));
            return false;
        }

        //false if the player doesn't have enough hydrogen
        if (playerResources[3] < syphonerHydrogenCost)
        {
            StartCoroutine(displayMiniMessage("You don't have enough hydrogen to launch a syphoning rig."));
            return false;
        }

        //if no problems with syphoning, return true to continue to syphon
        return true;
    }

    private bool CheckGoals()
    {
        //this method returns true if the player has
        //met all of the allotted goals of the game

        if (playerResources[0] >= goals[0] && playerResources[1] >= goals[1] &&
            playerResources[2] >= goals[2] && playerResources[3] >= goals[3])
            return true;

        return false;
    }
    #endregion

    #region gets and sets
    public static Planet getCurrentPlanet()
    {
        //this method returns the current planet as a gameObject

        if (currentPlanetIndex != 0)
            return currentPlanet.GetComponent<Planet>();
        else return null;
    }

    public void setTechLevel(int i)
    {
        this.techLevel = i;
    }

    public int getTechLevel()
    {
        return techLevel;
    }

    private void SetResources()
    {
        //this method is called in Main.Start() and is used to 
        //vary starting resources based on chosen difficulty

        switch (difficulty)
        {
            case 0:
                playerResources = new int[] { 750, 0, 750, 750, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                break;

            case 1:
                playerResources = new int[] { 500, 0, 500, 500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                break;

            case 2:
                playerResources = new int[] { 250, 0, 250, 250, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                break;

            case 3:
                playerResources = new int[] { 100, 0, 150, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                break;

            case 4:
                playerResources = new int[] { 50, 0, 100, 75, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                break;
        }

    }

    private void SetGoals()
    {
        //this method is called in Main.Start() and is used to 
        //vary player goals based on chosen difficulty

        switch (difficulty)
        {
            case 0:
                goals = new int[] { 75, 50, 50, 75 };
                break;

            case 1:
                goals = new int[] { 750, 500, 500, 750 };
                break;

            case 2:
                goals = new int[] { 1000, 750, 750, 1000 };
                break;

            case 3:
                goals = new int[] { 1500, 1000, 1000, 1500 };
                break;

            case 4:
                goals = new int[] { 1500, 1000, 1000, 1500 };
                break;
        }
    }

    
    public void SetResourceText()
    {
        //this method updates the text in the resources panel

        resourcePanel.updatePanel();
        /*
        ironText.text = "Iron: " + playerResources[0];
        sulfurText.text = "Sulfur: " + playerResources[1];
        moneyText.text = "Money: " + playerResources[2];
        hydrogenText.text = "Hydrogen: " + playerResources[3];
        */
    }

    public void SetGoalText()
    {
        //this method updates the text in the goals panel

        ironGoalText.text = "Iron: " + goals[0];
        sulfurGoalText.text = "Sulfur: " + goals[1];
        moneyGoalText.text = "Money: " + goals[2];
        hydrogenGoalText.text = "Hydrogen: " + goals[3];
    }
    #endregion

    #region tutorial
    public void CloseTut1()
    {
        //this method is called in when clicking the 
        //first tutorial panel and launches the second panel

        tutorialPanel.SetActive(false);
        tutorialPanel2.SetActive(true);
    }

    public void CloseTut2()
    {
        //this method is called when clicking the second tutorial
        //panel and closes the panel and unpauses the game

        tutorialPanel2.SetActive(false);
        Time.timeScale = (1f);
    }
    #endregion

    #region achievements
    public IEnumerator DisplayAchievementText(string msg)
    {
        //this method gets display text and display the panel for 4 seconds

        achievementText.text = msg;
        achievementPanel.SetActive(true);
        yield return new WaitForSeconds(4);
        achievementPanel.SetActive(false);
    }

    void AddAchievement(string msg, string code)
    {
        //this method changes the false achievement flag to true
        //by removing and readding the achievement to the dictionary

        achievements.Remove(code);                      //remove the current achievement from the dictionary
        achievements.Add(code, true);                   //replace the achievement with a true value
        StartCoroutine("DisplayAchievementText", msg);  //display the achievement text box
    }

    void CheckAchievements()
    {
        //if the Built 100 Craft achievement hasn't been obtained, check for it
        if (!achievements["Built100Craft"] == true)
            if (craftBuilt >= 100)
                AddAchievement("Achievement Earned:\n BUILT 100 CRAFT", "Built100Craft");

        //if the Got 10k Money achievement hasn't been obtained, check for it
        if (!achievements["Got10kMoney"] == true)
            if (playerResources[2] >= 10000)
                AddAchievement("Achievement Earned:\n COLLECTED $10,000", "Got10kMoney");

        //if the Got 10k Hydrogen achievement hasn't been obtained, check for it
        if (!achievements["Got10kHydrogen"] == true)
            if (playerResources[3] >= 10000)
                AddAchievement("Achievement Earned:\n COLLECTED 10,000 HYDROGEN", "Got10kHydrogen");

        //if the Got 10k Sulfur achievement hasn't been obtained, check for it
        if (!achievements["Got10kSulfur"] == true)
            if (playerResources[1] >= 10000)
                AddAchievement("Achievement Earned:\n COLLECTED 10,000 SULFUR", "Got10kSulfur");

        //if the Got 10k Iron achievement hasn't been obtained, check for it
        if (!achievements["Got10kIron"] == true)
            if (playerResources[0] >= 10000)
                AddAchievement("Achievement Earned:\n COLLECTED 10,000 IRON", "Got10kIron");

        //if the All Commercial Buildings achievement hasn't been obtained, check for it
        if (!achievements["BuiltAllCommercial"] == true)
            if (builtCasino && builtFactory && builtStudio)
                AddAchievement("Achievement Earned:\n BUILT ALL COMMERCIAL", "BuiltAllCommercial");

        //if the Syphoned Venus achievement hasn't been obtained, check for it
        if (!achievements["SyphonedVenus"] == true)
            if (syphonedVenus)
                AddAchievement("Achievement Earned:\n SYPHONED VENUS", "SyphonedVenus");
    }
    
    void SetAchievements()
    {
        //this method adds all possible achievements to a dictionary, defaulting them to being false

        if (stats.firstPlay)
        {
            achievements.Add("ExploredAllPlanets", false);
            achievements.Add("SyphonedVenus", false);
            achievements.Add("BuiltAllCommercial", false);
            achievements.Add("Got10kMoney", false);
            achievements.Add("Got10kIron", false);
            achievements.Add("Got10kSulfur", false);
            achievements.Add("Got10kHydrogen", false);
            achievements.Add("Built100Craft", false);
        }
    }

    public void OpenIdentCard()
    {
        //this method is called when the ID card button
        //is clicked and opens or closes the panel

        //if the card is closed, open it and activate any new achievements
        if (!identCard.active)
        {
            SetAchievementIcons();
            identCard.SetActive(true);
        }

        //if the card is already open, close it
        else
            identCard.SetActive(false);
    }

    private void SetAchievementIcons()
    {
        //this method is called when the ID badge panel is open
        //and sets the visibility of the achievement icons

        if (achievements["SyphonedVenus"] == true)
            venusAchievement.GetComponent<Image>().color = Color.white;

        if (achievements["ExploredAllPlanets"] == true)
            exploreAchievement.GetComponent<Image>().color = Color.white;

        if (achievements["BuiltAllCommercial"] == true)
            commerceAchievement.GetComponent<Image>().color = Color.white;

        if (achievements["Got10kMoney"] == true)
            moneyAchievement.GetComponent<Image>().color = Color.white;

        if (achievements["Got10kSulfur"] == true)
            ironAchievement.GetComponent<Image>().color = Color.white;

        if (achievements["Got10kIron"] == true)
            sulfurAchievement.GetComponent<Image>().color = Color.white;

        if (achievements["Got10kHydrogen"] == true)
            hydrogenAchievement.GetComponent<Image>().color = Color.white;

        if (achievements["Built100Craft"] == true)
            craftAchievement.GetComponent<Image>().color = Color.white;
    }
    #endregion

}
