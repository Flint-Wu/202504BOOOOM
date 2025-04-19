using UnityEngine;
using UnityEngine.UI;

public class MadeID : MonoBehaviour
{
    public CodeTrans TransTool;
    public InputField IdInput;

    public string IDCount, ID, PushID;

    private string[] sign = { "`", "~", "!", "@", "#"};

    public void TransID()
    {
        ID = IdInput.text;
        IDCount = ID.Length.ToString();

        int randomIndex = Random.Range(0, sign.Length);
        string randomElement = sign[randomIndex];

        PushID = TransTool.StrTransCode(IDCount) + randomElement + TransTool.StrTransCode(ID);
    }
}
