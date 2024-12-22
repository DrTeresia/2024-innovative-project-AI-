/*
这是一个关于六边形方向处理的脚本。主要功能包括：

1. 定义了一个名为HexDirection的枚举类型，包含了六边形的六个方向：东北（NE）、东（E）、东南（SE）、西南（SW）、西（W）、西北（NW）。
2. 定义了一个静态类HexDirectionExtensions，用于扩展HexDirection的功能。该类包含以下几个方法：


	* `Opposite(this HexDirection direction)`：返回给定方向的相反方向。例如，如果给定方向是东（E），则返回西（W）。这是通过简单的算术运算实现的，如果方向值小于3（即NE、E、SE），则加3，否则减3。
	* `Previous(this HexDirection direction)`：返回给定方向的前一个方向。例如，如果给定方向是东北（NE），则返回西北（NW），其他方向依次类推减一。
	* `Next(this HexDirection direction)`：返回给定方向的后一个方向。例如，如果给定方向是西北（NW），则返回东北（NE），其他方向依次类推加一。
	* `Previous2(this HexDirection direction)` 和 `Next2(this HexDirection direction)`：这两个方法的功能和Previous及Next相似，但移动的方向间隔是两步，而非一步。比如Previous2会跳过前一个方向的相邻方向，直接返回第二个前一个方向。这两个方法在处理方向循环时使用了取模运算的技巧，保证了结果的正确性。
*/

﻿public enum HexDirection
{
	NE, E, SE, SW, W, NW,
}
public static class HexDirectionExtensions
{
	public static HexDirection Opposite(this HexDirection direction) =>
		(int)direction < 3 ? direction + 3 : direction - 3;
	public static HexDirection Previous(this HexDirection direction) =>
		direction == HexDirection.NE ? HexDirection.NW : direction - 1;
	public static HexDirection Next(this HexDirection direction) =>
		direction == HexDirection.NW ? HexDirection.NE : direction + 1;
	public static HexDirection Previous2(this HexDirection direction) {
		direction -= 2;
		return direction >= HexDirection.NE ? direction : direction + 6;
	}
	public static HexDirection Next2(this HexDirection direction) {
		direction += 2;
		return direction <= HexDirection.NW ? direction : direction - 6;
	}
}