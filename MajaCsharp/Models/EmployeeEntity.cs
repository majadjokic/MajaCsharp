namespace MajaCsharp.Models
{
    
        public class EmployeeEntity
        {
            public string EmployeeName { get; set; } = default!;
            public DateTime StarTimeUtc { get; set; }
            public DateTime EndTimeUtc { get; set; }
        }

        public class EmployeeSummary
        {
            public string Name { get; set; } = default!;
            public double TotalHours { get; set; }
        }
    

}
