using System;
using System.Collections.Generic;

namespace ColdStoreManagement.BLL.Models.DTOs
{
    public class SaveDemandsRequestDto
    {
        public List<string> DemandIRNs { get; set; } = new List<string>();
        public DemandOrderDto EditModel { get; set; } = new DemandOrderDto();
        public int Unit { get; set; }
    }
}
