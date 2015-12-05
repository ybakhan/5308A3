using System;

namespace RingElection.Algorithm
{
  public class AsFarAsBidirectional : ElectionBase
  {
    public AsFarAsBidirectional(int id)
        : base(id)
    { }

    protected override void ProcessInitiator()
    {
      SendToRight(new ElectionMessage(Id));
      SendToLeft(new ElectionMessage(Id));
      State = NodeState.Candidate;
    }

    protected override void ProcessAsleep()
    {
      object leftWakeup;
      if (LeftPort.TryDequeue(out leftWakeup))
      {
        var electionMsg = leftWakeup as ElectionMessage;
        if (electionMsg.Value < mMin)
        {
          SendToRight(electionMsg);
          mMin = electionMsg.Value;
        }
        else
        {
          ProcessInitiator();
        }
        State = NodeState.Candidate;
        return;
      }

      object rightWakeUp;
      if (!RightPort.TryDequeue(out rightWakeUp))
        return;

      var rightElecMsg = rightWakeUp as ElectionMessage;
      if (rightElecMsg.Value < mMin)
      {
        SendToLeft(rightElecMsg);
        mMin = rightElecMsg.Value;
      }
      else
      {
        ProcessInitiator();
      }
      State = NodeState.Candidate;
    }

    protected override void ProcessCandidate()
    {
      object leftMsg;
      if (LeftPort.TryDequeue(out leftMsg))
      {
        var leftElecMsg = leftMsg as ElectionMessage;
        if (leftElecMsg != null)
        {
          if (leftElecMsg.Value < Id)
          {
            SendToRight(leftElecMsg);
            mMin = leftElecMsg.Value;
          }
          else if (leftElecMsg.Value == Id)
          {
            Notify(new Notification(Id), Direction.Left);
            State = NodeState.Leader;
            Console.WriteLine("Node {0} is leader", Id);
            return;
          }
        }

        var notifyMsg = leftMsg as Notification;
        if (notifyMsg != null)
        {
          Notify(notifyMsg, Direction.Right);
          State = NodeState.Follower;

          Console.WriteLine("Node {0} is follower", Id);
          return;
        }
      }

      object rightMessage;
      if (!RightPort.TryDequeue(out rightMessage))
        return;

      var rightElectMsg = rightMessage as ElectionMessage;
      if (rightElectMsg != null)
      {
        if (rightElectMsg.Value < Id)
        {
          SendToLeft(rightElectMsg);
          mMin = rightElectMsg.Value;
        }
        else if (rightElectMsg.Value == Id)
        {
          Notify(new Notification(Id), Direction.Right);
          State = NodeState.Leader;
          Console.WriteLine("Node {0} is leader", Id);
          return;
        }
      }

      var rightNotify = rightMessage as Notification;
      if (rightNotify != null)
      {
        Notify(rightNotify, Direction.Left);
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
