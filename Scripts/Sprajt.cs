using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sprajt : MonoBehaviour
{
    public Sprite ikonica;
    Objekt objekt;
    Image slika;
    Sprajtovi sprajetevi;

    public void addItem()
    {
        for (int i = 0; i < sprajetevi.sprajtovi.Length; i++)
        {
            if (gameObject.name == sprajetevi.sprajtovi[i].ToString())
            {
                Debug.Log("gameObject.name == objekt.naziv_objekta "+" Odgovara naziv sprajta s sprajtom");
            }
        }
    }

}
