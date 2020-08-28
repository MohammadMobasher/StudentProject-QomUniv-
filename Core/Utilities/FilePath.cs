using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Core.Utilities
{
    public enum FilePath
    {
        [Description("Uploads/Product")]
        Product,

        [Description("Uploads/ProductGallery")]
        ProductGallery,

        [Description("Uploads/ProductPackage")]
        productPackage,

        [Description("Uploads/Cars")]
        Cars,
    }
}
