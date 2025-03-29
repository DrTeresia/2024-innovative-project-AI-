using System;
using UnityEngine;
using Assets.Map;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Globalization;
using System.Security.Cryptography;

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
    private int _pointNum = 500;
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
        //Random.InitState((int)DateTime.Now.Ticks);

        _txtTexture = GetTextTexture();

        Map.Width = Width;
        Map.Height = Height;

        Map map = new Map();
        map.SetPointNum(_pointNum);
        map.Init(CheckIsland());
        //扰乱边缘
        //noisyEdge = new NoisyEdges();
        //noisyEdge.BuildNoisyEdges(map);

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

        //new MapTexture(TextureScale).AttachTexture(_showMap, map, noisyEdge, indexOfTexture);
        //new MapTexture(TextureScale).AttachCampTexture(_showCamp, map, noisyEdge, indexOfTexture);

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
            // 生成概率，大于0.8则生成城镇
            float f = Random.value;
            if (f > 0.95)
            {
                p.property = 1;
                GameObject town = Instantiate(_townPrefab);
                town.transform.position = new Vector3(p.point.x/4 - 75, p.point.y/4 - 50, 0);
                //将物体挂载到_showMap上
                town.transform.parent = _showMap.transform;
            }
            else if (f > 0.925)
            {
                p.property = 2;
                GameObject bush1 = Instantiate(_bush1Prefab);
                bush1.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                bush1.transform.parent = _showMap.transform;
            }
            else if (f > 0.90)
            {
                p.property = 3;
                GameObject bush2 = Instantiate(_bush2Prefab);
                bush2.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                bush2.transform.parent = _showMap.transform;
            }
            else if (f > 0.875)
            {
                p.property = 4;
                GameObject boulder = Instantiate(_BoulderPrefab);
                boulder.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                boulder.transform.parent = _showMap.transform;
            }
            else if (f > 0.85)
            {
                p.property = 5;
                GameObject goldVein = Instantiate(_GoldVeinPrefab);
                goldVein.transform.position = new Vector3(p.point.x / 4 - 75, p.point.y / 4 - 50, 0);
                goldVein.transform.parent = _showMap.transform;
            }
        }
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
