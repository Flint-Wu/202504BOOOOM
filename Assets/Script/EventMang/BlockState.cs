using System.Linq;
using UnityEngine;

public class BlockState : MonoBehaviour
{
    public ActRecord ActRecord;
    public string[] GivenValues;
    public string NowState;
    public int tmp;

    void Start()
    {
        ActRecord = GetComponent<ActRecord>();
    }

    // Update is called once per frame
    void Update()
    {
        GivenValues = ActRecord.givenValues;

        if(GivenValues.Length > 0)
        {

            tmp = GetMaxNumber(GivenValues);//获取GivenValus组中所有数字的最大数值
        }

        ActRecord.LocState = tmp.ToString();//使actrecord中的状态数值等同于作弊码中最大的数值
    }

    static int GetMaxNumber(string[] numbers)//获取string组中所有数字的最大数值方法
    {
        return numbers.Select(int.Parse).Max();
    }
}
