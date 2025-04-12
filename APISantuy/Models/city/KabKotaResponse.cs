public class KabKotaResponse
{
    public int status { get; set; }
    public string message { get; set; }
    public List<KabKotaResult> result { get; set; }
}

public class KabKotaResult
{
    public string id { get; set; }
    public string text { get; set; }
}
