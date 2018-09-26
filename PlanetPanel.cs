/*
    This class is used to control the planet panel in the bottom right of
    the UI panel.

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;
using UnityEngine.UI;

public class PlanetPanel : MonoBehaviour {

    #region variables
    private string[] shortDescriptions;             //short description to be displayed in the collapsed planet panel
    private string[] longDescriptions;              //long description to be displayed in the expanded planet panel
    private bool extended = false;                  //true when the panel is expanded into its full size

    public Text planetName, shortDesc, longDesc;    //text objects for the planet name, and short and long descriptions     
    public Text explorePlanet;                      //text object used in leu of long description before the planet is expored

    public Text lowTemp, highTemp;                  //text objects for the planet's temperature range 

    public Text ironText, sulfurText, hydrogenText; //text objects for the planet's resources                

    public Button panelButton;                      //button object used to collapse and expand the planet panel
    public GameObject backgroundPanel;              //panel object containing all other text and panels

    public NavPanel navPanel;                       //the navigation panel must be referenced to control the moon panel



    public Sprite[] allResourceImages = new Sprite[36];
    public Text[] resourceText = new Text[4];
    public Image[] resourceImages = new Image[4];



    #endregion

    void Awake ()
    {
        //initialize arrays for short andl ong description strings and set description texts
        shortDescriptions = new string[9];
        longDescriptions = new string[9];
        setDescriptions();
    }

    public void updatePanel()
    {
        //this method is called when switching between planets and updates the text in the panel

        //set the name and desctiptions of the current planet or system
        planetName.text = Main.planets[Main.currentPlanetIndex];
        shortDesc.text = shortDescriptions[Main.currentPlanetIndex];
        longDesc.text = longDescriptions[Main.currentPlanetIndex];

        //if the player is not on the map, set the temp and resources text, as showing/hiding the moon panel
        if (Main.currentPlanetIndex != 0)
        {
            lowTemp.text = Main.getCurrentPlanet().tempRange.x.ToString() + "°C";
            highTemp.text = Main.getCurrentPlanet().tempRange.y.ToString() + "°C";

            //auto generates grid of up to 4 most abundent elements
            for (int i = 0; i < 4; i++)
            {
                if (Main.getCurrentPlanet().topFourResources[i] == -1)
                {
                    resourceImages[i].GetComponent<Image>().color = new Vector4(0,0,0,0);
                    resourceText[i].text = "";
                }

                else
                {
                    
                    int r = Main.getCurrentPlanet().topFourResources[i];
                    Debug.Log("planet " + Main.resourceName[r]);
                    resourceImages[i].sprite = allResourceImages[r];
                    resourceText[i].text = Main.getCurrentPlanet().resources[r].ToString();
                }
            }
        }
    }

    public void updateResources()
    {
        for (int i = 0; i < 4; i++)
        { 
            if(Main.getCurrentPlanet().topFourResources[i] == -1)
                resourceText[i].text = "";

            else
                resourceText[i].text = Main.getCurrentPlanet().resources[Main.getCurrentPlanet().topFourResources[i]].ToString();
        }
    }

    public void movePanel()
    {
        //this method is used to collapse and expand the planet panel

        //if the panel is collapsed, move into the expanded position
        if (!extended)
        {
            backgroundPanel.GetComponent<Image>().color = new Vector4(255, 255, 255, .7f);
            panelButton.transform.localEulerAngles = new Vector3(0, 0, 180);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(21.7f, 111.1f);
            extended = true;
        }

        //if the panel is expanded, move back into the collapsed position
        else
        {
            panelButton.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(21.7f, 0f);
            backgroundPanel.GetComponent<Image>().color = new Vector4(255, 255,255, 0);
            extended = false;
        }

    }

    private void setDescriptions()
    {
        //this method is called in Start() and sets the descriptive text for
        //all of the planets and the system

        //system
        shortDescriptions[0] = "The local area of the galaxy. Definitely not to scale.";
        longDescriptions[0] = "This solar system is roughly 1 light year across and is situated on the galaxy's Orion arm, a fair distance from the chaotic galactic core. It contains 4 terrestrial planets and 4 gas giants in addition toe an innumerable number of dwarf planets, asteroids, and comets.";


        //mercury
        shortDescriptions[1] = "A small rocky planet baked dry by the Sun.";
        longDescriptions[1] = "The smallest of the planets, Mercury's proximity to the Sun causes its close side to be over 400° C, while it's lengthy 52 day rotation period causes it's dark side to plunge below -170° C.\n" +
                                "While any atmosphere that may have once existed has long since evaporated, this planet is rich in iron rich rock.";

        //venus
        shortDescriptions[2] = "A medium sized planet with a very thick carbon dioxide atmosphere.";
        longDescriptions[2] = "The atmosphere on Venus is so thick and acidic that every probe we send to the surface would be crushed under the pressure and/or eroded by sulfuric acid and/or melted in the sweltering 462° C temperatures.";

        //earth
        shortDescriptions[3] = "The nice planet in this star's habitable zone.";
        longDescriptions[3] = "Earth is roughly in the middle of the local habitable zone where it is cool enough for liquid water not to freeze and warm enough for liquid water not to boil away from the atmosphere.";

        //mars
        shortDescriptions[4] = "A smallish red planet with heavy iron deposits.";
        longDescriptions[4] = "Mars gets its red color from a layer of oxidized iron dust coating its surface. While liquid water does exist here, the thin atmosphere, lack of magnetic field, and weak gravity pose serious problems for future colonization.";

        //jupiter
        shortDescriptions[5] = "The largest planet in the system.";
        longDescriptions[5] = "The magnetic field of Jupiter is so strong that it funnels radiation from the Sun into a protective shield of death around the planet and its incredible mass protects the terrestrial planets by deflecting asteroids and comets.";

        //saturn
        shortDescriptions[6] = "A beautiful planet with a complex system of rings and moons.";
        longDescriptions[6] = "Having just one fewer moon than Jupiter, and being the only planet with a clearly visible ring system, Saturn is one of the most recognizable objects in this solar system.";

        //uranus
        shortDescriptions[7] = "A faint blue gas giant with thin rings.";
        longDescriptions[7] =  "The first planet discovered by modern day astronomers, Uranus gets its mostly uniform blue color from its uniform hydrogen atmosphere. Having just 27 moons and a very faint ring system, Uranus is often overlooked and thought to be uninteresting.";

        //neptune
        shortDescriptions[8] = "The last of the main planets.";
        longDescriptions[8] = "It's great distance from the sun means that Neptune gets very little heat from the sun and maintains a temperature of just over -220° C. White clouds of methane vapor can be seen storming over the blue hydrogen atmosphere.";
    }
}
