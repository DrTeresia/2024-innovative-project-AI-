/*
该代码是一个用于处理六边形网格（HexGrid）的类，其中包含了用于创建和处理六边形网格块（HexCell）的方法。这个类中的方法包括初始化网格、添加和刷新六边形单元格、三角剖分网格等。

1. `AddCell(int index, HexCell cell)`: 添加一个六边形单元格到当前网格中，并设置其父子关系以及将其UI元素添加到UI画布上。

2. `Refresh()`: 重置网格的状态，使得它可以被重新绘制。

3. `ShowUI(bool visible)`: 显示或隐藏网格的UI。

4. `Triangulate()`: 清空所有的六边形单元格和对应的网格层，然后对所有单元格进行三角剖分，并将结果应用到各个网格层上。

5. `Triangulate(HexCell cell)`: 对一个六边形单元格进行三角剖分，并根据其状态添加特征。

6. `Triangulate(HexDirection direction, HexCell cell)`: 根据给定的方向和六边形单元格，计算该单元格与相邻单元格之间的边缘，并进行三角剖分。

7. `TriangulateWater(HexDirection direction, HexCell cell, Vector3 center)`: 对水下单元格进行三角剖分，包括处理与陆地单元格的交界（waterShore）。

8. `TriangulateWithoutRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)`: 对没有河流的单元格进行三角剖分，如果单元格有道路，则添加道路特征。

9. `TriangulateAdjacentToRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)`: 对与河流相邻的单元格进行三角剖分，如果单元格有道路，则添加道路特征。

10. `TriangulateWithRiver(HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e)`: 对有河流的单元格进行三角剖分，包括处理河流开始和结束的情况。

11. `TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1)`: 对与单元格相连的单元格进行三角剖分，并根据边缘类型添加特征。

12. `TriangulateWaterfallInWater(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y1, float y2, float waterY, Vector3 indices)`: 处理从水面到河流底部的水流瀑布情况。

13. `TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)`: 处理三个相邻单元格形成的角落情况，根据边缘类型添加特征。

14. `TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index)`: 对一个六边形单元格的边缘进行三角剖分。

15. `TriangulateEdgeStrip(EdgeVertices e1, Color w1, float index1, EdgeVertices e2, Color w2, float index2, bool hasRoad = false)`: 对两个六边形单元格之间的边缘进行三角剖分，如果单元格有道路，则添加道路特征。

16. `TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y, float v, bool reversed, Vector3 indices)`: 对河流进行三角剖分。

17. `TriangulateRoad(Vector3 center, Vector3 mL, Vector3 mR, EdgeVertices e, bool hasRoadThroughCellEdge, float index)`: 对有道路的六边形单元格进行三角剖分。

18. `TriangulateRoadEdge(Vector3 center, Vector3 mL, Vector3 mR, float index)`: 对六边形单元格的边缘进行三角剖分，并添加道路特征。

19. `TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6, Color w1, Color w2, Vector3 indices)`: 对两个六边形单元格之间的道路进行三角剖分。
*/

