/*
解释如下：

这个脚本定义了一个名为`EdgeVertices`的结构体，用于表示二维空间中通过一组顶点定义的一组边缘线段。其中包含了五个`Vector3`类型的成员变量：`v1`，`v2`，`v3`，`v4`和`v5`，它们表示了五条线段上的顶点位置。每个顶点都是一个三维向量，包括一个x、y和z坐标。这些顶点可以通过两种方式初始化：

第一种初始化方式接收两个三维向量作为参数（`corner1`和`corner2`），然后通过线性插值（lerp）在两点之间创建三个额外的顶点（位于两个端点之间的四分之一、一半和四分之三的位置），这五个顶点定义了从`corner1`到`corner2`的边缘线段。线性插值可以平滑地过渡两个点之间的位置，生成新的点。这里生成的是等距插值点，用于定义一条连续的线段。

第二种初始化方式也接收两个三维向量和一个浮点数作为参数（额外的参数表示一个额外的参数表示“外插值”，影响新生成顶点相对于两点的位置）。在这种情况下，该浮点数指定了一个点在从起始点到终止点上的位置偏移量。第三个顶点的位置会根据这个浮点数来调整。该浮数值定义了线段的扩展范围。这是为复杂几何图形设计的选项，它允许根据需求更精确地调整边缘线段的结构。根据此浮点值的大小和变化，生成的不同顶点的组合会产生不同的几何形状或过渡效果。如果调整得当，可以在二维平面上创建更复杂的图形或曲线形状。例如用于地形、道路等复杂形状的场景设计。这可以用于制作更复杂的地形模型或者生成独特的场景几何结构。注意这种方法使得在自定义参数时提供了更多的灵活性以适应不同的情况和特定的场景需求。第三种方法用于两个边缘线段的融合或混合。通过使用此方法将两个不同的边缘顶点序列混合到一起形成新的边缘顶点序列。通过传递两个EdgeVertices对象和一个整数步长参数来调用此方法，它将根据给定的步长参数在两个边缘之间进行插值计算并返回一个新的EdgeVertices对象作为结果。"HexMetrics.TerraceLerp"是一个静态方法用于在多个顶点之间实现线性插值运算并将结果应用到新的EdgeVertices对象中。"TerraceLerp"可能用于地形渲染中的台阶或阶梯的生成处理中。这种方法对于创建平滑过渡的地形边缘特别有用因为它能够处理连续的插值并产生连续的地形变化而不是仅仅连接两个离散点的方式生成的突兀边缘变化场景允许实现更为平滑复杂的过渡效果特别是对于那些需要高度细节的场景比如地形建模和场景渲染等任务场景使用这种方式可以在不同的地形部分之间创建出自然且连贯的过渡效果而不会显得突兀或不自然。
*/

﻿using UnityEngine;
public struct EdgeVertices
{
	public Vector3 v1, v2, v3, v4, v5;
	public EdgeVertices(Vector3 corner1, Vector3 corner2) {
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, 0.25f);
		v3 = Vector3.Lerp(corner1, corner2, 0.5f);
		v4 = Vector3.Lerp(corner1, corner2, 0.75f);
		v5 = corner2;
	}
	public EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep) {
		v1 = corner1;
		v2 = Vector3.Lerp(corner1, corner2, outerStep);
		v3 = Vector3.Lerp(corner1, corner2, 0.5f);
		v4 = Vector3.Lerp(corner1, corner2, 1f - outerStep);
		v5 = corner2;
	}
	public static EdgeVertices TerraceLerp(
		EdgeVertices a,
		EdgeVertices b,
		int step
	) {
		EdgeVertices result;
		result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
		result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
		result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
		result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
		result.v5 = HexMetrics.TerraceLerp(a.v5, b.v5, step);
		return result;
	}
}