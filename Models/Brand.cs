using System;

namespace Kindred.ConsoleApp.LogoUpdater.Models
{
    public class Brand
    {
        public Guid Id { get; set; }
        public int MerchantId { get; set; }
        public string Logo { get; set; }
    }
}
