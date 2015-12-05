using System.Collections.Concurrent;

namespace RingElection
{
  public interface INode
  {
    int Id { get; }
    INode Next { get; set; } //right neighbor
    INode Previous { get; set; } //left neighbor
    NodeState State { get; set; }
    ConcurrentQueue<object> LeftPort { get; }
    ConcurrentQueue<object> RightPort { get; }
    int MessagesSent { get; }
    void Elect();
  }
}
