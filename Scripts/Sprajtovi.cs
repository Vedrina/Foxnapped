using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprajtovi : MonoBehaviour
{
    public Sprite[] sprajtovi;
    int kolicinaSprajteva;



    public int kolicinaSprajt()
    {
        kolicinaSprajteva = sprajtovi.Length;
        return kolicinaSprajteva;
    }
}
