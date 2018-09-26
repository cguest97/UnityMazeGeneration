using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class endGameScript : MonoBehaviour {

    public Text gameWinText;

    private void Start()
    {
        gameWinText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "End") {
            gameWinText.text = "Congratulations! Press R to generate a new map!";
            Time.timeScale = 0;
        }
    }
}
