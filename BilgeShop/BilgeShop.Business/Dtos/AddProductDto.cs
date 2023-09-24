using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Dtos
{
    public class AddProductDto
    {
        // Id zaten 0 olacağı için eklemede taşımaya gerek yok!
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public int UnitStock { get; set; }
        public int CategoryId { get; set; }
        public string ImagePath { get; set; }
        //WEBUI SONRASI GÖRSELİ ORADA BIRAKIYORUM, YALNIZCA DOSYA ADIYLA İLGİLENİYORUM.
    }
}
