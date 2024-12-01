/*
这是一个名为HexFeatureCollection的结构体，用于在Unity游戏引擎中管理一系列的预制体（prefabs）。这个结构体的主要功能是存储预制体的Transform组件数组并允许通过一定的概率或选择来获取预制体。

关键部分解释如下：

* `[Serializable]`：这是一个特性标签，用于确保此结构体可以被序列化。序列化是将对象的状态信息转换为可以存储或传输的形式的过程。在Unity中，这通常用于保存场景或游戏对象的状态。
* `Transform[] prefabs`：这是一个Transform数组，用于存储预制体的引用。预制体是Unity中一种重要的资源，可以用于实例化游戏对象。Transform组件是每个游戏对象都有的组件，它包含了对象的位置、旋转和缩放信息。
* `public Transform Pick(float choice)`：这是一个公共方法，接受一个浮点数作为输入参数（代表某种选择或概率），然后返回对应的预制体的Transform组件。这个方法通过计算输入参数与预制体数组长度的乘积的整数部分来选择一个预制体。例如，如果输入是0.5且预制体数组有3个元素，那么它将返回数组中的第二个预制体的Transform组件。
*/

﻿using System;
using UnityEngine;
[Serializable]
public struct HexFeatureCollection
{
	#region Serialized Fields
	public Transform[] prefabs;
	#endregion
	public Transform Pick(float choice) => prefabs[(int)(choice * prefabs.Length)];
}