using UnityEngine;
using System.Collections.Generic;

public struct CaseSideNodePair
{
	public Vector2 FrontNode;
	public Vector2 RearNode;

	public CaseSideNodePair(Vector2 frontNode)
	{
		FrontNode = frontNode;
		RearNode = frontNode - frontNode.x * Vector2.right;
	}
	public CaseSideNodePair(float frontNodeX, float frontNodeY)
		: this(new Vector2(frontNodeX, frontNodeY))
	{}

	public CaseSideNodePair(Vector2 frontNode, Vector2 rearNode)
	{
		FrontNode = frontNode;
		RearNode = rearNode;
	}
}
