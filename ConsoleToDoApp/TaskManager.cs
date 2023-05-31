using ConsoleToDoApp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
namespace ConsoleToDoApp
{
    public  class TaskManager
    {
        public static List<Task> Tasks { get; set; }
        

        public  TaskManager()
        {
            Tasks = new List<Task>();
          

        }

        public void AddTask()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Add a new task:\n");

            string taskName;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Task name: ");
                taskName = Console.ReadLine();

                if (string.IsNullOrEmpty(taskName)) // Check if the task name is empty or consists only of whitespace
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task name cannot be blank. Press any key to return to the main menu.");

                }
                else
                {
                    Console.ResetColor();
                    break;
                }
            }
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
            Tasks.Add(newTask);
            Program.SaveTasksToFile();
            Console.WriteLine("\nTask added successfully!");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ResetColor();
            Console.ReadKey();
        }


        public void EditTask()
        {
            Console.Clear();
            Console.WriteLine("Edit a task:\n");

            if (Tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to edit.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                Console.Write("Enter the task number you want to edit (1 to {0}): ", Tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= Tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = Tasks[taskIndex];

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
            Program.SaveTasksToFile();
            Console.WriteLine("\nTask updated successfully!");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }


        public void DeleteTask()
        {
            Console.Clear();
            Console.WriteLine("Delete a task:\n");

            if (Tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to delete.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                Console.Write("Enter the task number you want to delete (1 to {0}): ", Tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= Tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = Tasks[taskIndex];

            Console.WriteLine($"You have selected the task: {selectedTask.Name}");
            Console.Write("Are you sure you want to delete this task? (y/n): ");
            char confirmation = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (confirmation == 'y')
            {
                Tasks.RemoveAt(taskIndex);
                Program.SaveTasksToFile();
                Console.WriteLine("Task deleted successfully!");
            }
            else
            {
                Console.WriteLine("Task deletion canceled.");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }


        public void CompleteTask()
        {
            Console.Clear();
            Console.WriteLine("Mark a task as complete:\n");

            if (Tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to mark as complete.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                Console.Write("Enter the task number you want to mark as complete (1 to {0}): ", Tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= Tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = Tasks[taskIndex];

            if (selectedTask.IsComplete)
            {
                Console.WriteLine("This task is already marked as complete.");
            }
            else
            {
                selectedTask.IsComplete = true;
                Program.SaveTasksToFile();
                Console.WriteLine("Task marked as complete!");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

       
    }
}
