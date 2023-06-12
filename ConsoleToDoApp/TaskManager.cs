using ConsoleToDoApp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ConsoleToDoApp
{
    public class TaskManager
    {
       

        public TaskManager()
        {
          
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
                Console.Write("Task name (type 'cancel' to cancel, max 25 characters): ");
                taskName = Console.ReadLine();


                if (string.Equals(taskName, "cancel", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Task addition canceled.");
                    return;
                }

                if (string.IsNullOrEmpty(taskName)) // Check if the task name is empty or consists only of whitespace
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task name cannot be blank. Please try again.");
                    Console.ResetColor();
                }
                else if (taskName.Length > 25) // Check if the task name is longer than 25 characters
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Task name cannot be longer than 25 characters. Please try again.");
                    Console.ResetColor();
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
                Console.Write("Task category (N - Normal, I - Important, U - Urgent, type 'cancel' to cancel): ");
                string categoryInput = Console.ReadLine();

                if (string.Equals(categoryInput, "cancel", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Task addition canceled.");
                    return;
                }

                switch (categoryInput.ToUpper())
                {
                    case "N":
                        taskCategory = TaskCategory.Normal;
                        break;
                    case "I":
                        taskCategory = TaskCategory.Important;
                        break;
                    case "U":
                        taskCategory = TaskCategory.Urgent;
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please try again.");
                        continue;
                }
                break;
            }

           DateTime dueDate = DateTime.Now;  // Default value for due date
            while (true)
            {
                Console.Write($"Due date (MM-DD-YYYY, default is today's date - {dueDate.ToString("MM-dd-yyyy")}): ");
                string dateInput = Console.ReadLine();

                if (string.Equals(dateInput, "cancel", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Task addition canceled.");
                    return;
                }

                // If the user just presses enter without typing anything, keep the default date
                if (string.IsNullOrEmpty(dateInput))
                {
                    break;
                }

                if (DateTime.TryParseExact(dateInput, "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out dueDate))
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
            TaskListManager.tasks.Add(newTask);
            TaskListManager.SaveTasksToFile();
            Console.WriteLine("\nTask added successfully!");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ResetColor();
            Console.ReadKey();
        }



        public void EditTask()
        {
            Console.Clear();
            TaskListManager.ShowTasks();

            Console.WriteLine("Edit a task:\n");

            if (TaskListManager.tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to edit.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }



            int taskIndex;
            while (true)
            {
                Console.Write("Enter the task number you want to edit (1 to {0}, type 'cancel' to cancel): ", TaskListManager.tasks.Count);
                string taskIndexInput = Console.ReadLine();

                if (string.Equals(taskIndexInput, "cancel", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Task edit canceled.");
                    return;
                }

                if (int.TryParse(taskIndexInput, out taskIndex) && taskIndex >= 1 && taskIndex <= TaskListManager.tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = TaskListManager.tasks[taskIndex];

            // Create a copy of the task
            Task originalTask = new Task
            {
                Name = selectedTask.Name,
                Category = selectedTask.Category,
                DueDate = selectedTask.DueDate,
                IsComplete = selectedTask.IsComplete,
                SubTasks = new List<SubTask>(selectedTask.SubTasks)
            };

            Console.Write($"Task name ({selectedTask.Name}, type 'cancel' to cancel): ");
            string taskName = Console.ReadLine();
            if (string.Equals(taskName, "cancel", StringComparison.OrdinalIgnoreCase))
            {
                selectedTask.Name = originalTask.Name;
                selectedTask.Category = originalTask.Category;
                selectedTask.DueDate = originalTask.DueDate;
                selectedTask.IsComplete = originalTask.IsComplete;
                selectedTask.SubTasks = originalTask.SubTasks;
                Console.WriteLine("Task edit canceled.");
                return;
            }
            if (!string.IsNullOrEmpty(taskName))
            {
                selectedTask.Name = taskName;
            }

            TaskCategory taskCategory;
            while (true)
            {
                Console.Write($"Task category ({selectedTask.Category}) (N - Normal, I - Important, U - Urgent, type 'cancel' to cancel): ");
                string categoryInput = Console.ReadLine();

                if (string.Equals(categoryInput, "cancel", StringComparison.OrdinalIgnoreCase))
                {
                    selectedTask.Name = originalTask.Name;
                    selectedTask.Category = originalTask.Category;
                    selectedTask.DueDate = originalTask.DueDate;
                    selectedTask.IsComplete = originalTask.IsComplete;
                    selectedTask.SubTasks = originalTask.SubTasks;
                    Console.WriteLine("Task edit canceled.");
                    return;
                }

                switch (categoryInput.ToUpper())
                {
                    case "N":
                        taskCategory = TaskCategory.Normal;
                        break;
                    case "I":
                        taskCategory = TaskCategory.Important;
                        break;
                    case "U":
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
                Console.Write($"Due date ({selectedTask.DueDate.ToString("MM-dd-yyyy")}, type 'cancel' to cancel): ");
                string dateInput = Console.ReadLine();

                if (string.Equals(dateInput, "cancel", StringComparison.OrdinalIgnoreCase))
                {
                    selectedTask.Name = originalTask.Name;
                    selectedTask.Category = originalTask.Category;
                    selectedTask.DueDate = originalTask.DueDate;
                    selectedTask.IsComplete = originalTask.IsComplete;
                    selectedTask.SubTasks = originalTask.SubTasks;
                    Console.WriteLine("Task edit canceled.");
                    return;
                }

                if (DateTime.TryParseExact(dateInput, "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out dueDate))
                {
                    break;
                }
                Console.WriteLine("Invalid date format. Please try again.");
            }
            selectedTask.DueDate = dueDate;

            TaskListManager.SaveTasksToFile();
            Console.WriteLine("\nTask updated successfully!");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ResetColor();
            Console.ReadKey();
        }



        public void DeleteTask()
        {
            Console.Clear();
            Console.WriteLine("Delete a task:\n");

            if (TaskListManager.tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to delete.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                TaskListManager.ShowTasks();
                Console.Write("Enter the task number you want to delete (1 to {0}): ", TaskListManager.tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= TaskListManager.tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = TaskListManager.tasks[taskIndex];

            Console.WriteLine($"You have selected the task: {selectedTask.Name}");
            Console.Write("Are you sure you want to delete this task? (y/n): ");
            char confirmation = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (confirmation == 'y')
            {
                TaskListManager.tasks.RemoveAt(taskIndex);
                TaskListManager.SaveTasksToFile();
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

            if (TaskListManager.tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to mark as complete.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                TaskListManager.ShowTasks();
                Console.Write("Enter the task number you want to mark as complete (1 to {0}): ", TaskListManager.tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= TaskListManager.tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = TaskListManager.tasks[taskIndex];

            if (selectedTask.IsComplete)
            {
                Console.WriteLine("This task is already marked as complete.");
            }
            else
            {
                if (selectedTask.SubTasks.Count > 0)
                {
                    Console.Write("This task has subtasks. Do you want to mark all subtasks as complete as well? (y/n): ");
                    char input = char.ToLower(Console.ReadKey().KeyChar);
                    Console.WriteLine();
                    if (input == 'y')
                    {
                        selectedTask.CompleteTaskAndSubTasks();
                        Console.WriteLine("Task and its subtasks marked as complete!");
                    }
                    else if (input == 'n')
                    {
                        Console.Write("Enter the subtask number you want to mark as complete (1 to {0}): ", selectedTask.SubTasks.Count);
                        int subTaskIndex;
                        if (int.TryParse(Console.ReadLine(), out subTaskIndex) && subTaskIndex >= 1 && subTaskIndex <= selectedTask.SubTasks.Count)
                        {
                            subTaskIndex--;
                            selectedTask.SubTasks[subTaskIndex].IsComplete = true;
                            Console.WriteLine("Subtask marked as complete!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid subtask number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Task completion canceled.");
                    }
                }
                else
                {
                    selectedTask.IsComplete = true;
                    Console.WriteLine("Task marked as complete!");
                }

                TaskListManager.SaveTasksToFile();
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }


        public void UncompleteTask()
        {
            Console.Clear();
            Console.WriteLine("Mark a task as incomplete:\n");

            if (TaskListManager.tasks.Count == 0)
            {
                Console.WriteLine("No tasks available to mark as incomplete.");
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
                return;
            }

            int taskIndex;
            while (true)
            {
                TaskListManager.ShowTasks();
                Console.Write("Enter the task number you want to mark as incomplete (1 to {0}): ", TaskListManager.tasks.Count);
                if (int.TryParse(Console.ReadLine(), out taskIndex) && taskIndex >= 1 && taskIndex <= TaskListManager.tasks.Count)
                {
                    taskIndex--; // Convert to zero-based index
                    break;
                }
                Console.WriteLine("Invalid input. Please try again.");
            }

            Task selectedTask = TaskListManager.tasks[taskIndex];

            if (!selectedTask.IsComplete)
            {
                Console.WriteLine("This task is already marked as incomplete.");
            }
            else
            {
                selectedTask.IsComplete = false;

                // If a task is unmarked as complete, all its subtasks are also unmarked as complete
                foreach (SubTask subtask in selectedTask.SubTasks)
                {
                    subtask.IsComplete = false;
                }

                TaskListManager.SaveTasksToFile();
                Console.WriteLine("Task and its subtasks marked as incomplete!");
            }

            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }


        public void AddSubtask()
        {
            Console.Clear();
            TaskListManager.ShowTasks();

            Console.Write("Enter the task number to add a subtask: ");
            int taskNumber = int.Parse(Console.ReadLine()) - 1;
            if (taskNumber < 0 || taskNumber >= TaskListManager.tasks.Count)
            {
                Console.WriteLine("Invalid task number.");
                return;
            }

            Task parentTask = TaskListManager.tasks[taskNumber];

            Console.Write("Enter the subtask name: ");
            string subtaskName = Console.ReadLine();

            SubTask newSubtask = new SubTask { Name = subtaskName, IsComplete = false };
            parentTask.SubTasks.Add(newSubtask);

            TaskListManager.SaveTasksToFile();
        }

    }
}
