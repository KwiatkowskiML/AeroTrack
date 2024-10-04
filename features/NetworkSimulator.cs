using PoProj.publishers;
using PoProj.Publishers;

namespace PoProj.features;
using NetworkSourceSimulator;

public class NetworkSimulator
{
    private readonly NetworkSourceSimulator _simulator;
    private readonly Thread _simulatorThread;
    private static readonly Queue<Message> MessageQueue = new Queue<Message>();
    private static readonly object QueueLock = new object();
    private readonly EventManager _eventManager;

    // Constructor setting everything up for the simulator to be ready to be run on another thread
    public NetworkSimulator(string dataFilePath, int minDiff, int maxDiff, EventManager eventManager)
    {
        _simulator = new NetworkSourceSimulator(dataFilePath, minDiff, maxDiff);
        _eventManager = eventManager;
        
        // OnNewDataReady event init
        _simulator.OnNewDataReady += (sender, args) =>
        {
            var message = _simulator.GetMessageAt(args.MessageIndex);
            lock (QueueLock)
            {
                MessageQueue.Enqueue(message);
            }
        };
        
        // OnIDUpdate event
        _simulator.OnIDUpdate += (sender, args) =>
        {
            _eventManager.IdUpdate.NotifySubscribers(args.ObjectID, args.NewObjectID);
        };

        // OnPositionUpdate event
        _simulator.OnPositionUpdate += (sender, args) =>
        {
            _eventManager.PosUpdate.NotifySubscribers(args.ObjectID, args.Longitude, args.Latitude, args.AMSL);
        };
        
        // OnContactInfoUpdate event
        _simulator.OnContactInfoUpdate += (sender, args) =>
        {
            _eventManager.ContactUpdate.NotifySubscribers(args.ObjectID, args.PhoneNumber, args.EmailAddress);
        };
        
        _simulatorThread = new Thread(() => _simulator.Run())
        {
            IsBackground = true
        };
    }

    // Starting thread work
    public void Start()
    {
        _simulatorThread.Start();
    }

    // Accessing last message sent
    public static Message GetMessage()
    {
        Message message;
        lock (QueueLock)
        {
            message = MessageQueue.Dequeue();
        }
        return message;
    }
    
    // Checking if the queue is empty
    public static bool IsQueueEmpty()
    {
        bool isEmpty;
        lock (QueueLock)
        {
            isEmpty = MessageQueue.Count == 0;
        }
        return isEmpty;
    }
}