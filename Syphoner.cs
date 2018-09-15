﻿/*
    This class inherits Craft and contains Syphon() method
    which is used to calculate iron income from the craft

    RICH MOORE
    March 5, 2016
*/

using UnityEngine;
using System.Collections;

public class Syphoner : Craft {

    //resources contained by the craft used with Main.resourceNames[]  
    public int[] resources = new int[4];
    private int targetResource = 3;

    //statistics of the miner craft
    private int capacity = 5;                       //carrying capacity of the craft
    private float syphonSpeed = .25f;               //syphoning speed variable, expressed in 

    void Start()
    {

        base.Start();

        //if venus is selected, change resource to sulfur and add achivement
        if (Main.currentPlanetIndex == 2)
        {
            targetResource = 1;
            Main.syphonedVenus = true;
        }
    }

    void Update()
    {
        base.Update();

        //start the syphoning routine if the craft is just reaching its target location
        if (!startedCollecting && reachedTarget)
        {
            startedCollecting = true;
            StartCoroutine(Syphon(syphonSpeed));
        }

    }

    IEnumerator Syphon(float waitTime)
    {
        //THIS METHOD CONTROLS MINING OF RESOUCES FROM PLANETS
        //PLANET -> CRAFT -> PLAYER

        while (targetPlanet.resources[targetResource] > 0)
        {
            //if the craft has no remaining capacity, transfer to player
            if (this.resources[targetResource] - this.capacity == 0)
            {
                this.resources[targetResource] = 0;
                Main.playerResources[targetResource] += this.capacity;
            }

            //if the planet has available resources, transfer to craft
            targetPlanet.resources[targetResource]--;
            this.resources[targetResource]++;

            yield return new WaitForSeconds(waitTime);
        }

        //transfer all resources from this craft to the player 
        Main.playerResources[targetResource] += this.resources[targetResource];
        this.resources[targetResource] = 0;
    }
}