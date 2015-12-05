using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RingElection
{
  public class Ring : IEnumerable<INode>
  {
    public INode Tail { get; private set; }
    public INode Head { get; private set; }
    public int Count { get; private set; }

    public Ring(IEnumerable<INode> collection)
    {
      foreach (INode item in collection)
        AddLast(item);
    }

    public INode Elect()
    {
      return Elect(this.Select(n => n.Id));
    }

    public INode Elect(IEnumerable<int> initiators)
    {
      foreach (var initId in initiators)
      {
        var node = this.Single(n => n.Id == initId);
        node.State = NodeState.Initiator;
      }

      Parallel.ForEach(this, n => n.Elect());
      return this.Single(n => n.State == NodeState.Leader);
    }

    public void AddLast(INode item)
    {
      // if Head is null, then this will be the first item
      if (Head == null)
      {
        Head = item;
        Tail = Head;
        Head.Next = Tail;
        Head.Previous = Tail;
      }
      else
      {
        var newNode = item;
        Tail.Next = newNode;
        newNode.Next = Head;
        newNode.Previous = Tail;
        Tail = newNode;
        Head.Previous = Tail;
      }
      ++Count;
    }

    public INode Find(INode item)
    {
      INode node = FindNode(Head, item);
      return node;
    }

    private INode FindNode(INode node, INode valueToCompare)
    {
      INode result = null;
      if (node.Equals(valueToCompare)/*comparer.Equals(node, valueToCompare)*/)
        result = node;
      else if (result == null && node.Next != Head)
        result = FindNode(node.Next, valueToCompare);
      return result;
    }

    public IEnumerator<INode> GetEnumerator()
    {
      INode current = Head;
      if (current != null)
      {
        do
        {
          yield return current;
          current = current.Next;
        } while (current != Head);
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
