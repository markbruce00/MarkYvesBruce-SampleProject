using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float delay;

    void OnEnable() {
        StartCoroutine(StartEvent());
    }
    IEnumerator StartEvent() { 
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }
}
