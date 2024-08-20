namespace DotnetAPI2.Models
{
    //definicija klase se moze razbiti na fise fajlova, dobra praksa
    //ako se bude menjao model kasnije
    public partial class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Gender { get; set; } = "";
        public bool Active { get; set; }
    }
}