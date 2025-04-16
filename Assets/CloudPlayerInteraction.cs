using UnityEngine;

public class CloudPlayerInteraction : MonoBehaviour {
    [Header("消散效果设置")]
    public Transform player;                // 玩家位置
    public float influenceRadius = 50f;     // 影响半径
    public float clearRadius = 20f;         // 完全消散半径
    public float updateInterval = 0.2f;     // 更新间隔（秒）
    
    [Range(0, 1)]
    public float minDensity = 0f;           // 最小云密度
    
    private WeatherMap weatherMap;
    private Texture2D weatherTexture;
    private float timer;
    
    void Start() {
        weatherMap = GetComponent<WeatherMap>();
        
        // 如果没有指定玩家，尝试查找
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) {
                Debug.LogWarning("未找到玩家，请手动指定玩家Transform");
            }
        }
    }
    
    void Update() {
        if (player == null || weatherMap == null || weatherMap.weatherMap == null)
        {        
            Debug.Log("缺失player or weatherMap.weatherMap or weatherMap");
            return;
        } 
        // 按间隔更新天气图
        timer += Time.deltaTime;
        if (timer >= updateInterval) {
            timer = 0f;
            UpdateWeatherMapAroundPlayer();
        }
    }
    
    void UpdateWeatherMapAroundPlayer() 
    {
        // 获取当前的天气图 (RenderTexture)
        RenderTexture renderTexture = weatherMap.weatherMap;
        
        // 创建或重用 Texture2D
        if (weatherTexture == null || weatherTexture.width != renderTexture.width || 
            weatherTexture.height != renderTexture.height) {
            // 释放旧纹理
            if (weatherTexture != null) {
                Destroy(weatherTexture);
            }
            
            // 创建新纹理
            weatherTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);
        }
        
        // 保存当前激活的渲染纹理
        RenderTexture previousActive = RenderTexture.active;
        
        // 设置当前渲染纹理为云层的渲染纹理
        RenderTexture.active = renderTexture;
        
        // 读取像素到 Texture2D
        weatherTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        weatherTexture.Apply();
        
        // 恢复之前的激活渲染纹理
        RenderTexture.active = previousActive;
        
        // 获取天气图尺寸
        int width = weatherTexture.width;
        int height = weatherTexture.height;
        // Debug.Log($"天气图尺寸: {width}x{height}");
        
        // 获取像素数据
        Color[] pixels = weatherTexture.GetPixels();
        bool modified = false;
        
        // 将玩家位置转换为天气图坐标
        Vector3 playerPos = player.position;
        Vector3 containerPos = weatherMap.container.position;
        Vector3 containerScale = weatherMap.container.localScale;
        
        // 计算天气图上玩家的位置 (改进的坐标转换)
        Vector2 playerPosOnMap = new Vector2(
            ((playerPos.x-450f - containerPos.x) / containerScale.x + 0.5f) * width,
            ((playerPos.z - containerPos.z) / containerScale.z + 0.5f) * height
        );
        Debug.Log($"玩家在天气图上的位置: {playerPosOnMap.x:F0}, {playerPosOnMap.y:F0}");
        
        // 影响半径（以像素为单位）
        float influenceRadiusPixels = influenceRadius / containerScale.x * width;
        float clearRadiusPixels = clearRadius / containerScale.x * width;
        
        // 修改玩家周围的像素
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // 计算当前像素与玩家在天气图上的距离
                float dist = Vector2.Distance(new Vector2(x, y), playerPosOnMap);
                
                // 如果在影响半径内
                if (dist < influenceRadiusPixels) {
                    int index = y * width + x;
                    
                    // 获取当前云密度（通常存储在R通道）
                    float currentDensity = pixels[index].r;
                    
                    // 根据距离计算新的云密度
                    float t = Mathf.Clamp01((dist - clearRadiusPixels) / 
                                        (influenceRadiusPixels - clearRadiusPixels));
                    float newDensity = Mathf.Lerp(minDensity, currentDensity, t);
                    
                    // 更新像素
                    pixels[index] = new Color(newDensity, pixels[index].g, pixels[index].b, pixels[index].a);
                    modified = true;
                }
            }
        }
        
        // 应用修改（如果有的话）
        if (modified) {
            // 更新 Texture2D
            weatherTexture.SetPixels(pixels);
            weatherTexture.Apply();
            
            // 创建临时 RenderTexture 来复制回原始纹理
            RenderTexture tempRT = RenderTexture.GetTemporary(width, height, 0, 
                renderTexture.graphicsFormat);
            
            // 把修改后的 Texture2D 复制到临时 RenderTexture
            Graphics.Blit(weatherTexture, tempRT);
            
            // 然后复制回原始 RenderTexture
            Graphics.Blit(tempRT, renderTexture);
            
            // 释放临时纹理
            RenderTexture.ReleaseTemporary(tempRT);
        }
        
        // 调试输出
        Debug.Log($"已更新天气图: 玩家位置({playerPosOnMap.x:F0}, {playerPosOnMap.y:F0}), 影响半径={influenceRadiusPixels:F0}像素");
    }

    void OnDrawGizmos()
    {
        if (player == null || weatherMap == null || weatherMap.container == null) return;
        
        // 显示影响半径
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, influenceRadius);
        
        // 显示完全消散半径
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, clearRadius);
        
        // 可视化天气图区域
        Gizmos.color = Color.cyan;
        Gizmos.matrix = weatherMap.container.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        
        // 计算玩家在天气图空间中的相对位置
        Vector3 containerPos = weatherMap.container.position;
        Vector3 containerScale = weatherMap.container.localScale;
        Vector3 normalizedPos = new Vector3(
            (player.position.x - containerPos.x) / containerScale.x + 0.5f,
            0.5f,
            (player.position.z - containerPos.z) / containerScale.z + 0.5f
        );
        
        // 计算天气图世界坐标对应位置
        Vector3 mappedWorldPos = new Vector3(
            containerPos.x - containerScale.x/2 + normalizedPos.x * containerScale.x,
            player.position.y,
            containerPos.z - containerScale.z/2 + normalizedPos.z * containerScale.z
        );
        
        // 显示映射后的位置
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawSphere(mappedWorldPos, 2f);
        
        // 绘制从玩家到映射位置的线
        Gizmos.color = Color.white;
        Gizmos.DrawLine(player.position, mappedWorldPos);
    }
}