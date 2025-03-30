using System;
using UnityEngine;
using Assets.Map;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;

public class DirectGenMap : MonoBehaviour
{
    private InputField _inputName;
    private Button _btnGen;
    // Use this for initialization
    private Font _dFont;
    private RawImage _image;
    private Text _mouseBiome;
    private Text _descLabel;
    private GameObject _showMap;
    private GameObject _showCamp;
    public Map globalMap;

    private GameObject _townPrefab;
    private GameObject _bush1Prefab;
    private GameObject _bush2Prefab;
    private GameObject _GoldVeinPrefab;
    private GameObject _BoulderPrefab;


    public string _name;

    
    void Start()
    {
        _inputName = transform.Find("inputName").GetComponent<InputField>();
        _btnGen = transform.Find("btnGen").GetComponent<Button>();
        _image = transform.Find("RawImage").GetComponent<RawImage>();
        _mouseBiome = transform.Find("MouseBiome").GetComponent<Text>();
        _descLabel = transform.Find("DescLabel").GetComponent<Text>();
        _dFont = _inputName.textComponent.font;

        _townPrefab = GameObject.Find("Town");
        _bush1Prefab = GameObject.Find("Bush1");
        _bush2Prefab = GameObject.Find("Bush2");
        _GoldVeinPrefab = GameObject.Find("GoldVein");
        _BoulderPrefab = GameObject.Find("Boulder");

        //_btnGen.onClick.AddListener(GenMap);

        _showMap = GameObject.Find("Map");
        _showCamp = GameObject.Find("CampMap");

        GenMap();
        
    }

    private static Texture2D _txtTexture;
    private const int Width = 600;
    private const int Height = 400;
    private int _pointNum = 5000;
    private static bool _isLake = true;


    const int TextureScale = 20;
    public NoisyEdges noisyEdge;
    public int indexOfTexture = 0;

    void Toggle1(bool check)
    {
        if (check)
            _pointNum = 1000;
    }
    void Toggle2(bool check)
    {
        if (check)
            _pointNum = 2000;
    }
    void Toggle3(bool check)
    {
        if (check)
            _pointNum = 3000;
    }
    void Toggle4(bool check)
    {
        if (check)
            _pointNum = 5000;
    }
    void ToggleLand(bool check)
    {
        if (check)
            _isLake = false;
    }
    void ToggleLake(bool check)
    {
        if (check)
            _isLake = true;
    }
    private void GenMap()
    {
        //获取地图名字并作为随机种子, 地图名字位于Map物体下的MapName子物体下的Text组件，废弃inputname的使用
        _name = _showMap.transform.Find("MapName").GetComponent<Text>().text;
        Random.InitState(_name.GetHashCode());
        Random.InitState((int)DateTime.Now.Ticks);

        _txtTexture = GetTextTexture();

        Map.Width = Width;
        Map.Height = Height;

        Map map = new Map();
        map.SetPointNum(_pointNum);
        map.Init(CheckIsland());

        indexOfTexture = 0;
        //north is 0, JinZhou is 1, JiangDong is 2
        if (_name == "North")
        {
            indexOfTexture = 0;
        }
        else if (_name == "JinZhou")
        {
            indexOfTexture = 1;
        }
        else if (_name == "JiangDong")
        {
            indexOfTexture = 2;
        }
        else
        {
            indexOfTexture = 0;
        }

        map = AdjustMapToSanGuo(map);

        new MapTexture(TextureScale).FastAttachTexture(_showMap, map, indexOfTexture);
        new MapTexture(TextureScale).FastAttachCampTexture(_showCamp, map, indexOfTexture);

        globalMap = map;



        //将_showCamp显示在比_showMap更高的图层上，并将其透明度调整为0.5
        _showCamp.transform.position = _showMap.transform.position;
        _showCamp.transform.rotation = _showMap.transform.rotation;
        _showCamp.transform.localScale = _showMap.transform.localScale;
        _showCamp.SetActive(true);
        _showCamp.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");

        _showCamp.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);

