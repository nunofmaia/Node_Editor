using UnityEngine;

[System.Serializable]
public class ExampleNode : Node 
{
	public const string ID = "exampleNode";
	public override string GetID { get { return ID; } }
    
    public const int width = 220;
    public const int height = 100;
	
	public static ExampleNode Create (Vector2 position) 
	{
		ExampleNode node = CreateInstance<ExampleNode> ();
        Rect NodeRect = new Rect(position.x, position.y, width, height);
		
		node.rect = NodeRect;
		node.name = "Example Node";
		
		NodeInput.Create (node, "Value", TypeOf.Float);
		NodeOutput.Create (node, "Output val", TypeOf.Float);
		
		node.InitBase ();
		return node;
	}
	
	public override void NodeGUI () 
	{
		GUILayout.Label ("This is a custom Node!");
		
		GUILayout.Label ("Input");
		if (Event.current.type == EventType.Repaint)
			Inputs [0].SetRect (GUILayoutUtility.GetLastRect ());
			
		if (Event.current.type == EventType.Repaint) 
			Outputs [0].SetRect (GUILayoutUtility.GetLastRect ());
		
	}
	
	public override bool Calculate () 
	{
		if (!allInputsReady ())
			return false;
		Outputs [0].value = (float)Inputs [0].connection.value * 5;
		return true;
	}
}
