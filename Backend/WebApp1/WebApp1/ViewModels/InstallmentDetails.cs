namespace WebApp1.ViewModels
{
    public class InstallmentDetails
    {
        public int TotalAmount { get; set; }
        public int DownPayment { get; set; }
        public DateTime FirstInstallmentDate { get; set; }
        public int InstallmentYears { get; set; }

    }
    public class InstallmentViewModel
    {
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public int Months { get; set; }
        public decimal MonthlyAmount { get; set; }
        public int Paid { get; set; }

    }
}
