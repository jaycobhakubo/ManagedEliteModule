namespace GTI.Modules.Shared.Business
{
    public class PackageItem
    {
        #region Properties

        public int PackageId { get; set; }
        
        public string PackageName { get; set; }
        
        public string ReceiptText { get; set; }
        
        public bool ChargeDeviceFee { get; set; }
        
        public string PackagePrice { get; set; }
        
        public bool OverrideValidation { get; set; }

        public int ValidationQuantity { get; set; }

        public bool RequiresValidation { get; set; }

        public bool DefaultValidation { get; set; }

        public override string ToString()
        {
            return PackageName;
        }
        #endregion
    }
}
