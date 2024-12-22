/*
这个代码是一个名为HexFeatureManager的类，它是Unity中的一个MonoBehaviour脚本，用于管理一个基于六角形网格的特征系统。其主要功能包括：添加特殊功能、添加围墙、添加桥梁和添加其他功能等。下面是对代码的逐行解释：

首先，代码定义了一些序列化字段，包括各种预制件的数组（例如城市、农场和植物的预制件集合），围墙对象，塔和桥梁的变换对象等。这些字段将在后续的方法中被使用。

然后有一个Clear方法，它创建一个新的GameObject并设置为其父对象，同时清空围墙。

Apply方法用于应用所有已添加的特征。

PickPrefab方法根据给定的级别和哈希值选择一个预制件。

AddBridge方法创建并设置一个桥梁对象在两个给定的道路中心点上。它使用HexMetrics的Perturb方法来确保桥梁位于六角形的正确位置。然后它将桥梁添加到容器中。

AddFeature方法为一个给定的单元格添加特征。首先，它检查单元格是否是特殊的（如果是，则直接返回）。然后它使用HexHashGrid采样哈希值并选择适当的预制件来创建特征。然后它实例化预制件并设置其位置、旋转和父对象。特殊特征的添加方式略有不同，直接实例化特殊预制件并设置其位置和父对象。

AddWall方法用于添加围墙。它根据邻近单元格的状态（例如是否围起来、是否在水下等）来决定是否需要添加围墙段或墙帽。它还处理河流或道路的情况，添加额外的墙帽。AddWallSegment方法负责创建围墙段，包括计算位置、厚度偏移和高度等，然后将它们添加到围墙对象中。它还处理添加塔的情况。AddWallCap方法创建围墙帽，用于封闭围墙的末端。AddWallWedge方法处理创建斜角围墙的情况。最后是一些私有辅助方法用于处理围墙的各种细节。
*/

