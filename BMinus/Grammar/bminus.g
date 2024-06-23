@namespace BMinus.Parser
@classname @Parser
@using BMinus.AST
@using System.Globalization


integer <Expression> = value:([0-9]+) { new WordLiteral(int.Parse(value[0], NumberStyles.Integer, CultureInfo.InvariantCulture)) }

decimal <Expression>
    = value:([0-9]+ ("." [0-9]+)?) { new WordLiteral(double.Parse(value, CultureInfo.InvariantCulture)) }

