/*
这是一个名为HexMapEditor的Unity脚本，主要用于编辑一个名为HexGrid的六角形网格地图。主要功能包括设置地形类型、地形高度、水域等级、城市等级、农场等级、植物等级以及特殊指数等。以下是每个功能的简单解释：

* `HexGrid`: 一个用于存储地图信息的对象，包含了地图上的每一个六角形单元格（HexCell）。
* `SetTerrainTypeIndex`: 设置当前激活的地形类型索引。地形类型通常代表了不同的地形，如平原、山地等。
* `SetApplyElevation`, `SetElevation`: 控制是否应用地形高度变化，并设置具体的高度值。
* `SetApplyWaterLevel`, `SetWaterLevel`: 控制是否应用水域等级变化，并设置具体的水域等级。
* `SetApplyUrbanLevel`, `SetUrbanLevel`: 控制是否应用城市等级变化，并设置具体的城市等级。这可能会影响该区域的建筑和设施等。
* `SetApplyFarmLevel`, `SetFarmLevel`: 控制是否应用农场等级变化，并设置具体的农场等级。这可能会影响农作物的生长和产量等。
* `SetApplyPlantLevel`, `SetPlantLevel`: 控制是否应用植物等级变化，并设置具体的植物等级。这可能会影响植物的种类和生长状况等。
* `SetApplySpecialIndex`, `SetSpecialIndex`: 控制是否应用特殊指数变化，并设置具体的特殊指数值。特殊指数可能用于表示某些特殊的地图特性或事件。
* `SetBrushSize`: 设置编辑工具的画笔大小，影响地形编辑的范围。
* `SetRiverMode`, `SetRoadMode`, `SetWalledMode`: 分别设置河流模式、道路模式和围墙模式。这些模式可能用于在地图上创建或删除河流、道路和围墙等。
* `SetEditMode`: 控制编辑器是否处于激活状态。
* `ShowGrid`: 显示或隐藏地图网格。
* `CreateUnit` 和 `DestroyUnit`: 在地图上创建和删除单位（可能是NPC、玩家角色或其他实体）。
* `HandleInput`: 处理用户的输入，包括鼠标移动和点击等，用于编辑地图。
* `EditCells` 和 `EditCell`: 编辑选定区域内的单元格，应用上述设置的各项参数。
* `OptionalToggle`: 一个枚举类型，用于表示可选的开关状态（忽略、是、否）。在脚本中用于控制是否应用某些功能，如是否开启河流模式等。

总的来说，这是一个非常强大的地图编辑器脚本，可以用于创建和编辑一个复杂的六角形网格地图，包含多种地形特征和对象。
*/

﻿using UnityEngine;
using UnityEngine.EventSystems;
public class HexMapEditor : MonoBehaviour
{
	#region Serialized Fields
	public HexGrid hexGrid;
	public Material terrainMaterial;
	#endregion
	private int activeElevation;
	private int activeTerrainTypeIndex;
	private int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;
	private int activeWaterLevel;
	private bool applyElevation = true;
	private bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;
	private bool applyWaterLevel = true;
	private int brushSize;
	private HexDirection dragDirection;
	private bool isDrag;
	private HexCell previousCell;
	private OptionalToggle riverMode, roadMode, walledMode;
	#region Event Functions
	private void Awake() {
		terrainMaterial.DisableKeyword("GRID_ON");
		SetEditMode(false);
	}
	private void Update() {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
			if (Input.GetKeyDown(KeyCode.U)) {
				if (Input.GetKey(KeyCode.LeftShift))
					DestroyUnit();
				else
					CreateUnit();
				return;
			}
		}
		previousCell = null;
	}
	#endregion
	public void SetTerrainTypeIndex(int index) {
		activeTerrainTypeIndex = index;
	}
	public void SetApplyElevation(bool toggle) {
		applyElevation = toggle;
	}
	public void SetElevation(float elevation) {
		activeElevation = (int)elevation;
	}
	public void SetApplyWaterLevel(bool toggle) {
		applyWaterLevel = toggle;
	}
	public void SetWaterLevel(float level) {
		activeWaterLevel = (int)level;
	}
	public void SetApplyUrbanLevel(bool toggle) {
		applyUrbanLevel = toggle;
	}
	public void SetUrbanLevel(float level) {
		activeUrbanLevel = (int)level;
	}
	public void SetApplyFarmLevel(bool toggle) {
		applyFarmLevel = toggle;
	}
	public void SetFarmLevel(float level) {
		activeFarmLevel = (int)level;
	}
	public void SetApplyPlantLevel(bool toggle) {
		applyPlantLevel = toggle;
	}
	public void SetPlantLevel(float level) {
		activePlantLevel = (int)level;
	}
	public void SetApplySpecialIndex(bool toggle) {
		applySpecialIndex = toggle;
	}
	public void SetSpecialIndex(float index) {
		activeSpecialIndex = (int)index;
	}
	public void SetBrushSize(float size) {
		brushSize = (int)size;
	}
	public void SetRiverMode(int mode) {
		riverMode = (OptionalToggle)mode;
	}
	public void SetRoadMode(int mode) {
		roadMode = (OptionalToggle)mode;
	}
	public void SetWalledMode(int mode) {
		walledMode = (OptionalToggle)mode;
	}
	public void SetEditMode(bool toggle) {
		enabled = toggle;
	}
	public void ShowGrid(bool visible) {
		if (visible)
			terrainMaterial.EnableKeyword("GRID_ON");
		else
			terrainMaterial.DisableKeyword("GRID_ON");
	}
	private HexCell GetCellUnderCursor() => hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	private void CreateUnit() {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit)
			hexGrid.AddUnit(
				Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f)
			);
	}
	private void DestroyUnit() {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) hexGrid.RemoveUnit(cell.Unit);
	}
	private void HandleInput() {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if (previousCell && previousCell != currentCell)
				ValidateDrag(currentCell);
			else
				isDrag = false;
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}
	private void ValidateDrag(HexCell currentCell) {
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		) {
			if (previousCell.GetNeighbor(dragDirection) == currentCell) {
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}
	private void EditCells(HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;
		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
			for (int x = centerX - brushSize; x <= centerX + r; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}
	private void EditCell(HexCell cell) {
		if (cell) {
			if (activeTerrainTypeIndex >= 0) cell.TerrainTypeIndex = activeTerrainTypeIndex;
			if (applyElevation) cell.Elevation = activeElevation;
			if (applyWaterLevel) cell.WaterLevel = activeWaterLevel;
			if (applySpecialIndex) cell.SpecialIndex = activeSpecialIndex;
			if (applyUrbanLevel) cell.UrbanLevel = activeUrbanLevel;
			if (applyFarmLevel) cell.FarmLevel = activeFarmLevel;
			if (applyPlantLevel) cell.PlantLevel = activePlantLevel;
			if (riverMode == OptionalToggle.No) cell.RemoveRiver();
			if (roadMode == OptionalToggle.No) cell.RemoveRoads();
			if (walledMode != OptionalToggle.Ignore) cell.Walled = walledMode == OptionalToggle.Yes;
			if (isDrag) {
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell) {
					if (riverMode == OptionalToggle.Yes) otherCell.SetOutgoingRiver(dragDirection);
					if (roadMode == OptionalToggle.Yes) otherCell.AddRoad(dragDirection);
				}
			}
		}
	}
	#region Nested type: ${0}
	private enum OptionalToggle
	{
		Ignore, Yes, No,
	}
	#endregion
}