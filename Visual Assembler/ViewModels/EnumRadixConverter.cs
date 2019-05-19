﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Visual_Assembler.ViewModels
{
    class EnumRadixConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string parameterString = (string)parameter;
            if ((parameterString == null) || (Enum.IsDefined(value.GetType(), value) == false))
                return DependencyProperty.UnsetValue;
            object parameterValue = Enum.Parse(value.GetType(), parameterString);
            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            string parameterString = (string)parameter;
            if (parameterString == null) return DependencyProperty.UnsetValue;
            return Enum.Parse(targetType, parameterString);
        }

    }
}