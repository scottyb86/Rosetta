﻿/// <summary>
/// FieldDeclaration.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.AST.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Rosetta.AST.Helpers;
    using Rosetta.AST.Utilities;
    using Rosetta.Translation;

    /// <summary>
    /// Helper for accessing interface in AST
    /// </summary>
    public class FieldDeclaration : Helper
    {
        // Acting as a decorated element (lazily initialized)
        private VariableDeclaration variableDeclaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDeclaration"/> class.
        /// </summary>
        /// <param name="syntaxNode"></param>
        public FieldDeclaration(FieldDeclarationSyntax syntaxNode)
            : this(syntaxNode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDeclaration"/> class.
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <param name="semanticModel"></param>
        public FieldDeclaration(FieldDeclarationSyntax syntaxNode, SemanticModel semanticModel)
            : base(syntaxNode, semanticModel)
        {
        }

        /// <summary>
        /// Gets the visibility of the field.
        /// </summary>
        public VisibilityToken Visibility
        {
            get
            {
                SyntaxTokenList modifiers = this.FieldDeclarationSyntaxNode.Modifiers;

                if (modifiers.Count == 0)
                {
                    // No visibility modifier specified
                    return VisibilityToken.None;
                }
                
                return this.FieldDeclarationSyntaxNode.Modifiers.Get();
            }
        }

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public virtual string Name
        {
            get { return this.VariableDeclaration.Names[0]; }
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        public virtual TypeReference Type => this.VariableDeclaration.Type;

        protected VariableDeclaration VariableDeclaration
        {
            get
            {
                if (this.variableDeclaration == null)
                {
                    this.variableDeclaration = this.CreateVariableDeclarationHelper(this.FieldDeclarationSyntaxNode.Declaration, this.SemanticModel);
                }

                return this.variableDeclaration;
            }
        }

        /// <summary>
        /// Creates the variable declaration helper.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Must return a type derived from <see cref="VariableDeclaration"/>.
        /// </remarks>
        protected virtual VariableDeclaration CreateVariableDeclarationHelper(VariableDeclarationSyntax node, SemanticModel semanticModel)
        {
            return new VariableDeclaration(node, semanticModel);
        }

        protected FieldDeclarationSyntax FieldDeclarationSyntaxNode
        {
            get { return this.SyntaxNode as FieldDeclarationSyntax; }
        }
    }
}
