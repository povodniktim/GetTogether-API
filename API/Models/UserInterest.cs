namespace API.Models
{
    public class UserInterest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int InterestId { get; set; }

        public virtual Interest Interest { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
