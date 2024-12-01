/*
这个代码定义了一个名为HexMesh的类，它用于创建和管理一个六边形网格（Hex Mesh）。以下是每个部分的简单解释：

类定义开始部分：

* 类HexMesh需要组件MeshFilter和MeshRenderer，这是Unity中用于渲染网格的组件。

字段定义部分：

* 序列化字段包括是否使用碰撞器（Collider）、是否使用单元数据（Cell Data）、是否使用UV坐标和UV2坐标等。这些字段可以在Unity编辑器中进行设置。
* 非序列化字段包括用于存储顶点坐标、三角形索引、UV坐标和单元数据等的列表。这些列表在运行时动态分配和回收内存。

事件函数部分：

* Awake函数在对象实例化时调用，初始化MeshFilter组件的网格为新的Mesh对象，并根据需要添加MeshCollider组件。

公共函数部分：

* Clear函数清空网格的所有数据。
* Apply函数应用所有添加到网格的数据，包括顶点、颜色、UV坐标和三角形等，并重新计算网格的法线。如果启用了碰撞器，还会将网格设置为碰撞器的共享网格。
* AddTriangle系列函数添加三角形到网格，包括未扰动（原始坐标）和扰动（经过处理的坐标）的三角形。同时提供了添加UV坐标和单元数据的函数。
* AddQuad系列函数添加四边形到网格，处理方式与AddTriangle类似，但会为四边形添加两个三角形。同样提供了添加UV坐标和单元数据的函数。这些函数允许指定四个顶点的UV坐标或权重颜色。还提供了快速添加相同UV坐标或权重的四边形的方法。最后也提供了直接添加矩形UV坐标范围的方法。 

以上就是该类的主要功能，此类设计用来创建并操作六边形网格，对于开发需要展示复杂六边形网格的游戏或者应用非常有用。
*/

﻿using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
	#region Serialized Fields
	public bool useCollider, useCellData, useUVCoordinates, useUV2Coordinates;
	#endregion
	[NonSerialized] private List<Color> cellWeights;
	private Mesh hexMesh;
	private MeshCollider meshCollider;
	[NonSerialized] private List<int> triangles;
	[NonSerialized] private List<Vector2> uvs, uv2s;
	[NonSerialized] private List<Vector3> vertices, cellIndices;
	#region Event Functions
	private void Awake() {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		if (useCollider) meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
	}
	#endregion
	public void Clear() {
		hexMesh.Clear();
		vertices = ListPool<Vector3>.Get();
		if (useCellData) {
			cellWeights = ListPool<Color>.Get();
			cellIndices = ListPool<Vector3>.Get();
		}
		if (useUVCoordinates) uvs = ListPool<Vector2>.Get();
		if (useUV2Coordinates) uv2s = ListPool<Vector2>.Get();
		triangles = ListPool<int>.Get();
	}
	public void Apply() {
		hexMesh.SetVertices(vertices);
		ListPool<Vector3>.Add(vertices);
		if (useCellData) {
			hexMesh.SetColors(cellWeights);
			ListPool<Color>.Add(cellWeights);
			hexMesh.SetUVs(2, cellIndices);
			ListPool<Vector3>.Add(cellIndices);
		}
		if (useUVCoordinates) {
			hexMesh.SetUVs(0, uvs);
			ListPool<Vector2>.Add(uvs);
		}
		if (useUV2Coordinates) {
			hexMesh.SetUVs(1, uv2s);
			ListPool<Vector2>.Add(uv2s);
		}
		hexMesh.SetTriangles(triangles, 0);
		ListPool<int>.Add(triangles);
		hexMesh.RecalculateNormals();
		if (useCollider) meshCollider.sharedMesh = hexMesh;
	}
	public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(HexMetrics.Perturb(v1));
		vertices.Add(HexMetrics.Perturb(v2));
		vertices.Add(HexMetrics.Perturb(v3));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
	public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
	public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3) {
		uvs.Add(uv1);
		uvs.Add(uv2);
		uvs.Add(uv3);
	}
	public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3) {
		uv2s.Add(uv1);
		uv2s.Add(uv2);
		uv2s.Add(uv3);
	}
	public void AddTriangleCellData(
		Vector3 indices,
		Color weights1,
		Color weights2,
		Color weights3
	) {
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellWeights.Add(weights1);
		cellWeights.Add(weights2);
		cellWeights.Add(weights3);
	}
	public void AddTriangleCellData(Vector3 indices, Color weights) {
		AddTriangleCellData(indices, weights, weights, weights);
	}
	public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		int vertexIndex = vertices.Count;
		vertices.Add(HexMetrics.Perturb(v1));
		vertices.Add(HexMetrics.Perturb(v2));
		vertices.Add(HexMetrics.Perturb(v3));
		vertices.Add(HexMetrics.Perturb(v4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}
	public void AddQuadUnperturbed(
		Vector3 v1,
		Vector3 v2,
		Vector3 v3,
		Vector3 v4
	) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		vertices.Add(v4);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}
	public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4) {
		uvs.Add(uv1);
		uvs.Add(uv2);
		uvs.Add(uv3);
		uvs.Add(uv4);
	}
	public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4) {
		uv2s.Add(uv1);
		uv2s.Add(uv2);
		uv2s.Add(uv3);
		uv2s.Add(uv4);
	}
	public void AddQuadUV(float uMin, float uMax, float vMin, float vMax) {
		uvs.Add(new Vector2(uMin, vMin));
		uvs.Add(new Vector2(uMax, vMin));
		uvs.Add(new Vector2(uMin, vMax));
		uvs.Add(new Vector2(uMax, vMax));
	}
	public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax) {
		uv2s.Add(new Vector2(uMin, vMin));
		uv2s.Add(new Vector2(uMax, vMin));
		uv2s.Add(new Vector2(uMin, vMax));
		uv2s.Add(new Vector2(uMax, vMax));
	}
	public void AddQuadCellData(
		Vector3 indices,
		Color weights1,
		Color weights2,
		Color weights3,
		Color weights4
	) {
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellIndices.Add(indices);
		cellWeights.Add(weights1);
		cellWeights.Add(weights2);
		cellWeights.Add(weights3);
		cellWeights.Add(weights4);
	}
	public void AddQuadCellData(
		Vector3 indices,
		Color weights1,
		Color weights2
	) {
		AddQuadCellData(indices, weights1, weights1, weights2, weights2);
	}
	public void AddQuadCellData(Vector3 indices, Color weights) {
		AddQuadCellData(indices, weights, weights, weights, weights);
	}
}