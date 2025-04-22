using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[ExecuteInEditMode]
public class Liquid : MonoBehaviour
{
    /// <summary>
    /// 参考：https://www.patreon.com/posts/18245226
    /// </summary>
    public enum UpdateMode { Normal, UnscaledTime }
    public UpdateMode updateMode;
 
    [SerializeField]
    float MaxWobble = 0.03f;
    [SerializeField]
    float WobbleSpeedMove = 1f;
    [SerializeField]
    [Range(0, 1)]
    float fillAmount = 0.5f;
    [SerializeField]
    float Recovery = 1f;
    [SerializeField]
    float Thickness = 1f;
    [Range(0, 1)]
    public float CompensateShapeAmount;
    [SerializeField]
    Mesh mesh;
    [SerializeField]
    Renderer rend;
    Vector3 pos;
    Vector3 lastPos;
    Vector3 velocity;
    Quaternion lastRot;
    Vector3 angularVelocity;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float sinewave;
    float time = 0.5f;
    Vector3 comp;
    private float modelHeight;
    private float bottomY;
    private float topY;
    // Use this for initialization
    void Start()
    {
        GetMeshAndRend();
        CalculateModelDimensions();
        //计算容器Mesh的高度

    }
    void CalculateModelDimensions()
    {
        if (mesh == null) return;
        
        // 获取模型的边界框
        Bounds bounds = mesh.bounds;
        
        // 在本地坐标系中计算
        float localMinY = bounds.min.y;
        float localMaxY = bounds.max.y;
        
        // 转换为世界坐标(相对于模型原点)
        Vector3 worldMin = transform.TransformPoint(new Vector3(0, localMinY, 0));
        Vector3 worldMax = transform.TransformPoint(new Vector3(0, localMaxY, 0));
        
        // 计算高度和位置
        bottomY = worldMin.y - transform.position.y;
        topY = worldMax.y - transform.position.y;
        modelHeight = topY - bottomY;
        
        // 将计算结果传递给着色器
        rend.sharedMaterial.SetFloat("_ModelHeight", modelHeight);
        rend.sharedMaterial.SetFloat("_TopY", topY);
        
        Debug.Log($"模型高度: {modelHeight}, 底部Y: {bottomY}, 顶部Y: {topY}");
    }

    private void OnValidate()
    {
        GetMeshAndRend();
    }
 
