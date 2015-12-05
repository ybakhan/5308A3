using System;

namespace RingElection.Algorithm
{
  public class AsFarAs : ElectionBase
  {
    public AsFarAs(int id)
        : base(id)
    { }

    protected override void ProcessInitiator()
    {
      SendToRight(new ElectionMessage(Id));
      State = NodeState.Candidate;
    }

    protected override void ProcessAsleep()
    {
      object wakeup;
      if (!LeftPort.TryDequeue(out wakeup))
        return;

      SendToRight(new ElectionMessage(Id));
      var electionMsg = wakeup as ElectionMessage;
      if (electionMsg.Value < mMin)
      {
        SendToRight(electionMsg);
        mMin = electionMsg.Value;
      }
      State = NodeState.Candidate;
    }

    protected override void ProcessCandidate()
    {
      object message;
      if (!LeftPort.TryDequeue(out message))
        return;

      var electionMsg = message as ElectionMessage;
      if (electionMsg != null)
      {
        if (electionMsg.Value < mMin)
        {
          SendToRight(electionMsg);
          mMin = electionMsg.Value;
        }
        else if (electionMsg.Value == Id)
        {
          Notify(new Notification(Id), Direction.Right);
          State = NodeState.Leader;
          Console.WriteLine("Node {0} is leader", Id);
        }
        return;
      }

      var notifyMsg = message as Notification;
      if (notifyMsg != null)
      {
        Notify(notifyMsg, Direction.Right);
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
