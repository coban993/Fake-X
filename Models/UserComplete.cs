namespace DotnetAPI2.Models
{
    //definicija klase se moze razbiti na fise fajlova, dobra praksa
    //ako se bude menjao model kasnije
    public partial class UserComplete
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Gender { get; set; } = "";
        public bool Active { get; set; }
        public string JobTitle { get; set; } = "";
        public string Department { get; set; } = "";
        public decimal Salary { get; set; }
        public decimal AvgSalary { get; set; }
    }
}