﻿using FluentValidation.Validators;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Convience.Fluentvalidation.Validators
{
    public class IPValidator : PropertyValidator
    {
        private const string _ipPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        public IPValidator() : base("IP地址格式错误！") { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ipAddress = context.PropertyValue?.ToString();

            // 正则验证
            return Regex.IsMatch(ipAddress, _ipPattern);
        }
    }
}
