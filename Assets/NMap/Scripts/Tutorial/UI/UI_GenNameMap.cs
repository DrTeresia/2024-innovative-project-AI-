using System;
using UnityEngine;
using Assets.Map;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Globalization;
using System.Security.Cryptography;


public class UI_GenNameMap : MonoBehaviour
{
    public enum campType
    {
        None,
        Wei,
        Shu,
        Wu
    }
    private InputField _inputName;
    private Button _btnGen;
    // Use this for initialization
    private Font _dFont;
    private RawImage _image;
    private Text _mouseBiome;
    private Text _descLabel;
    private GameObject _showMap;
    private GameObject _showCampDivision;

    [SerializeField]
    private GameObject mainTownPrefab;
    [SerializeField]
    private GameObject townPrefab;
    [SerializeField]
    private GameObject forestPrefab;
    [SerializeField]
    private GameObject minePrefab;

    private string _name;
    void Start()
    {
        _inputName = transform.Find("inputName").GetComponent<InputField>();
        _btnGen = transform.Find("btnGen").GetComponent<Button>();
        _image = transform.Find("RawImage").GetComponent<RawImage>();
        _mouseBiome = transform.Find("MouseBiome").GetComponent<Text>();
        _descLabel = transform.Find("DescLabel").GetComponent<Text>();
        _dFont = _inputName.textComponent.font;

        _btnGen.onClick.AddListener(GenMap);

        _showMap = GameObject.Find("Map");
        _showCampDivision = GameObject.Find("CampDivision");
        //隐藏descLabel与mouseBiome
        transform.Find("DescLabel").gameObject.SetActive(false);
        transform.Find("MouseBiome").gameObject.SetActive(false);

        transform.Find("Toggles1/Toggle1").GetComponent<Toggle>().onValueChanged.AddListener(Toggle1);
        transform.Find("Toggles1/Toggle2").GetComponent<Toggle>().onValueChanged.AddListener(Toggle2);
        transform.Find("Toggles1/Toggle3").GetComponent<Toggle>().onValueChanged.AddListener(Toggle3);
        transform.Find("Toggles1/Toggle4").GetComponent<Toggle>().onValueChanged.AddListener(Toggle4);

        transform.Find("Toggles2/Toggle1").GetComponent<Toggle>().onValueChanged.AddListener(ToggleLand);
        transform.Find("Toggles2/Toggle2").GetComponent<Toggle>().onValueChanged.AddListener(ToggleLake);
    }

    void Update()
    {
        CheckMouseBiome();
    }

    private float _nextCheckTime;
    private void CheckMouseBiome()
    {
        if (Time.time < _nextCheckTime)
            return;
        _nextCheckTime = Time.time + 0.1f;

        Vector2 pos = Input.mousePosition; // Mouse position
        RaycastHit hit;
        Camera _cam = Camera.main; // Camera to use for raycasting
        Ray ray = _cam.ScreenPointToRay(pos);
        Physics.Raycast(_cam.transform.position, ray.direction, out hit, 10000.0f);
        Color c;
        if (hit.collider)
        {
            Texture2D tex = (Texture2D)hit.collider.gameObject.GetComponent<Renderer>().material.mainTexture; // Get texture of object under mouse pointer
            if (tex)
            {
                c = tex.GetPixelBilinear(hit.textureCoord2.x, hit.textureCoord2.y); // Get color from texture

                Biome b = ChangeColorToBiome(c);
                _mouseBiome.text = BiomeProperties.Chinese[b];
            }
        }
    }

    private Biome ChangeColorToBiome(Color color)
    {
        Biome b = Biome.Ocean;
        foreach (var bc in BiomeProperties.Colors)
        {
            if (ColorNearby(bc.Value, color))
            {
                b = bc.Key;
                break;
            }
        }
        return b;
    }

    bool ColorNearby(Color ls, Color rs)
    {
        bool rSame = Mathf.Abs(ls.r - rs.r) < 0.02f;
        bool gSame = Mathf.Abs(ls.g - rs.g) < 0.02f;
        bool bSame = Mathf.Abs(ls.b - rs.b) < 0.02f;
        return rSame && gSame && bSame;
    }

    private static Texture2D _txtTexture;
    const int TextureScale = 20;
    private const int Width = 300;
    private const int Height = 200;
    private int _pointNum = 1000;
    private static bool _isLake = true;
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
        //获取地图名字并作为随机种子
        _name = _inputName.text;
        Random.InitState(_name.GetHashCode());
        //Random.InitState((int)DateTime.Now.Ticks);

