using System;
using System.Collections.Generic;

public class BehaviorTree
{
    private List<Node> _nodes = new List<Node>();

    public void AddNode(Node node)
    {
        _nodes.Add(node);
    }

    public void Evaluate()
    {
        foreach (var node in _nodes)
        {
            if (!node.Evaluate())
            {
                return;
            }
        }
        // 모든 조건이 만족되면 상태 전이를 실행
    }
}

public abstract class Node
{
    public abstract bool Evaluate();
}

public class ConditionNode : Node
{
    private Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override bool Evaluate()
    {
        return _condition.Invoke();
    }
}