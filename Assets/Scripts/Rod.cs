using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct TransformSnapshot
{
	public Vector3 position;
	public Quaternion rotation;

	public TransformSnapshot(Transform t)
	{
		position = t.position;
		rotation = t.rotation;
	}

	public void ApplyTo(Transform t)
	{
		t.position = position;
		t.rotation = rotation;
	}
}

public class Rod : MonoBehaviour
{
	public List<TransformSnapshot> snapshots = new List<TransformSnapshot>();


	/// <summary>
	/// Adds the current transform as a new snapshot to the list.
	/// </summary>
	[ContextMenu("Add Snapshot")]
	public void AddSnapshot()
	{
		snapshots.Add(new TransformSnapshot(transform));
	}

	/// <summary>
	/// Applies the snapshot at the given index to the transform.
	/// </summary>
	/// <param name="index">Index in the snapshots list.</param>
	public void ApplySnapshotAt(int index)
	{
		if (index < 0 || index >= snapshots.Count)
		{
			Debug.LogWarning($"Snapshot index {index} is out of range.");
			return;
		}
		snapshots[index].ApplyTo(transform);
	}

	/// <summary>
	/// Captures and overwrites the snapshot at the given index.
	/// </summary>
	/// <param name="index">Index in the snapshots list.</param>
	public void SetSnapshotAt(int index)
	{
		if (index < 0 || index >= snapshots.Count)
		{
			Debug.LogWarning($"Snapshot index {index} is out of range.");
			return;
		}
		snapshots[index] = new TransformSnapshot(transform);
	}
}
