namespace Calendar.Api.DTO
{
    public class AssignmentDTO_LOGIN
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class AssignmentDTO_GET
    {
        public Guid AssignmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public int RoleLevel { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
    }
    public class AssignmentDTO_POST
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile? Image { get; set; }
    }
}
