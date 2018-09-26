/*
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
    public int targetResource = 3;

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
        if (targetPlanet.resources[targetResource] == 0)
        {
            this.resources[targetResource] = 0;
            Main.playerResources[targetResource] += this.capacity;
            planetpanel.updateResources();
            inUse = false;
        }

        //THIS METHOD CONTROLS MINING OF RESOUCES FROM PLANETS
        //PLANET -> CRAFT -> PLAYER

        while (targetPlanet.resources[targetResource] > 0 && inUse)
        {
            //if the craft has no remaining capacity, transfer to player
            if (this.resources[targetResource] - this.capacity == 0)
            {
                this.resources[targetResource] = 0;
                Main.playerResources[targetResource] += this.capacity;
                planetpanel.updateResources();
            }

            //if the planet has available resources, transfer to craft
            targetPlanet.resources[targetResource]--;
            this.resources[targetResource]++;

            //if the last of this resource has been mined
            if (targetPlanet.resources[targetResource] == 0)
                planetpanel.updateResources();

            yield return new WaitForSeconds(waitTime);
        }

        //transfer all resources from this craft to the player 
        Main.playerResources[targetResource] += this.resources[targetResource];
        this.resources[targetResource] = 0;
    }
}
