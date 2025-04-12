public class TripCreateResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int AssignedById { get; set; }
    public int CityId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Purpose { get; set; }
}