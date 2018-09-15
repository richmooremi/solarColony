/*
    This class inherits Craft and contains the Income() method
    which is used to calculate money accrued by this craft. 

    RICH MOORE
    April 23, 2016
*/

using UnityEngine;
using System.Collections;

public class Factory : Craft{

    private int waitTime = 2;   //how frequently the casino updates its income, default 2 seconds
    private int income = 5;     //the amount of money the factory creates, default 1

    void Start()
    {

        base.Start();

    }

    IEnumerator Income(int waitTime)
    {
        //this method gives $5 to the player every 2 seconds
        //and remains static over the course of the game

        while (true)
        {
            //wait for the alotted time
            yield return new WaitForSeconds(waitTime);

            //add income to player Resources
            Main.playerResources[2] += income;
        }
    }

    void Update()
    {
        base.Update();

        //start the income routine if the factory is just reaching its target location
        if (!startedCollecting && reachedTarget)
        {
            startedCollecting = true;
            StartCoroutine(Income(waitTime));
        }

    }
}