        _txtTexture = GetTextTexture();

        Map.Width = Width;
        Map.Height = Height;

        Map map = new Map();
        map.SetPointNum(_pointNum);
        map.Init(CheckIsland());
        //扰乱边缘
        NoisyEdges noisyEdge = new NoisyEdges();
        noisyEdge.BuildNoisyEdges(map);

        new MapTexture(TextureScale).AttachTexture(_showMap, map, noisyEdge);


        //隐藏地图设置按钮
        transform.Find("Toggles1").gameObject.SetActive(false);
        transform.Find("Toggles2").gameObject.SetActive(false);
        transform.Find("btnGen").gameObject.SetActive(false);
        transform.Find("inputName").gameObject.SetActive(false);

        //将descLabel与mouseBiome激活
        transform.Find("DescLabel").gameObject.SetActive(true);
        transform.Find("MouseBiome").gameObject.SetActive(true);

        //随机生成Main Town预制体
        GenMainTown();
        //生成城镇
        GenTown();
        //生成森林
        GenForest();
        //生成矿山
        GenMine();
    }

    private void GenCampDivision()
    {
        //将地图分成三个阵营
        int width = _txtWidth;
        int height = _txtHeight;
        int[] campDivision = new int[3];
        campDivision[0] = Random.Range(0, width);
        campDivision[1] = Random.Range(0, width);
        campDivision[2] = Random.Range(0, width);
        Array.Sort(campDivision);
        //生成阵营分界线
        Texture2D texture = new Texture2D(width, height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color color = Color.white;
                if (i == campDivision[0] || i == campDivision[1] || i == campDivision[2])
                    color = Color.black;
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();
        _showCampDivision.GetComponent<RawImage>().texture = texture;

    }

    private void GenMainTown()
    {
        //总共三个MainTown，分别位于三个阵营的中心
        Debug.Log(mainTownPrefab);
        GameObject mainTown1 = Instantiate(mainTownPrefab);
        GameObject mainTown2 = Instantiate(mainTownPrefab);
        GameObject mainTown3 = Instantiate(mainTownPrefab);
        //地图的范围为（-75， 40）到（75， -60），在这个范围内随机生成MainTown，避免生成在海洋上
        //三个主城的分布为上、左下、右下
        mainTown1.transform.position = new Vector3(Random.Range(-10, 10), Random.Range(20, 30), 0);
        mainTown2.transform.position = new Vector3(Random.Range(-70, -50), Random.Range(-40, -20), 0);
        mainTown3.transform.position = new Vector3(Random.Range(50, 70), Random.Range(0, 20), 0);
    }

    private void GenTown()
    {
        //城镇数量随机，分布在地图上
        int townNum = Random.Range(10, 20);
        for (int i = 0; i < townNum; i++)
        {
            GameObject town = Instantiate(townPrefab);
            town.transform.position = new Vector3(Random.Range(-75, 75), Random.Range(-60, 40), 0);
        }

    }

    private void GenMine()
    {
        int mineNum = Random.Range(10, 20);
        //按照正态分布生成矿山
        for (int i = 0; i < mineNum; i++)
        {
            GameObject mine = Instantiate(minePrefab);
            mine.transform.position = new Vector3(Random.Range(-20, 20), Random.Range(-10, 10), 0);
        }
    }
    private void GenForest()
    {
        int forestNum = Random.Range(10, 20);
        for (int i = 0; i < forestNum; i++)
        {
            GameObject forest = Instantiate(forestPrefab);
            forest.transform.position = new Vector3(Random.Range(-65, 65), Random.Range(-50, 30), 0);
            //如果森林生成在地图中心则重新生成
            while (forest.transform.position.x > -40 && forest.transform.position.x < 40 && forest.transform.position.y > -20 && forest.transform.position.y < 0)
            {
                forest.transform.position = new Vector3(Random.Range(-65, 65), Random.Range(-50, 30), 0);
            }
        }
    }
    public static System.Func<Vector2, bool> CheckIsland()
    {
        System.Func<Vector2, bool> inside = q =>
        {
            int x = Convert.ToInt32(q.x / Width * _txtWidth);
            int y = Convert.ToInt32(q.y / Height * _txtHeight);
            Color tColor = _txtTexture.GetPixel(x,y);
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
        tempObject.transform.position = new Vector3(0.5f,0.5f,0);
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
        gText.fontSize = gText.text.Length <=3?125: 100;

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

        Destroy(tempObject);

        return output;
    }
}
