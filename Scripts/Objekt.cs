using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objekt : MonoBehaviour
{
    GameObject objekt;
    public Sprite sprajt;
    string naziv_objekta;

    void Start()
    {
        naziviObjekata();
    }
    
    private string naziviObjekata()
    {
        naziv_objekta = this.gameObject.tag.ToString();
        gameObject.name = sprajt.name;
        Debug.Log("gleda objekt i tagove, naziv_objekta:"+ naziv_objekta+" naziv sprajta:" +" "+ gameObject.name);
        return naziv_objekta;
    }
}
