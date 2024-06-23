﻿using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace BMinus.Parser;

public class Tokenizer
{
	static TextParser<Unit> StringToken { get; } =
		from open in Character.EqualTo('"')
		from content in Character.EqualTo('\\').IgnoreThen(Character.AnyChar).Value(Unit.Value).Try()
			.Or(Character.Except('"').Value(Unit.Value))
			.IgnoreMany()
		from close in Character.EqualTo('"')
		select Unit.Value;
	
	static TextParser<Unit> IntegerLiteralToken { get; } =
		from sign in Character.EqualTo('-').OptionalOrDefault().Value(Unit.Value)
		from first in Character.Digit.Value(Unit.Value)
		from rest in Character.Digit.Many().Value(Unit.Value)
		select Unit.Value;
	
	static TextParser<Unit> DecimalLiteralToken { get; } =
		from sign in Character.EqualTo('-').OptionalOrDefault()
		from first in Character.Digit
		from rest in Character.Digit.Or(Character.In('.')).IgnoreMany()
		select Unit.Value;

	public static TextParser<string> HexInteger =
		Span.EqualTo("0x")
			.IgnoreThen(Character.Digit
				.Or(Character.Matching(ch => ch >= 'a' && ch <= 'f' || ch >= 'A' && ch <= 'F', "a-f"))
				.Named("hex digit")
				.AtLeastOnce())
			.Select(chrs => new string(chrs));
	public static Tokenizer<BMToken> Instance { get; } =
		new TokenizerBuilder<BMToken>()
			.Ignore(Span.WhiteSpace)
			.Match(Character.EqualTo('{'), BMToken.LBrace)
			.Match(Character.EqualTo('}'), BMToken.RBrace)
			.Match(Character.EqualTo('['), BMToken.LSquareBracket)
			.Match(Character.EqualTo(']'), BMToken.RSquareBracket)
			.Match(Character.EqualTo('*'), BMToken.Times)
			.Match(Character.EqualTo('+'),BMToken.Plus)
			.Match(Character.EqualTo('('), BMToken.LParen)
			.Match(Character.EqualTo(')'), BMToken.RParen)
			.Match(Character.EqualTo('='),BMToken.Assign)
			.Match(StringToken, BMToken.String)
			.Match(IntegerLiteralToken, BMToken.IntLiteral, requireDelimiters: true)
			.Match(Identifier.CStyle, BMToken.Identifier, requireDelimiters: true).Build();
}