using UnityEngine;
using UnityEngine.UI;

public class IDMade : MonoBehaviour
{
    public string ID, IDCountxt, PushID;
    public int IDCount;
    public CodeTrans TransTool;
    public Text IDInput;

    private string[] charstr = new string[] {"`","~","!","@","#"};

    void Start()
    {
        //ID = IDInput.text;
        ID = TransTool.inputtext;
        IDCount = ID.Length;
    }

    public void IDTrans()
    {


        ID = IDInput.text;
        IDCount = ID.Length;
        IDCountxt = TransTool.StrTransCode(IDCount.ToString());
        ID = TransTool.StrTransCode(ID);

        int randomint = Random.Range(0, charstr.Length);

        PushID = IDCountxt + charstr[randomint] + ID;
    }


}
