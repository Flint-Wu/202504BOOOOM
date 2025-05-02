using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class BottleText : MonoBehaviour
{
    public TranslationCheatCode CheatCode;
    public TextMeshProUGUI[] Bottles;
    List<TranslationCheatCode.PlayerLocMap> totalLocunNums;
    public List<string> LocunNums;


    private void Start()
    {        
        CheatCode = FindAnyObjectByType<TranslationCheatCode>();

        LocunNums = new List<string> ();

        totalLocunNums = CheatCode.TotalLocunNums;
        GiveName();
    }

    public void GiveName()
    {
        foreach (var item in totalLocunNums)
        {
            if (!LocunNums.Contains(item.PlayerID))
            {
                LocunNums.Add(item.PlayerID);
            }
        }

        for (int i = 0; i < LocunNums.Count(); i++)
        {
            Bottles[i].text = LocunNums[i];
        }

        if(LocunNums.Count() < Bottles.Count())
        {
            string[] Member = new string[] {"HajimaSian","Dtorm", "Enoch", "Fish" };
            List<string> Members = new List<string> ();

            for (int i = 0; i < Member.Length; i++)
            {
                Members.Add(Member[i]);
            }

            for (int i = LocunNums.Count(); i < Bottles.Count(); i++)
            {
                int randomInt =  Random.Range(0, Members.Count());

                Bottles[i].text = Members[randomInt];
                Members.RemoveAt(randomInt);
            }
        }

    }


}