﻿using UnityEngine;
public class HexFeatureManager : MonoBehaviour
{
	#region Serialized Fields
	public HexFeatureCollection[]
		urbanCollections, farmCollections, plantCollections;
	public HexMesh walls;
	public Transform wallTower, bridge;
	public Transform[] special;
	#endregion
	private Transform container;
	public void Clear() {
		if (container) Destroy(container.gameObject);
		container = new GameObject("Features Container").transform;
		container.SetParent(transform, false);
		walls.Clear();
	}
	public void Apply() {
		walls.Apply();
	}
	private Transform PickPrefab(
		HexFeatureCollection[] collection,
		int level,
		float hash,
		float choice
	) {
		if (level > 0) {
			float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
			for (int i = 0; i < thresholds.Length; i++) {
				if (hash < thresholds[i]) return collection[i].Pick(choice);
			}
		}
		return null;
	}
	public void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2) {
		roadCenter1 = HexMetrics.Perturb(roadCenter1);
		roadCenter2 = HexMetrics.Perturb(roadCenter2);
		Transform instance = Instantiate(bridge);
		instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
		instance.forward = roadCenter2 - roadCenter1;
		float length = Vector3.Distance(roadCenter1, roadCenter2);
		instance.localScale = new Vector3(
			1f, 1f, length * (1f / HexMetrics.bridgeDesignLength)
		);
		instance.SetParent(container, false);
	}
	public void AddFeature(HexCell cell, Vector3 position) {
		if (cell.IsSpecial) return;
		HexHash hash = HexMetrics.SampleHashGrid(position);
		Transform prefab = PickPrefab(
			urbanCollections, cell.UrbanLevel, hash.a, hash.d
		);
		Transform otherPrefab = PickPrefab(
			farmCollections, cell.FarmLevel, hash.b, hash.d
		);
		float usedHash = hash.a;
		if (prefab) {
			if (otherPrefab && hash.b < hash.a) {
				prefab = otherPrefab;
				usedHash = hash.b;
			}
		}
		else if (otherPrefab) {
			prefab = otherPrefab;
			usedHash = hash.b;
		}
		otherPrefab = PickPrefab(
			plantCollections, cell.PlantLevel, hash.c, hash.d
		);
		if (prefab) {
			if (otherPrefab && hash.c < usedHash) prefab = otherPrefab;
		}
		else if (otherPrefab) {
			prefab = otherPrefab;
		}
		else {
			return;
		}
		Transform instance = Instantiate(prefab);
		position.y += instance.localScale.y * 0.5f;
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
		instance.SetParent(container, false);
	}
	public void AddSpecialFeature(HexCell cell, Vector3 position) {
		HexHash hash = HexMetrics.SampleHashGrid(position);
		Transform instance = Instantiate(special[cell.SpecialIndex - 1]);
		instance.localPosition = HexMetrics.Perturb(position);
		instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
		instance.SetParent(container, false);
	}
	public void AddWall(
		EdgeVertices near,
		HexCell nearCell,
		EdgeVertices far,
		HexCell farCell,
		bool hasRiver,
		bool hasRoad
	) {
		if (
			nearCell.Walled != farCell.Walled &&
			!nearCell.IsUnderwater && !farCell.IsUnderwater &&
			nearCell.GetEdgeType(farCell) != HexEdgeType.Cliff
		) {
			AddWallSegment(near.v1, far.v1, near.v2, far.v2);
			if (hasRiver || hasRoad) {
				AddWallCap(near.v2, far.v2);
				AddWallCap(far.v4, near.v4);
			}
			else {
				AddWallSegment(near.v2, far.v2, near.v3, far.v3);
				AddWallSegment(near.v3, far.v3, near.v4, far.v4);
			}
			AddWallSegment(near.v4, far.v4, near.v5, far.v5);
		}
	}
	public void AddWall(
		Vector3 c1,
		HexCell cell1,
		Vector3 c2,
		HexCell cell2,
		Vector3 c3,
		HexCell cell3
	) {
		if (cell1.Walled) {
			if (cell2.Walled) {
				if (!cell3.Walled) AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
			}
			else if (cell3.Walled) {
				AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
			}
			else {
				AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
			}
		}
		else if (cell2.Walled) {
			if (cell3.Walled)
				AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
			else
				AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
		}
		else if (cell3.Walled) {
			AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
		}
	}
	private void AddWallSegment(
		Vector3 nearLeft,
		Vector3 farLeft,
		Vector3 nearRight,
		Vector3 farRight,
		bool addTower = false
	) {
		nearLeft = HexMetrics.Perturb(nearLeft);
		farLeft = HexMetrics.Perturb(farLeft);
		nearRight = HexMetrics.Perturb(nearRight);
		farRight = HexMetrics.Perturb(farRight);
		Vector3 left = HexMetrics.WallLerp(nearLeft, farLeft);
		Vector3 right = HexMetrics.WallLerp(nearRight, farRight);
		Vector3 leftThicknessOffset =
			HexMetrics.WallThicknessOffset(nearLeft, farLeft);
		Vector3 rightThicknessOffset =
			HexMetrics.WallThicknessOffset(nearRight, farRight);
		float leftTop = left.y + HexMetrics.wallHeight;
		float rightTop = right.y + HexMetrics.wallHeight;
		Vector3 v1, v2, v3, v4;
		v1 = v3 = left - leftThicknessOffset;
		v2 = v4 = right - rightThicknessOffset;
		v3.y = leftTop;
		v4.y = rightTop;
		walls.AddQuadUnperturbed(v1, v2, v3, v4);
		Vector3 t1 = v3, t2 = v4;
		v1 = v3 = left + leftThicknessOffset;
		v2 = v4 = right + rightThicknessOffset;
		v3.y = leftTop;
		v4.y = rightTop;
		walls.AddQuadUnperturbed(v2, v1, v4, v3);
		walls.AddQuadUnperturbed(t1, t2, v3, v4);
		if (addTower) {
			Transform towerInstance = Instantiate(wallTower);
			towerInstance.transform.localPosition = (left + right) * 0.5f;
			Vector3 rightDirection = right - left;
			rightDirection.y = 0f;
			towerInstance.transform.right = rightDirection;
			towerInstance.SetParent(container, false);
		}
	}
	private void AddWallSegment(
		Vector3 pivot,
		HexCell pivotCell,
		Vector3 left,
		HexCell leftCell,
		Vector3 right,
		HexCell rightCell
	) {
		if (pivotCell.IsUnderwater) return;
		bool hasLeftWall = !leftCell.IsUnderwater &&
							pivotCell.GetEdgeType(leftCell) != HexEdgeType.Cliff;
		bool hasRighWall = !rightCell.IsUnderwater &&
							pivotCell.GetEdgeType(rightCell) != HexEdgeType.Cliff;
		if (hasLeftWall) {
			if (hasRighWall) {
				bool hasTower = false;
				if (leftCell.Elevation == rightCell.Elevation) {
					HexHash hash = HexMetrics.SampleHashGrid(
						(pivot + left + right) * (1f / 3f)
					);
					hasTower = hash.e < HexMetrics.wallTowerThreshold;
				}
				AddWallSegment(pivot, left, pivot, right, hasTower);
			}
			else if (leftCell.Elevation < rightCell.Elevation) {
				AddWallWedge(pivot, left, right);
			}
			else {
				AddWallCap(pivot, left);
			}
		}
		else if (hasRighWall) {
			if (rightCell.Elevation < leftCell.Elevation)
				AddWallWedge(right, pivot, left);
			else
				AddWallCap(right, pivot);
		}
	}
	private void AddWallCap(Vector3 near, Vector3 far) {
		near = HexMetrics.Perturb(near);
		far = HexMetrics.Perturb(far);
		Vector3 center = HexMetrics.WallLerp(near, far);
		Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);
		Vector3 v1, v2, v3, v4;
		v1 = v3 = center - thickness;
		v2 = v4 = center + thickness;
		v3.y = v4.y = center.y + HexMetrics.wallHeight;
		walls.AddQuadUnperturbed(v1, v2, v3, v4);
	}
	private void AddWallWedge(Vector3 near, Vector3 far, Vector3 point) {
		near = HexMetrics.Perturb(near);
		far = HexMetrics.Perturb(far);
		point = HexMetrics.Perturb(point);
		Vector3 center = HexMetrics.WallLerp(near, far);
		Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);
		Vector3 v1, v2, v3, v4;
		Vector3 pointTop = point;
		point.y = center.y;
		v1 = v3 = center - thickness;
		v2 = v4 = center + thickness;
		v3.y = v4.y = pointTop.y = center.y + HexMetrics.wallHeight;
		walls.AddQuadUnperturbed(v1, point, v3, pointTop);
		walls.AddQuadUnperturbed(point, v2, pointTop, v4);
		walls.AddTriangleUnperturbed(pointTop, v3, v4);
	}
}