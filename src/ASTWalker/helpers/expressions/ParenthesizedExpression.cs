﻿/// <summary>
/// ParenthesizedExpression.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.AST.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Rosetta.Translation;

    /// <summary>
    /// Helper for accessing parenthesized expressions in AST.
    /// </summary>
    public class ParenthesizedExpression : Helper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParenthesizedExpression"/> class.
        /// </summary>
        /// <param name="syntaxNode"></param>
        public ParenthesizedExpression(ParenthesizedExpressionSyntax syntaxNode)
            : this(syntaxNode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParenthesizedExpression"/> class.
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <param name="semanticModel"></param>
        public ParenthesizedExpression(ParenthesizedExpressionSyntax syntaxNode, SemanticModel semanticModel)
            : base(syntaxNode, semanticModel)
        {
        }

        /// <summary>
        /// Gets the expression value.
        /// </summary>
        public ExpressionSyntax Expression
        {
            get { return this.ParenthesizedExpressionSyntaxNode.Expression; }
        }

        private ParenthesizedExpressionSyntax ParenthesizedExpressionSyntaxNode
        {
            get { return this.SyntaxNode as ParenthesizedExpressionSyntax; }
        }
    }
}
