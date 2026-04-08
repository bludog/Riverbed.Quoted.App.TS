using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public enum AdjustmentType
        {
            Discount,
            Surcharge
        }
    public class Adjustment
    {
        public Adjustment()
        {
            ProjectDataId = 0;
            Type = AdjustmentType.Discount;
            IsPercentage = false;
            Value = 0;
        }

        public Adjustment(int projectDataId, AdjustmentType adjustmentType = AdjustmentType.Discount, int Value = 0, bool isPercentage = true)
        {
            ProjectDataId = projectDataId;
            Type = adjustmentType;
            IsPercentage = isPercentage;
            this.Value = Value;
        }
        public int Id { get; set; }
        public int ProjectDataId { get; set; }
        public AdjustmentType Type { get; set; }
        public bool IsPercentage { get; set; } // True for percentage, False for fixed amount
        public int Value { get; set; } // Represents the percentage (e.g., 10 for 10%) or fixed amount

        public decimal Apply(decimal baseAmount)
        {
            int adjustmentAmount = (int)(IsPercentage ? baseAmount * (Value / 100) : Value);

            return Type == AdjustmentType.Discount
                ? baseAmount - adjustmentAmount
                : baseAmount + adjustmentAmount;
        }
    }


}
