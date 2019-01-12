using System;

public interface ParallelNodePolicyAccumulator
{
    NodeResult Policy(NodeResult result);
}

public class NSuccesIsSuccesAccumulator : ParallelNodePolicyAccumulator
{ 
    public static ParallelNode.Policy Factory(int n)
    {
        return () => new NSuccesIsSuccesAccumulator(n);
    }

    private readonly int _n;
    private int _count = 0;

    public NSuccesIsSuccesAccumulator(int n = 1)
    {
        _n = n;
    }

    public NodeResult Policy(NodeResult result)
    {
        if (result == NodeResult.Succes)
            _count++;

        return (_count >= _n) ? NodeResult.Succes : NodeResult.Failure;
    }
}