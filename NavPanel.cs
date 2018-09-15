/*
    This class is used to control the navigation panel along the right
    edge of the UI.

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NavPanel : MonoBehaviour {

    #region variables
    //icon image sprites, set in the inspector
    public Sprite IconMap, IconMercury, IconVenus, IconEarth,
        IconMars, IconJupiter, IconSarutn, IconUranus,
        IconNeptune;

    //image slots in the nav panel, set in the inspector
    public Image ImageUp, ImageDown, ImageJump1, ImageJump2,
        ImageJump3, ImageJump4, ImageJump5, ImageJump6;

    private Sprite[] icons;                 //holds available icons to be sorted throug, may be repalced with a list later
    private Image[] imageSlots;             //holds the available slots where images can be placed
    private int currentIconIndex;           //current icon index,  with main's current camera index

    private GameObject[] moons;             //holds the moons of the current planet
    private Sprite[] moonIcons;             //holds the icons of the planet's moons
    private List<GameObject> moonSlots;
    public GameObject moonButton;           //the prefab for the moons button
    private int numberOfMoons;              //the number of moons the current planet has

    public GameObject moonsPanel;           //panel that contains moons of the planet, if any exist

    //the main class must be instanciated in 
    //order to call its JumpToCamera() method
    private Main main;
    #endregion

    void Awake()
    {
        //create null colelction of moon button slots
        moonSlots = new List<GameObject> { null };
    }

    void Start () {   

        //assign main to the Main script on the Solar System object
        main = GameObject.Find("SolarSystem").GetComponent<Main>();

        //set current icon index to 0 at launch to correspond
        //with main's current camera inxex
        currentIconIndex = 0;

        //initializes the icon array
        icons = new Sprite[] {IconMap, IconMercury, IconVenus, IconEarth,
        IconMars, IconJupiter, IconSarutn, IconUranus,
        IconNeptune};

        //initializes the image slot array
        imageSlots = new Image[] {ImageJump1, ImageJump2,
        ImageJump3, ImageJump4, ImageJump5, ImageJump6};

        //places an sprite in each slot based
        //on the current icon index
        int i = 0;
        foreach (Image s in imageSlots)
        {
            s.sprite = icons[currentIconIndex + i];
            s.name = (currentIconIndex + i).ToString();
            i++;
        }

    }

    public void nextImages()
    {
        //called on clicking the down arrow in the panel,
        //cycles through sprites until the bottom is reached

        //return if there are no more images to display
        if (currentIconIndex + imageSlots.Length >= icons.Length)
            return;

        //increment current icon set
        currentIconIndex++;

        //places an sprite in each slot based
        //on the current icon index
        int i = 0;
        foreach (Image s in imageSlots)
        {
            s.sprite = icons[currentIconIndex + i];
            s.name = (currentIconIndex + i).ToString();
            i++;
        }
    }

    public void prevImages()
    {
        //called on clicking the up arrow in the panel,
        //cycles through sprites until the top is reached

        //return if at the top of the list
        if (currentIconIndex <= 0)
            return;

        //increment current icon set
        currentIconIndex--;

        //places an sprite in each slot based
        //on the current icon index
        int i = 0;
        foreach (Image s in imageSlots)
        {
            s.sprite = icons[currentIconIndex + i];
            s.name = (currentIconIndex + i).ToString();
            i++;
        }
    }

    public void onClick()
    {
        //called when clicking on a planet button, gets the name
        //of the current button and passes it as an index
        //parameter into main's JumpToCamera method,
        //along with the current camera index to be disabled

        int ndx = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        int currentNdx = Main.currentPlanetIndex;
        main.JumpToCamera(currentNdx, ndx);
    }

    #region moons
    public void setMoonPanel(bool moons)
    {
        //opens the moon panel if the currently selected planet has moons, closes it if there are no moons
        moonsPanel.SetActive(moons);

        //sets numberofMoons to the number of moons set in the currentPlanet object
        numberOfMoons = Main.getCurrentPlanet().moons.Count;

        //change local height of moon panel
        moonsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(22, 30 * numberOfMoons);

        //populates the moon panel if there are an moons
        if (moons)
            populateMoonPanel();
    }

    public void populateMoonPanel()
    {
        //destroy any existing moon buttons
        {
            foreach (GameObject t in moonSlots)
            {
                Destroy(t);
            }
        }

        //create null moon slots list
        moonSlots = new List<GameObject> { null };

        //get list of moons from current planet object
        moons = Main.getCurrentPlanet().moons.ToArray();

        
        float posX = 0;
        float posY = 0;
        float firstPosY = (((float)numberOfMoons -1) * 15f);
        float newPosY = firstPosY;
        int i = 0;

        foreach (GameObject m in moons)
        {
            GameObject button;
            button = GameObject.Instantiate(moonButton, Vector3.zero, Quaternion.identity) as GameObject;

            //set scale based to inverse of how the panel was scaled to keep the boxes square
            //float scaleY = 1f / (float)numberOfMoons;
            button.transform.SetParent(moonsPanel.transform);
            button.transform.localScale = new Vector3(1f, 1f ,1f);

            posY = newPosY;
            newPosY -= 30f;
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            moonSlots.Add(button);

            i++;
        }
    }
    #endregion
}

