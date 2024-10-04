using PoProj.classes;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.features;
using PoProj.Publishers;

namespace PoProj
{
    static class Program
    {
        static void Main()
        {
            // Setting up necessary variables
            string dataFilePath = "data/example_data.ftr";
            string updateFile = "data/example.ftre";
            
            // Initializing EventManager
            var eventManager = new EventManager();
            var objectInitializer = new ObjectInitializer(eventManager);
            objectInitializer.InitFromFile(dataFilePath);
            
            #region og main
            ReadFromServer(updateFile, 100, 200, objectInitializer, eventManager);
            
            // Starting GUI thread
            var guiThread = new Thread(ThreadWork.Gui)
            {
                IsBackground = true
            };
            guiThread.Start();
            
            // Starting thread updating gui
            var guiUpdater = new Thread(ThreadWork.GuiUpdater)
            {
                IsBackground = true
            };
            guiUpdater.Start();
            
            // Console support
            ThreadWork.ConsoleRead();
            #endregion
        }

        private static void ReadFromServer(string dataFilePath, int minTimeDiff, int maxTimeDiff, ObjectInitializer objectInitializer, EventManager eventManager)
        {
            // Starting network simulator
            var networkSimulator = new NetworkSimulator(dataFilePath, minTimeDiff, maxTimeDiff, eventManager);
            networkSimulator.Start();
            
            // Starting message reader thread
            var messageReader = new Thread(ThreadWork.MessageRead)
            {
                IsBackground = true
            };
            messageReader.Start(objectInitializer);
        }
    }
}