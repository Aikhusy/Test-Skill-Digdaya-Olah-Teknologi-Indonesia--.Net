using System.ComponentModel.DataAnnotations.Schema;

[Table("tbl_m_city")]
public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
}
