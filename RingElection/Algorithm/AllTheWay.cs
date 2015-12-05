using System;

namespace RingElection.Algorithm
{
  public class AllTheWay : ElectionBase
  {
    private int mReceived;
    private int mNetworkSize = 1;
    private bool mKnown;

    public AllTheWay(int id)
        : base(id)
    { }

    protected override void ProcessInitiator()
    {
      SendToRight(new ElectionMessage
      {
        Value = Id,
        Counter = mNetworkSize
      });
      mMin = Id;
      State = NodeState.Candidate;
    }

    protected override void ProcessAsleep()
    {
      object wakeup;
      if (!LeftPort.TryDequeue(out wakeup))
        return;

      ProcessInitiator();
      var electionMsg = wakeup as ElectionMessage;
      electionMsg.Counter++;
      SendToRight(electionMsg);
      mMin = Math.Min(mMin, electionMsg.Value);
      mReceived++;
    }

    protected override void ProcessCandidate()
    {
      object message;
      if (!LeftPort.TryDequeue(out message))
        return;

      mReceived++;
      var electionMsg = message as ElectionMessage;
      if (electionMsg.Value != Id)
      {
        electionMsg.Counter++;
        SendToRight(electionMsg);
        mMin = Math.Min(mMin, electionMsg.Value);
        if (mKnown)
          Check();
      }
      else
      {
        mNetworkSize = electionMsg.Counter;
        mKnown = true;
        Check();
      }
    }

    private void Check()
    {
      if (mReceived != mNetworkSize)
        return;

      if (mMin == Id)
      {
        State = NodeState.Leader;
        Console.WriteLine("Node {0} is leader", Id);
      }
      else
      {
        State = NodeState.Follower;
        Console.WriteLine("Node {0} is follower", Id);
      }
    }

    protected override void ProcessPassive()
    {
      throw new NotImplementedException();
    }
  }
}
