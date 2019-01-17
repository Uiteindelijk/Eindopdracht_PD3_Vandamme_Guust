using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{

    private void Update()
    {
        if(Input.GetButtonDown("RestartButton"))
        {
            Restart();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //om ons level te herstarten

        Time.timeScale = 1f;                        //voor als het spel gepauzeerd was dat de tijd teru doorloopt

        Cursor.lockState = CursorLockMode.Locked;   //om onze muis vast te zetten
        Cursor.visible = false;                     //om onze muis onzichtbaar te maken

    }
}
