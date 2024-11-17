using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float delay;
    IEnumerator Start() { 
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }
}
