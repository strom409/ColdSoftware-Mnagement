using System;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Grower
{
    public class GrowerModel
    {
        public int Growerid { get; set; }
        
        // Mapped to PartyGroupName in GetallGrowers
        public string? GrowerGroupName { get; set; }
        
        public string? GrowerName { get; set; }
        public string? GrowerContact { get; set; }
        public string? GrowerCity { get; set; }
        public string? GrowerState { get; set; }
        public string? GrowerEmail { get; set; } = "";
        public string GrowerVillage { get; set; } = string.Empty;
        public string GrowerRemarks { get; set; } = string.Empty;
        public string GrowerGrace { get; set; } = string.Empty;
        public string GrowerAddress { get; set; } = string.Empty;
        public string? Cdated { get; set; }
        public string? Grstatus { get; set; }
        
        public decimal GrowerCrateissue { get; set; } = 0;
        public decimal GrowerCratelimit { get; set; } = 0;
        
        public string? GrowerLock { get; set; }
        public string? GrowerFlexi { get; set; }
        public string? GrowerActive { get; set; }
        public string? GrowerGst { get; set; }
        public string GrowerCountry { get; set; } = "India";
        public string? GrowerPan { get; set; }
        public string? GrowerPincode { get; set; }
        public string? GrowerStatecode { get; set; }
        
        // Return flags for validation logic
        // Return flags for validation logic
        public string? RetMessage { get; set; }
        public string? RetFlag { get; set; }
        
        // Permissions
        public bool GroAdd { get; set; }
        public bool GroEdit { get; set; }
        public bool GroView { get; set; }
        public bool GroDel { get; set; }
    }
}
