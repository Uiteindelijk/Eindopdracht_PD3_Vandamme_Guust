using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallelNode : CompositeNode
{
    public delegate ParallelNodePolicyAccumulator Policy();

    private Policy _policy;

    public ParallelNode(Policy policy, params INode[] nodes) : base(nodes)
    {
        _policy = policy;
    }
    public override IEnumerator<NodeResult> Tick()
    {
        ParallelNodePolicyAccumulator acc = _policy();

        NodeResult returnNodeResult = NodeResult.Failure;

        foreach (INode node in _nodes)
        {
            IEnumerator<NodeResult> result = node.Tick();

            while (result.MoveNext() && result.Current == NodeResult.Running)
            {
                yield return NodeResult.Running;
            }

            returnNodeResult = acc.Policy(result.Current);
        }
        yield return returnNodeResult;
    }
}
