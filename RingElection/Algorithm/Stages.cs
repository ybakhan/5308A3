using System;

namespace RingElection.Algorithm
{
    public class Stages : ControlledDistance
    {
        private PortState mLeftState;
        private PortState mRightState;
        private ElectionMessage mLeftMessage;
        private ElectionMessage mRightMessage;

        public Stages(int id)
            : base(id) { }

        protected override void ProcessInitiator()
        {
            Console.WriteLine("Node {0} initiated election", Id);
            var message = new ElectionMessage(Id);
            SendToLeft(message);
            SendToRight(message);
            State = NodeState.Candidate;
            Console.WriteLine("Node {0} became candidate", Id);
        }

        protected override void ProcessAsleep()
        {
            object rightMsg;
            if (mRightState == PortState.Open && RightPort.TryDequeue(out rightMsg))
            {
                mRightMessage = rightMsg as ElectionMessage;
                if (State == NodeState.Asleep)
                {
                    Console.WriteLine("Node {0} waked up by node {1}", Id, mRightMessage.Value);
                    ProcessInitiator();
                }
                mRightState = PortState.Closed;
                Console.WriteLine("Node {0} received {1} on right link, and closed right port", Id, mRightMessage.Value);
            }

            object leftMsg;
            if (mLeftState == PortState.Open && LeftPort.TryDequeue(out leftMsg))
            {
                mLeftMessage = leftMsg as ElectionMessage;
                if (State == NodeState.Asleep)
                {
                    Console.WriteLine("Node {0} waked up by node {1}", Id, mLeftMessage.Value);
                    ProcessInitiator();
                }
                mLeftState = PortState.Closed;
                Console.WriteLine("Node {0} received {1} on left link, and closed left port", Id, mLeftMessage.Value);
            }
        }

        protected override void ProcessCandidate()
        {
            if (mRightState == PortState.Open)
            {
                object rightMsg;
                if (RightPort.TryDequeue(out rightMsg))
                {
                    mRightMessage = rightMsg as ElectionMessage;
                    mRightState = PortState.Closed;
                    Console.WriteLine("Node {0} received {1} on right link, and closed right port", Id, mRightMessage.Value);
                }
            }

            if (mLeftState == PortState.Open)
            {
                object leftMsg;
                if (LeftPort.TryDequeue(out leftMsg))
                {
                    mLeftMessage = leftMsg as ElectionMessage;
                    mLeftState = PortState.Closed;
                    Console.WriteLine("Node {0} received {1} on left link, and closed left port", Id, mLeftMessage.Value);
                }
            }

            if (mRightState == PortState.Closed && mLeftState == PortState.Closed)
            {
                if (Id > mRightMessage.Value || Id > mLeftMessage.Value)
                {
                    State = NodeState.Passive;
                    Console.WriteLine("Node {0} became passive after receiving {1} on left link, and {2} on right link", Id, mLeftMessage.Value, mRightMessage.Value);
                }
                else if (Id == mRightMessage.Value && Id == mLeftMessage.Value)
                {
                    State = NodeState.Leader;
                    Console.WriteLine("Node {0} became leader, because it received {1} on left link and {2} on right link", Id, mLeftMessage.Value, mRightMessage.Value);
                    Notify(new Notification(Id), Direction.Right);
                }
                else
                {
                    var message = new ElectionMessage(Id);
                    SendToLeft(message);
                    SendToRight(message);
                    Console.WriteLine("Node {0} survived stage {1}, now moving to stage {2}", Id, mStage, ++mStage);
                }
                mLeftState = PortState.Open;
                mRightState = PortState.Open;
                Console.WriteLine("Node {0} opened left and right ports", Id);
                mRightMessage = null;
                mLeftMessage = null;
            }
        }

        protected override void ProcessPassive()
        {
            object rightMsg;
            if (RightPort.TryDequeue(out rightMsg))
            {
                var message = rightMsg as ElectionMessage;
                SendToLeft(message);
                Console.WriteLine("Passive node {0} forwarded {1} to left neightbor {2}", Id, message.Value, Previous.Id);
            }

            object leftMsg;
            if (LeftPort.TryDequeue(out leftMsg))
            {
                var message = leftMsg as ElectionMessage;
                if (message != null)
                {
                    SendToRight(message);
                    Console.WriteLine("Passive Node {0} forwarded {1} to right neightbor {2}", Id, message.Value, Next.Id);
                }
                else
                {
                    var notifyMessage = leftMsg as Notification; //notify sent on leader's right link so received on follower's left links
                    if (notifyMessage != null)
                    {
                        Notify(notifyMessage, Direction.Right);
                        State = NodeState.Follower;
                        Console.WriteLine("Passive Node {0} became follower", Id);
                    }
                }
            }
        }

        private enum PortState { Open, Closed }
    }
}
