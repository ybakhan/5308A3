using System;
using System.Collections.Concurrent;
using System.Threading;

namespace RingElection.Algorithm
{
  public abstract class ElectionBase : INode
  {
    protected int mMin;
    protected static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(100);

    public int Id { get; private set; }
    public NodeState State { get; set; }
    public int MessagesSent { get; private set; }

    public INode Next { get; set; }
    public INode Previous { get; set; }

    public ConcurrentQueue<object> LeftPort { get; private set; }
    public ConcurrentQueue<object> RightPort { get; private set; }

    protected ElectionBase(int id)
    {
      Id = id;
      mMin = Id;
      State = NodeState.Asleep;
      LeftPort = new ConcurrentQueue<object>();
      RightPort = new ConcurrentQueue<object>();
    }

    public void Elect()
    {
      while (true)
      {
        switch (State)
        {
          case NodeState.Initiator:
            ProcessInitiator();
            break;

          case NodeState.Asleep:
            ProcessAsleep();
            break;

          case NodeState.Candidate:
            ProcessCandidate();
            break;

          case NodeState.Passive:
            ProcessPassive();
            break;

          case NodeState.Follower:
            return;

          case NodeState.Leader:
            return;
        }
        Thread.Sleep(Delay);
      }
    }

    protected abstract void ProcessInitiator();
    protected abstract void ProcessAsleep();
    protected abstract void ProcessCandidate();
    protected abstract void ProcessPassive();

    protected void SendToRight(ElectionMessage message)
    {
      Next.LeftPort.Enqueue(message);
      MessagesSent++;
      Console.WriteLine("Node {0} sent {1} to node {2}; messages sent {3}", Id, message.Value, Next.Id, MessagesSent);
    }

    protected void SendToLeft(ElectionMessage message)
    {
      Previous.RightPort.Enqueue(message);
      MessagesSent++;
      Console.WriteLine("Node {0} sent {1} to node {2}; messages sent {3}", Id, message.Value, Previous.Id, MessagesSent);
    }

    protected void Notify(Notification message, Direction direction)
    {
      if (direction == Direction.Right)
      {
        Next.LeftPort.Enqueue(message);
        Console.WriteLine("Node {0} send notification on right link", Id);
      }
      else
      {
        Previous.RightPort.Enqueue(message);
        Console.WriteLine("Node {0} send notification on left link", Id);
      }
      MessagesSent++;
      Console.WriteLine("Node {0} sent notification to node {1}; messages sent {2}", Id, Next.Id, MessagesSent);
    }

    public override bool Equals(object obj)
    {
      var other = obj as ElectionBase;
      if (other == null)
        return false;

      return Id == other.Id;
    }
  }
}
