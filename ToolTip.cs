/*
    This class controls the location, visibility,
    and text which apperas on the tooltip when
    the player hovers the cursor over a button.

    RICH MOORE
    May 2, 2016
*/

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {

    //general local variables
    RectTransform rt;                                       //the Rect Transform of the tooltip, used to determing the width and positioning
    public Text tipTitle, tipText;                          //the tooltip title and text, used with the messages dictionary
    public GameObject costPanel;                            //the cost panel located below the tooltip, disabled when on the Build Button
    public Text moneyCost, ironCost, hydrogenCost;          //the cost to perform the selected action, shown in the cost panel

    //the default position of the tooltip, used to hide it offscreen
    Vector2 defaultPosition = new Vector2 (-1000, -1000);

    bool tipActive = false;         //true when the mouse is over a button
    bool tipfollowing = false;      //true when the tooltip is being displayed

    //key: tooltip name / value: tooltip text
    public Dictionary<string, string> messages = new Dictionary<string, string>();

    void Start ()
    {
        //populate the possible tooltip messages
        setMessages();

        //hide object offscreen
        this.transform.position = defaultPosition;

        //get the rect transform of this object
        rt = this.GetComponent<RectTransform>();
    }
	

	void Update ()
    {
        //if the tooltip is being shown, have it follow the mouse
        if (tipfollowing)
            FollowMouse();

        //if the tooltip is not being shown, hide it offscreen
        else
            this.transform.position = defaultPosition;
    }

    public void setCosts(int m, int h, int i)
    {
        //this method is called in Main() and used to
        //set the resource costs of the current action

        moneyCost.text = "Money: " + m;
        hydrogenCost.text = "Hydrogen " + h;
        ironCost.text = "Iron: " + i;
     }

    public void FollowMouse()
    {
        //this method defines the position of the tooltip
        //while it is being shown onscreen

        //get current mouse position
        Vector2 mousePos = Input.mousePosition;

        //clamp the x and y mouse position to prevent tooltip from overflowing the screen
        mousePos.y = Mathf.Clamp(mousePos.y, 0, 510);
        mousePos.x = Mathf.Clamp(mousePos.x, 0, 300);

        //as close as I can get to the correct formula
        this.transform.position = new Vector2(mousePos.x + (rt.sizeDelta.x / 4), mousePos.y + (rt.sizeDelta.y / 4));
    }

    #region getsAndsets
    public void setStatus(bool boolIn)
    {
        tipActive = boolIn;
    }

    public void setFollow(bool boolIn)
    {
        tipfollowing = boolIn;
    }

    public bool getStatus()
    {
        return tipActive;
    }

    public void setMessage(string inString)
    {
        tipTitle.text = inString;
        tipText.text = messages[inString];
    }
    #endregion

    private void setMessages()
    {
        //this method is called in Start() and sets all of the possible
        //tooltip name and text combinations

        messages.Add("Mining\nRobot",               "Launch a robotic mining craft to collect resources from this planet. Click for further options.");
        messages.Add("Exploration\nSatellite",      "Launch a satellite to collect data about this planet. The first step to gathering resouces.");
        messages.Add("Building\nOptions",           "Launch a commercial structure to orbit this planet and generate revenue. Click for further options.");
        messages.Add("Syphoning\nRobot",            "Launch a robot to syphon hydrogen from this planet's atmosphere, which can be used as fuel.");             //on any non venus planet, no space
        messages.Add("Syphoning\nRobot ",           "Launch a robot to syphon sulfur from this planet's atmosphere, which has many industrial uses.");          //on Venus, there is a space
        messages.Add("Factory",                     "An inexpensive manufacturing facility which provides a slow, but steady income.");
        messages.Add("Resort",                      "A resort will attract many clients when first built, but income will taper off with age.");
        messages.Add("Movie\nStudio",               "A studio will provide to be a good, but somewhat unpredictable source of income.");

    }
}
