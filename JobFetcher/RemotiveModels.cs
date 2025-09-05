public class RemotiveResponse
{
    public List<RemotiveJob>? Jobs { get; set; }
}

public class RemotiveJob
{
    public string? Title { get; set; }
    public string? CompanyName { get; set; }
    public string? CandidateRequiredLocation { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? PublicationDate { get; set; }
}