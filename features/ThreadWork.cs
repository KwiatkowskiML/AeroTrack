using FlightTrackerGUI;
using PoProj.CommandsSupport;
using PoProj.DataStorage;
using PoProj.MediaClasses;

namespace PoProj.features;

public static class ThreadWork
{
    // Reading messages from the queue
    public static void MessageRead(object? objectInitializer)
    {
        while (true)
        {
            if (!NetworkSimulator.IsQueueEmpty())
            {
                var message = NetworkSimulator.GetMessage();
                ((ObjectInitializer)objectInitializer!).AddBytes(message.MessageBytes);
            }
        }
    }

    // Reading commands from command line
    public static void ConsoleRead()
    {
        string? command = "";
        var mediaList = new List<IMedia>
        {
            new Television("Telewizja abelowa"),
            new Television("Kanał TV-tensor"),
            new Radio("Radio Kwantyfikator"),
            new Radio("Radio Shmem"),
            new Newspaper("Gazeta Kategoryczna"),
            new Newspaper("Dziennik Politechniczny")
        };
        
        do
        {
            Console.WriteLine("Enter command:");
            command = Console.ReadLine();
            if (command == null)
                continue;

            command = command.ToLower();
            var tokens = command.Split(' ');
            
            switch (tokens[0])
            {
                case "print":
                {
                    string path = $"snapshots/snapshot_{DateTime.Now:HH_mm_ss}.json";
                    DataSingleton.Instance.JsonSerialization(path);
                    break;
                }
                case "report":
                {
                    var reportableList = DataSingleton.Instance.GetReportableList();
                    var newsGenerator = new NewsGenerator(mediaList, reportableList);

                    string? report;
                    while ((report = newsGenerator.GenerateNextNews()) != null)
                    {
                        Console.WriteLine(report);
                    }

                    break;
                }
                case "display":
                {
                    var displayQuery = new DisplayQuery(command);
                    DataSingleton.Instance.Execute(displayQuery);
                    break;
                }
                case "delete":
                {
                    var deleteQuery = new DeleteQuery(command);
                    DataSingleton.Instance.Execute(deleteQuery);
                    break;
                }
                case "add":
                {
                    var addQuery = new AddQuery(command);
                    DataSingleton.Instance.Execute(addQuery);
                    break;
                }
                case "update":
                {
                    var updateQuery = new UpdateQuery(command);
                    DataSingleton.Instance.Execute(updateQuery);
                    break;
                }
            }
        } while (command != "exit");
    }

    // Updating GUI
    public static void GuiUpdater()
    {
        while(true)
        {
            // Setting up current time
            var currentTime = DateTime.Now;
            
            // Updating GUI
            var flightGuiData = new FlightsGUIData(DataSingleton.Instance.GetCurrentFlightsGui(currentTime));
            Runner.UpdateGUI(flightGuiData);
            
            // Waiting one second
            Thread.Sleep(1000);
        }
    }

    public static void Gui()
    {
        Runner.Run();
    }
}