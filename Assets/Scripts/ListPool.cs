/*
代码功能解释：

这是一个名为 `ListPool<T>` 的泛型静态类，用于管理一个列表的池（或栈）。其主要目的是减少频繁创建和销毁列表对象所带来的性能开销。

* `Get()` 方法：从池中获取一个列表对象。如果池中有列表可用（即栈不为空），则返回栈顶（最近使用的）列表并将其从栈中移除。如果没有可用的列表，则创建一个新的空列表并返回。
* `Add(List<T> list)` 方法：将一个列表对象添加到池中。在添加之前，会先清空列表的内容（避免保留旧数据）。然后，将这个列表对象推入栈中，以便将来可以再次使用。这允许重用列表对象，从而避免不必要的内存分配和垃圾回收。
*/

﻿using System.Collections.Generic;
public static class ListPool<T>
{
	private static readonly Stack<List<T>> stack = new Stack<List<T>>();
	public static List<T> Get() {
		if (stack.Count > 0) return stack.Pop();
		return new List<T>();
	}
	public static void Add(List<T> list) {
		list.Clear();
		stack.Push(list);
	}
}