    void GetMeshAndRend()
    {
        //得到Mesh和Renderer组件
        if (mesh == null)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
        }
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
    }
    void Update()
    {
        float deltaTime = 0;
        switch (updateMode)
        {
            case UpdateMode.Normal:
                deltaTime = Time.deltaTime;
                break;
 
            case UpdateMode.UnscaledTime:
                deltaTime = Time.unscaledDeltaTime;
                break;
        }
 
        time += deltaTime;
 
        if (deltaTime != 0)
        {
 
            // decrease wobble over time
            //随着时间的推移，减少波动
            wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, (deltaTime * Recovery));
            wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, (deltaTime * Recovery));
 
 
 
            // make a sine wave of the decreasing wobble
            //波动沿着正弦波下降
            pulse = 2 * Mathf.PI * WobbleSpeedMove;
            sinewave = Mathf.Lerp(sinewave, Mathf.Sin(pulse * time), deltaTime * Mathf.Clamp(velocity.magnitude + angularVelocity.magnitude, Thickness, 10));
 
            wobbleAmountX = wobbleAmountToAddX * sinewave;
            wobbleAmountZ = wobbleAmountToAddZ * sinewave;
 
 
 
            // velocity
            velocity = (lastPos - transform.position) / deltaTime;
 
            angularVelocity = GetAngularVelocity(lastRot, transform.rotation);
 
            // add clamped velocity to wobble
            wobbleAmountToAddX += Mathf.Clamp((velocity.x + (velocity.y * 0.2f) + angularVelocity.z + angularVelocity.y) * MaxWobble, -MaxWobble, MaxWobble);
            wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (velocity.y * 0.2f) + angularVelocity.x + angularVelocity.y) * MaxWobble, -MaxWobble, MaxWobble);
        }
        
        // send it to the shader
        rend.sharedMaterial.SetFloat("_WobbleX", wobbleAmountX);
        rend.sharedMaterial.SetFloat("_WobbleZ", wobbleAmountZ);
 
        // set fill amount
        UpdatePos(deltaTime);
 
        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation;
    }
 
    /// <summary>
    /// 更新位置
    /// </summary>
    /// <param name="deltaTime"></param>
    void UpdatePos(float deltaTime)
    {
        //得到几何体形状的重心
        Vector3 worldPos = transform.TransformPoint(new Vector3(mesh.bounds.center.x, mesh.bounds.center.y, mesh.bounds.center.z));
        if (CompensateShapeAmount > 0)
        {
            // only lerp if not paused/normal update
            if (deltaTime != 0)
            {
                comp = Vector3.Lerp(comp, (worldPos - new Vector3(0, GetLowestPoint(), 0)), deltaTime * 10);
            }
            else
            {
                comp = (worldPos - new Vector3(0, GetLowestPoint(), 0));
            }
 
            pos = worldPos - transform.position - new Vector3(0, fillAmount - (comp.y * CompensateShapeAmount), 0);
        }
        else
        {
            pos = worldPos - transform.position - new Vector3(0, fillAmount, 0);
        }
        rend.sharedMaterial.SetVector("_FillAmount", pos);
    }
 
    //https://forum.unity.com/threads/manually-calculate-angular-velocity-of-gameobject.289462/#post-4302796
    Vector3 GetAngularVelocity(Quaternion foreLastFrameRotation, Quaternion lastFrameRotation)
    {
        var q = lastFrameRotation * Quaternion.Inverse(foreLastFrameRotation);
        // no rotation?
        // You may want to increase this closer to 1 if you want to handle very small rotations.
        // Beware, if it is too close to one your answer will be Nan
        if (Mathf.Abs(q.w) > 1023.5f / 1024.0f)
            return Vector3.zero;
        float gain;
        // handle negatives, we could just flip it but this is faster
        if (q.w < 0.0f)
        {
            var angle = Mathf.Acos(-q.w);
            gain = -2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
        }
        else
        {
            var angle = Mathf.Acos(q.w);
            gain = 2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
        }
        Vector3 angularVelocity = new Vector3(q.x * gain, q.y * gain, q.z * gain);
 
        if (float.IsNaN(angularVelocity.z))
        {
            angularVelocity = Vector3.zero;
        }
        return angularVelocity;
    }
 
    float GetLowestPoint()
    {
        float lowestY = float.MaxValue;
        Vector3 lowestVert = Vector3.zero;
        Vector3[] vertices = mesh.vertices;
 
        for (int i = 0; i < vertices.Length; i++)
        {
 
            Vector3 position = transform.TransformPoint(vertices[i]);
 
            if (position.y < lowestY)
            {
                lowestY = position.y;
                lowestVert = position;
            }
        }
        return lowestVert.y;
    }

    float CalculateBottomHeight()
    {
        // 根据Mesh计算出最低点和最高点的高度差
        if (mesh == null)
        {
            Debug.LogWarning("无法计算高度：网格引用为空");
            return 0f;
        }
        
        float lowestY = float.MaxValue;
        float highestY = float.MinValue;
        Vector3[] vertices = mesh.vertices;
        
        // 遍历所有顶点找出最高和最低点
        for (int i = 0; i < vertices.Length; i++)
        {
            // 将本地坐标转换为世界坐标
            Vector3 worldPosition = transform.TransformPoint(vertices[i]);
            
            // 更新最低点
            if (worldPosition.y < lowestY)
            {
                lowestY = worldPosition.y;
            }
            
            // 更新最高点
            if (worldPosition.y > highestY)
            {
                highestY = worldPosition.y;
            }
        }
        
        // 计算高度差
        float heightDifference = highestY - lowestY;
        
        // 确保结果有效
        if (heightDifference < 0 || float.IsNaN(heightDifference))
        {
            Debug.LogWarning("计算的高度差无效：" + heightDifference);
            return 0f;
        }
        
        return heightDifference;
    }
    void OnDrawGizmos()
    {
        Vector3 worldPos = transform.TransformPoint(new Vector3(mesh.bounds.center.x, mesh.bounds.center.y, mesh.bounds.center.z));
        //绘制world坐标系的xyz轴
        Gizmos.color = Color.red; // X轴为红色
        Gizmos.DrawLine(worldPos, worldPos + transform.right * 2f); // X轴长度为2
        Gizmos.color = Color.green; // Y轴为绿色
        Gizmos.DrawLine(worldPos, worldPos + transform.up * 2f); // Y轴长度为2
        Gizmos.color = Color.blue; // Z轴为蓝色
        Gizmos.DrawLine(worldPos, worldPos + transform.forward * 2f); // Z轴长度为2
    }
}