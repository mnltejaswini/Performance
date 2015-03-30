using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNet.Test.Perf.WebFx.Apps.HelloMvc.ViewData
{
    /// <summary>
    /// Summary description for Order
    /// </summary>
    public class Order
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Product Name can't be empty")]
        public string ProductName { get; set; }

        [Range(0, 9999)]
        public int Unit { get; set; }
    }
}