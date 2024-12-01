/*

**SaveLoadMenu.cs** 是Unity中用于保存和加载地图的一个脚本。它主要负责管理保存和加载地图的用户界面交互逻辑。以下是详细的功能解释：

1. **字段定义**: 定义了一些Unity UI元素（如文本、输入框、列表等）和游戏对象（如HexGrid和SaveLoadItem预制件）。同时定义了一个常量 `mapFileVersion` 用于表示地图文件的版本。
2. **Open方法**: 打开保存/加载菜单界面。根据传入的 `saveMode` 参数决定是打开保存地图模式还是加载地图模式，并更新菜单标签和动作按钮的标签。同时，填充地图列表（展示已保存的地图文件）。
3. **Close方法**: 关闭保存/加载菜单界面，并解锁HexMapCamera（可能是用于地图渲染的摄像机）。
4. **Action方法**: 执行保存或加载动作。首先获取选中的地图名称或路径，然后根据 `saveMode` 参数决定是保存还是加载地图。完成后关闭菜单。
5. **SelectItem方法**: 设置选中的地图名称。
6. **Delete方法**: 删除选中的地图文件，并重新填充地图列表。
7. **FillList方法**: 填充地图列表。从Application.persistentDataPath路径下获取所有的地图文件，并实例化SaveLoadItem预制件展示在列表中。
8. **GetSelectedPath方法**: 根据输入的地图名称，获取对应的完整路径（包括文件名和扩展名）。
9. **Save和Load方法**: 这两个方法是核心功能，分别用于保存和加载地图数据。保存时，将地图数据以二进制形式写入文件；加载时，从文件中读取二进制数据并恢复地图状态。

整体来说，这个脚本提供了一个用户友好的界面，允许用户保存和加载地图数据。
*/

﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class SaveLoadMenu : MonoBehaviour
{
	private const int mapFileVersion = 3;
	#region Serialized Fields
	public Text menuLabel, actionButtonLabel;
	public InputField nameInput;
	public RectTransform listContent;
	public SaveLoadItem itemPrefab;
	public HexGrid hexGrid;
	#endregion
	private bool saveMode;
	public void Open(bool saveMode) {
		this.saveMode = saveMode;
		if (saveMode) {
			menuLabel.text = "Save Map";
			actionButtonLabel.text = "Save";
		}
		else {
			menuLabel.text = "Load Map";
			actionButtonLabel.text = "Load";
		}
		FillList();
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}
	public void Close() {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}
	public void Action() {
		string path = GetSelectedPath();
		if (path == null) return;
		if (saveMode)
			Save(path);
		else
			Load(path);
		Close();
	}
	public void SelectItem(string name) {
		nameInput.text = name;
	}
	public void Delete() {
		string path = GetSelectedPath();
		if (path == null) return;
		if (File.Exists(path)) File.Delete(path);
		nameInput.text = "";
		FillList();
	}
	private void FillList() {
		for (int i = 0; i < listContent.childCount; i++) {
			Destroy(listContent.GetChild(i).gameObject);
		}
		string[] paths =
			Directory.GetFiles(Application.persistentDataPath, "*.map");
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++) {
			SaveLoadItem item = Instantiate(itemPrefab);
			item.menu = this;
			item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
		}
	}
	private string GetSelectedPath() {
		string mapName = nameInput.text;
		if (mapName.Length == 0) return null;
		return Path.Combine(Application.persistentDataPath, mapName + ".map");
	}
	private void Save(string path) {
		using (
			var writer =
			new BinaryWriter(File.Open(path, FileMode.Create))
		) {
			writer.Write(mapFileVersion);
			hexGrid.Save(writer);
		}
	}
	private void Load(string path) {
		if (!File.Exists(path)) {
			Debug.LogError("File does not exist " + path);
			return;
		}
		using (var reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header <= mapFileVersion) {
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
}