/*
    This class inherits Craft and contains no unique methods at this time
    but is useful for identifying the mothership on the map screen

    RICH MOORE
    April 23, 2016
*/

public class Mothership : Craft {

	void Start ()
    {

        base.Start();

        //set speed of mothership to 2, makes moving around the map tolerable
        speed = 2;
	
	}

	void Update () {

        base.Update();
	
	}
}