﻿using UnityEngine;
public class HexGridChunk : MonoBehaviour
{
	private static readonly Color weights1 = new Color(1f, 0f, 0f);
	private static readonly Color weights2 = new Color(0f, 1f, 0f);
	private static readonly Color weights3 = new Color(0f, 0f, 1f);
	#region Serialized Fields
	public HexMesh terrain, rivers, roads, water, waterShore, estuaries;
	public HexFeatureManager features;
	#endregion
	private HexCell[] cells;
	private Canvas gridCanvas;
	#region Event Functions
	private void Awake() {
		gridCanvas = GetComponentInChildren<Canvas>();
		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}
	private void LateUpdate() {
		Triangulate();
		enabled = false;
	}
	#endregion
	public void AddCell(int index, HexCell cell) {
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}
	public void Refresh() {
		enabled = true;
	}
	public void ShowUI(bool visible) {
		gridCanvas.gameObject.SetActive(visible);
	}
	public void Triangulate() {
		terrain.Clear();
		rivers.Clear();
		roads.Clear();
		water.Clear();
		waterShore.Clear();
		estuaries.Clear();
		features.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		terrain.Apply();
		rivers.Apply();
		roads.Apply();
		water.Apply();
		waterShore.Apply();
		estuaries.Apply();
		features.Apply();
	}
	private void Triangulate(HexCell cell) {
		for (var d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
		if (!cell.IsUnderwater) {
			if (!cell.HasRiver && !cell.HasRoads) features.AddFeature(cell, cell.Position);
			if (cell.IsSpecial) features.AddSpecialFeature(cell, cell.Position);
		}
	}
	private void Triangulate(HexDirection direction, HexCell cell) {
		Vector3 center = cell.Position;
		var e = new EdgeVertices(
			center + HexMetrics.GetFirstSolidCorner(direction),
			center + HexMetrics.GetSecondSolidCorner(direction)
		);
		if (cell.HasRiver) {
			if (cell.HasRiverThroughEdge(direction)) {
				e.v3.y = cell.StreamBedY;
				if (cell.HasRiverBeginOrEnd)
					TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
				else
					TriangulateWithRiver(direction, cell, center, e);
			}
			else {
				TriangulateAdjacentToRiver(direction, cell, center, e);
			}
		}
		else {
			TriangulateWithoutRiver(direction, cell, center, e);
			if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction))
				features.AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
		}
		if (direction <= HexDirection.SE) TriangulateConnection(direction, cell, e);
		if (cell.IsUnderwater) TriangulateWater(direction, cell, center);
	}
	private void TriangulateWater(
		HexDirection direction,
		HexCell cell,
		Vector3 center
	) {
		center.y = cell.WaterSurfaceY;
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor != null && !neighbor.IsUnderwater)
			TriangulateWaterShore(direction, cell, neighbor, center);
		else
			TriangulateOpenWater(direction, cell, neighbor, center);
	}
	private void TriangulateOpenWater(
		HexDirection direction,
		HexCell cell,
		HexCell neighbor,
		Vector3 center
	) {
		Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
		Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);
		water.AddTriangle(center, c1, c2);
		Vector3 indices;
		indices.x = indices.y = indices.z = cell.Index;
		water.AddTriangleCellData(indices, weights1);
		if (direction <= HexDirection.SE && neighbor != null) {
			Vector3 bridge = HexMetrics.GetWaterBridge(direction);
			Vector3 e1 = c1 + bridge;
			Vector3 e2 = c2 + bridge;
			water.AddQuad(c1, c2, e1, e2);
			indices.y = neighbor.Index;
			water.AddQuadCellData(indices, weights1, weights2);
			if (direction <= HexDirection.E) {
				HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
				if (nextNeighbor == null || !nextNeighbor.IsUnderwater) return;
				water.AddTriangle(
					c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next())
				);
				indices.z = nextNeighbor.Index;
				water.AddTriangleCellData(
					indices, weights1, weights2, weights3
				);
			}
		}
	}
	private void TriangulateWaterShore(
		HexDirection direction,
		HexCell cell,
		HexCell neighbor,
		Vector3 center
	) {
		var e1 = new EdgeVertices(
			center + HexMetrics.GetFirstWaterCorner(direction),
			center + HexMetrics.GetSecondWaterCorner(direction)
		);
		water.AddTriangle(center, e1.v1, e1.v2);
		water.AddTriangle(center, e1.v2, e1.v3);
		water.AddTriangle(center, e1.v3, e1.v4);
		water.AddTriangle(center, e1.v4, e1.v5);
		Vector3 indices;
		indices.x = indices.z = cell.Index;
		indices.y = neighbor.Index;
		water.AddTriangleCellData(indices, weights1);
		water.AddTriangleCellData(indices, weights1);
		water.AddTriangleCellData(indices, weights1);
		water.AddTriangleCellData(indices, weights1);
		Vector3 center2 = neighbor.Position;
		center2.y = center.y;
		var e2 = new EdgeVertices(
			center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
			center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
		);
		if (cell.HasRiverThroughEdge(direction)) {
			TriangulateEstuary(
				e1, e2, cell.IncomingRiver == direction, indices
			);
		}
		else {
			waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
			waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
			waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
			waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadUV(0f, 0f, 0f, 1f);
			waterShore.AddQuadCellData(indices, weights1, weights2);
			waterShore.AddQuadCellData(indices, weights1, weights2);
			waterShore.AddQuadCellData(indices, weights1, weights2);
			waterShore.AddQuadCellData(indices, weights1, weights2);
		}
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (nextNeighbor != null) {
			Vector3 v3 = nextNeighbor.Position + (nextNeighbor.IsUnderwater
				? HexMetrics.GetFirstWaterCorner(direction.Previous())
				: HexMetrics.GetFirstSolidCorner(direction.Previous()));
			v3.y = center.y;
			waterShore.AddTriangle(e1.v5, e2.v5, v3);
			waterShore.AddTriangleUV(
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)
			);
			indices.z = nextNeighbor.Index;
			waterShore.AddTriangleCellData(
				indices, weights1, weights2, weights3
			);
		}
	}
	private void TriangulateEstuary(
		EdgeVertices e1,
		EdgeVertices e2,
		bool incomingRiver,
		Vector3 indices
	) {
		waterShore.AddTriangle(e2.v1, e1.v2, e1.v1);
		waterShore.AddTriangle(e2.v5, e1.v5, e1.v4);
		waterShore.AddTriangleUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
		waterShore.AddTriangleUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
		waterShore.AddTriangleCellData(indices, weights2, weights1, weights1);
		waterShore.AddTriangleCellData(indices, weights2, weights1, weights1);
		estuaries.AddQuad(e2.v1, e1.v2, e2.v2, e1.v3);
		estuaries.AddTriangle(e1.v3, e2.v2, e2.v4);
		estuaries.AddQuad(e1.v3, e1.v4, e2.v4, e2.v5);
		estuaries.AddQuadUV(
			new Vector2(0f, 1f), new Vector2(0f, 0f),
			new Vector2(1f, 1f), new Vector2(0f, 0f)
		);
		estuaries.AddTriangleUV(
			new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)
		);
		estuaries.AddQuadUV(
			new Vector2(0f, 0f), new Vector2(0f, 0f),
			new Vector2(1f, 1f), new Vector2(0f, 1f)
		);
		estuaries.AddQuadCellData(
			indices, weights2, weights1, weights2, weights1
		);
		estuaries.AddTriangleCellData(indices, weights1, weights2, weights2);
		estuaries.AddQuadCellData(indices, weights1, weights2);
		if (incomingRiver) {
			estuaries.AddQuadUV2(
				new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f),
				new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
			);
			estuaries.AddTriangleUV2(
				new Vector2(0.5f, 1.1f),
				new Vector2(1f, 0.8f),
				new Vector2(0f, 0.8f)
			);
			estuaries.AddQuadUV2(
				new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
				new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)
			);
		}
		else {
			estuaries.AddQuadUV2(
				new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f),
				new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
			);
			estuaries.AddTriangleUV2(
				new Vector2(0.5f, -0.3f),
				new Vector2(0f, 0f),
				new Vector2(1f, 0f)
			);
			estuaries.AddQuadUV2(
				new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f),
				new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)
			);
		}
	}
	private void TriangulateWithoutRiver(
		HexDirection direction,
		HexCell cell,
		Vector3 center,
		EdgeVertices e
	) {
		TriangulateEdgeFan(center, e, cell.Index);
		if (cell.HasRoads) {
			Vector2 interpolators = GetRoadInterpolators(direction, cell);
			TriangulateRoad(
				center,
				Vector3.Lerp(center, e.v1, interpolators.x),
				Vector3.Lerp(center, e.v5, interpolators.y),
				e, cell.HasRoadThroughEdge(direction), cell.Index
			);
		}
	}
	private Vector2 GetRoadInterpolators(HexDirection direction, HexCell cell) {
		Vector2 interpolators;
		if (cell.HasRoadThroughEdge(direction)) {
			interpolators.x = interpolators.y = 0.5f;
		}
		else {
			interpolators.x =
				cell.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
			interpolators.y =
				cell.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
		}
		return interpolators;
	}
	private void TriangulateAdjacentToRiver(
		HexDirection direction,
		HexCell cell,
		Vector3 center,
		EdgeVertices e
	) {
		if (cell.HasRoads) TriangulateRoadAdjacentToRiver(direction, cell, center, e);
		if (cell.HasRiverThroughEdge(direction.Next())) {
			if (cell.HasRiverThroughEdge(direction.Previous()))
				center += HexMetrics.GetSolidEdgeMiddle(direction) *
						(HexMetrics.innerToOuter * 0.5f);
			else if (
				cell.HasRiverThroughEdge(direction.Previous2())
			)
				center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
		}
		else if (
			cell.HasRiverThroughEdge(direction.Previous()) &&
			cell.HasRiverThroughEdge(direction.Next2())
		) {
			center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
		}
		var m = new EdgeVertices(
			Vector3.Lerp(center, e.v1, 0.5f),
			Vector3.Lerp(center, e.v5, 0.5f)
		);
		TriangulateEdgeStrip(
			m, weights1, cell.Index,
			e, weights1, cell.Index
		);
		TriangulateEdgeFan(center, m, cell.Index);
		if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction))
			features.AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
	}
	private void TriangulateRoadAdjacentToRiver(
		HexDirection direction,
		HexCell cell,
		Vector3 center,
		EdgeVertices e
	) {
		bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
		bool previousHasRiver = cell.HasRiverThroughEdge(direction.Previous());
		bool nextHasRiver = cell.HasRiverThroughEdge(direction.Next());
		Vector2 interpolators = GetRoadInterpolators(direction, cell);
		Vector3 roadCenter = center;
		if (cell.HasRiverBeginOrEnd) {
			roadCenter += HexMetrics.GetSolidEdgeMiddle(
				cell.RiverBeginOrEndDirection.Opposite()
			) * (1f / 3f);
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite()) {
			Vector3 corner;
			if (previousHasRiver) {
				if (
					!hasRoadThroughEdge &&
					!cell.HasRoadThroughEdge(direction.Next())
				)
					return;
				corner = HexMetrics.GetSecondSolidCorner(direction);
			}
			else {
				if (
					!hasRoadThroughEdge &&
					!cell.HasRoadThroughEdge(direction.Previous())
				)
					return;
				corner = HexMetrics.GetFirstSolidCorner(direction);
			}
			roadCenter += corner * 0.5f;
			if (cell.IncomingRiver == direction.Next() && (
					cell.HasRoadThroughEdge(direction.Next2()) ||
					cell.HasRoadThroughEdge(direction.Opposite())
				))
				features.AddBridge(roadCenter, center - corner * 0.5f);
			center += corner * 0.25f;
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Previous()) {
			roadCenter -= HexMetrics.GetSecondCorner(cell.IncomingRiver) * 0.2f;
		}
		else if (cell.IncomingRiver == cell.OutgoingRiver.Next()) {
			roadCenter -= HexMetrics.GetFirstCorner(cell.IncomingRiver) * 0.2f;
		}
		else if (previousHasRiver && nextHasRiver) {
			if (!hasRoadThroughEdge) return;
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(direction) *
							HexMetrics.innerToOuter;
			roadCenter += offset * 0.7f;
			center += offset * 0.5f;
		}
		else {
			HexDirection middle;
			if (previousHasRiver)
				middle = direction.Next();
			else if (nextHasRiver)
				middle = direction.Previous();
			else
				middle = direction;
			if (
				!cell.HasRoadThroughEdge(middle) &&
				!cell.HasRoadThroughEdge(middle.Previous()) &&
				!cell.HasRoadThroughEdge(middle.Next())
			)
				return;
			Vector3 offset = HexMetrics.GetSolidEdgeMiddle(middle);
			roadCenter += offset * 0.25f;
			if (
				direction == middle &&
				cell.HasRoadThroughEdge(direction.Opposite())
			)
				features.AddBridge(
					roadCenter,
					center - offset * (HexMetrics.innerToOuter * 0.7f)
				);
		}
		Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
		Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
		TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge, cell.Index);
		if (previousHasRiver) TriangulateRoadEdge(roadCenter, center, mL, cell.Index);
		if (nextHasRiver) TriangulateRoadEdge(roadCenter, mR, center, cell.Index);
	}
	private void TriangulateWithRiverBeginOrEnd(
		HexDirection direction,
		HexCell cell,
		Vector3 center,
		EdgeVertices e
	) {
		var m = new EdgeVertices(
			Vector3.Lerp(center, e.v1, 0.5f),
			Vector3.Lerp(center, e.v5, 0.5f)
		);
		m.v3.y = e.v3.y;
		TriangulateEdgeStrip(
			m, weights1, cell.Index,
			e, weights1, cell.Index
		);
		TriangulateEdgeFan(center, m, cell.Index);
		if (!cell.IsUnderwater) {
			bool reversed = cell.HasIncomingRiver;
			Vector3 indices;
			indices.x = indices.y = indices.z = cell.Index;
			TriangulateRiverQuad(
				m.v2, m.v4, e.v2, e.v4,
				cell.RiverSurfaceY, 0.6f, reversed, indices
			);
			center.y = m.v2.y = m.v4.y = cell.RiverSurfaceY;
			rivers.AddTriangle(center, m.v2, m.v4);
			if (reversed)
				rivers.AddTriangleUV(
					new Vector2(0.5f, 0.4f),
					new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)
				);
			else
				rivers.AddTriangleUV(
					new Vector2(0.5f, 0.4f),
					new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)
				);
			rivers.AddTriangleCellData(indices, weights1);
		}
	}
	private void TriangulateWithRiver(
		HexDirection direction,
		HexCell cell,
		Vector3 center,
		EdgeVertices e
	) {
		Vector3 centerL, centerR;
		if (cell.HasRiverThroughEdge(direction.Opposite())) {
			centerL = center +
					HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
			centerR = center +
					HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
		}
		else if (cell.HasRiverThroughEdge(direction.Next())) {
			centerL = center;
			centerR = Vector3.Lerp(center, e.v5, 2f / 3f);
		}
		else if (cell.HasRiverThroughEdge(direction.Previous())) {
			centerL = Vector3.Lerp(center, e.v1, 2f / 3f);
			centerR = center;
		}
		else if (cell.HasRiverThroughEdge(direction.Next2())) {
			centerL = center;
			centerR = center +
					HexMetrics.GetSolidEdgeMiddle(direction.Next()) *
					(0.5f * HexMetrics.innerToOuter);
		}
		else {
			centerL = center +
					HexMetrics.GetSolidEdgeMiddle(direction.Previous()) *
					(0.5f * HexMetrics.innerToOuter);
			centerR = center;
		}
		center = Vector3.Lerp(centerL, centerR, 0.5f);
		var m = new EdgeVertices(
			Vector3.Lerp(centerL, e.v1, 0.5f),
			Vector3.Lerp(centerR, e.v5, 0.5f),
			1f / 6f
		);
		m.v3.y = center.y = e.v3.y;
		TriangulateEdgeStrip(
			m, weights1, cell.Index,
			e, weights1, cell.Index
		);
		terrain.AddTriangle(centerL, m.v1, m.v2);
		terrain.AddQuad(centerL, center, m.v2, m.v3);
		terrain.AddQuad(center, centerR, m.v3, m.v4);
		terrain.AddTriangle(centerR, m.v4, m.v5);
		Vector3 indices;
		indices.x = indices.y = indices.z = cell.Index;
		terrain.AddTriangleCellData(indices, weights1);
		terrain.AddQuadCellData(indices, weights1);
		terrain.AddQuadCellData(indices, weights1);
		terrain.AddTriangleCellData(indices, weights1);
		if (!cell.IsUnderwater) {
			bool reversed = cell.IncomingRiver == direction;
			TriangulateRiverQuad(
				centerL, centerR, m.v2, m.v4,
				cell.RiverSurfaceY, 0.4f, reversed, indices
			);
			TriangulateRiverQuad(
				m.v2, m.v4, e.v2, e.v4,
				cell.RiverSurfaceY, 0.6f, reversed, indices
			);
		}
	}
	private void TriangulateConnection(
		HexDirection direction,
		HexCell cell,
		EdgeVertices e1
	) {
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null) return;
		Vector3 bridge = HexMetrics.GetBridge(direction);
		bridge.y = neighbor.Position.y - cell.Position.y;
		var e2 = new EdgeVertices(
			e1.v1 + bridge,
			e1.v5 + bridge
		);
		bool hasRiver = cell.HasRiverThroughEdge(direction);
		bool hasRoad = cell.HasRoadThroughEdge(direction);
		if (hasRiver) {
			e2.v3.y = neighbor.StreamBedY;
			Vector3 indices;
			indices.x = indices.z = cell.Index;
			indices.y = neighbor.Index;
			if (!cell.IsUnderwater) {
				if (!neighbor.IsUnderwater)
					TriangulateRiverQuad(
						e1.v2, e1.v4, e2.v2, e2.v4,
						cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.8f,
						cell.HasIncomingRiver && cell.IncomingRiver == direction,
						indices
					);
				else if (cell.Elevation > neighbor.WaterLevel)
					TriangulateWaterfallInWater(
						e1.v2, e1.v4, e2.v2, e2.v4,
						cell.RiverSurfaceY, neighbor.RiverSurfaceY,
						neighbor.WaterSurfaceY, indices
					);
			}
			else if (
				!neighbor.IsUnderwater &&
				neighbor.Elevation > cell.WaterLevel
			) {
				TriangulateWaterfallInWater(
					e2.v4, e2.v2, e1.v4, e1.v2,
					neighbor.RiverSurfaceY, cell.RiverSurfaceY,
					cell.WaterSurfaceY, indices
				);
			}
		}
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
			TriangulateEdgeTerraces(e1, cell, e2, neighbor, hasRoad);
		else
			TriangulateEdgeStrip(
				e1, weights1, cell.Index,
				e2, weights2, neighbor.Index, hasRoad
			);
		features.AddWall(e1, cell, e2, neighbor, hasRiver, hasRoad);
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) {
			Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
			v5.y = nextNeighbor.Position.y;
			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation)
					TriangulateCorner(
						e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor
					);
				else
					TriangulateCorner(
						v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
					);
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation) {
				TriangulateCorner(
					e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell
				);
			}
			else {
				TriangulateCorner(
					v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
				);
			}
		}
	}
	private void TriangulateWaterfallInWater(
		Vector3 v1,
		Vector3 v2,
		Vector3 v3,
		Vector3 v4,
		float y1,
		float y2,
		float waterY,
		Vector3 indices
	) {
		v1.y = v2.y = y1;
		v3.y = v4.y = y2;
		v1 = HexMetrics.Perturb(v1);
		v2 = HexMetrics.Perturb(v2);
		v3 = HexMetrics.Perturb(v3);
		v4 = HexMetrics.Perturb(v4);
		float t = (waterY - y2) / (y1 - y2);
		v3 = Vector3.Lerp(v3, v1, t);
		v4 = Vector3.Lerp(v4, v2, t);
		rivers.AddQuadUnperturbed(v1, v2, v3, v4);
		rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
		rivers.AddQuadCellData(indices, weights1, weights2);
	}
	private void TriangulateCorner(
		Vector3 bottom,
		HexCell bottomCell,
		Vector3 left,
		HexCell leftCell,
		Vector3 right,
		HexCell rightCell
	) {
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);
		if (leftEdgeType == HexEdgeType.Slope) {
			if (rightEdgeType == HexEdgeType.Slope)
				TriangulateCornerTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
			else if (rightEdgeType == HexEdgeType.Flat)
				TriangulateCornerTerraces(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
			else
				TriangulateCornerTerracesCliff(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
		}
		else if (rightEdgeType == HexEdgeType.Slope) {
			if (leftEdgeType == HexEdgeType.Flat)
				TriangulateCornerTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			else
				TriangulateCornerCliffTerraces(
					bottom, bottomCell, left, leftCell, right, rightCell
				);
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			if (leftCell.Elevation < rightCell.Elevation)
				TriangulateCornerCliffTerraces(
					right, rightCell, bottom, bottomCell, left, leftCell
				);
			else
				TriangulateCornerTerracesCliff(
					left, leftCell, right, rightCell, bottom, bottomCell
				);
		}
		else {
			terrain.AddTriangle(bottom, left, right);
			Vector3 indices;
			indices.x = bottomCell.Index;
			indices.y = leftCell.Index;
			indices.z = rightCell.Index;
			terrain.AddTriangleCellData(indices, weights1, weights2, weights3);
		}
		features.AddWall(bottom, bottomCell, left, leftCell, right, rightCell);
	}
	private void TriangulateEdgeTerraces(
		EdgeVertices begin,
		HexCell beginCell,
		EdgeVertices end,
		HexCell endCell,
		bool hasRoad
	) {
		EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
		Color w2 = HexMetrics.TerraceLerp(weights1, weights2, 1);
		float i1 = beginCell.Index;
		float i2 = endCell.Index;
		TriangulateEdgeStrip(begin, weights1, i1, e2, w2, i2, hasRoad);
		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			EdgeVertices e1 = e2;
			Color w1 = w2;
			e2 = EdgeVertices.TerraceLerp(begin, end, i);
			w2 = HexMetrics.TerraceLerp(weights1, weights2, i);
			TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hasRoad);
		}
		TriangulateEdgeStrip(e2, w2, i1, end, weights2, i2, hasRoad);
	}
	private void TriangulateCornerTerraces(
		Vector3 begin,
		HexCell beginCell,
		Vector3 left,
		HexCell leftCell,
		Vector3 right,
		HexCell rightCell
	) {
		Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
		Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
		Color w3 = HexMetrics.TerraceLerp(weights1, weights2, 1);
		Color w4 = HexMetrics.TerraceLerp(weights1, weights3, 1);
		Vector3 indices;
		indices.x = beginCell.Index;
		indices.y = leftCell.Index;
		indices.z = rightCell.Index;
		terrain.AddTriangle(begin, v3, v4);
		terrain.AddTriangleCellData(indices, weights1, w3, w4);
		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color w1 = w3;
			Color w2 = w4;
			v3 = HexMetrics.TerraceLerp(begin, left, i);
			v4 = HexMetrics.TerraceLerp(begin, right, i);
			w3 = HexMetrics.TerraceLerp(weights1, weights2, i);
			w4 = HexMetrics.TerraceLerp(weights1, weights3, i);
			terrain.AddQuad(v1, v2, v3, v4);
			terrain.AddQuadCellData(indices, w1, w2, w3, w4);
		}
		terrain.AddQuad(v3, v4, left, right);
		terrain.AddQuadCellData(indices, w3, w4, weights2, weights3);
	}
	private void TriangulateCornerTerracesCliff(
		Vector3 begin,
		HexCell beginCell,
		Vector3 left,
		HexCell leftCell,
		Vector3 right,
		HexCell rightCell
	) {
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		if (b < 0) b = -b;
		Vector3 boundary = Vector3.Lerp(
			HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b
		);
		Color boundaryWeights = Color.Lerp(weights1, weights3, b);
		Vector3 indices;
		indices.x = beginCell.Index;
		indices.y = leftCell.Index;
		indices.z = rightCell.Index;
		TriangulateBoundaryTriangle(
			begin, weights1, left, weights2, boundary, boundaryWeights, indices
		);
		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			TriangulateBoundaryTriangle(
				left, weights2, right, weights3,
				boundary, boundaryWeights, indices
			);
		}
		else {
			terrain.AddTriangleUnperturbed(
				HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary
			);
			terrain.AddTriangleCellData(
				indices, weights2, weights3, boundaryWeights
			);
		}
	}
	private void TriangulateCornerCliffTerraces(
		Vector3 begin,
		HexCell beginCell,
		Vector3 left,
		HexCell leftCell,
		Vector3 right,
		HexCell rightCell
	) {
		float b = 1f / (leftCell.Elevation - beginCell.Elevation);
		if (b < 0) b = -b;
		Vector3 boundary = Vector3.Lerp(
			HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b
		);
		Color boundaryWeights = Color.Lerp(weights1, weights2, b);
		Vector3 indices;
		indices.x = beginCell.Index;
		indices.y = leftCell.Index;
		indices.z = rightCell.Index;
		TriangulateBoundaryTriangle(
			right, weights3, begin, weights1, boundary, boundaryWeights, indices
		);
		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
			TriangulateBoundaryTriangle(
				left, weights2, right, weights3,
				boundary, boundaryWeights, indices
			);
		}
		else {
			terrain.AddTriangleUnperturbed(
				HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary
			);
			terrain.AddTriangleCellData(
				indices, weights2, weights3, boundaryWeights
			);
		}
	}
	private void TriangulateBoundaryTriangle(
		Vector3 begin,
		Color beginWeights,
		Vector3 left,
		Color leftWeights,
		Vector3 boundary,
		Color boundaryWeights,
		Vector3 indices
	) {
		Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
		Color w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, 1);
		terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
		terrain.AddTriangleCellData(indices, beginWeights, w2, boundaryWeights);
		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v2;
			Color w1 = w2;
			v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
			w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, i);
			terrain.AddTriangleUnperturbed(v1, v2, boundary);
			terrain.AddTriangleCellData(indices, w1, w2, boundaryWeights);
		}
		terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
		terrain.AddTriangleCellData(indices, w2, leftWeights, boundaryWeights);
	}
	private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index) {
		terrain.AddTriangle(center, edge.v1, edge.v2);
		terrain.AddTriangle(center, edge.v2, edge.v3);
		terrain.AddTriangle(center, edge.v3, edge.v4);
		terrain.AddTriangle(center, edge.v4, edge.v5);
		Vector3 indices;
		indices.x = indices.y = indices.z = index;
		terrain.AddTriangleCellData(indices, weights1);
		terrain.AddTriangleCellData(indices, weights1);
		terrain.AddTriangleCellData(indices, weights1);
		terrain.AddTriangleCellData(indices, weights1);
	}
	private void TriangulateEdgeStrip(
		EdgeVertices e1,
		Color w1,
		float index1,
		EdgeVertices e2,
		Color w2,
		float index2,
		bool hasRoad = false
	) {
		terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
		Vector3 indices;
		indices.x = indices.z = index1;
		indices.y = index2;
		terrain.AddQuadCellData(indices, w1, w2);
		terrain.AddQuadCellData(indices, w1, w2);
		terrain.AddQuadCellData(indices, w1, w2);
		terrain.AddQuadCellData(indices, w1, w2);
		if (hasRoad)
			TriangulateRoadSegment(
				e1.v2, e1.v3, e1.v4, e2.v2, e2.v3, e2.v4, w1, w2, indices
			);
	}
	private void TriangulateRiverQuad(
		Vector3 v1,
		Vector3 v2,
		Vector3 v3,
		Vector3 v4,
		float y,
		float v,
		bool reversed,
		Vector3 indices
	) {
		TriangulateRiverQuad(v1, v2, v3, v4, y, y, v, reversed, indices);
	}
	private void TriangulateRiverQuad(
		Vector3 v1,
		Vector3 v2,
		Vector3 v3,
		Vector3 v4,
		float y1,
		float y2,
		float v,
		bool reversed,
		Vector3 indices
	) {
		v1.y = v2.y = y1;
		v3.y = v4.y = y2;
		rivers.AddQuad(v1, v2, v3, v4);
		if (reversed)
			rivers.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
		else
			rivers.AddQuadUV(0f, 1f, v, v + 0.2f);
		rivers.AddQuadCellData(indices, weights1, weights2);
	}
	private void TriangulateRoad(
		Vector3 center,
		Vector3 mL,
		Vector3 mR,
		EdgeVertices e,
		bool hasRoadThroughCellEdge,
		float index
	) {
		if (hasRoadThroughCellEdge) {
			Vector3 indices;
			indices.x = indices.y = indices.z = index;
			Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
			TriangulateRoadSegment(
				mL, mC, mR, e.v2, e.v3, e.v4,
				weights1, weights1, indices
			);
			roads.AddTriangle(center, mL, mC);
			roads.AddTriangle(center, mC, mR);
			roads.AddTriangleUV(
				new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)
			);
			roads.AddTriangleUV(
				new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)
			);
			roads.AddTriangleCellData(indices, weights1);
			roads.AddTriangleCellData(indices, weights1);
		}
		else {
			TriangulateRoadEdge(center, mL, mR, index);
		}
	}
	private void TriangulateRoadEdge(
		Vector3 center,
		Vector3 mL,
		Vector3 mR,
		float index
	) {
		roads.AddTriangle(center, mL, mR);
		roads.AddTriangleUV(
			new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)
		);
		Vector3 indices;
		indices.x = indices.y = indices.z = index;
		roads.AddTriangleCellData(indices, weights1);
	}
	private void TriangulateRoadSegment(
		Vector3 v1,
		Vector3 v2,
		Vector3 v3,
		Vector3 v4,
		Vector3 v5,
		Vector3 v6,
		Color w1,
		Color w2,
		Vector3 indices
	) {
		roads.AddQuad(v1, v2, v4, v5);
		roads.AddQuad(v2, v3, v5, v6);
		roads.AddQuadUV(0f, 1f, 0f, 0f);
		roads.AddQuadUV(1f, 0f, 0f, 0f);
		roads.AddQuadCellData(indices, w1, w2);
		roads.AddQuadCellData(indices, w1, w2);
	}
}