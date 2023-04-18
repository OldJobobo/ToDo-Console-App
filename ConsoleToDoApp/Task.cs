using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleToDoApp
{
    enum TaskCategory
    {
        Normal,
        Important,
        Urgent
    }

    class Task
    {
        public string Name { get; set; }
        public TaskCategory Category { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsComplete { get; set; }
    }
}
