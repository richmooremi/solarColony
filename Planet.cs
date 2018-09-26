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
    public int[] resources = new int[16];

    //the high and low temperatures of the planet (not fully implemented)
    public Vector2 tempRange = new Vector2(0, 0);

    //mining locations (depricated)
    public List<GameObject> mineTargets = new List<GameObject>();
    public int miningIndex =  0;

    //syphon locations (depricated)
    public List<GameObject> syphonTargets = new List<GameObject>();
    public int syphonIndex = 0;

    //explorer target (depricated)
    public Transform exploreTarget;

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
	}
	
    void SetTargets()
    {
        //this method is called in Start() and used to create
        //lists of mining syphoning, and exploration targets,
        //has been replaced by the OrbitSphere system

        //get all of the children in this planet and create list of mine targets
        Transform[] getChildren = this.GetComponentsInChildren<Transform>();

        //for every child that is a mine target, add it to the list
        foreach (Transform t in getChildren)
        {
            if (t.gameObject.tag == "MineTarget")
            {
                mineTargets.Add(t.gameObject);
            }

            else if (t.gameObject.tag == "ExploreTarget")
            {
                exploreTarget = t;
            }

            else if (t.gameObject.tag == "SyphonTarget")
            {
                syphonTargets.Add(t.gameObject);
            }
        }
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

    public void updateActions()
    {
        //this method is called when the menu is redrawn and
        //adds the adds the appropriate actions to the list

        if (explored)
        {
            availableActions.Remove("Explore");

            if (canMine)
                availableActions.Add("Mine");

            if (canSyphon)
                availableActions.Add("Syphon");

            availableActions.Add("Build");
        }
    }
}
