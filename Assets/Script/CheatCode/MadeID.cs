using TMPro;
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
        if(IdInput!= null)
        {
            ID = IdInput.text;
        }
        else
        {
            Debug.LogWarning("IdInput is null or empty. Please assign a value.");
        }
        
        IDCount = ID.Length.ToString();

        int randomIndex = Random.Range(0, sign.Length);
        string randomElement = sign[randomIndex];

        PushID = TransTool.StrTransCode(IDCount) + randomElement + TransTool.StrTransCode(ID);
    }

    void Update()
    {
        if(IdInput != null && !string.IsNullOrEmpty(IdInput.text))
        {
            ID = IdInput.text;
        }
    }

    void Awake()
    {
        //找到名字为头像＆ID的物体
        GameObject obj = GameObject.Find("头像＆ID");
        if (obj != null)
        {
            //获取物体上的Image组件
            obj.GetComponentInChildren<TextMeshProUGUI>().text = ID;
        }
        else
        {
            Debug.LogWarning("头像＆ID object not found in the scene,无法赋值ID");
        }
    }
}
