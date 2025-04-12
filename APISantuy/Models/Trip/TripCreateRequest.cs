public class TripCreateRequest
{
    public int EmployeeId { get; set; }
    public int CityId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Purpose { get; set; }
}