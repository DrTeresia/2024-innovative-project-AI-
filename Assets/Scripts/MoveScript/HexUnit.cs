/*
这是一个名为HexUnit的Unity脚本，用于处理游戏中的单位（Unit）的行为和属性。以下是每个部分的简单解释：

1. **常量定义**: 定义了一些常量，如旋转速度（rotationSpeed）和旅行速度（travelSpeed）。


```csharp
private const float rotationSpeed = 180f;
private const float travelSpeed = 4f;
```
2. **类变量**:


	* `HexUnit unitPrefab`: 预定义的一个HexUnit模型实例。
	* `HexCell location`和`currentTravelLocation`: 分别代表单位当前所在和当前旅行所在位置的单元格。
	* `float orientation`: 单位的朝向角度。
	* `List<HexCell> pathToTravel`: 存储单位需要前往的路径上的单元格列表。这些单元构成了一种路线系统，可以让单位移动从一个地点到另一个地点。等等其他一些变量如Grid（代表单位所在的网格）、Speed（单位的速度）、VisionRange（单位的视野范围）等。
3. **事件函数**: 当单位被启用时，会检查其位置是否有效并更新位置信息。例如，当单位被激活时(`OnEnable()`)，它将根据其位置来更新自身的位置和朝向信息。这在游戏中每当游戏对象激活时会触发，确保单位是有效的和放在正确的位置。其中也定义了旅行(`Travel`)方法，验证位置(`ValidateLocation`)方法等事件处理函数。旅行函数包含了移动到路径中的每个点以及路径完成之后的一些动作的逻辑。还包含了判断目的地是否有效的函数(`IsValidDestination`)。最后还有单位的死亡(`Die`)函数用于销毁游戏对象并降低其当前位置的可视性。以及用于保存和加载单位状态的函数。此外还有一个关于移动的代价计算函数`GetMoveCost`，根据源和目标单元格以及移动方向计算移动代价，以便后续计算行动规则。除此之外的私有函数大都是实现路径绘制相关逻辑等细节逻辑代码实现逻辑过程的功能方法，在玩家创建的新游戏中具有关键作用。总的来说，这个脚本主要负责处理游戏中的单位的行为和属性，包括移动、朝向、视野、移动代价等。同时还有一些关于路径的绘制以及数据的保存和加载等功能实现逻辑细节的实现方法部分在注释中被暂时关闭（因为不是代码运行关键部分），可以在需要的时候开启使用。
*/

﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class HexUnit : MonoBehaviour
{
	private const float rotationSpeed = 180f;
	private const float travelSpeed = 4f;
	public static HexUnit unitPrefab;
	private HexCell location, currentTravelLocation;
	private float orientation;
	private List<HexCell> pathToTravel;
	public HexGrid Grid { get; set; }
	public HexCell Location {
		get => location;
		set {
			if (location) {
				Grid.DecreaseVisibility(location, VisionRange);
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
			Grid.IncreaseVisibility(value, VisionRange);
			transform.localPosition = value.Position;
		}
	}
	public float Orientation {
		get => orientation;
		set {
			orientation = value;
			transform.localRotation = Quaternion.Euler(0f, value, 0f);
		}
	}
	public int Speed => 24;
	public int VisionRange => 3;
	#region Event Functions
	private void OnEnable() {
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
				Grid.IncreaseVisibility(location, VisionRange);
				Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
				currentTravelLocation = null;
			}
		}
	}
	#endregion
	public void ValidateLocation() {
		transform.localPosition = location.Position;
	}
	public bool IsValidDestination(HexCell cell) => cell.IsExplored && !cell.IsUnderwater && !cell.Unit;
	public void Travel(List<HexCell> path) {
		location.Unit = null;
		location = path[path.Count - 1];
		location.Unit = this;
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
	}
	private IEnumerator TravelPath() {
		Vector3 a, b, c = pathToTravel[0].Position;
		yield return LookAt(pathToTravel[1].Position);
		Grid.DecreaseVisibility(
			currentTravelLocation ? currentTravelLocation : pathToTravel[0],
			VisionRange
		);
		float t = Time.deltaTime * travelSpeed;
		for (int i = 1; i < pathToTravel.Count; i++) {
			currentTravelLocation = pathToTravel[i];
			a = c;
			b = pathToTravel[i - 1].Position;
			c = (b + currentTravelLocation.Position) * 0.5f;
			Grid.IncreaseVisibility(pathToTravel[i], VisionRange);
			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation(d);
				yield return null;
			}
			Grid.DecreaseVisibility(pathToTravel[i], VisionRange);
			t -= 1f;
		}
		currentTravelLocation = null;
		a = c;
		b = location.Position;
		c = b;
		Grid.IncreaseVisibility(location, VisionRange);
		for (; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation(d);
			yield return null;
		}
		transform.localPosition = location.Position;
		orientation = transform.localRotation.eulerAngles.y;
		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;
	}
	private IEnumerator LookAt(Vector3 point) {
		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);
		if (angle > 0f) {
			float speed = rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed;
				t < 1f;
				t += Time.deltaTime * speed
			) {
				transform.localRotation =
					Quaternion.Slerp(fromRotation, toRotation, t);
				yield return null;
			}
		}
		transform.LookAt(point);
		orientation = transform.localRotation.eulerAngles.y;
	}
	public int GetMoveCost(
		HexCell fromCell,
		HexCell toCell,
		HexDirection direction
	) {
		if (!IsValidDestination(toCell)) return -1;
		HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
		if (edgeType == HexEdgeType.Cliff) return -1;
		int moveCost;
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = 1;
		}
		else if (fromCell.Walled != toCell.Walled) {
			return -1;
		}
		else {
			moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
			moveCost +=
				toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		}
		return moveCost;
	}
	public void Die() {
		if (location) Grid.DecreaseVisibility(location, VisionRange);
		location.Unit = null;
		Destroy(gameObject);
	}
	public void Save(BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
	}
	public static void Load(BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddUnit(
			Instantiate(unitPrefab), grid.GetCell(coordinates), orientation
		);
	}
}