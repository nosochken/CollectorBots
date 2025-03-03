using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public event Action FlagWasPutUp;
    
    public Vector3 FlagPosition {get; private set;}

    private void Start()
    {
        FlagPosition = Vector3.one;
    }
    

}
