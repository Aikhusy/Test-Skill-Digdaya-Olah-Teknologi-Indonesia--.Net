using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_t_trip")]
public class Trip
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public User Employee { get; set; }

    public int AssignedById { get; set; }
    public User AssignedBy { get; set; }

    public int CityId { get; set; }
    public City City { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string Purpose { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}