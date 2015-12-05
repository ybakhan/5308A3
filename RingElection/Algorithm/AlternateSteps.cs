using System;

namespace RingElection.Algorithm
{
    public class AlternateSteps : Stages
    {
        private Direction mStep;

        public AlternateSteps(int id)
            : base(id)
        {
            mStep = Direction.Right;
        }

        protected override void ProcessInitiator()
        {
            Console.WriteLine("Node {0} initiated election", Id);
            var message = new ElectionMessage(Id);
            SendToRight(message);
            State = NodeState.Candidate;
            Console.WriteLine("Node {0} became candidate", Id);
        }

        protected override void ProcessAsleep()
        {
            object leftMsg;
            if (LeftPort.TryDequeue(out leftMsg))
            {
                var message = leftMsg as ElectionMessage;
                Console.WriteLine("Node {0} waked up by node {1}", Id, message.Value);
                SendToRight(new ElectionMessage(Id));
                if (message.Value > Id)
                {
                    State = NodeState.Candidate;
                    mStage++;
                    SendToLeft(new ElectionMessage(Id));
                    mStep = Direction.Left;
                    Console.WriteLine("Node {0} became candidate", Id);
                }
                else if (message.Value < Id)
                {
                    State = NodeState.Passive;
                    Console.WriteLine("Node {0} became passive", Id);
                }
            }
        }

        protected override void ProcessCandidate()
        {
            object rightMsg;
            if (mStep == Direction.Left && RightPort.TryDequeue(out rightMsg))
            {
                mStep = Direction.Right;
                var message = rightMsg as ElectionMessage;
                if (message.Value > Id)
                {
                    mStage++;
                    Console.WriteLine("Node {0} received {1} on right link; moved to stage {2}", Id, message.Value, mStage);
                    SendToRight(new ElectionMessage(Id));
                }
                else if (message.Value < Id)
                {
                    State = NodeState.Passive;
                    Console.WriteLine("Node {0} became passive after receiving {1} on right link", Id, message.Value);
                }
                else
                {
                    State = NodeState.Leader;
                    Notify(new Notification(Id), Direction.Right);
                    Console.WriteLine("Node {0} became leader after receiving {1} on right link, and sent notification to right neighbor {2}", Id, message.Value, Next.Id);
                }
                return;
            }

            object leftMsg;
            if (mStep == Direction.Right && LeftPort.TryDequeue(out leftMsg))
            {
                mStep = Direction.Left;
                var message = leftMsg as ElectionMessage;
                if (message.Value > Id)
                {
                    mStage++;
                    Console.WriteLine("Node {0} received {1} on left link; moved to stage {2}", Id, message.Value, mStage);
                    SendToLeft(new ElectionMessage(Id));
                }
                else if (message.Value < Id)
                {
                    State = NodeState.Passive;
                    Console.WriteLine("Node {0} became passive after receiving {1} on left link", Id, message.Value);
                }
                else
                {
                    State = NodeState.Leader;
                    Notify(new Notification(Id), Direction.Left);
                    Console.WriteLine("Node {0} became leader after receiving {1} on left link and sent notification to left neighbor {2}", Id, message.Value, Previous.Id);
                }
            }
        }

        protected override void ProcessPassive()
        {
            object rightMsg;
            if (RightPort.TryDequeue(out rightMsg))
            {
                var message = rightMsg as ElectionMessage;
                if (message != null)
                {
                    SendToLeft(message);
                    Console.WriteLine("Passive node {0} forwarded {1} to left neighbor {2}", Id, message.Value, Previous.Id);
                }

                var notifcation = rightMsg as Notification;
                if (notifcation != null)
                {
                    Notify(notifcation, Direction.Left);
                    State = NodeState.Follower;
                    Console.WriteLine("Passive node {0} became follower", Id);
                }
                return;
            }

            object leftMsg;
            if (LeftPort.TryDequeue(out leftMsg))
            {
                var message = leftMsg as ElectionMessage;
                if (message != null)
                {
                    SendToRight(message);
                    Console.WriteLine("Passive node {0} forwarded {1} to right neighbor {2}", Id, message.Value, Next.Id);
                }

                var notifcation = leftMsg as Notification;
                if (notifcation != null)
                {
                    Notify(notifcation, Direction.Right);
                    State = NodeState.Follower;
                    Console.WriteLine("Passive node {0} became follower", Id);
                }
            }
        }
    }
}
