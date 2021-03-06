﻿/*
    This class inherits Craft and contains Mine() method
    which is used to calculate iron income from the craft

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;
using System.Collections;

public class Miner : Craft {

    //resources contained by the craft used with Main.resourceNames[]  
    //to be used to craft with larger capacities that can mine more than one element
    public int[] resources = new int[16];
    public int targetResource;


    //statistics of the miner craft
    private int capacity = 5;                       //carrying capacity of the craft
    //private float miningSpeed = .25f;               //mining speed variable, expressed in seconds

    void Start ()
    {

        base.Start();

        //set iron to 0
        resources[targetResource] = 0;

        cost = new int[] { 25, 0, 50, 100 };   //the cost to build the Miner

    }

    void Update()
    {
        base.Update();

        //start the mining routine if the craft is just reaching its target location
        if (!startedCollecting && reachedTarget)
        {
            startedCollecting = true;
            StartCoroutine(Mine(useSpeed));
        }
    }
    
    public void SetResource(int resource)
    {
        //this method is called when craft is created
        //and sets target resource

        targetResource = resource;
    }

    IEnumerator Mine(float waitTime)
    {
        //THIS METHOD CONTROLS MINING OF RESOUCES FROM PLANETS
        //PLANET -> CRAFT -> PLAYER

        if (targetPlanet.resources[targetResource] == 0)
        {
            this.resources[targetResource] = 0;
            Main.playerResources[targetResource] += this.capacity;
            planetpanel.updateResources();
            inUse = false;
        }

       // else if (targetPlanet.resources[targetResource] < 5)
       //     planetpanel.updateResources();

        //while the target planet still has iron
        while (targetPlanet.resources[targetResource] > 0 && inUse)
        {
            //if the craft has no remaining capacity, transfer to player
            if (this.resources[targetResource] - this.capacity == 0)
            {
                this.resources[targetResource] = 0;
                Debug.Log("Calling Player resources index " + targetResource);
                Debug.Log("Player resources length: " + Main.playerResources.Length);
                Main.playerResources[targetResource] += this.capacity;
                planetpanel.updateResources();
            }

            //if the planet has available resources, transfer to craft
            targetPlanet.resources[targetResource]--;
            this.resources[targetResource]++;

            yield return new WaitForSeconds(waitTime);
        }

        //if the last of this resouce has been mined
        if (targetPlanet.resources[targetResource] == 0)
            planetpanel.updateResources();

        //transfer all resources from this craft to the player    
        Main.playerResources[targetResource] += this.resources[targetResource];
        this.resources[targetResource] = 0;      
    }
}
