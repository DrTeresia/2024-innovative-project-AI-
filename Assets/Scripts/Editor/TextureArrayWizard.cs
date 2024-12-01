/*
该代码是关于Unity中的TextureArrayWizard类，它的主要功能是创建一个纹理数组（Texture Array）。下面是代码的详细解释：

### 字段解释

* `textures`: 是一个Texture2D类型的数组，用于存储多个纹理。

### 函数解释

#### OnWizardCreate函数

* 当创建纹理数组的向导被触发时，此函数会被执行。
* 首先检查是否有纹理存储在`textures`数组中，如果没有则直接返回。
* 使用`EditorUtility.SaveFilePanelInProject`函数弹出一个保存文件的对话框，让用户选择保存纹理数组的路径和名称。
* 如果用户没有选择路径或者路径为空，则函数直接返回。
* 使用第一个纹理的宽度、高度、格式等信息创建一个新的Texture2DArray对象。同时设置其各项属性（如anisoLevel、filterMode和wrapMode）。这些属性与纹理的外观和渲染效果有关。
* 使用循环遍历所有纹理，并将每个纹理的每个mipmap复制到新创建的纹理数组中。这一步是为了将多个纹理合并到一个纹理数组中。
* 最后使用`AssetDatabase.CreateAsset`函数将新创建的纹理数组保存到用户指定的路径。

#### CreateWizard函数

* 该函数通过Unity的菜单系统（MenuItems）添加一个名为“Create Texture Array”的选项。当用户在Unity编辑器中点击这个选项时，会触发TextureArrayWizard向导的创建过程。它接受两个参数：向导的标题（"Create Texture Array"）和创建按钮的标签（"Create"）。这个函数是用来启动纹理数组创建向导的入口点。点击该菜单项后，会显示一个向导界面，用户可以在其中选择并处理纹理数组。

总的来说，这个脚本提供了一个方便的工具，允许用户在Unity编辑器中创建包含多个纹理的纹理数组。这对于处理多个相关纹理时非常有用，特别是当这些纹理需要作为一个整体进行渲染时（例如在Unity的Shader中使用纹理数组）。
*/

﻿using UnityEditor;
using UnityEngine;
public class TextureArrayWizard : ScriptableWizard
{
	#region Serialized Fields
	public Texture2D[] textures;
	#endregion
	#region Event Functions
	private void OnWizardCreate() {
		if (textures.Length == 0) return;
		string path = EditorUtility.SaveFilePanelInProject(
			"Save Texture Array", "Texture Array", "asset", "Save Texture Array"
		);
		if (path.Length == 0) return;
		Texture2D t = textures[0];
		var textureArray = new Texture2DArray(
			t.width, t.height, textures.Length, t.format, t.mipmapCount > 1
		);
		textureArray.anisoLevel = t.anisoLevel;
		textureArray.filterMode = t.filterMode;
		textureArray.wrapMode = t.wrapMode;
		for (int i = 0; i < textures.Length; i++) {
			for (int m = 0; m < t.mipmapCount; m++) {
				Graphics.CopyTexture(textures[i], 0, m, textureArray, i, m);
			}
		}
		AssetDatabase.CreateAsset(textureArray, path);
	}
	#endregion
	[MenuItem("Assets/Create/Texture Array")]
	private static void CreateWizard() {
		DisplayWizard<TextureArrayWizard>(
			"Create Texture Array", "Create"
		);
	}
}