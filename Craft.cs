/*
    This class controls the creation, cost, and movement of 
    various spacecraft and is intended as a base for other classes.

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;

public class Craft : MonoBehaviour {

    //target planet and resources, passed in when the craft is created
    public Planet targetPlanet;

    //the cost to build the craft
    public int[] cost;   

    //variables for moving into position
    public GameObject target;
    public float speed = .1f;
    public float useSpeed = 1f;
    public bool reachedTarget = false;
    public int currentTechLevel =0;

    //set true from the Income(), Mine(), and Syphon() methods in various subclasses
    protected bool startedCollecting = false;

    //the distance between the manufacturer and target locations,
    //passed in when the craft is created
    public int distance;

    //reference to the planet panel
    protected PlanetPanel planetpanel;
    protected Main main;

    //determines whether the craft is currently in use
    protected bool inUse = true;

    protected void Start ()
    {

        //get the planet that was selected when this craft was instantiated
        targetPlanet = Main.getCurrentPlanet();

        planetpanel = GameObject.FindObjectOfType<PlanetPanel>();
        main = GameObject.FindObjectOfType<Main>();

        //set tech level to control how fast this craft mines
        currentTechLevel = main.getTechLevel();
        speed *= main.getTechLevel() * 1.5f;
        useSpeed *= main.getTechLevel() * 1.5f;

        //craft are in use by default
        inUse = true;

    }


    public void setDistance(int dist)
    {
        //this method calculates the hydrogen cost based on the
        //distance the craft must travel to its target

        distance = dist;
        cost[3] = dist * 5;
    }

    public void SetPlanet(GameObject planet)
    {
        //this method sets the currently selected Planet
        //as the targetPlanet for this craft

        targetPlanet = planet.GetComponent<Planet>();
    }

    protected void Update()
    {
        if (target)
        {
            //if the reached target flag has already been set, do nothing and return
            if (reachedTarget)
            {
                return;
            }

            //if the craft is at it's target destination, set the reached target flag
            if (transform.position == target.transform.position)
                reachedTarget = true;

            //if the target hasn't been reached yet, keep moving toward the target
            if (!reachedTarget)
            {
                //move toward target's current position
                transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, speed);

                //keep the the craft oriented toward the planet, this can be changed
                //later to keep the craft oriented with thrusters away from the planet
                if (targetPlanet)
                {
                    this.transform.LookAt(targetPlanet.transform);
                    transform.up = transform.forward * -1;
                }
            }

            //if the target has been reached for the first time, set the parent and flag
            else
            {
                //orient the bottom of the craft toward the planet, this will
                //remain the same after thrusters are added to the craft
                if (targetPlanet)
                {
                    this.transform.LookAt(targetPlanet.transform);
                    transform.up = transform.forward * -1;
                    this.transform.SetParent(target.transform);
                    reachedTarget = true;
                }

                //set the parent to the targeted planet
                if (targetPlanet)
                    this.transform.SetParent(targetPlanet.gameObject.transform);

                //destroy the target gameObject to prevent buildup of unneeded objects
                Destroy(target);

                
            }
        }
    }



}
