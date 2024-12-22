/*
这是一个优先队列的实现，专门用于处理HexCell对象，其中每个HexCell对象都有一个搜索优先级（SearchPriority）。主要功能如下：

1. `Enqueue(HexCell cell)`：此方法用于向队列中添加一个HexCell对象。它首先检查新对象的优先级是否低于当前的最小优先级，如果是，则更新最小优先级。然后，根据优先级的值在列表中的相应位置插入对象。如果列表的长度小于优先级的值，将在列表的相应位置添加null值。同时，更新队列的计数。
2. `Dequeue()`：此方法用于从队列中移除并返回具有最小优先级的HexCell对象。如果列表中的所有位置都是null或者已经处理完所有具有最小优先级的对象，那么将返回null。同时，更新队列的计数和最小优先级。
3. `Change(HexCell cell, int oldPriority)`：此方法用于修改已存在的HexCell对象的优先级。首先找到旧优先级的对象并移除它，然后将对象添加到新的优先级位置，并更新队列的计数和最小优先级。如果新的优先级大于当前的最小优先级，那么还需要更新最小优先级。
4. `Clear()`：此方法用于清空队列的所有内容，重置计数和最小优先级。

此优先队列对于需要根据某种优先级进行处理的任务非常有用，例如在路径查找或游戏AI中。
*/

﻿using System.Collections.Generic;
public class HexCellPriorityQueue
{
	private readonly List<HexCell> list = new List<HexCell>();
	private int minimum = int.MaxValue;
	public int Count { get; private set; }
	public void Enqueue(HexCell cell) {
		Count += 1;
		int priority = cell.SearchPriority;
		if (priority < minimum) minimum = priority;
		while (priority >= list.Count) {
			list.Add(null);
		}
		cell.NextWithSamePriority = list[priority];
		list[priority] = cell;
	}
	public HexCell Dequeue() {
		Count -= 1;
		for (; minimum < list.Count; minimum++) {
			HexCell cell = list[minimum];
			if (cell != null) {
				list[minimum] = cell.NextWithSamePriority;
				return cell;
			}
		}
		return null;
	}
	public void Change(HexCell cell, int oldPriority) {
		HexCell current = list[oldPriority];
		HexCell next = current.NextWithSamePriority;
		if (current == cell) {
			list[oldPriority] = next;
		}
		else {
			while (next != cell) {
				current = next;
				next = current.NextWithSamePriority;
			}
			current.NextWithSamePriority = cell.NextWithSamePriority;
		}
		Enqueue(cell);
		Count -= 1;
	}
	public void Clear() {
		list.Clear();
		Count = 0;
		minimum = int.MaxValue;
	}
}