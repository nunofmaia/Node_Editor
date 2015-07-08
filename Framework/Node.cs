using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public abstract class Node : ScriptableObject
{
    public Rect rect = new Rect();
    public List<NodeInput> Inputs = new List<NodeInput>();
    public List<NodeOutput> Outputs = new List<NodeOutput>();
    public List<Node> Nodes = new List<Node>();
    public bool calculated = true;
    // Abstract member to get the ID of the node
    public abstract string GetID { get; }

    /// <summary>
    /// Gets the zoomed rect: the rect of this node how it's actually represented on the screen.
    /// </summary>
    public Rect screenRect
    {
        get
        {
            Rect nodeRect = new Rect(rect);
            nodeRect.position += NodeEditor.zoomPos;
            return NodeEditor.ScaleRect(nodeRect, NodeEditor.zoomPos, new Vector2(1 / NodeEditor.nodeCanvas.zoom, 1 / NodeEditor.nodeCanvas.zoom));
        }
    }

    /// <summary>
    /// Function implemented by the children to draw the node
    /// </summary>
    public void NodeGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        foreach (var input in Inputs)
        {
            input.DisplayLayout();
        }

        GUILayout.EndVertical();
        GUILayout.BeginVertical();

        foreach (var output in Outputs)
        {
            output.DisplayLayout();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public virtual void SideGUI() { }

    /// <summary>
    /// Function implemented by the children to calculate their outputs
    /// Should return Success/Fail
    /// </summary>
    public abstract bool Calculate();

    public object Receive(NodeInput input)
    {
        input.hasResult = false;
        return input.connection.value;
    }

    public void Send(object obj, NodeOutput output)
    {
        output.value = obj;
        output.Notify();
    }

    public NodeInput InputPort(string name)
    {
        return Inputs.Find(i => i.name.Equals(name));
    }

    public NodeOutput OutputPort(string name)
    {
        return Outputs.Find(o => o.name.Equals(name));
    }
    
    public virtual void AddInput(NodeInput input)
    {
        Inputs.Add(input);
    }
    
    public virtual void AddOutput(NodeOutput output)
    {
        Outputs.Add(output);
    }
    
    public Node Clone()
    {
        return (Node)Instantiate(this);
    }

    public virtual void Start() { }

    /// <summary>
    /// Optional callback when the node is deleted
    /// </summary>
    public virtual void OnDelete() { }

    #region Member Functions

    /// <summary>
    /// Checks if there are no unassigned and no null-value inputs.
    /// </summary>
    public bool allInputsReady()
    {
        foreach (var input in Inputs)
        {
            //  if (Inputs [inCnt].connection == null || Inputs [inCnt].connection.value == null)
            //  	return false;
            if (!input.hasResult)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Checks if there are any unassigned inputs.
    /// </summary>
    public bool hasNullInputs()
    {
        foreach (var input in Inputs)
        {
            if (input.connection == null)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Checks if there are any null-value inputs.
    /// </summary>
    public bool hasNullInputValues()
    {
        foreach (var input in Inputs)
        {
            if (input.connection != null && input.connection.value == null)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Recursively checks whether this node is a child of the other node
    /// </summary>
    public bool isChildOf(Node otherNode)
    {
        if (otherNode == null)
            return false;
        foreach (var input in Inputs)
        {
            if (input.connection != null)
            {
                if (input.connection.body == otherNode)
                    return true;
                else if (input.connection.body.isChildOf(otherNode)) // Recursively searching
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Init this node. Has to be called when creating a child node
    /// </summary>
    protected void InitBase()
    {
        //  Calculate ();
        NodeEditor.nodeCanvas.nodes.Add(this);
        if (!String.IsNullOrEmpty(AssetDatabase.GetAssetPath(NodeEditor.nodeCanvas)))
        {
            AssetDatabase.AddObjectToAsset(this, NodeEditor.nodeCanvas);
            foreach (var input in Inputs)
            {
                AssetDatabase.AddObjectToAsset(input, this);
            }

            foreach (var output in Outputs)
            {
                AssetDatabase.AddObjectToAsset(output, this);
            }

            AssetDatabase.ImportAsset(NodeEditor.openedCanvasPath);
            AssetDatabase.Refresh();
        }
    }
    
    /// <summary>
    /// Returns the input knob that is at the position on this node or null
    /// </summary>
    public NodeInput GetInputAtPos(Vector2 pos)
    {
        foreach (var input in Inputs)
        { // Search for an input at the position
            if (input.GetScreenKnob().Contains(new Vector3(pos.x, pos.y)))
                return input;
        }
        return null;
    }
    /// <summary>
    /// Returns the output knob that is at the position on this node or null
    /// </summary>
    public NodeOutput GetOutputAtPos(Vector2 pos)
    {
        foreach (var output in Outputs)
        { // Search for an output at the position
            if (output.GetScreenKnob().Contains(new Vector3(pos.x, pos.y)))
                return output;
        }
        return null;
    }

    /// <summary>
    /// Draws the node knobs; splitted from curves because of the render order
    /// </summary>
    public void DrawKnobs()
    {
        foreach (var output in Outputs)
        {
            GUI.DrawTexture(output.GetGUIKnob(), ConnectionTypes.types[output.type].OutputKnob);
        }
        foreach (var input in Inputs)
        {
            GUI.DrawTexture(input.GetGUIKnob(), ConnectionTypes.types[input.type].InputKnob);
        }
    }
    /// <summary>
    /// Draws the node curves; splitted from knobs because of the render order
    /// </summary>
    public void DrawConnections()
    {
        foreach (var output in Outputs)
        {
            for (int conCnt = 0; conCnt < output.connections.Count; conCnt++)
            {
                NodeEditor.DrawNodeCurve(output.GetGUIKnob().center,
                                           output.connections[conCnt].GetGUIKnob().center,
                                           ConnectionTypes.types[output.type].col);
            }
        }
    }

    /// <summary>
    /// Deletes this Node
    /// </summary>
    public void Delete()
    {
        NodeEditor.nodeCanvas.nodes.Remove(this);
        foreach (var output in Outputs)
        {
            for (int conCnt = 0; conCnt < output.connections.Count; conCnt++)
                output.connections[conCnt].connection = null;
        }
        foreach (var input in Inputs)
        {
            if (input.connection != null)
                input.connection.connections.Remove(input);
        }

        DestroyImmediate(this, true);

        if (!String.IsNullOrEmpty(NodeEditor.openedCanvasPath))
        {
            AssetDatabase.ImportAsset(NodeEditor.openedCanvasPath);
            AssetDatabase.Refresh();
        }
        OnDelete();
    }

    #endregion

    #region Static Functions

    /// <summary>
    /// Check if an output and an input can be connected (same type, ...)
    /// </summary>
    public static bool CanApplyConnection(NodeOutput output, NodeInput input)
    {
        if (input == null || output == null)
            return false;

        if (input.body == output.body || input.connection == output)
            return false;

        if (input.type != output.type)
            return false;

        if (output.body.isChildOf(input.body))
        {
            NodeEditor.editor.ShowNotification(new GUIContent("Recursion detected!"));
            return false;
        }
        return true;
    }

    /// <summary>
    /// Applies a connection between output and input. 'CanApplyConnection' has to be checked before
    /// </summary>
    public static void ApplyConnection(NodeOutput output, NodeInput input)
    {
        if (input != null && output != null)
        {
            if (input.connection != null)
            {
                input.connection.connections.Remove(input);
            }
            input.connection = output;
            output.connections.Add(input);

            //  Node_Editor.editor.RecalculateFrom (input.body);
        }
    }

    #endregion
}
