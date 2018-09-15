/*
    This class inherits Craft and contains the Income() method
    which is used to calculate money accrued by this craft.

    RICH MOORE
    April 23, 2016
*/

using UnityEngine;
using System.Collections;

public class Studio : Craft
{
    
    private int waitTime = 30;    //how frequently the studio updates its income, default 30 seconds
    private int income;           //income based on random range

    void Start()
    {

        base.Start();

    }

    void Update()
    {
        base.Update();

        //start the income routine if the studio is just reaching its target location
        if (!startedCollecting && reachedTarget)
        {
            startedCollecting = true;
            StartCoroutine(Income(waitTime));
        }
    }

    IEnumerator Income(int waitTime)
    {
        //this method gives money to the player every 30 seconds,
        //with a random range, skewed toward a lower value

        while (true)
        {
            //wait for the alotted time
            yield return new WaitForSeconds(waitTime);

            //generate income within random range
            income = Random.Range(300, 650);

            //skew income to low range
            if (income < 600)
                income -= 100;

            //add income to player Resources
            Main.playerResources[2] += income;
        }
    }
}
