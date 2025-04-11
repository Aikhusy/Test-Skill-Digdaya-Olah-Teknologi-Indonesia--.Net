using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_t_log_user")]
public class UserLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public string LogMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
