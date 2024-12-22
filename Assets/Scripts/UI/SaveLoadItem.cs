/*
解释：

该文件名为 SaveLoadItem.cs，它可能是一个关于游戏或应用的保存与加载界面的 UI 组件脚本。脚本是使用 Unity 框架写的，其对象位于 Assets\Scripts\UI 目录之下。下面是对这个脚本的功能解释：

1. **引用字段**: `public SaveLoadMenu menu;` 这一行声明了一个公开的 SaveLoadMenu 对象，这意味着此脚本与 SaveLoadMenu 脚本有某种关联或交互。
2. **私有字段**: `private string mapName;` 定义了一个私有字符串变量 mapName，用于存储地图的名称或其他信息。这里用于识别并标记每一个保存负载的项。
3. **MapName 属性**: 它包含了获取（get）和设置（set）功能。当你获取 MapName 时，它会返回 mapName 的值；当你设置 MapName 时，它会更新 mapName 的值并同时更新该对象的子对象（通过 `transform.GetChild(0).GetComponent<Text>().text = value;` 更新其文本组件的值）。这可能是在界面上实时反映名字更改的情况。
4. **Select 方法**: 这个方法似乎被设计为当用户选择某个保存负载项时被调用。它调用 menu 的 SelectItem 方法并传递 mapName 作为参数。这意味着当用户点击或选中此 SaveLoadItem 时，它通知菜单该项目的名称被选中或选中。具体实现可能会因 SaveLoadMenu 的定义而异，可能涉及加载或保存地图或其他操作。
*/

﻿using UnityEngine;
using UnityEngine.UI;
public class SaveLoadItem : MonoBehaviour
{
	#region Serialized Fields
	public SaveLoadMenu menu;
	#endregion
	private string mapName;
	public string MapName {
		get => mapName;
		set {
			mapName = value;
			transform.GetChild(0).GetComponent<Text>().text = value;
		}
	}
	public void Select() {
		menu.SelectItem(mapName);
	}
}