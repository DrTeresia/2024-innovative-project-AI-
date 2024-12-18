/*
这是一个关于贝塞尔曲线的简单实现。这个脚本包含两个静态方法：

### Bezier.GetPoint 方法：

此方法用于计算贝塞尔曲线上的一个点。给定三个控制点（a、b、c）和一个参数 t（通常介于 0 和 1 之间），此方法计算贝塞尔曲线的特定位置。这是一种常见的线性插值形式，利用参数 t 对三个控制点进行加权平均来计算点。当 t=0 时，点位于 a 处；当 t=1 时，点位于 c 处；在其他值上，点在控制线段的混合上。这个方法在计算过程中涉及到公式： r * r * a + 2f * r * t * b + t * t * c，其中 r 是 1-t 的值。这种方法基于贝塞尔曲线的线性表示形式。

### Bezier.GetDerivative 方法：

此方法计算贝塞尔曲线在给定点上的切线方向或导数。它基于贝塞尔曲线的导数公式计算，该公式描述了曲线在某一点的斜率或方向变化率。这个方法的输出是一个向量，表示曲线在该点的切线方向。此方法使用了控制点 a、b 和 c 以及参数 t 来计算导数。它主要用于曲线动画和路径平滑等场景。
*/

﻿using UnityEngine;
public static class Bezier
{
	public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t) {
		float r = 1f - t;
		return r * r * a + 2f * r * t * b + t * t * c;
	}
	public static Vector3 GetDerivative(
		Vector3 a,
		Vector3 b,
		Vector3 c,
		float t
	) =>
		2f * ((1f - t) * (b - a) + t * (c - b));
}