using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleToDoApp
{
    class Program
    {

      

        static void Main(string[] args)
        {
            TaskManager taskManager = new TaskManager();
            //TaskListManager taskListManager = new TaskListManager();
            ShowWelcomeScreen();

            TaskListManager.LoadTasksFromFile();

            while (true)
            {
                ShowMainMenu();
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.KeyChar)
                {
                    case 'a':
                        taskManager.AddTask();
                        break;
                  
                    case 'e':
                        taskManager.EditTask();
                        break;
                    case 'd':
                        taskManager.DeleteTask();
                        break;
                    case 'c':
                        taskManager.CompleteTask();
                        break;
                    case 'u':
                        taskManager.AddSubtask();
                        break;
                    case 's':
                        SortTasks();
                        break;
                    case 'l':
                        TaskListManager.SwitchList();
                        break;
                    case 'x':
                        TaskListManager.DeleteTaskList();
                        break;
                    case 'v':
                        taskManager.UncompleteTask();
                        break;
                    case 'q':
                        return;
                    default:
                        break;
                }
            }
        }

        static void ShowWelcomeScreen()
        {
            Console.Clear();
            string logo = @"
                          _____          _        _   _      _
                         |_   _|        | |      | | (_)    | |
                           | | _ __  ___| |_ __ _| |_ _  ___| | __
                           | || '_ \/ __| __/ _` | __| |/ __| |/ /
                          _| || | | \__ \ || (_| | |_| | (__|   <
                          \___/_| |_|___/\__\__,_|\__|_|\___|_|\_\
";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(logo);
            Console.ForegroundColor = ColorScheme.normalColor;

            Console.WriteLine("Welcome to the ToDo List Manager!");
            Console.WriteLine("An easy-to-use text-based console app for managing your tasks.");
            Console.WriteLine();
            Console.Write("Press 'Enter' to continue...");
            Console.ReadLine();
        }

        


        static void ShowMainMenu()
        {
            Console.Clear();

            DrawTitleBar();

            Console.ForegroundColor = ColorScheme.titleColor;
            //Console.WriteLine("ToDo List Manager - Main Menu\n");
            Console.ResetColor();


            TaskListManager.ShowTasks();
            ShowTasksDueToday();

            ConsoleColor commandKeyColor = ConsoleColor.Yellow;
            ConsoleColor commandDescriptionColor = ConsoleColor.Green;
           

            Console.ResetColor();
            
            DisplayStatusBar();
        }


        static void DrawTitleBar()
        {
            int windowWidth = Console.WindowWidth;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ColorScheme.titleBarColor;
            Console.ForegroundColor = ColorScheme.titleBarTextColor;

            string title = " InstaTick - ToDo List Manager ";
            int titleStart = (windowWidth - title.Length) / 2;
            int paddingLeft = titleStart - 1;
            int paddingRight = windowWidth - title.Length - paddingLeft - 2;

            Console.Write(" " + new string(' ', paddingLeft));
            Console.Write(title);

            string dateTimeText = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            paddingRight -= (dateTimeText.Length);
            Console.Write(new string(' ', paddingRight));

            
            Console.Write(dateTimeText + " ");

            Console.ResetColor();
        }




        static void DisplayStatusBar()
        {
            int currentCursorPosition = Console.CursorTop;

            // Calculate the position of the status bar
            int statusBarTop = Console.WindowHeight - 1;
            Console.SetCursorPosition(0, statusBarTop);

            // Clear the status bar area
            Console.BackgroundColor = ColorScheme.statusBarColor;
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, statusBarTop);

            // Write the status bar content
            Console.ForegroundColor = ColorScheme.statusBarKeyColor;
            Console.Write("Commands: ");
            Console.ForegroundColor = ColorScheme.statusBarDescColor;
            Console.Write("a-Add, e-Edit, d-Delete, c-Complete, u-SubTask, s-Sort, l-Switch List, x-Delete List, q-Quit");

            // Restore the original cursor position
            Console.SetCursorPosition(0, currentCursorPosition);
            Console.ResetColor();
        }


        

        static void SortTasks()
        {
            Console.Clear();
            Console.WriteLine("Sort tasks:\n");

            if (TaskListManager.tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to sort.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Choose a sorting method:");
            Console.WriteLine("1 - By due date");
            Console.WriteLine("2 - By category");
            Console.WriteLine("3 - By completion status");
            Console.WriteLine("4 - By name");

            int sortMethod;
            while (true)
            {
                Console.Write("Enter the number corresponding to your preferred sorting method (1 to 4): ");
                if (int.TryParse(Console.ReadLine(), out sortMethod) && sortMethod >= 1 && sortMethod <= 4)
                {
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            switch (sortMethod)
            {
                case 1:
                    TaskListManager.tasks.Sort((a, b) => a.DueDate.CompareTo(b.DueDate));
                    Console.WriteLine("Tasks sorted by due date.");
                    break;
                case 2:
                    TaskListManager.tasks.Sort((a, b) => a.Category.CompareTo(b.Category));
                    Console.WriteLine("Tasks sorted by category.");
                    break;
                case 3:
                    TaskListManager.tasks.Sort((a, b) => a.IsComplete.CompareTo(b.IsComplete));
                    Console.WriteLine("Tasks sorted by completion status.");
                    break;
                case 4:
                    TaskListManager.tasks.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
                    Console.WriteLine("Tasks sorted by name.");
                    break;
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        private static List<Task> GetTasksDueToday()
        {
            DateTime today = DateTime.Today;
            return TaskListManager.tasks.Where(task => task.DueDate.Date == today).ToList();
        }


        private static void ShowTasksDueToday()
        {
            List<Task> tasksDueToday = GetTasksDueToday();
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            int columnWidth = 20; // Set the width of the right column
            int columnStart = windowWidth - columnWidth - 1;
            int lineStart = 2; // Adjust this value based on the position of the title bar

            Console.SetCursorPosition(columnStart, lineStart);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Tasks Due Today:");
            Console.ResetColor();

            foreach (Task task in tasksDueToday)
            {
                Console.SetCursorPosition(columnStart, ++lineStart);
                Console.ForegroundColor = task.IsComplete ? ColorScheme.completedColor : ColorScheme.normalColor;
                Console.WriteLine(task.Name);
                Console.ResetColor();
            }
        }




       


    }
}
