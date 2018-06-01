namespace KargoTakip.Helper
{
    public class businessModel
    {
        public string bsName { get; set; }
        public string  bossName { get; set; }
        public string bossPass { get; set; }
        public string secQuestion { get; set; }
        public string secAnswer { get; set; }
        public string mailAddress { get; set; }
        public string mailPassword { get; set; }
        public int businessId { get; set; }
    }
   public class customerModel
    {
        public string nameSurname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string tcNo { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public string mailAddress { get; set; }
    }
    public class employeeModel
    {
        public string nameSurname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
    }
    public class orderModel
    {
        public string orderId { get; set; }
        public string businessName { get; set; }
        public string orderEmployeeName { get; set; }
        public string orderState { get; set; }
        public string deliveryTC { get; set; }
    }
    public class feedbacksModel
    {
        public string orderId { get; set; }
        public string feedBacksText { get; set; }
        public string employeeName { get; set; }
        public string businessName { get; set; }
    }
}
