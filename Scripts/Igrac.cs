using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Igrac : MonoBehaviour
{
    GameObject obj;
    Image Icon;
    Sprajt sprajt;
    Sprajtovi sprajetevi;
    //public bool item;
    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKey(KeyCode.F))
        {

            for (int i = 0; i < sprajetevi.sprajtovi.Length; i++)

                Icon.enabled = true;
                Destroy(other.gameObject);
        }
    }
}
