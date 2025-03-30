using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Map
{
    public class MapTexture
    {
        private readonly int _textureScale;
        
        public MapTexture(int textureScale)
        {
            _textureScale = textureScale;
        }

        public Texture2D GetTexture(Map map, NoisyEdges noisyEdge, int indexOfTexture = 0)
        {
            int textureWidth = (int)Map.Width * _textureScale;
            int textureHeight = (int)Map.Height * _textureScale;

            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
            
            //绘制扰乱的边缘
            List<Vector2> periphery = new List<Vector2>();
            foreach (Center p in map.Graph.centers)
            {
                foreach (var r in p.neighbors)
                {
                    Edge edge = map.Graph.lookupEdgeFromCenter(p, r);
                    if (!noisyEdge.path0.ContainsKey(edge.index) || !noisyEdge.path1.ContainsKey(edge.index))
                    {
                        // It's at the edge of the map, where we don't have
                        // the noisy edges computed. TODO: figure out how to
                        // fill in these edges from the voronoi library.
                        continue;
                    }
                    
                    //绘制扰乱后的形状
                    //DrawNoisyPolygon(texture, p, noisyEdge.path0[edge.index], indexOfTexture);
                    //DrawNoisyPolygon(texture, p, noisyEdge.path1[edge.index], indexOfTexture);
                    periphery.Clear();
                    periphery.Add(edge.v0.point);
                    periphery.Add(edge.v1.point);
                    periphery.Add(p.point);
                    texture.FillPolygon(periphery.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(), BiomeProperties.Colors[p.biome]);

                }
            }
            //绘制边界线
            foreach (var line in map.Graph.edges.Where(p => !p.d0.water && !p.d1.water))
            {
                //绘制扰乱后的边缘
                List<Vector2> edge0 = noisyEdge.path0[line.index];
                for (int i = 0; i < edge0.Count - 1; i++)
                    DrawLine(texture, edge0[i].x, edge0[i].y, edge0[i + 1].x, edge0[i + 1].y, Color.black);
                List<Vector2> edge1 = noisyEdge.path1[line.index];
                for (int i = 0; i < edge1.Count - 1; i++)
                    DrawLine(texture, edge1[i].x, edge1[i].y, edge1[i + 1].x, edge1[i + 1].y, Color.black);
            }
            //绘制扰乱后的河流
            foreach (var line in map.Graph.edges.Where(p => p.river > 0 && !p.d0.water && !p.d1.water))
            {
                //绘制扰乱后的边缘
                List<Vector2> edge0 = noisyEdge.path0[line.index];
                for (int i = 0; i < edge0.Count - 1; i++)
                    DrawLine(texture, edge0[i].x, edge0[i].y, edge0[i + 1].x, edge0[i + 1].y, Color.blue);

                List<Vector2> edge1 = noisyEdge.path1[line.index];
                for (int i = 0; i < edge1.Count - 1; i++)
                    DrawLine(texture, edge1[i].x, edge1[i].y, edge1[i + 1].x, edge1[i + 1].y, Color.blue);
            }

            texture.Apply();

            return texture;
        }

        // 绘制不需扰乱的地块
        public Texture2D FastGetTexture(Map map, int indexOfTexture = 0)
        {
            int textureWidth = (int)Map.Width * _textureScale;
            int textureHeight = (int)Map.Height * _textureScale;
            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);

            // 绘制camp底色为ocean, 色号为44447a
            Color[] fillColorArray = texture.GetPixels();
            for (int i = 0; i < fillColorArray.Length; ++i)
            {
                fillColorArray[i] = BiomeProperties.Colors[Biome.Ocean];
            }
            texture.SetPixels(fillColorArray);

            List<Vector2> periphery = new List<Vector2>();
            foreach (Center p in map.Graph.centers)
            {
                foreach (var r in p.neighbors)
                {
                    Edge edge = map.Graph.lookupEdgeFromCenter(p, r);
                    if (edge == null || edge.v0 == null || edge.v1 == null || edge.d0 == null || edge.d1 == null)
                        continue;
                    periphery.Clear();
                    periphery.Add(edge.v0.point);
                    periphery.Add(edge.v1.point);
                    periphery.Add(p.point);
                    texture.FillPolygon(periphery.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(), BiomeProperties.Colors[p.biome]);
                }
            }
            texture.Apply();
            return texture;
        }

        public Texture2D GetCampTexture(Map map, NoisyEdges noisyEdge, int indexOfTexture = 0)
        {
            int textureWidth = (int)Map.Width * _textureScale;
            int textureHeight = (int)Map.Height * _textureScale;
            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
           
            List<Vector2> periphery = new List<Vector2>();
            foreach (Center p in map.Graph.centers)
            {
                foreach (var r in p.neighbors)
                {
                    Edge edge = map.Graph.lookupEdgeFromCenter(p, r);
                    if (!noisyEdge.path0.ContainsKey(edge.index) || !noisyEdge.path1.ContainsKey(edge.index))
                    {
                        // It's at the edge of the map, where we don't have
                        // the noisy edges computed. TODO: figure out how to
                        // fill in these edges from the voronoi library.
                        continue;
                    }
                    //绘制扰乱后的形状
                    periphery.Clear();
                    periphery.Add(edge.v0.point);
                    periphery.Add(edge.v1.point);
                    periphery.Add(p.point);
                    Color campColor = Camp.Colors[p.camp];
                    texture.FillPolygon(periphery.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(), campColor);

                }
            }
            //绘制边界线
            foreach (var line in map.Graph.edges.Where(p => !p.d0.water && !p.d1.water))
            {
                //绘制扰乱后的边缘
                List<Vector2> edge0 = noisyEdge.path0[line.index];
                for (int i = 0; i < edge0.Count - 1; i++)
                    DrawLine(texture, edge0[i].x, edge0[i].y, edge0[i + 1].x, edge0[i + 1].y, Color.black);
                List<Vector2> edge1 = noisyEdge.path1[line.index];
                for (int i = 0; i < edge1.Count - 1; i++)
                    DrawLine(texture, edge1[i].x, edge1[i].y, edge1[i + 1].x, edge1[i + 1].y, Color.black);
            }
            texture.Apply();
            return texture;
        }

        // 不需要计算扰乱边缘
        public Texture2D FastGetCampTexture(Map map, int indexOfTexture = 0)
        {
            int textureWidth = (int)Map.Width * _textureScale;
            int textureHeight = (int)Map.Height * _textureScale;
            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB565, true);
            
            List<Vector2> periphery = new List<Vector2>();
            foreach (Center p in map.Graph.centers)
            {
                foreach (var r in p.neighbors)
                {
                    Edge edge = map.Graph.lookupEdgeFromCenter(p, r);
                    if (edge == null || edge.v0 == null || edge.v1 == null || edge.d0 == null || edge.d1 == null)
                        continue;
                    periphery.Clear();
                    periphery.Add(edge.v0.point);
                    periphery.Add(edge.v1.point);
                    periphery.Add(p.point);
                    texture.FillPolygon(periphery.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(), Camp.Colors[p.camp]);
                }
            }
            // 绘制边界线
            foreach (var line in map.Graph.edges.Where(p => !p.d0.water && !p.d1.water))
            {
                DrawLine(texture, line.v0.point.x, line.v0.point.y, line.v1.point.x, line.v1.point.y, Color.black);
            }
            texture.Apply();
            return texture;
        }


        public void AttachTexture(GameObject plane, Map map, NoisyEdges noisyEdge, int indexOfTexture = 0)
        {
            Texture2D texture = GetTexture(map, noisyEdge, indexOfTexture);
            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }
        // 不需要计算扰乱边缘
        public void FastAttachTexture(GameObject plane, Map map, int indexOfTexture = 0)
        {
            Texture2D texture = FastGetTexture(map, indexOfTexture);
            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }
        public void AttachCampTexture(GameObject plane, Map map, NoisyEdges noisyEdge, int indexOfTexture = 0)
        {
            Texture2D texture = GetCampTexture(map, noisyEdge, indexOfTexture);
            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }
        //不需要计算扰乱边缘
        public void FastAttachCampTexture(GameObject plane, Map map, int indexOfTexture = 0)
        {
            Texture2D texture = FastGetCampTexture(map, indexOfTexture);
            plane.GetComponent<Renderer>().material.mainTexture = texture;
        }

        readonly List<Vector2> _edgePoints = new List<Vector2>();
        private void DrawNoisyPolygon(Texture2D texture, Center p, List<Vector2> orgEdges, int indexOfTexture = 0)
        {
            _edgePoints.Clear();
            _edgePoints.AddRange(orgEdges);
            _edgePoints.Add(p.point);

            if (indexOfTexture == 0)
                texture.FillPolygon(
                    _edgePoints.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(),
                    BiomeProperties.Colors[p.biome]);
            else if (indexOfTexture == 1)
                texture.FillPolygon(
                    _edgePoints.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(),
                    BiomeProperties.ColorsForJinZhou[p.biome]);
            else if (indexOfTexture == 2)
                texture.FillPolygon(
                    _edgePoints.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(),
                    BiomeProperties.ColorsForJiangdong[p.biome]);
            else
                texture.FillPolygon(
                _edgePoints.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(),
                BiomeProperties.Colors[p.biome]);
        }
        private void DrawCampPolygon(Texture2D texture, Center p, List<Vector2> orgEdges, int indexOfTexture = 0)
        {
            _edgePoints.Clear();
            _edgePoints.AddRange(orgEdges);
            _edgePoints.Add(p.point);

            texture.FillPolygon(
                _edgePoints.Select(x => new Vector2(x.x * _textureScale, x.y * _textureScale)).ToArray(),
                Camp.Colors[p.camp]);
        }

        private void DrawLine(Texture2D texture, float x0, float y0, float x1, float y1, Color color)
        {
            texture.DrawLine((int) (x0*_textureScale), (int) (y0*_textureScale), (int) (x1*_textureScale),
                (int) (y1*_textureScale), color);
        }

        
    }
}