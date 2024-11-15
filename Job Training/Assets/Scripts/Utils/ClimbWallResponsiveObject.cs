using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable] public class FloatEvent : UnityEvent<float> { };

public class ClimbWallResponsiveObject : MonoBehaviour
{
    public UnityEvent responseToGrasp;

    [SerializeField]public FloatEvent responseToTractionMeasure;

    public void TractionIdentified()
    {
        responseToGrasp?.Invoke();
    }

    public void TractionIdentified(float strenght)
    {
        responseToTractionMeasure?.Invoke(strenght);
    }
}
