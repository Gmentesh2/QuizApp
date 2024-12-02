public class User
    {
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Coins { get; set; }
    public List<int> CreatedQuizIds { get; set; } = new List<int>();
    }
