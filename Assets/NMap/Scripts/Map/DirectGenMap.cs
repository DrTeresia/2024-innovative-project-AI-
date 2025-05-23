using System;
using UnityEngine;
using Assets.Map;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;
using TMPro;

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

    public GameObject[] blockList; // 0 is weiMainTown, 1 is shuMainTown, 2 is wuMainTown
    public List<Center> mainTownCenterList = new List<Center>();
    public List<Sprite> earthTextures = new List<Sprite>();

    public string _name;

    
    void Start()
    {
        _inputName = transform.Find("inputName").GetComponent<InputField>();
        _btnGen = transform.Find("btnGen").GetComponent<Button>();
        _image = transform.Find("RawImage").GetComponent<RawImage>();
        _mouseBiome = transform.Find("MouseBiome").GetComponent<Text>();
        _descLabel = transform.Find("DescLabel").GetComponent<Text>();
        _dFont = _inputName.textComponent.font;

        _showMap = GameObject.Find("Map");
        _showCamp = GameObject.Find("CampMap");

        GenMap();
        
    }

    private static Texture2D _txtTexture;
    private const int Width = 600;
    private const int Height = 400;
    private int _pointNum = 5000;
    private static bool _isLake = false;

    const int TextureScale = 20;
    public NoisyEdges noisyEdge;

    public int indexOfTexture = 0; // 依据地理环境选择不同的纹理

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
    private void GenMap()
    {
        //  ȡ  ͼ   ֲ   Ϊ       ,   ͼ    λ  Map     µ MapName       µ Text         inputname  ʹ  
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

        _showCamp.transform.position = _showMap.transform.position;
        _showCamp.transform.rotation = _showMap.transform.rotation;
        _showCamp.transform.localScale = _showMap.transform.localScale;
        _showCamp.SetActive(true);
        _showCamp.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");

        _showCamp.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
    }

    private Map AdjustMapToSanGuo(Map map)
    {
        Texture2D texture = GetTextureFromImage("地图");
        // 遍历地图上的每个点，根据纹理判断是陆地还是海洋
        foreach (Center c in map.Graph.centers)
        {
            if (c.water || c.ocean)
                continue;
            int x = (int)(c.point.x * texture.width / Width);
            int y = (int)(c.point.y * texture.height / Height);
            Color pixelColor = texture.GetPixel(x, y);

            if (pixelColor == Color.black)
            {
                c.water = false;
                c.ocean = true;
                c.biome = Biome.Ocean;
            }
        }

        // 从MainTownLocation中获取主要城镇的坐标
        Dictionary<string, Vector2> weiMainTown = MainTownLocation.weiMainTown;
        Dictionary<string, Vector2> shuMainTown = MainTownLocation.shuMainTown;
        Dictionary<string, Vector2> wuMainTown = MainTownLocation.wuMainTown;

        Vector2 offset = new Vector2(-10.0f, -48.0f);

        // 遍历主要城镇的坐标
        foreach (var town in weiMainTown)
        {
            string townName = town.Key;
            Vector2 townPosition = town.Value;
            // 在地图上找到与主要城镇坐标最接近的点
            Center closestCenter = null;
            float closestDistance = float.MaxValue;
            foreach (Center center in map.Graph.centers)
            {
                if (center.water || center.ocean)
                    continue;
                float distance = Vector2.Distance(center.point, townPosition + offset);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCenter = center;
                }
            }
            // 将该点设置为主要城镇
            if (closestCenter != null)
            {
                closestCenter.property = 0;
                closestCenter.camp = 0;
                mainTownCenterList.Add(closestCenter);
                GameObject newTown = Instantiate(blockList[0], new Vector3(closestCenter.point.x / 4 - 75, closestCenter.point.y / 4 - 50, 0), Quaternion.identity);
                GameObject BuildingSet = GameObject.Find("BuildingSet");
                newTown.transform.SetParent(BuildingSet.transform);
                newTown.name = town.Key;
                // 子Canvas/Name下的TextMeshPro展示城池名字
                GameObject nameText = newTown.transform.Find("Canvas/Name").gameObject;
                TextMeshPro textMeshPro = nameText.GetComponent<TextMeshPro>();
                textMeshPro.text = townName;
            }
        }
        foreach (var town in shuMainTown)
        {
            string townName = town.Key;
            Vector2 townPosition = town.Value;
            // 在地图上找到与主要城镇坐标最接近的点
            Center closestCenter = null;
            float closestDistance = float.MaxValue;
            foreach (Center center in map.Graph.centers)
            {
                if (center.water || center.ocean)
                    continue;
                float distance = Vector2.Distance(center.point, townPosition + offset);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCenter = center;
                }
            }
            // 将该点设置为主要城镇
            if (closestCenter != null)
            {
                closestCenter.property = 0;
                closestCenter.camp = 1;
                mainTownCenterList.Add(closestCenter);
                GameObject newTown = Instantiate(blockList[1], new Vector3(closestCenter.point.x / 4 - 75, closestCenter.point.y / 4 - 50, 0), Quaternion.identity);
                GameObject BuildingSet = GameObject.Find("BuildingSet");
                newTown.transform.SetParent(BuildingSet.transform);
                newTown.name = town.Key;
                // 子Canvas/Name下的TextMeshPro展示城池名字
                GameObject nameText = newTown.transform.Find("Canvas/Name").gameObject;
                TextMeshPro textMeshPro = nameText.GetComponent<TextMeshPro>();
                textMeshPro.text = townName;

            }
        }
        foreach (var town in wuMainTown)
        {
            string townName = town.Key;
            Vector2 townPosition = town.Value;
            // 在地图上找到与主要城镇坐标最接近的点
            Center closestCenter = null;
            float closestDistance = float.MaxValue;
            foreach (Center center in map.Graph.centers)
            {
                if (center.water || center.ocean)
                    continue;
                float distance = Vector2.Distance(center.point , townPosition + offset);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCenter = center;
                }
            }
            // 将该点设置为主要城镇
            if (closestCenter != null)
            {
                closestCenter.property = 0;
                closestCenter.camp = 2;
                mainTownCenterList.Add(closestCenter);
                GameObject newTown = Instantiate(blockList[2], new Vector3(closestCenter.point.x / 4 - 75, closestCenter.point.y / 4 - 50, 0), Quaternion.identity);
                GameObject BuildingSet = GameObject.Find("BuildingSet");
                newTown.transform.SetParent(BuildingSet.transform);
                newTown.name = town.Key;
                // 子Canvas/Name下的TextMeshPro展示城池名字
                GameObject nameText = newTown.transform.Find("Canvas/Name").gameObject;
                TextMeshPro textMeshPro = nameText.GetComponent<TextMeshPro>();
                textMeshPro.text = townName;
            }
        }

        //// 生成城镇
        //foreach (Center center in mainTownCenterList)
        //{
        //    // 生成城镇
        //    GameObject town = Instantiate(blockList[0], new Vector3(center.point.x/4 - 75, center.point.y/4 - 50, 0), Quaternion.identity);
        //    // Set BuildingSet as parent
        //    GameObject BuildingSet = GameObject.Find("BuildingSet");
        //    town.transform.SetParent(BuildingSet.transform);
        //    town.name = "MainTown";

        //}


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
            return true;
        };
        return inside;
    }

    private static int _txtWidth = 400;
    private static int _txtHeight = 200;

    private Texture2D GetTextureFromImage(string imagePath)
    {
        Texture2D texture = Resources.Load<Texture2D>(imagePath);
        if (texture == null)
        {
            Debug.LogError("Image not found: " + imagePath);
        }
        return texture;
    }
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
