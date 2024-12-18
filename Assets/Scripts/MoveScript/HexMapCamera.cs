/*
这是一个关于HexMapCamera的Unity脚本，主要功能是控制摄像机在六角形地图上的行为。以下是详细的代码功能解释：

1. 定义了一个HexMapCamera类，继承自MonoBehaviour，用于创建Unity中的行为。
2. 在类内部定义了一些可序列化的字段（Serialized Fields），包括最小和最大缩放值、旋转速度、移动速度等。还有一个HexGrid字段，可能是用来表示地图的网格。
3. 在Awake函数中，获取子对象swivel和stick的Transform组件。
4. 在Update函数中，通过获取鼠标滚轮、旋转和移动输入来调整摄像机的缩放、旋转和位置。
5. 定义了一个静态方法ValidatePosition()，用于将摄像机的位置调整到默认位置。
6. 调整缩放（AdjustZoom）的方法是通过调整stick对象的localPosition和swivel对象的localRotation来实现的，依赖于输入的delta值。
7. 调整旋转（AdjustRotation）的方法是通过改变摄像机的localRotation来实现的，依赖于输入的delta值和旋转速度。
8. 调整位置（AdjustPosition）的方法是通过改变摄像机的localPosition来实现的，依赖于输入的xDelta和zDelta值，同时根据当前的缩放值来调整移动速度。
9. ClampPosition方法用于限制摄像机的位置，确保它在地图的边界内。它通过计算地图的边界来限制摄像机的x和z坐标。
10. Locked属性是一个静态属性，用于控制摄像机的启用/禁用状态。当该属性被设置为true时，摄像机将被禁用；当被设置为false时，摄像机将被启用。
*/

﻿using UnityEngine;
public class HexMapCamera : MonoBehaviour
{
	private static HexMapCamera instance;
	#region Serialized Fields
	public float stickMinZoom, stickMaxZoom;
	public float swivelMinZoom, swivelMaxZoom;
	public float moveSpeedMinZoom, moveSpeedMaxZoom;
	public float rotationSpeed;
	public HexGrid grid;
	#endregion
	private float rotationAngle;
	private Transform swivel, stick;
	private float zoom = 1f;
	public static bool Locked { set => instance.enabled = !value; }
	#region Event Functions
	private void Awake() {
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
	}
	private void Update() {
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
		if (zoomDelta != 0f) AdjustZoom(zoomDelta);
		float rotationDelta = Input.GetAxis("Rotation");
		if (rotationDelta != 0f) AdjustRotation(rotationDelta);
		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");
		if (xDelta != 0f || zDelta != 0f) AdjustPosition(xDelta, zDelta);
	}
	private void OnEnable() {
		instance = this;
	}
	#endregion
	public static void ValidatePosition() {
		instance.AdjustPosition(0f, 0f);
	}
	private void AdjustZoom(float delta) {
		zoom = Mathf.Clamp01(zoom + delta);
		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);
		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}
	private void AdjustRotation(float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f)
			rotationAngle += 360f;
		else if (rotationAngle >= 360f) rotationAngle -= 360f;
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}
	private void AdjustPosition(float xDelta, float zDelta) {
		Vector3 direction =
			transform.localRotation *
			new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance =
			Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
			damping * Time.deltaTime;
		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = ClampPosition(position);
	}
	private Vector3 ClampPosition(Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);
		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);
		return position;
	}
}