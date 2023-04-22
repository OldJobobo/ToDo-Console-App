using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleToDoApp
{
    public class TaskListManager
    {
        static Dictionary<string, List<Task>> taskLists = new Dictionary<string, List<Task>>();
        static string currentList = "default";
        public static List<Task> tasks;

       public static void SwitchList()
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

        public static void DeleteTaskList()
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

        public static void ShowTasks()
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
                            taskColor = ColorScheme.urgentColor;
                            break;
                        case TaskCategory.Important:
                            taskColor = ColorScheme.importantColor;
                            break;
                        case TaskCategory.Normal:
                            taskColor = ColorScheme.normalColor;
                            break;
                    }

                    if (task.IsComplete)
                    {
                        taskColor = ColorScheme.completedColor;
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

        public static void LoadTasksFromFile()
        {
            if (File.Exists("tasks.json"))
            {
                string json = File.ReadAllText("tasks.json");
                taskLists = JsonConvert.DeserializeObject<Dictionary<string, List<Task>>>(json);
            }
            tasks = taskLists.ContainsKey(currentList) ? taskLists[currentList] : new List<Task>();
        }

        public static void SaveTasksToFile()
        {
            taskLists[currentList] = tasks;
            string json = JsonConvert.SerializeObject(taskLists);
            File.WriteAllText("tasks.json", json);
        }
    }
}
