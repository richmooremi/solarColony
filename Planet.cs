/*
    This class controls the rotation and orbit of
    planets as well as the resources which it contains.

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;
using System.Collections.Generic;

public class Planet : MonoBehaviour {

    //variables to control the revolution and rotation of each planet
    private GameObject sun;
    public float orbitSpeed;
    public float rotationSpeed;

    //resources contained by the planet, used with Main.resourceNames[]  
    public int[] resources = new int[7];

    //the top 4 elements that this planet has, used in the PlanetPanel
    public int[] topFourResources = new int[4];
    public int numberOfResources;

    //the high and low temperatures of the planet (not fully implemented)
    public Vector2 tempRange = new Vector2(0, 0);

    public bool explored = false;       //true when planet has been explored by the player
    public bool canMine, canSyphon;     //true when the planet can be mined or syphoned

    //list of available actions which translate into menu items
    public List<string> availableActions;

    //a list of the planet's moons, if any exist
    public List<GameObject> moons = new List<GameObject>();

    public bool hasMoons = false;           //true if the planet has moons
    public int maxMiners, currentMiners;    //the current and maximum number of miners that the planet can support
    public string type;                     //gas or rock, used to control if the planet is mined or syphoned

    void Start ()
    {
        //add Explore to the available actions menu
        availableActions.Add("Explore");

        //find the sun, the transform is used with RotateAround()
        sun = GameObject.Find("Sun");

        SetTargets();

        calculateTopFourResources();

    }
	
    void SetTargets()
    {
        //this method is called in Start() and used to create
        //lists of mining syphoning, and exploration targets,
        //has been replaced by the OrbitSphere system

        //get all of the children in this planet and create list of mine targets
        Transform[] getChildren = this.GetComponentsInChildren<Transform>();

    }

	void FixedUpdate()
    {
        //this method updates at a set rate so that
        //the planets have smooth motion and rotation

        Orbit();
    }

    void Orbit()
    {
        //this method is called in FixedUpdate() and controls
        //the orbit and rotation of the planet

        //if this is a moon, rotate around parent planet most moons
        //are tidally locked, so no further correction is needed
        if (this.tag == "Moon")
        {
            this.transform.RotateAround(this.transform.parent.position, Vector3.up, orbitSpeed * Time.deltaTime * -1);  // rotate moon CCW around planet
            return;
        }

        //rotate planet around center of sun
        GameObject parent = this.transform.parent.gameObject;                                                 //get the planet's parent object (the Sun or Planet)
        Quaternion rot = parent.transform.rotation;                                                           //get the planet's current rotation
        parent.transform.RotateAround(sun.transform.position, Vector3.up, orbitSpeed * Time.deltaTime * -1);  //rotate planet CCW around sun
        parent.transform.rotation = rot;                                                                      //reset planet's rotation

        //rotate planet around axis
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * -1);                              //rotate planet CCW around axis
    }

    private void calculateTopFourResources()
    {
        //initialize array to -1
        for (int i = 0; i < 4; i++)
            topFourResources[i] = -1;
        
        //create new sorted list
        SortedList<int, int> list = new SortedList<int, int>();

        //add non-zero values to list
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i] != 0)
                list.Add(resources[i], i);
        }

        //iterate backward through the list, but forward through the array
        int j = 0;
        for (int i = list.Count -1; i >= 0; i--)
        {
            topFourResources[j] =  list.Values[i];
            j++;
        }

        numberOfResources = list.Count;
        Debug.Log(this.name + " " + numberOfResources + " " + list.Count);
        
    }

    public void updateActions()
    {
        //this method is called when the menu is redrawn and
        //adds the adds the appropriate actions to the list

        if (explored)
        {
            if(canMine)
                availableActions.Add("Mine");

            if (canSyphon)
                availableActions.Add("Syphon");

            availableActions.Add("Build");
        }
    }
}
