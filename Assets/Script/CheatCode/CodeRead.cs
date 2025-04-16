using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus.Input;
using UnityEngine;
using UnityEngine.UI;

public class CodeRead : MonoBehaviour
{
    public InputField[] inputFields;
    public string[] codes;
    public InputField PlayerFields;
    public string PlayerName;

    void Awake()
    {
        codes = new string[inputFields.Length];
    }

    public void GetCode()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            codes[i] = inputFields[i].text;
        }
    }

    public void GetName()
    {
        PlayerName = PlayerFields.text;
    }
}
