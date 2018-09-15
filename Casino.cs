/*
    This class inherits Craft and contains the Income() method
    which is used to calculate money accrued by this craft.

    RICH MOORE
    April 23, 2016
*/

using UnityEngine;
using System.Collections;

public class Casino : Craft {

    private int waitTime = 2;       //how frequently the casino updates its income, default 2 seconds
    private int lifeTime = 0;       //how long the casino has existed, used in income calculation

	void Start () {

        base.Start();

    }

    void Update()
    {

        base.Update();

        //start the income routine if the casino is just reaching its target location
        if (!startedCollecting && reachedTarget)
        {
            startedCollecting = true;
            StartCoroutine(Income(waitTime));
        }

    }

    IEnumerator Income(int waitTime)
    {
        //this method gives money to the player every two seconds,
        //decreasing the amount of money earned after 30 seconds
        //and again after 60 seconds

        while (true)
        {
            //wait for the alotted time
            yield return new WaitForSeconds(waitTime);

            //add waitTime to lifeTime, both measured in seconds
            lifeTime += waitTime;

            //income based on how long the casino has existed
            if (lifeTime < 30)
                Main.playerResources[2] += 25;

            else if (lifeTime < 60)
                Main.playerResources[2] += 10;

            else
                Main.playerResources[2] += 5;
        }
    }


}
