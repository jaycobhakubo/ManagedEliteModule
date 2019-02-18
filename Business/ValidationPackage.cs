using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared.Business
{
    public class ValidationPackage : IEquatable<ValidationPackage>
    {
        public enum ValidationType
        {
            Paper,
            Electronic
        }

        public ValidationPackage()
        {
            Name = "Validation";
        }

        #region Properties

        public int PackageId { get; set; }

        public string Name { get; set; }

        public int CardCount { get; set; }
        
        public int MaxQuantity { get; set; }

        public ValidationType Type { get; set; }

        #endregion

        public static ValidationPackage Clone(ValidationPackage original)
        {
            var copy = new ValidationPackage
            {
                Name = original.Name,
                CardCount = original.CardCount,
                MaxQuantity =  original.MaxQuantity,
                Type =  original.Type,
                PackageId = original.PackageId
            };

            return copy;
        }

        public bool Equals(ValidationPackage other)
        {
            return other.Name == Name &&
                       other.MaxQuantity == MaxQuantity &&
                       other.CardCount == CardCount &&
                       other.PackageId == PackageId;
        }
    }
}
