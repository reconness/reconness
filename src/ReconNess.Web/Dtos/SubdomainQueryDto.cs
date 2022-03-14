namespace ReconNess.Web.Dtos
{
    public class SubdomainQueryDto
    {
        public string Query { get; set; }
        public int Limit { get; set; }
        public int Ascending { get; set; }
        public int Page { get; set; }
        public int ByColumn { get; set; }
    }
}
