
    public class NegotiationsDetailsView
    {
    public int NegotiationID { get; set; }
    public int? serialCode { get; set; }
    public int? ClientID { get; set; }
    public string? ClientName { get; set; }
    public int? ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public int? UnitID { get; set; }
    public string? unitName { get; set; }
    public int? OriginalPrice { get; set; }
    public int? NegotiationPrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? NegotiationStatus { get; set; }
    public DateTime? NegotiationDate { get; set; }
    public Boolean? checkedByAdmin { get; set; }
    public string? Requester { get; set; }
    public Boolean? Reserved { get; set; }
    public Boolean? NegotiationCondition { get; set; }
    public int? SuggestedPrice { get; set; }
    public string? ReasonOfReject { get; set; }
    public DateTime? CheckedDate { get; set; }
}