        //在center上生成城镇
        foreach (Center p in map.Graph.centers)
        {
            if (p.water || p.ocean)
                continue;
            
            float f = Random.value;
            if (f > 0.97)
            {
                p.property = 1;
                GameObject town = Instantiate(_townPrefab);
                town.transform.position = new Vector3(p.point.x/4 - 75, p.point.y/4 - 50, 0);
                //将物体挂载到_showMap上
                town.transform.parent = _showMap.transform;
            }
            else if (f > 0.96)
            {
                p.property = 2;
                GameObject bush1 = Instantiate(_bush1Prefab);
                bush1.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                bush1.transform.parent = _showMap.transform;
            }
            else if (f > 0.95)
            {
                p.property = 3;
                GameObject bush2 = Instantiate(_bush2Prefab);
                bush2.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                bush2.transform.parent = _showMap.transform;
            }
            else if (f > 0.94)
            {
                p.property = 4;
                GameObject boulder = Instantiate(_BoulderPrefab);
                boulder.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                boulder.transform.parent = _showMap.transform;
            }
            else if (f > 0.93)
            {
                p.property = 5;
                GameObject goldVein = Instantiate(_GoldVeinPrefab);
                goldVein.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                goldVein.transform.parent = _showMap.transform;
            }
        }
    }

    private Map AdjustMapToSanGuo(Map map)
    {
        float p1Range = Width/5;
        float p2Range = Width/8;
        float p3Range = Width/7;
        Vector2 p1 = new Vector2(Width*2 / 5, Height);
        Vector2 p2 = new Vector2(0, Height / 2);
        Vector2 p3 = new Vector2(Width, Height * 3 / 4);

        //由p1, p2, p3 向外遍历非海洋节点，并将遍历到的节点设置为海洋
        foreach (Center c in map.Graph.centers)
        {
            if (c.water || c.ocean)
                continue;
            if (Vector2.Distance(c.point, p1) < p1Range || Vector2.Distance(c.point, p2) < p2Range || Vector2.Distance(c.point, p3) < p3Range)
            {
                c.water = true;
                c.ocean = true;
                c.biome = Biome.Ocean;
            }
        }

        // l1: (0, Height/4) -> (Width/5, 0) -> (0, 0)
        // l2: (Width*4/5, 0) -> (Width, Height/4) -> (Width, 0)
        // 将在两条直线以下的节点设置为海洋
        foreach (Center c in map.Graph.centers)
        {
            if (c.water || c.ocean)
                continue;
            if (IsPointInTriangle(c.point, new Vector2(0, Height / 4), new Vector2(Width / 3, 0), new Vector2(0, 0)) ||
                IsPointInTriangle(c.point, new Vector2(Width  / 2, 0), new Vector2(Width, Height / 4), new Vector2(Width, 0)))
            {
                c.water = true;
                c.ocean = true;
                c.biome = Biome.Ocean;
            }
        }

        // 将湖泊设置为陆地
        foreach (Center c in map.Graph.centers)
        {
            if (c.water && !c.ocean)
            {
                c.water = false;
                c.biome = Biome.Ocean;
            }
        }

        // 在坐标大于Height/2的地区随机选择一个地块，设为主城，将周边地区camp设为0
        foreach (Center c in map.Graph.centers)
        {
            if (c.water || c.ocean)
                continue;
            List<Center> town = new List<Center>();
            if (c.point.y > Height / 2 )
            {
                if (c.property == -1)
                {
                    town.Add(c);
                }
            }
        }
        Center mainTown0 = map.Graph.centers[Random.Range(0, map.Graph.centers.Count)];
        while (mainTown0.water || mainTown0.ocean || mainTown0.property != -1)
        {
            mainTown0 = map.Graph.centers[Random.Range(0, map.Graph.centers.Count)];
        }
        mainTown0.property = 1;
        mainTown0.camp = 0;
        GameObject town0 = Instantiate(_townPrefab);
        town0.transform.position = new Vector3(mainTown0.point.x / 4 - 75, mainTown0.point.y / 4 - 50, 0);
        town0.transform.parent = _showMap.transform;
        TextMesh textMesh0 = town0.transform.Find("name").GetComponent<TextMesh>();
        if (textMesh0 != null) {
            textMesh0.text = "Main Town 0";
        }


        HashSet<Center> visited = new HashSet<Center>();
        TraverseNeighbors(mainTown0, 5, visited);
        foreach (Center c in visited)
        {
            c.camp = 0;
        }

        // 在竖坐标小于Height/2 且横坐标小于Width/2 的地区随机选择一个地块，设为主城，将周边地区camp设为1
        foreach (Center c in map.Graph.centers)
        {
            if (c.water || c.ocean)
                continue;
            List<Center> town = new List<Center>();
            if (c.point.y < Height / 2 && c.point.x < Width / 2)
            {
                if (c.property == -1)
                {
                    town.Add(c);
                }
            }
        }
        Center mainTown1 = map.Graph.centers[Random.Range(0, map.Graph.centers.Count)];
        while (mainTown1.water || mainTown1.ocean || mainTown1.property != -1)
        {
            mainTown1 = map.Graph.centers[Random.Range(0, map.Graph.centers.Count)];
        }
        mainTown1.property = 1;
        mainTown1.camp = 1;
        GameObject town1 = Instantiate(_townPrefab);
        town1.transform.position = new Vector3(mainTown1.point.x / 4 - 75, mainTown1.point.y / 4 - 50, 0);
        town1.transform.parent = _showMap.transform;
        TextMesh textMesh1 = town1.transform.Find("name").GetComponent<TextMesh>();
        if (textMesh1 != null)
        {
            textMesh1.text = "Main Town 1";
        }


        visited = new HashSet<Center>();
        TraverseNeighbors(mainTown1, 5, visited);
        foreach (Center c in visited)
        {
            c.camp = 1;
        }

        // 在竖坐标小于Height/2 且横坐标大于Width/2 的地区随机选择一个地块，设为主城，将周边地区camp设为2
        foreach (Center c in map.Graph.centers)
        {
            if (c.water || c.ocean)
                continue;
            List<Center> town = new List<Center>();
            if (c.point.y < Height / 2 && c.point.x > Width / 2)
            {
                if (c.property == -1)
                {
                    town.Add(c);
                }
            }
        }
        Center mainTown2 = map.Graph.centers[Random.Range(0, map.Graph.centers.Count)];
        while (mainTown2.water || mainTown2.ocean || mainTown2.property != -1)
        {
            mainTown2 = map.Graph.centers[Random.Range(0, map.Graph.centers.Count)];
        }
        mainTown2.property = 1;
        mainTown2.camp = 2;
        GameObject town2 = Instantiate(_townPrefab);
        town2.transform.position = new Vector3(mainTown2.point.x / 4 - 75, mainTown2.point.y / 4 - 50, 0);
        town2.transform.parent = _showMap.transform;
        TextMesh textMesh2 = town2.transform.Find("name").GetComponent<TextMesh>();
        if (textMesh2 != null)
        {
            textMesh2.text = "Main Town 2";
        }


        visited = new HashSet<Center>();
        TraverseNeighbors(mainTown2, 5, visited);
        foreach (Center c in visited)
        {
            c.camp = 2;
        }
        return map;
    }

    private void TraverseNeighbors(Center center, int depth, HashSet<Center> visited)
    {
        if (depth == 0 || visited.Contains(center))
            return;

        visited.Add(center);

        foreach (Center neighbor in center.neighbors)
        {
            TraverseNeighbors(neighbor, depth - 1, visited);
        }
    }

    bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float denom = (b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y);
        float alpha = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / denom;
        float beta = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / denom;
        float gamma = 1.0f - alpha - beta;

        return (alpha >= 0 && alpha <= 1) && (beta >= 0 && beta <= 1) && (gamma >= 0 && gamma <= 1);
    }

    public static System.Func<Vector2, bool> CheckIsland()
    {
        System.Func<Vector2, bool> inside = q =>
        {
            int x = Convert.ToInt32(q.x / Width * _txtWidth);
            int y = Convert.ToInt32(q.y / Height * _txtHeight);
            Color tColor = _txtTexture.GetPixel(x, y);  
            bool isLand = false;
            if (_isLake)
                isLand = tColor != Color.white;
            else
                isLand = tColor == Color.white;
            return isLand;
        };
        return inside;
    }

    private static int _txtWidth = 400;
    private static int _txtHeight = 200;
    private Texture2D GetTextTexture()
    {
        Texture2D output = new Texture2D(_txtWidth, _txtHeight);
        RenderTexture renderTexture = new RenderTexture(_txtWidth, _txtHeight, 24);
        RenderTexture.active = renderTexture;
        GameObject tempObject = new GameObject("Temporary");
        tempObject.transform.position = new Vector3(0.5f, 0.5f, 0);
        Camera myCamera = Camera.main;
        myCamera.orthographic = true;
        myCamera.orthographicSize = 100;
        myCamera.targetTexture = renderTexture;
        Text gText = tempObject.AddComponent<Text>();
        gText.text = _inputName.text;
        //gText.anchor = TextAnchor.MiddleCenter;
        //gText.alignment = TextAlignment.Center;
        gText.alignment = TextAnchor.MiddleCenter;
        gText.font = _dFont;
        gText.fontSize = gText.text.Length <= 3 ? 125 : 100;

        _showMap.SetActive(false);
        myCamera.Render();
        _showMap.SetActive(true);

        output.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        output.Apply();
        RenderTexture.active = null;

        //_image.texture = renderTexture;
        myCamera.targetTexture = null;
        myCamera.orthographic = false;
        myCamera.Render();
        myCamera.orthographic = true;

        Destroy(tempObject);

        return output;
    }
    
    public int getTextureScale()
    {
        return TextureScale;
    }

}
