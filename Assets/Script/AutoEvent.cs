using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoEvent : MonoBehaviour
{
    public UnityEvent OnStart;
    public float delay;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        OnStart?.Invoke();
    }

  
}
