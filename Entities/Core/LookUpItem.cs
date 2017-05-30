using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace naturalgas.Entities.Core
{
    [NotMapped]
    public class LookUpItem
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        
    }
}