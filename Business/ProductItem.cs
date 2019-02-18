using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    public class ProductItem
    {
        public int ProductItemId;
        public int ProductTypeId;
        public int SalesSourceId;
        public int ProductGroupId;
        public bool IsActive;
        public string ProductItemName;
        public string ProductTypeName;
        public string ProductSalesSourceName;
        public string ProductGroupName;
        //START RALLY TA 5744
        public int PaperLayoutId { get; set; }
        public string PaperLayoutName { get; set; }
        public List<Accrual> AccuralList { get; set; } //RALLY US 1796
        //END RALLY TA 5744
        public int PaperLayoutCount { get; set; }

        // US2826
        public bool BarcodedPaper { get; set; }

        //US4059 Adding perm file
        public int PermFileId { get; set; }
        public bool Validate { get; set; }

        public override string ToString()
        {
            if (!String.IsNullOrWhiteSpace(ProductItemName))
                return ProductItemName;
            else
                return String.Format("[Product Id {0}]", ProductItemId);
        }
    }
}
