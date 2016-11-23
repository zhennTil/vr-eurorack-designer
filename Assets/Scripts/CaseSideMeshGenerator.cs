using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Generates a mesh based on a list of vertices on the front of the case and a side thickness value, assuming straight case rear.
/// </summary>
public class CaseSideMeshGenerator : MeshGenerator
{

	private Mesh _mesh;
	private List<CaseSideNodePair> _nodePairs = new List<CaseSideNodePair>();
	private float _thickness = 0.02f;

	public override Mesh GeneratedMesh
	{
		get { return _mesh; }
	}

	private void Awake()
	{
		// Set up initial mesh
		_mesh = new Mesh();

		// Set up initial vertices
		_nodePairs.Add(new CaseSideNodePair(0.32f, 0.0f));
		_nodePairs.Add(new CaseSideNodePair(0.32f, 0.16f));
		_nodePairs.Add(new CaseSideNodePair(0.16f, 0.32f));

		RegenerateMesh();
	}

	private void RegenerateMesh()
	{
		// We need two vertices per node pair, per side of the panel.
		int vertexCountPerSide = _nodePairs.Count * 2;
		int triangleCountPerSide = vertexCountPerSide - 2;
		int edgeTriangleCount = vertexCountPerSide * 2 - 2;
		int endCapTriangleCount = 4;

		Vector3[] faceVertices = new Vector3[vertexCountPerSide * 2];
		int[] faceTriangles = new int[(triangleCountPerSide * 2 + edgeTriangleCount + endCapTriangleCount) * 3];

		// Front faces
		GenerateFaceVertices(
			faceVertices,
			0,
			false);
		GenerateFaceTriangleStrip(
			faceTriangles,
			0,
			0,
			vertexCountPerSide);

		// Rear faces
		GenerateFaceVertices(
			faceVertices,
			vertexCountPerSide,
			true);
		GenerateFaceTriangleStrip(
			faceTriangles,
			triangleCountPerSide,
			vertexCountPerSide,
			vertexCountPerSide);

		// Edge faces
		GenerateEdgeFaces(
			faceTriangles,
			faceVertices.Length,
			triangleCountPerSide * 2);
		GenerateEndCaps(
			faceTriangles,
			faceVertices.Length,
			triangleCountPerSide * 2 + edgeTriangleCount);

		_mesh.vertices = faceVertices;
		_mesh.uv = faceVertices.Select(v3 => (Vector2)v3).ToArray();
		_mesh.triangles = faceTriangles;
	}
	
	private void GenerateFaceVertices(Vector3[] outputArray, int start, bool isRearFaces)
	{
		int arrayIndex = start - 1;

		if (isRearFaces)
		{
			foreach (CaseSideNodePair nodePair in _nodePairs)
			{
				outputArray[++arrayIndex] = (Vector3)nodePair.FrontNode + _thickness * Vector3.forward;
				outputArray[++arrayIndex] = (Vector3)nodePair.RearNode + _thickness * Vector3.forward;
			}
		}
		else
		{
			foreach (CaseSideNodePair nodePair in _nodePairs)
			{
				outputArray[++arrayIndex] = (Vector3)nodePair.RearNode - _thickness * Vector3.forward;
				outputArray[++arrayIndex] = (Vector3)nodePair.FrontNode - _thickness * Vector3.forward;
			}
		}
	}
	
	private void GenerateFaceTriangleStrip(int[] outputArray, int startTriangle, int startVertex, int vertexCount)
	{
		for (int triangleIndex = 0; triangleIndex < vertexCount - 2; ++triangleIndex)
		{
			int triangleArrayOffset = startTriangle * 3 + triangleIndex * 3;

			if (triangleIndex % 2 == 0)
			{
				outputArray[triangleArrayOffset + 0] = startVertex + triangleIndex + 0;
				outputArray[triangleArrayOffset + 1] = startVertex + triangleIndex + 2;
				outputArray[triangleArrayOffset + 2] = startVertex + triangleIndex + 1;
			}
			else
			{
				outputArray[triangleArrayOffset + 0] = startVertex + triangleIndex + 0;
				outputArray[triangleArrayOffset + 1] = startVertex + triangleIndex + 1;
				outputArray[triangleArrayOffset + 2] = startVertex + triangleIndex + 2;
			}
		}
	}

	private void GenerateEdgeFaces(int[] outputArray, int totalVertexCount, int startTriangle)
	{
		int halfVertexCount = totalVertexCount / 2;
		for (int startVertex = 0; startVertex < halfVertexCount-1; ++startVertex)
		{
			int i1 = startVertex;
			int i2 = (startVertex + 2) % halfVertexCount;
			int j1 = (startVertex % 2 == 0) ? (i1 + halfVertexCount + 1) : (i1 + halfVertexCount - 1);
			int j2 = (startVertex % 2 == 0) ? (i2 + halfVertexCount + 1) : (i2 + halfVertexCount - 1);

			GenerateTriangleStrip(outputArray, startTriangle + 2 * startVertex, new[] { i1, j1, i2, j2 });
		}
	}

	private void GenerateEndCaps(int[] outputArray, int totalVertexCount, int startTriangle)
	{
		int halfVertexCount = totalVertexCount / 2;

		GenerateTriangleStrip(outputArray, startTriangle, new[] {
			0,
			halfVertexCount + 1,
			1,
			halfVertexCount,
		});
		GenerateTriangleStrip(outputArray, startTriangle + 2, new[] {
			halfVertexCount - 2,
			halfVertexCount - 1,
			totalVertexCount - 1,
			totalVertexCount - 2,
		});
	}

	private void GenerateTriangleStrip(int[] outputArray, int startTriangle, int[] vertexIndices)
	{
		int vertexCount = vertexIndices.Count();

		for (int triangleIndex = 0; triangleIndex < vertexCount - 2; ++triangleIndex)
		{
			int triangleArrayOffset = startTriangle * 3 + triangleIndex * 3;

			if (triangleIndex % 2 == 0)
			{
				outputArray[triangleArrayOffset + 0] = vertexIndices[triangleIndex + 0];
				outputArray[triangleArrayOffset + 1] = vertexIndices[triangleIndex + 2];
				outputArray[triangleArrayOffset + 2] = vertexIndices[triangleIndex + 1];
			}
			else
			{
				outputArray[triangleArrayOffset + 0] = vertexIndices[triangleIndex + 0];
				outputArray[triangleArrayOffset + 1] = vertexIndices[triangleIndex + 1];
				outputArray[triangleArrayOffset + 2] = vertexIndices[triangleIndex + 2];
			}
		}
	}

}
