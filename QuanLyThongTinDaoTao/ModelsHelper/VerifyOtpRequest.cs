﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyThongTinDaoTao.ModelsHelper
{
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}