﻿@namespace PegExamples
@classname ExpressionParser
@using System.Globalization

additive <double> -memoize
    = left:additive "+" right:multiplicative { left + right }
    / left:additive "-" right:multiplicative { left - right }
    / multiplicative

multiplicative <double> -memoize
    = left:multiplicative "*" right:power { left * right }
    / left:multiplicative "/" right:power { left / right }
    / power
power <double>
    = left:primary "^" right:power { Math.Pow(left, right) }
    / primary

primary <double> -memoize
    = decimal
    / "-" primary:primary { -primary }
    / "(" additive:additive ")" { additive }

decimal <double>
    = value:([0-9]+ ("." [0-9]+)?) { double.Parse(value, CultureInfo.InvariantCulture) }