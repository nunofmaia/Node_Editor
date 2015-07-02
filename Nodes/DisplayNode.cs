using UnityEngine;

[System.Serializable]
public class DisplayNode : Node 
{
	public const string ID = "displayNode";
	public override string GetID { get { return ID; } }
	
	public const int width = 100;
	public const int height = 50;

	public bool assigned = false;
	public float value = 0;

	public static DisplayNode Create (Vector2 position) 
	{
		DisplayNode node = CreateInstance <DisplayNode> ();
		Rect NodeRect = new Rect(position.x, position.y, width, height);
		
		node.name = "Display Node";
		node.rect = NodeRect;
		
		NodeInput.Create (node, "in", TypeOf.Float);
		
		node.InitBase ();
		return node;
	}
	
	public override void SideGUI()
	{
		GUILayout.Label (new GUIContent ("Value : " + (assigned? value.ToString () : ""), "The input value to display"));		
	}
	
	public override bool Calculate () 
	{
		if (!allInputsReady ()) 
		{
			value = 0;
			assigned = false;
			return false;
		}

		value = (float)Inputs [0].connection.value;
		assigned = true;

		return true;
	}
}
