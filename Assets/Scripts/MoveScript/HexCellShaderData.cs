/*
这是一个名为HexCellShaderData的类，主要用于处理一个六边形网格（HexGrid）的渲染数据。以下是关于这个类的功能解释：

1. `HexCellShaderData` 类具有多个属性和方法，它管理着HexGrid中六边形单元格的纹理数据和可见性状态。此脚本的目的是生成和管理一种特定的着色器数据，这种数据将在图形渲染中使用，展示如地形、可见性以及探索状态等信息。
2. 在类的定义部分中：


	* `HexGrid Grid`: 该属性表示正在管理的六边形网格。
	* `ImmediateMode`: 一个布尔值，表示是否立即更新单元格的可见性和探索状态。当该值为真时，它会立即更新相关单元格的纹理数据，否则会通过渐变过渡的方式来更新。
	* `transitionSpeed`: 定义过渡速度的常量，单位是每秒帧数（fps）。用于决定过渡更新的速度。
	* `transitioningCells`: 一个存储正在过渡的单元格的列表。
	* `cellTexture`: 用于存储单元格纹理数据的Texture2D对象。
	* `cellTextureData`: 存储每个单元格的颜色信息的数组。每个颜色通道（红、绿、蓝和透明度）都代表特定的信息（如地形类型索引、可见性和探索状态）。
	* `needsVisibilityReset`: 一个布尔值，表示是否需要重置可见性。当某些事件发生时（如视图高度变化），可能需要重置所有单元格的可见性状态。
3. 在事件函数部分中：


	* `LateUpdate`: 这个方法在每一帧的后期更新阶段被调用。它负责处理单元格的过渡更新，重置可见性以及更新和应用纹理数据。这是主要的逻辑处理部分。
	* `Initialize`: 这个方法初始化这个类的一些基础资源（如Texture2D对象）。它也负责设置着色器数据的全局纹理和尺寸。此方法需要输入x和z坐标作为参数，这些坐标用于初始化纹理的尺寸和内容。同时，它还负责设置全局纹理的尺寸和坐标比例。此外，它还负责初始化或重置cellTextureData数组以及设置类的启用状态。这个方法是在脚本启动时或者重置时需要调用的初始化方法。
	* `RefreshTerrain`: 这个方法根据输入的HexCell对象刷新地形类型索引信息。它更新cellTextureData数组中对应单元格的地形类型索引值，并设置类的启用状态为true。这意味着当需要更新地形数据时，需要调用这个方法。例如，地形类型发生改变时就会调用此方法更新地形数据。当多个单元格地形类型发生变化时，通常会有循环调用此方法来处理多个单元格的更新。需要注意的是此方法只更新地形信息并不改变其他属性（如可见性或探索状态）。如果需要对其他属性进行更新需要使用其他方法如RefreshVisibility等。此方法在需要更新地形数据时调用。
	* `RefreshVisibility`: 这个方法根据输入的HexCell对象刷新可见性和探索状态信息。它更新cellTextureData数组中对应单元格的可见性和探索状态颜色值，并设置类的启用状态为true。这个方法通常在视图角度改变或需要显示隐藏或者已探索状态的改变时调用。此方法会检查ImmediateMode的值来决定是否立即更新单元格的可见性和探索状态信息或者通过过渡的方式更新这些信息。此方法在需要更新可见性或探索状态时调用。需要注意的是此方法不会改变地形信息只是更新了单元格的可见性和探索状态信息如果需要改变地形信息需要使用RefreshTerrain方法调用此方法之前需要调用RefreshTerrain方法确保地形信息已经正确更新然后调用RefreshVisibility来更新可见性和探索状态信息以保持同步显示；`ViewElevationChanged`: 这个方法会设置一个标志表示需要重新计算可见性然后启用类这是因为在视图高度变化时可能影响到某些单元格的可见性因此需要重置可见性标记以便于后续更新计算新的可见性信息该方法在视图高度变化时调用用来通知系统需要重新计算可见性信息；`UpdateCellData`: 这个私有方法用于更新单个单元格的颜色数据它根据单元格的探索状态和可见性来更新对应的颜色值并返回是否还在更新的状态用于在LateUpdate方法中循环处理多个单元格的数据此方法在处理每个单元格的数据时会被调用以计算并更新对应的颜色值并根据过渡速度来逐渐改变颜色值的变化效果实现平滑过渡的效果；最后这个类的主要作用是管理六边形网格中的单元格数据包括地形类型索引、可见性和探索状态等信息通过处理这些数据来生成特定的着色器数据在图形渲染中使用从而展示所需的效果通过不同的事件触发相关的处理方法来处理这些数据的变化以便于在界面上正确地显示相应的信息和效果从而让用户能够看到和交互到六边形网格中的不同单元格的信息和状态信息以此来增强游戏的体验性和交互性为用户带来更好的游戏效果和内容展示效果是管理和处理游戏中地图渲染数据的关键类之一用于在游戏中展示地图信息和交互效果等重要的功能需求和数据管理任务之一
*/

