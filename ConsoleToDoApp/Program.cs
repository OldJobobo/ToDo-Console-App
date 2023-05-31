using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsoleToDoApp
{
    class Program
    {
        static Dictionary<string, List<Task>> taskLists = new Dictionary<string, List<Task>>();
        static List<Task> tasks;
        static string currentList = "default";

       

        static ConsoleColor titleColor = ConsoleColor.Cyan;
        static ConsoleColor urgentColor = ConsoleColor.Red;
        static ConsoleColor importantColor = ConsoleColor.Yellow;
        static ConsoleColor normalColor = ConsoleColor.Green;
        static ConsoleColor completedColor = ConsoleColor.Magenta;
        static ConsoleColor commandsColor = ConsoleColor.Magenta;

        static void Main(string[] args)
        {
            TaskManager taskManager = new TaskManager();
            ShowWelcomeScreen();

            TaskManager.LoadTasksFromFile();

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
                    case 's':
                        SortTasks();
                        break;
                    case 'l':
                        SwitchList();
                        break;
                    case 'x': 
                        DeleteTaskList();
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
            Console.ForegroundColor = normalColor;

            Console.WriteLine("Welcome to the ToDo List Manager!");
            Console.WriteLine("An easy-to-use text-based console app for managing your tasks.");
            Console.WriteLine();
            Console.Write("Press 'Enter' to continue...");
            Console.ReadLine();
        }

        static void ShowTasks()
        {
           

            // Display colorful title with current task list information
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Current Task List: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(currentList);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ({tasks.Count} tasks)");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nTasks:\n");
            Console.ResetColor();

            if (tasks.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("No tasks yet. Add a task by pressing 'a'.\n");
                Console.ResetColor();
            }
            else
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    Task task = tasks[i];

                    string taskStatus = task.IsComplete ? "[x]" : "[ ]";
                    ConsoleColor taskColor = ConsoleColor.White;

                    switch (task.Category)
                    {
                        case TaskCategory.Urgent:
                            taskColor = urgentColor;
                            break;
                        case TaskCategory.Important:
                            taskColor = importantColor;
                            break;
                        case TaskCategory.Normal:
                            taskColor = normalColor;
                            break;
                    }

                    if (task.IsComplete)
                    {
                        taskColor = completedColor;
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{i + 1}. ");
                    Console.ResetColor();
                    Console.Write($"{taskStatus} ");
                    Console.ForegroundColor = taskColor;
                    Console.Write(task.Name);
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" - Due: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{task.DueDate.ToShortDateString()}");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }


        static void ShowMainMenu()
        {
            Console.Clear();

            DrawTitleBar();

            Console.ForegroundColor = titleColor;
            //Console.WriteLine("ToDo List Manager - Main Menu\n");
            Console.ResetColor();


            ShowTasks();
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
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;

            string title = " InstaTick - ToDo List Manager ";
            int titleStart = (windowWidth - title.Length) / 2;
            int paddingLeft = titleStart - 1;
            int paddingRight = windowWidth - title.Length - paddingLeft - 2;

            Console.Write(" " + new string(' ', paddingLeft));
            Console.Write(title);
            Console.Write(new string(' ', paddingRight) + " ");

            Console.ResetColor();
        }



        static void DisplayStatusBar()
        {
            int currentCursorPosition = Console.CursorTop;

            // Calculate the position of the status bar
            int statusBarTop = Console.WindowHeight - 1;
            Console.SetCursorPosition(0, statusBarTop);

            // Clear the status bar area
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, statusBarTop);

            // Write the status bar content
            ConsoleColor commandKeyColor = ConsoleColor.Yellow;
            ConsoleColor commandDescriptionColor = ConsoleColor.Green;

            Console.ForegroundColor = commandKeyColor;
            Console.Write("Commands: ");
            Console.ForegroundColor = commandDescriptionColor;
            Console.Write("a - Add, e - Edit, d - Delete, c - Complete, s - Sort, l - Switch List, x - Delete List, q - Quit");

            // Restore the original cursor position
            Console.SetCursorPosition(0, currentCursorPosition);
            Console.ResetColor();
        }


        

        static void SortTasks()
        {
            Console.Clear();
            Console.WriteLine("Sort tasks:\n");

            if (tasks.Count == 0)
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
                    tasks.Sort((a, b) => a.DueDate.CompareTo(b.DueDate));
                    Console.WriteLine("Tasks sorted by due date.");
                    break;
                case 2:
                    tasks.Sort((a, b) => a.Category.CompareTo(b.Category));
                    Console.WriteLine("Tasks sorted by category.");
                    break;
                case 3:
                    tasks.Sort((a, b) => a.IsComplete.CompareTo(b.IsComplete));
                    Console.WriteLine("Tasks sorted by completion status.");
                    break;
                case 4:
                    tasks.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase));
                    Console.WriteLine("Tasks sorted by name.");
                    break;
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void SwitchList()
        {
            Console.Clear();
            Console.WriteLine("Available task lists:\n");

            int listIndex = 1;
            List<string> listNames = new List<string>();
            foreach (string listName in taskLists.Keys)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{listIndex}. ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(listName);
                Console.ResetColor();
                listNames.Add(listName);
                listIndex++;
            }

            Console.WriteLine("\nSwitch or create task list:\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Enter the index number of the list you want to switch to, or type a new name to create a list (type 'cancel' to cancel): ");
            Console.ResetColor();
            string input = Console.ReadLine();

            if (input.ToLower() == "cancel")
            {
                Console.WriteLine("Switching list canceled.");
            }
            else if (int.TryParse(input, out int selectedIndex) && selectedIndex > 0 && selectedIndex <= listNames.Count)
            {
                currentList = listNames[selectedIndex - 1];
                tasks = taskLists[currentList];
                Console.WriteLine($"Switched to list '{currentList}'.");
            }
            else
            {
                currentList = input;
                if (taskLists.ContainsKey(currentList))
                {
                    tasks = taskLists[currentList];
                    Console.WriteLine($"Switched to list '{currentList}'.");
                }
                else
                {
                    tasks = new List<Task>();
                    taskLists[currentList] = tasks;
                    Console.WriteLine($"Created and switched to list '{currentList}'.");
                }
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void DeleteTaskList()
        {
            Console.Clear();
            Console.WriteLine("Delete task list:\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Enter the name of the task list you want to delete (type 'cancel' to cancel): ");
            Console.ResetColor();
            string listToDelete = Console.ReadLine();

            if (listToDelete.ToLower() == "cancel")
            {
                Console.WriteLine("Task list deletion canceled.");
            }
            else if (taskLists.ContainsKey(listToDelete))
            {
                if (listToDelete == currentList)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot delete the currently active task list. Switch to another list before deleting this one.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"Are you sure you want to delete the task list '{listToDelete}'? This action cannot be undone. (y/n): ");
                    Console.ResetColor();
                    char confirmation = char.ToLower(Console.ReadKey().KeyChar);
                    Console.WriteLine();

                    if (confirmation == 'y')
                    {
                        taskLists.Remove(listToDelete);
                        Console.WriteLine($"Task list '{listToDelete}' has been deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Task list deletion canceled.");
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Task list '{listToDelete}' not found.");
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        private static List<Task> GetTasksDueToday()
        {
            DateTime today = DateTime.Today;
            return tasks.Where(task => task.DueDate.Date == today).ToList();
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
                Console.ForegroundColor = task.IsComplete ? completedColor : normalColor;
                Console.WriteLine(task.Name);
                Console.ResetColor();
            }
        }




       


    }
}
