using System.Collections.Generic;
using UnityEngine;

public class GridEffect : MonoBehaviour
{
    [Header("Target")]
    public SpriteRenderer target;              // 你的正方形SpriteRenderer
    public bool autoFindTargetOnSameObject = true;
    public List<Material> lineMaterials;

    [Header("Line Renderers (Top/Bottom/Left/Right)")]
    public LineRenderer top;
    public LineRenderer bottom;
    public LineRenderer left;
    public LineRenderer right;

    [Header("Shape")]
    [Min(2)] public int segmentsPerEdge = 12;  // 每条边分段数（越大越“手绘”但更耗）
    public float outlineOffset = 0.02f;        // 描边向外偏移
    public float lineWidth = 0.06f;            // 线宽（世界单位）

    [Header("Hand-drawn Jitter")]
    public float jitterAmplitude = 0.015f;     // 抖动幅度（世界单位）
    public float jitterFrequency = 1.8f;       // 噪声频率（越大越碎）
    public float jitterSpeed = 1.5f;           // 动态抖动速度（0=静态）
    public int seed = 12345;                   // 保持抖动“风格”一致

    [Header("Sorting")]
    public int sortingOrderOffset = 1;
    public string sortingLayerName = "";

    void OnEnable()
    {
        if (autoFindTargetOnSameObject && target == null)
            target = GetComponent<SpriteRenderer>();

        ApplyLineDefaults(top);
        ApplyLineDefaults(bottom);
        ApplyLineDefaults(left);
        ApplyLineDefaults(right);
        
        lineMaterials.Add(top.material);
        lineMaterials.Add(bottom.material);
        lineMaterials.Add(left.material);
        lineMaterials.Add(right.material);
    }

    void Update()
    {
        if (target == null) return;
        if(top == null || bottom == null || left == null || right == null) return;

        // 正方形在本地坐标下的四个角（基于sprite bounds）
        // 注意：target.bounds 是世界坐标；我们需要转到 target 的本地空间
        var b = target.sprite.bounds; // 这是 sprite local space bounds（以 sprite pivot 为原点）
        Vector2 min = b.min;
        Vector2 max = b.max;

        // 四角（local）
        Vector2 bl = new(min.x, min.y);
        Vector2 br = new(max.x, min.y);
        Vector2 tr = new(max.x, max.y);
        Vector2 tl = new(min.x, max.y);

        // 生成四条边（local space）
        BuildEdge(top,    tl, tr, Vector2.up);
        BuildEdge(bottom, bl, br, Vector2.down);
        BuildEdge(left,   bl, tl, Vector2.left);
        BuildEdge(right,  br, tr, Vector2.right);

        // 排序层跟随 target
        SyncSorting(top);
        SyncSorting(bottom);
        SyncSorting(left);
        SyncSorting(right);


        LineMove();
    }

    void ApplyLineDefaults(LineRenderer lr)
    {
        if (lr == null) return;
        lr.useWorldSpace = false;
        lr.loop = false;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.numCapVertices = 2;       // 线头稍微圆润
        lr.numCornerVertices = 2;    // 角稍微圆润
        if (lr.sharedMaterial == null)
            lr.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.black;
        lr.endColor = Color.black;
    }

    void SyncSorting(LineRenderer lr)
    {
        if (lr == null) return;

        // Unity 的 LineRenderer 有 sortingLayer/sortingOrder
        // 如果你想强制指定层名，可以填 sortingLayerName
        if (!string.IsNullOrEmpty(sortingLayerName))
            lr.sortingLayerName = sortingLayerName;
        else
            lr.sortingLayerID = target.sortingLayerID;

        lr.sortingOrder = target.sortingOrder + sortingOrderOffset;
    }

    void BuildEdge(LineRenderer lr, Vector2 a, Vector2 b, Vector2 outwardNormal)
    {
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        int points = Mathf.Max(2, segmentsPerEdge + 1);
        lr.positionCount = points;

        float tTime = Application.isPlaying ? Time.time : 0f;
        float baseTime = tTime * jitterSpeed;

        // 为了保持不同边抖动不完全同步，引入 seed 和边的哈希偏移
        int edgeHash = lr.GetInstanceID() ^ seed;

        for (int i = 0; i < points; i++)
        {
            float t = (float)i / (points - 1);
            Vector2 p = Vector2.Lerp(a, b, t);

            // 向外偏移，确保描边在 sprite 外侧
            p += outwardNormal * outlineOffset;

            // Perlin 噪声：让点沿 outwardNormal 抖动（手绘感）
            // 对于水平边，用 x 作为噪声输入；垂直边用 y
            float axis = Mathf.Abs(a.x - b.x) > Mathf.Abs(a.y - b.y) ? p.x : p.y;

            float n = Mathf.PerlinNoise(
                axis * jitterFrequency + (edgeHash & 1023) * 0.01f,
                baseTime + ((edgeHash >> 10) & 1023) * 0.01f
            );

            // 抖动中心化到 [-1,1]
            float jitter = (n * 2f - 1f) * jitterAmplitude;

            p += outwardNormal * jitter;

            lr.SetPosition(i, p);
        }
    }

    [SerializeField] private float speed = 1f;

    //计算让线条移动
    private void LineMove()
    {

        float delta = speed;

        //上
        var offset = lineMaterials[0].mainTextureOffset;
        offset.x += delta * Time.deltaTime;
        lineMaterials[0].mainTextureOffset = offset;

        //下
        offset = lineMaterials[1].mainTextureOffset;
        offset.x -= delta * Time.deltaTime;
        lineMaterials[1].mainTextureOffset = offset;

        //左
        offset = lineMaterials[2].mainTextureOffset;
        offset.x += delta * Time.deltaTime;
        lineMaterials[2].mainTextureOffset = offset;

        //右
        offset = lineMaterials[3].mainTextureOffset;
        offset.x -= delta * Time.deltaTime;
        lineMaterials[3].mainTextureOffset = offset;
    }

}
