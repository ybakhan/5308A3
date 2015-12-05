namespace RingElection
{
  public class ElectionMessage
  {
    public int Value { get; set; }
    public int Counter { get; set; }
    public int Distance { get; set; }
    public Direction Direction { get; set; }

    public ElectionMessage(int value)
    {
      Value = value;
    }

    public ElectionMessage(ElectionMessage message)
    {
      Value = message.Value;
    }

    public ElectionMessage() { }
  }
}
