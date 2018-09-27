using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour {

    public bool extended = false;

    public Text[] resourceTexts= new Text[16];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updatePanel()
    {
        for (int i = 0; i < 16; i++)
            resourceTexts[i].text = Main.resourceName[i] + ": " + Main.playerResources[i];
    }

    public void movePanel()
    {
        //this method is used to collapse and expand the planet panel

        //if the panel is collapsed, move into the expanded position
        if (!extended)
        {
            //panelButton.transform.localEulerAngles = new Vector3(0, 0, 180);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(-107.4f, -3f);
            extended = true;
        }

        //if the panel is expanded, move back into the collapsed position
        else
        {
            //panelButton.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(-107.4f, -60f);
            extended = false;
        }

    }
}
