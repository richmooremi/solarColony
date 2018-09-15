/*
    This class is used to draw planets' orbital lines
    to be displayed on the map screen

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;
using System.Collections;

public class LineDrawer : MonoBehaviour
{
    //variables used to set up the line
    public int segments;
    public float xradius;
    public float yradius;
    LineRenderer line;

    void Start()
    {
        //create the line
        line = gameObject.GetComponent<LineRenderer>();

        //set the line to world space so that it is centered on
        //the sun and add points to create line segments
        line.SetVertexCount(segments + 1);
        line.useWorldSpace = true;
        CreatePoints();
    }

    void CreatePoints()
    {
        //this method is called in Start() and creates the lines to be displayed.

        //position variables
        float x;
        float y = 0f;
        float z;
        float angle = 20f;

        //combine line segments to create a circular line
        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }
}