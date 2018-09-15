/*
    This class inherits Craft and contains methods to set
    the current planet as having been explored by the player

    RICH MOORE
    April 3, 2016
*/

using UnityEngine;
using System.Collections;

public class Explorer : Craft{

    bool alreadyExplored = false;   //true if this craft has already expored the target planet, but its flag hasn't been set yet

    void Start()
    {

        base.Start();

        cost = new int[] { 25, 0, 50, 100 };   //the cost to build the Explorer

        //add target planet to list of explored planets for this session,
        //prevents player from launching another explorer to the same planet
        Main.exploredPlanets.Add(targetPlanet.name);
    }

    void Update()
    {
        base.Update();

        //set the planet as having been explored when the target location is reached
        if (!alreadyExplored && reachedTarget)
        {
            //set the target planet as explored and update its available menu items
            alreadyExplored = true;
            targetPlanet.explored = true;
            targetPlanet.updateActions();

            //force Main to redraw the menu of the player is looking at this planet
            Main.forcedRedrawMenu = true;
            Main.forcedRedrawMenuPlanet = targetPlanet;
        }
    }


            
}
