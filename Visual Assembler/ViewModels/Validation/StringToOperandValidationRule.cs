using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Visual_Assembler.ViewModels.Validation {
    class OperandValidation : ValidationRule {

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            byte b;
            if (byte.TryParse(value.ToString(), out b)) {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Valor inválido");
        }
    }
}
