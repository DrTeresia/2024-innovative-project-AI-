/*
这是一个名为HexHash的结构体脚本，位于Unity引擎的环境中。

主要功能：

1. 定义了一个名为HexHash的结构体，包含五个公共浮点型字段：a、b、c、d和e。
2. 提供了静态方法Create()，用于创建并返回一个HexHash对象。在这个方法中，每个字段都被赋值为一个介于0（不包括）和0.999之间的随机浮点数。这个随机数是使用Unity的Random.value函数生成的。

简单来说，这个脚本定义了一个生成随机数值的结构体，这些随机数值可以用于各种计算或逻辑处理，特别是在需要生成随机哈希值或随机数据的场合。
*/

﻿using UnityEngine;
public struct HexHash
{
	public float a, b, c, d, e;
	public static HexHash Create() {
		HexHash hash;
		hash.a = Random.value * 0.999f;
		hash.b = Random.value * 0.999f;
		hash.c = Random.value * 0.999f;
		hash.d = Random.value * 0.999f;
		hash.e = Random.value * 0.999f;
		return hash;
	}
}