using UnityEngine;
using System.Collections.Generic;

public abstract class MeshGenerator : MonoBehaviour
{
	public abstract Mesh GeneratedMesh { get; }
}
