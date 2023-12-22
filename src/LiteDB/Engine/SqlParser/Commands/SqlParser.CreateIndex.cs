﻿namespace LiteDB.Engine;

/// <summary>
/// Internal class to parse and execute sql-like commands
/// </summary>
internal partial class SqlParser
{
    /// <summary>
    /// create_index:: 
    ///   "CREATE" [_ "UNIQUE"] _ "INDEX" _ word _ "ON" _ user_collection. "(" . expr_single. ")"
    /// </summary>
    private IEngineStatement ParseCreateIndex()
    {
        var token = _tokenizer.ReadToken().Expect(TokenType.Word);
        var unique = token.Value.Eq("UNIQUE");

        if (unique)
        {
            _tokenizer.ReadToken().Expect("INDEX");
        }
        else
        {
            token.Expect("INDEX");
        }

        var indexName = _tokenizer.ReadToken().Expect(TokenType.Word).Value; // get index name

        _tokenizer.ReadToken().Expect("ON");

        // get collection name
        var collectionName = this.ParseUserCollection();

        // read (
        _tokenizer.ReadToken().Expect(TokenType.OpenParenthesis);

        // read index expression
        var expr = BsonExpression.Create(_tokenizer, true);

        // read )
        _tokenizer.ReadToken().Expect(TokenType.CloseParenthesis);

        // expect end of statement
        _tokenizer.ReadToken().Expect(TokenType.EOF, TokenType.SemiColon);

        return new CreateIndexStatement(collectionName, indexName, expr, unique);
    }
}
