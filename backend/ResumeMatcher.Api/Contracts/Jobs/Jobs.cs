public class JobMatchRequest
{
    public Guid DocumentId { get; set; }
    public string ?JobText { get; set; }
    public int TopK { get; set; } = 5;
    public bool useLlm { get; set; } = false;
}