﻿using System.Collections.Generic;
using UnityEngine;
public class HexCellShaderData : MonoBehaviour
{
	private const float transitionSpeed = 255f;
	private readonly List<HexCell> transitioningCells = new List<HexCell>();
	private Texture2D cellTexture;
	private Color32[] cellTextureData;
	private bool needsVisibilityReset;
	public HexGrid Grid { get; set; }
	public bool ImmediateMode { get; set; }
	#region Event Functions
	private void LateUpdate() {
		if (needsVisibilityReset) {
			needsVisibilityReset = false;
			Grid.ResetVisibility();
		}
		int delta = (int)(Time.deltaTime * transitionSpeed);
		if (delta == 0) delta = 1;
		for (int i = 0; i < transitioningCells.Count; i++) {
			if (!UpdateCellData(transitioningCells[i], delta)) {
				transitioningCells[i--] =
					transitioningCells[transitioningCells.Count - 1];
				transitioningCells.RemoveAt(transitioningCells.Count - 1);
			}
		}
		cellTexture.SetPixels32(cellTextureData);
		cellTexture.Apply();
		enabled = transitioningCells.Count > 0;
	}
	#endregion
	public void Initialize(int x, int z) {
		if (cellTexture) {
			cellTexture.Reinitialize(x, z);
		}
		else {
			cellTexture = new Texture2D(
				x, z, TextureFormat.RGBA32, false, true
			);
			cellTexture.filterMode = FilterMode.Point;
			cellTexture.wrapMode = TextureWrapMode.Clamp;
			Shader.SetGlobalTexture("_HexCellData", cellTexture);
		}
		Shader.SetGlobalVector(
			"_HexCellData_TexelSize",
			new Vector4(1f / x, 1f / z, x, z)
		);
		if (cellTextureData == null || cellTextureData.Length != x * z)
			cellTextureData = new Color32[x * z];
		else
			for (int i = 0; i < cellTextureData.Length; i++) {
				cellTextureData[i] = new Color32(0, 0, 0, 0);
			}
		transitioningCells.Clear();
		enabled = true;
	}
	public void RefreshTerrain(HexCell cell) {
		cellTextureData[cell.Index].a = (byte)cell.TerrainTypeIndex;
		enabled = true;
	}
	public void RefreshVisibility(HexCell cell) {
		int index = cell.Index;
		if (ImmediateMode) {
			cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
			cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
		}
		else if (cellTextureData[index].b != 255) {
			cellTextureData[index].b = 255;
			transitioningCells.Add(cell);
		}
		enabled = true;
	}
	public void ViewElevationChanged() {
		needsVisibilityReset = true;
		enabled = true;
	}
	private bool UpdateCellData(HexCell cell, int delta) {
		int index = cell.Index;
		Color32 data = cellTextureData[index];
		bool stillUpdating = false;
		if (cell.IsExplored && data.g < 255) {
			stillUpdating = true;
			int t = data.g + delta;
			data.g = t >= 255 ? (byte)255 : (byte)t;
		}
		if (cell.IsVisible) {
			if (data.r < 255) {
				stillUpdating = true;
				int t = data.r + delta;
				data.r = t >= 255 ? (byte)255 : (byte)t;
			}
		}
		else if (data.r > 0) {
			stillUpdating = true;
			int t = data.r - delta;
			data.r = t < 0 ? (byte)0 : (byte)t;
		}
		if (!stillUpdating) data.b = 0;
		cellTextureData[index] = data;
		return stillUpdating;
	}
}