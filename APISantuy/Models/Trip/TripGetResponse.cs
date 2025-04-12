public class TripGetResponse
{
    public int Id { get; set; }
    public string EmployeeName { get; set; }
    public string AssignedByName { get; set; }
    public string CityName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Purpose { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}