using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marketplace : MonoBehaviour
{
    float[] resouceScalar = new float[4];

    Main main;

    bool extended = false;              //true when the panel is on the screen
    public Button panelButton;          //button object used to collapse and expand the planet panel
    public Dropdown dropdown;
    public Text buyText, sellText;

    int currentResource = 0;            //the resource currently selected in the dropdown (default to 1)


    // Use this for initialization
    void Start ()
    {
        main = GameObject.FindObjectOfType<Main>();

        resouceScalar[0] = 50;
        resouceScalar[1] = 100;
        resouceScalar[3] = 150;

        setPrices();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void sellResource(int q)
    {
        if (Main.playerResources[currentResource] >= q)
        {
            Main.playerResources[currentResource] -= q;                                       //remove the item from the player's inventory
            Main.playerResources[2] += (int)Mathf.Floor(q * resouceScalar[currentResource]);  //add money to the player's inventory
            resouceScalar[currentResource] -= (float)(resouceScalar[currentResource] * .01);                //modify the resource's current value 
            setPrices();
        }

        else
            main.sendMiniMessage("You don't have " + q + " " + Main.resourceName[currentResource]);
    }

    public void buyResouces(int q)
    {
        if (Main.playerResources[2] > resouceScalar[currentResource] * q)
        {
            Main.playerResources[2] -= (int)Mathf.Floor(q * resouceScalar[currentResource]);
            Main.playerResources[currentResource] += q;
            resouceScalar[currentResource] += (float)(resouceScalar[currentResource] * .05);
            setPrices();
        }

        else
            main.sendMiniMessage("You don't have money to buy " + q + " " + Main.resourceName[currentResource]);
    }

    public void movePanel()
    {
        //this method is used to collapse and expand the planet panel

        //if the panel is collapsed, move into the expanded position
        if (!extended)
        {
            panelButton.transform.localEulerAngles = new Vector3(0, 0, 180);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(-104.1f, 107.7f);
            extended = true;
        }

        //if the panel is expanded, move back into the collapsed position
        else
        {
            panelButton.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(-104.1f, 131f);
            extended = false;
        }

    }

    public void setCurrentResource(int i)
    {
        int r = dropdown.value;

        //account for the fact that I stupidly made money resource 2
        if (r >= 2)
            currentResource = r + 1;

        else
            currentResource = r;

        setPrices();
    }

    private void setPrices()
    {
        buyText.text = "Buy: $" + Mathf.Floor(resouceScalar[currentResource]);
        sellText.text = "Sell: $" + Mathf.Floor(resouceScalar[currentResource]);
    }
}
