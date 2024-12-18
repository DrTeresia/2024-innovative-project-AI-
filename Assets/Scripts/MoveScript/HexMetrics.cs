/*
这个代码文件 `HexMetrics.cs` 是一个包含多个静态变量和函数的类，它用于定义六角形地图的某些属性和提供一系列关于地图数据操作的静态方法。下面是这个代码的逐项解释：

### 常量定义

定义了一系列的常量，这些常量描述了六角形地图的各种尺寸和比例关系。例如 `outerRadius` 和 `innerRadius` 定义了六角形的外部和内部半径，`solidFactor` 和 `waterFactor` 定义了实体和水体的比例等。这些常量用于后续的计算和绘图操作。

### 噪声源纹理和噪声采样函数

定义了一个静态的 `Texture2D` 对象 `noiseSource` 作为噪声源，以及一个 `SampleNoise` 函数，该函数根据提供的三维位置从噪声源中采样。通常，这在地图生成或高度扰动中用于引入随机性。

### 哈希网格相关函数

定义了初始化哈希网格的函数 `InitializeHashGrid` 和一个采样哈希网格的函数 `SampleHashGrid`。哈希网格通常用于空间索引或快速查找特定位置的特定数据。这里的哈希网格可能与地图生成或某种空间数据的检索有关。

### 特征阈值函数

定义了获取特征阈值的函数 `GetFeatureThresholds`，特征阈值可能与地形分类或地貌特征有关。这里有三个不同级别的阈值数组，可能用于不同的地形或地貌分类。

### 六角形相关函数

定义了一系列与六角形相关的函数，包括获取角点、边的中点、不同类型角落（固体、水体、桥梁）的位置等。这些函数对于绘制和定位六角形地图上的实体和水体边界非常有用。还定义了一些与地形坡度相关的函数，如计算台阶之间的线性插值等。此外，还有一个 `Perturb` 函数用于根据噪声源扰动位置数据。这些函数在生成地形和渲染地图时非常有用。最后还定义了获取边缘类型的函数 `GetEdgeType`，该函数根据两个高程之间的差异来确定边缘的类型（平坦、斜坡还是悬崖）。总体而言，该类是用于六角形地图创建、处理和渲染的关键部分。
*/

﻿using UnityEngine;
public static class HexMetrics
{
	public const float outerToInner = 0.866025404f;
	public const float innerToOuter = 1f / outerToInner;
	public const float outerRadius = 10f;
	public const float innerRadius = outerRadius * outerToInner;
	public const float solidFactor = 0.8f;
	public const float blendFactor = 1f - solidFactor;
	public const float waterFactor = 0.6f;
	public const float waterBlendFactor = 1f - waterFactor;
	public const float elevationStep = 3f;
	public const int terracesPerSlope = 2;
	public const int terraceSteps = terracesPerSlope * 2 + 1;
	public const float horizontalTerraceStepSize = 1f / terraceSteps;
	public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
	public const float cellPerturbStrength = 4f;
	public const float elevationPerturbStrength = 1.5f;
	public const float streamBedElevationOffset = -1.75f;
	public const float waterElevationOffset = -0.5f;
	public const float wallHeight = 4f;
	public const float wallYOffset = -1f;
	public const float wallThickness = 0.75f;
	public const float wallElevationOffset = verticalTerraceStepSize;
	public const float wallTowerThreshold = 0.5f;
	public const float bridgeDesignLength = 7f;
	public const float noiseScale = 0.003f;
	public const int chunkSizeX = 5, chunkSizeZ = 5;
	public const int hashGridSize = 256;
	public const float hashGridScale = 0.25f;
	private static HexHash[] hashGrid;
	private static readonly Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius),
	};
	private static readonly float[][] featureThresholds = {
		new[] { 0.0f, 0.0f, 0.4f },
		new[] { 0.0f, 0.4f, 0.6f },
		new[] { 0.4f, 0.6f, 0.8f },
	};
	public static Texture2D noiseSource;
	public static Vector4 SampleNoise(Vector3 position) =>
		noiseSource.GetPixelBilinear(
			position.x * noiseScale,
			position.z * noiseScale
		);
	public static void InitializeHashGrid(int seed) {
		hashGrid = new HexHash[hashGridSize * hashGridSize];
		Random.State currentState = Random.state;
		Random.InitState(seed);
		for (int i = 0; i < hashGrid.Length; i++) {
			hashGrid[i] = HexHash.Create();
		}
		Random.state = currentState;
	}
	public static HexHash SampleHashGrid(Vector3 position) {
		int x = (int)(position.x * hashGridScale) % hashGridSize;
		if (x < 0) x += hashGridSize;
		int z = (int)(position.z * hashGridScale) % hashGridSize;
		if (z < 0) z += hashGridSize;
		return hashGrid[x + z * hashGridSize];
	}
	public static float[] GetFeatureThresholds(int level) => featureThresholds[level];
	public static Vector3 GetFirstCorner(HexDirection direction) => corners[(int)direction];
	public static Vector3 GetSecondCorner(HexDirection direction) => corners[(int)direction + 1];
	public static Vector3 GetFirstSolidCorner(HexDirection direction) => corners[(int)direction] * solidFactor;
	public static Vector3 GetSecondSolidCorner(HexDirection direction) => corners[(int)direction + 1] * solidFactor;
	public static Vector3 GetSolidEdgeMiddle(HexDirection direction) =>
		(corners[(int)direction] + corners[(int)direction + 1]) *
		(0.5f * solidFactor);
	public static Vector3 GetFirstWaterCorner(HexDirection direction) => corners[(int)direction] * waterFactor;
	public static Vector3 GetSecondWaterCorner(HexDirection direction) => corners[(int)direction + 1] * waterFactor;
	public static Vector3 GetBridge(HexDirection direction) =>
		(corners[(int)direction] + corners[(int)direction + 1]) *
		blendFactor;
	public static Vector3 GetWaterBridge(HexDirection direction) =>
		(corners[(int)direction] + corners[(int)direction + 1]) *
		waterBlendFactor;
	public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step) {
		float h = step * horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = (step + 1) / 2 * verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}
	public static Color TerraceLerp(Color a, Color b, int step) {
		float h = step * horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}
	public static Vector3 WallLerp(Vector3 near, Vector3 far) {
		near.x += (far.x - near.x) * 0.5f;
		near.z += (far.z - near.z) * 0.5f;
		float v =
			near.y < far.y ? wallElevationOffset : 1f - wallElevationOffset;
		near.y += (far.y - near.y) * v + wallYOffset;
		return near;
	}
	public static Vector3 WallThicknessOffset(Vector3 near, Vector3 far) {
		Vector3 offset;
		offset.x = far.x - near.x;
		offset.y = 0f;
		offset.z = far.z - near.z;
		return offset.normalized * (wallThickness * 0.5f);
	}
	public static HexEdgeType GetEdgeType(int elevation1, int elevation2) {
		if (elevation1 == elevation2) return HexEdgeType.Flat;
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1) return HexEdgeType.Slope;
		return HexEdgeType.Cliff;
	}
	public static Vector3 Perturb(Vector3 position) {
		Vector4 sample = SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
		return position;
	}
}