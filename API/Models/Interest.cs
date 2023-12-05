namespace API.Models
{
    public class Interest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
    }
}
