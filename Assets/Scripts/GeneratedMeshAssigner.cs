using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class GeneratedMeshAssigner : MonoBehaviour
{

	public MeshGenerator SourceMeshGenerator;

	private void Start()
	{
		GetComponent<MeshFilter>().mesh = SourceMeshGenerator.GeneratedMesh;
	}

}
