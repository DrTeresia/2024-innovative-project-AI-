/*
该代码是一个名为HexGameUI的Unity脚本，用于处理与六角形格子游戏界面相关的交互逻辑。以下是代码功能的解释：

1. **字段声明**:
   - `HexGrid grid`: 引用了一个名为HexGrid的组件，用于管理游戏中的六角形格子地图。
   - `HexCell currentCell`: 当前选中的格子。
   - `HexUnit selectedUnit`: 当前选中的单位或角色。

2. **事件函数**:
   - `Update()`: 每帧更新时调用。当鼠标没有在UI对象上时，根据鼠标的左键和右键点击执行不同的操作。左键点击进行选中操作（DoSelection），右键点击进行移动操作（DoMove），如果已有选中的单位且处于移动模式，进行路径查找操作（DoPathfinding）。
   - `SetEditMode(bool toggle)`: 设置编辑模式。当toggle为true时，进入编辑模式，禁用UI并启用特定的Shader关键词；当toggle为false时，退出编辑模式，启用UI并禁用Shader关键词。
   - `DoSelection()`: 清除当前路径并更新当前选中的格子，如果有格子则选中格子中的单位。
   - `DoPathfinding()`: 如果更新了当前选中的格子且选中的单位的目的地是有效的，则在地图上查找从当前位置到目的地的路径。否则清除路径。
   - `DoMove()`: 如果地图上存在路径，则选中单位沿着路径移动并清除路径。
   - `UpdateCurrentCell()`: 更新当前鼠标所在的格子。如果格子发生变化则返回true，否则返回false。

简而言之，这个脚本主要负责处理游戏中用户与六角形格子地图的交互逻辑，包括选择单元格、路径查找和单位移动等功能。
*/

﻿using UnityEngine;
using UnityEngine.EventSystems;
public class HexGameUI : MonoBehaviour
{
	#region Serialized Fields
	public HexGrid grid;
	#endregion
	private HexCell currentCell;
	private HexUnit selectedUnit;
	#region Event Functions
	private void Update() {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0)) {
				DoSelection();
			}
			else if (selectedUnit) {
				if (Input.GetMouseButtonDown(1))
					DoMove();
				else
					DoPathfinding();
			}
		}
	}
	#endregion
	public void SetEditMode(bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		if (toggle)
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		else
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
	}
	private void DoSelection() {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) selectedUnit = currentCell.Unit;
	}
	private void DoPathfinding() {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell))
				grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
			else
				grid.ClearPath();
		}
	}
	private void DoMove() {
		if (grid.HasPath) {
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}
	private bool UpdateCurrentCell() {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
}