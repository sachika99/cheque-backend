namespace MotorStores.Application.DTOs
{
    public class UserIdDto
    {
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public string? Role { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class UpdateUserIdDto
    {
        public string? Role { get; set; }
    }
}
