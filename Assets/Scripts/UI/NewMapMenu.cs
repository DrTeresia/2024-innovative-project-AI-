/*
这是一个名为 `NewMapMenu` 的Unity脚本，主要功能是与地图生成相关的用户界面交互。以下是每个方法的简单明了的代码功能解释：

1. `Open()` 方法：激活此游戏对象并锁定HexMapCamera。这意味着当用户选择打开地图菜单时，地图界面会被激活，并且地图摄像机将被锁定。
2. `Close()` 方法：关闭此游戏对象并解锁HexMapCamera。当用户选择关闭地图菜单时，地图界面会被隐藏，并且摄像机不再被锁定。
3. `CreateSmallMap()` 方法：创建一个小型地图。这个方法会调用 `CreateMap()` 方法并传入参数 (20, 15)，代表地图的大小。具体的地图大小取决于HexGrid的实现。
4. `CreateMediumMap()` 方法：创建一个中型地图。这个方法会调用 `CreateMap()` 方法并传入参数 (40, 30)。
5. `CreateLargeMap()` 方法：创建一个大型地图。这个方法会调用 `CreateMap()` 方法并传入参数 (80, 60)。
6. `CreateMap(int x, int z)` 方法：这是一个私有方法，用于创建地图。它接受两个参数（x和z），代表地图的大小，然后调用hexGrid的 `CreateMap` 方法来生成地图。之后验证摄像机位置并关闭此菜单。具体的地图大小和行为取决于hexGrid的实现和配置。

此脚本似乎是用于六角形网格地图的用户界面交互部分，允许用户创建不同大小的地图并管理地图菜单的显示和隐藏。
*/

﻿using UnityEngine;
public class NewMapMenu : MonoBehaviour
{
	#region Serialized Fields
	public HexGrid hexGrid;
	#endregion
	public void Open() {
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}
	public void Close() {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}
	public void CreateSmallMap() {
		CreateMap(20, 15);
	}
	public void CreateMediumMap() {
		CreateMap(40, 30);
	}
	public void CreateLargeMap() {
		CreateMap(80, 60);
	}
	private void CreateMap(int x, int z) {
		hexGrid.CreateMap(x, z);
		HexMapCamera.ValidatePosition();
		Close();
	}
}