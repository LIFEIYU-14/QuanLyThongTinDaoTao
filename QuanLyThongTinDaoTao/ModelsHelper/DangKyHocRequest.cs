using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class DangKyHocRequest
    {
        public string HoVaTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string CoQuanLamViec { get; set; }
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime NgaySinh { get; set; }

        public Guid LopHocId { get; set; }
    }
}