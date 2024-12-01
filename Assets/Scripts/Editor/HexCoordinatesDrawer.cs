/*
此代码是一个Unity编辑器脚本，用于自定义属性绘制器（PropertyDrawer）。其主要功能是为HexCoordinates类型的属性提供一个自定义的GUI展示方式。以下是详细的代码功能解释：

* `using UnityEditor;` 和 `using UnityEngine;`：这两行代码引入了Unity编辑器和Unity引擎的命名空间，使得脚本可以访问Unity提供的各种功能和类。
* `[CustomPropertyDrawer(typeof(HexCoordinates))]`：这是一个自定义属性绘制器的属性标签，表明这个类是为HexCoordinates类型的属性提供绘制功能的。
* `public class HexCoordinatesDrawer : PropertyDrawer`：定义了一个名为HexCoordinatesDrawer的类，继承自PropertyDrawer，表示这是一个自定义的属性绘制器类。
* `public override void OnGUI(...)`：这是PropertyDrawer类中必须重写的方法，用于定义如何绘制属性的GUI。此方法接收三个参数：位置（Rect），属性（SerializedProperty）和标签（GUIContent）。


	+ `var coordinates = new HexCoordinates(...)`：创建一个新的HexCoordinates对象，使用属性的x和z值（通过FindPropertyRelative方法找到对应的子属性并获取其整数值）。
	+ `position = EditorGUI.PrefixLabel(position, label)`：更新位置以包含标签，并返回新的位置。这是为了先显示标签再显示属性值。
	+ `GUI.Label(position, coordinates.ToString())`：在新的位置上显示HexCoordinates对象的字符串表示（通过ToString方法转换）。这样，在Unity编辑器中查看或编辑属性时，就会以自定义的方式展示HexCoordinates的值。

总的来说，这个脚本的主要作用就是在Unity编辑器中提供一种自定义的GUI展示方式，用以更友好地展示和管理HexCoordinates类型的属性值。
*/

﻿using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{
	public override void OnGUI(
		Rect position,
		SerializedProperty property,
		GUIContent label
	) {
		var coordinates = new HexCoordinates(
			property.FindPropertyRelative("x").intValue,
			property.FindPropertyRelative("z").intValue
		);
		position = EditorGUI.PrefixLabel(position, label);
		GUI.Label(position, coordinates.ToString());
	}
}