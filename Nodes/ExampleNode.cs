using UnityEngine;

[System.Serializable]
public class ExampleNode : Node 
{
	public const string ID = "exampleNode";
	public override string GetID { get { return ID; } }
    
    public const int width = 100;
    public const int height = 50;
	
	public static ExampleNode Create (Vector2 position) 
	{
		ExampleNode node = CreateInstance<ExampleNode> ();
        Rect NodeRect = new Rect(position.x, position.y, width, height);
		
		node.rect = NodeRect;
		node.name = "Example Node";
		
		NodeInput.Create (node, "in", "Float");
		NodeOutput.Create (node, "out", "Float");
		
		node.InitBase ();
		return node;
	}
	
	public override void SideGUI()
	{
		
	}
	
	public override bool Calculate () 
	{
		if (!allInputsReady ())
			return false;
		Outputs [0].value = (float)Inputs [0].connection.value * 5;
		return true;
	}
}
