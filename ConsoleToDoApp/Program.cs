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
            ShowWelcomeScreen();

            LoadTasksFromFile();

            while (true)
            {
                ShowMainMenu();
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.KeyChar)
                {
                    case 'a':
                        AddTask();
                        break;
                    case 'e':
                        EditTask();
                        break;
                    case 'd':
                        DeleteTask();
                        break;
                    case 'c':
                        CompleteTask();
                        break;
                    case 's':
                        SortTasks();
                        break;
                    case 'l':
                        SwitchList();
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


        static void ShowMainMenu()
        {
            Console.Clear();

            
            Console.ForegroundColor = titleColor;
            Console.WriteLine("ToDo List Manager - Main Menu\n");
            Console.ResetColor();

            Console.WriteLine("Tasks:\n");

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

                    Console.Write($"{i + 1}. {taskStatus} ");
                    Console.ForegroundColor = taskColor;
                    Console.Write(task.Name);
                    Console.ResetColor();
                    Console.WriteLine($" - Due: {task.DueDate.ToShortDateString()}");
                }
                Console.WriteLine();
            }

          

            ConsoleColor commandKeyColor = ConsoleColor.Yellow;
            ConsoleColor commandDescriptionColor = ConsoleColor.Green;

            Console.ForegroundColor = commandsColor;
            Console.WriteLine("Commands:");

            Console.ForegroundColor = commandKeyColor;
            Console.Write("'a'");
            Console.ForegroundColor = commandDescriptionColor;
            Console.WriteLine(" - Add a task");

            Console.ForegroundColor = commandKeyColor;
            Console.Write("'e'");
            Console.ForegroundColor = commandDescriptionColor;
            Console.WriteLine(" - Edit a task");

            Console.ForegroundColor = commandKeyColor;
            Console.Write("'d'");
            Console.ForegroundColor = commandDescriptionColor;
            Console.WriteLine(" - Delete a task");

            Console.ForegroundColor = commandKeyColor;
            Console.Write("'c'");
            Console.ForegroundColor = commandDescriptionColor;
            Console.WriteLine(" - Mark a task as complete");

            Console.ForegroundColor = commandKeyColor;
            Console.Write("'s'");
            Console.ForegroundColor = commandDescriptionColor;
            Console.WriteLine(" - Sort tasks");

            Console.ForegroundColor = commandKeyColor;
            Console.Write("'l'");
            Console.ForegroundColor = commandDescriptionColor;
            Console.WriteLine(" - Switch or create list");

            Console.ForegroundColor = commandKeyColor;
            Console.Write("'q'");
            Console.ForegroundColor = commandDescriptionColor;
            Console.WriteLine(" - Quit");

            
            Console.ForegroundColor = normalColor;
            Console.WriteLine("\nPress the corresponding key for the command you'd like to execute.");

            Console.ResetColor();
        }



        static void AddTask()
        {
            Console.Clear();
            Console.WriteLine("Add a new task:\n");

            Console.Write("Task name: ");
            string taskName = Console.ReadLine();

            TaskCategory taskCategory;
            while (true)
            {
                Console.Write("Task category (N - Normal, I - Important, U - Urgent): ");
                char categoryInput = char.ToUpper(Console.ReadKey().KeyChar);
                Console.WriteLine();

                switch (categoryInput)
                {
                    case 'N':
                        taskCategory = TaskCategory.Normal;
                        break;
                    case 'I':
                        taskCategory = TaskCategory.Important;
                        break;
                    case 'U':
                        taskCategory = TaskCategory.Urgent;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please try again.");
                        continue;
                }
                break;
            }

            DateTime dueDate;
            while (true)
            {
                Console.Write("Due date (MM-DD-YYYY): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out dueDate))
                {
                    break;
                }
                Console.WriteLine("Invalid date format. Please try again.");
            }

            Task newTask = new Task
            {
                Name = taskName,
                Category = taskCategory,
                DueDate = dueDate,
                IsComplete = false
            };

            tasks.Add(newTask);
            SaveTasksToFile();
            Console.WriteLine("\nTask added successfully!");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }


        static void EditTask()
        {
            Console.Clear();
            Console.WriteLine("Edit a task:\n");

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to edit.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                Console.Write("Enter the task number you want to edit (1 to {0}): ", tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = tasks[taskIndex];

            Console.Write($"Task name ({selectedTask.Name}): ");
            string taskName = Console.ReadLine();
            if (!string.IsNullOrEmpty(taskName))
            {
                selectedTask.Name = taskName;
            }

            TaskCategory taskCategory;
            while (true)
            {
                Console.Write($"Task category ({selectedTask.Category}) (N - Normal, I - Important, U - Urgent): ");
                char categoryInput = char.ToUpper(Console.ReadKey().KeyChar);
                Console.WriteLine();

                switch (categoryInput)
                {
                    case 'N':
                        taskCategory = TaskCategory.Normal;
                        break;
                    case 'I':
                        taskCategory = TaskCategory.Important;
                        break;
                    case 'U':
                        taskCategory = TaskCategory.Urgent;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please try again.");
                        continue;
                }
                break;
            }
            selectedTask.Category = taskCategory;

            DateTime dueDate;
            while (true)
            {
                Console.Write($"Due date ({selectedTask.DueDate.ToString("MM-dd-yyyy")}): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out dueDate))
                {
                    break;
                }
                Console.WriteLine("Invalid date format. Please try again.");
            }
            selectedTask.DueDate = dueDate;
            SaveTasksToFile();
            Console.WriteLine("\nTask updated successfully!");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }


        static void DeleteTask()
        {
            Console.Clear();
            Console.WriteLine("Delete a task:\n");

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to delete.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                Console.Write("Enter the task number you want to delete (1 to {0}): ", tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = tasks[taskIndex];

            Console.WriteLine($"You have selected the task: {selectedTask.Name}");
            Console.Write("Are you sure you want to delete this task? (y/n): ");
            char confirmation = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (confirmation == 'y')
            {
                tasks.RemoveAt(taskIndex);
                SaveTasksToFile();
                Console.WriteLine("Task deleted successfully!");
            }
            else
            {
                Console.WriteLine("Task deletion canceled.");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }


        static void CompleteTask()
        {
            Console.Clear();
            Console.WriteLine("Mark a task as complete:\n");

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to mark as complete.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                Console.Write("Enter the task number you want to mark as complete (1 to {0}): ", tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = tasks[taskIndex];

            if (selectedTask.IsComplete)
            {
                Console.WriteLine("This task is already marked as complete.");
            }
            else
            {
                selectedTask.IsComplete = true;
                SaveTasksToFile();
                Console.WriteLine("Task marked as complete!");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
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
            Console.WriteLine("Switch or create task list:\n");
            Console.Write("Enter the name of the list you want to switch to or create (type 'cancel' to cancel): ");
            string newList = Console.ReadLine();

            if (newList.ToLower() == "cancel")
            {
                Console.WriteLine("Switching list canceled.");
            }
            else
            {
                currentList = newList;
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


        static void LoadTasksFromFile()
        {
            if (File.Exists("taskLists.json"))
            {
                string json = File.ReadAllText("tasks.json");
                taskLists = JsonConvert.DeserializeObject<Dictionary<string, List<Task>>>(json);
            }
            tasks = taskLists.ContainsKey(currentList) ? taskLists[currentList] : new List<Task>();
        }

        static void SaveTasksToFile()
        {
            taskLists[currentList] = tasks;
            string json = JsonConvert.SerializeObject(taskLists);
            File.WriteAllText("tasks.json", json);
        }


    }
}
