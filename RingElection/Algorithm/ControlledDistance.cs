using System;

namespace RingElection.Algorithm
{
    public class ControlledDistance : AsFarAs
    {
        protected int mStage = 1;
        private bool mReceivedOnLeftLink;
        private bool mReceivedOnRightLink;

        public ControlledDistance(int id)
            : base(id) { }

        protected override void ProcessInitiator()
        {
            var rightMsg = new ElectionMessage
            {
                Value = Id,
                Distance = GetDistance(),
                Direction = Direction.Right
            };
            SendToRight(rightMsg);

            var leftMsg = new ElectionMessage
            {
                Value = Id,
                Distance = GetDistance(),
                Direction = Direction.Left
            };
            SendToLeft(leftMsg);

            State = NodeState.Candidate;
        }

        #region Asleep state

        protected override void ProcessAsleep()
        {
            object rightMsg;
            if (RightPort.TryDequeue(out rightMsg))
            {
                var message = rightMsg as ElectionMessage;
                if (message != null)
                {
                    ProcessWakeupOnRightLink(message);
                }
            }

            object leftMsg;
            if (LeftPort.TryDequeue(out leftMsg))
            {
                var message = leftMsg as ElectionMessage;
                if (message != null)
                {
                    ProcessWakeupOnLeftLink(message);
                }
            }
        }

        private void ProcessWakeupOnRightLink(ElectionMessage message)
        {
            if (message.Value > Id && State == NodeState.Asleep)
            {
                ProcessInitiator();
            }
            State = NodeState.Candidate;
            ProcessElectionMessageOnRight(message);
        }

        private void ProcessWakeupOnLeftLink(ElectionMessage message)
        {
            if (message.Value > Id && State == NodeState.Asleep)
            {
                ProcessInitiator();
            }
            State = NodeState.Candidate;
            ProcessElectionMessageOnLeft(message);
        }

        #endregion

        #region Candidate state

        protected override void ProcessCandidate()
        {
            object rightMsg;
            if (RightPort.TryDequeue(out rightMsg))
            {
                var electionMessage = rightMsg as ElectionMessage;
                if (electionMessage != null)
                {
                    ProcessElectionMessageOnRight(electionMessage);
                }
            }

            object leftMsg;
            if (LeftPort.TryDequeue(out leftMsg))
            {
                var electionMessage = leftMsg as ElectionMessage;
                if (electionMessage != null)
                {
                    ProcessElectionMessageOnLeft(electionMessage);
                }
            }
        }

        private void ProcessElectionMessageOnRight(ElectionMessage message)
        {
            Console.WriteLine("Node {0} is {1} received {2} on right link", Id, State, message.Value);
            message.Counter++;
            mMin = Math.Min(mMin, message.Value);

            if (message.Value > Id)
                return;

            if (message.Value < Id)
            {
                State = NodeState.Passive;
                Console.WriteLine("Node {0} is passive", Id);
                ProcessMessageOnRight(message);
                return;
            }

            //message.Value == ID
            if (message.Counter == 2 * message.Distance)
            {
                mReceivedOnRightLink = true;
                if (mReceivedOnLeftLink && mReceivedOnRightLink)
                    IncrementStage();
                return;
            }

            if (message.Direction == Direction.Left)
            {
                State = NodeState.Leader;
                Notify(new Notification(Id), Direction.Left);
                Console.WriteLine("Node {0} is leader", Id);
            }
        }

        private void ProcessElectionMessageOnLeft(ElectionMessage message)
        {
           Console.WriteLine("Node {0} is {1} received {2} on left link", Id, State, message.Value);
            message.Counter++;
            mMin = Math.Min(mMin, message.Value);

            if (message.Value > Id)
                return;

            if (message.Value < Id)
            {
                State = NodeState.Passive;
                Console.WriteLine("Node {0} is passive", Id);
                ProcessMessageOnLeft(message);
                return;
            }

            //message.Value == ID
            if (message.Counter == 2 * message.Distance)
            {
                mReceivedOnLeftLink = true;
                if (mReceivedOnLeftLink && mReceivedOnRightLink)
                    IncrementStage();
                return;
            }

            if (message.Direction == Direction.Right)
            {
                State = NodeState.Leader;
                Notify(new Notification(Id), Direction.Right);
                Console.WriteLine("Node {0} is leader", Id);
            }
        }

        private void IncrementStage()
        {
            mStage++;
            ProcessInitiator();
            mReceivedOnLeftLink = false;
            mReceivedOnRightLink = false;
            Console.WriteLine("Node {0} moved to stage {1}", Id, mStage);
        }

        #endregion

        #region Passive state

        protected override void ProcessPassive()
        {
            object rightMsg;
            if (RightPort.TryDequeue(out rightMsg))
            {
                var electionMessage = rightMsg as ElectionMessage;
                if (electionMessage != null)
                {
                    electionMessage.Counter++;
                    ProcessMessageOnRight(electionMessage);
                }

                var notificationMsg = rightMsg as Notification;
                if (notificationMsg != null)
                {
                    ProcessNotification(notificationMsg, Direction.Left);
                    return;
                }
            }

            object leftMsg;
            if (LeftPort.TryDequeue(out leftMsg))
            {
                var electionMessage = leftMsg as ElectionMessage;
                if (electionMessage != null)
                {
                    electionMessage.Counter++;
                    ProcessMessageOnLeft(electionMessage);
                }

                var notificationMsg = leftMsg as Notification;
                if (notificationMsg != null)
                {
                    ProcessNotification(notificationMsg, Direction.Right);
                }
            }
        }

        private void ProcessMessageOnRight(ElectionMessage message)
        {
            Console.WriteLine("Node {0} is {1} and received {2} from right", Id, State, message.Value);
            if (message.Counter != message.Distance)
            {
                SendToLeft(message);
                Console.WriteLine("Node {0} forwarded {1} to left node {2}", Id, message.Value, Previous.Id);
            }
            else
            {
                SendToRight(message);
                Console.WriteLine("Node {0} sent back {1} to right node {2}", Id, message.Value, Next.Id);
            }
        }

        private void ProcessMessageOnLeft(ElectionMessage message)
        {
            Console.WriteLine("Node {0} is {1} and received {2} from left", Id, State, message.Value);
            if (message.Counter != message.Distance)
            {
                SendToRight(message);
                Console.WriteLine("Node {0} forwarded {1} to right node {2}", Id, message.Value, Next.Id);
            }
            else
            {
                SendToLeft(message);
                Console.WriteLine("Node {0} sent back {1} to left node {2}", Id, message.Value, Previous.Id);
            }
        }

        private void ProcessNotification(Notification message, Direction direction)
        {
            State = NodeState.Follower;
            Notify(message, direction);
            Console.WriteLine("Node {0} is follower; leader is {1}", Id, mMin);
        }

        #endregion

        private int GetDistance()
        {
            return (int)Math.Pow(2, mStage - 1);
        }
    }
}
