using System;

namespace ColdStoreManagement.BLL.Models.Grower
{
    public class GrowerAgreementModel
    {
        // Identification
        public int AgreementId { get; set; }
        public int Growerid { get; set; }
        public string? GrowerGroupName { get; set; } // Grpname (Code-Name)
        public string? GrowerGroupSelectName { get; set; } // Partyname

        // Header Flags (from GetGroupName)
        public bool GrowerRetLock { get; set; }
        public bool GrowerRetFlexi { get; set; }
        public bool GrowerRetAcitve { get; set; }

        // Agreement Details
        public string Fyear { get; set; } = "25-26";
        public DateTime Adated { get; set; } = DateTime.Today; // Dated
        public DateTime Idated { get; set; } = DateTime.Today; // In-Date Estimate
        public decimal? Aqty { get; set; }
        public string? Orderby { get; set; }
        public string? Itemname { get; set; }
        public string? Atype { get; set; } // Service
        public string? RentType { get; set; } // Rate Type (KG/CRATES)
        public string? Achambers { get; set; }
        public string SalesType { get; set; } = "NA";

        // Rates
        public decimal? StoreRent { get; set; }
        public decimal? BinRent { get; set; }
        public decimal? CrateRate { get; set; }
        public decimal? LabourRate { get; set; }
        public decimal? GradingRent { get; set; }
        public decimal? PackingRent { get; set; }

        // Investment Details
        public bool Investmentflag { get; set; } // Investment boolean
        public string InvestFlag { get; set; } = "0"; // "1" or "0" string for SP
        public decimal? InvestQty { get; set; }
        public decimal? InvestRate { get; set; }
        public decimal? Invpercent { get; set; }
        
        // Calculated fields for UI
        public decimal? TotalValue { get; set; }
        public decimal? DoubleValue { get; set; }
        public decimal? percentvalue { get; set; }
        public decimal? Returnvalue { get; set; }

        // Validation/Response
        public string? RetMessage { get; set; }
        public string? RetFlag { get; set; }
    }
}
