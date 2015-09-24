/// <summary>
/// ClassASTWalker.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.AST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Symbols;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    using Rosetta.Translation;

    /// <summary>
    /// Walks a class AST node.
    /// </summary>
    public class ClassASTWalker : CSharpSyntaxWalker, IASTWalker
    {
        private ClassDeclarationTranslationUnit classDeclaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassASTWalker"/> class.
        /// </summary>
        private ClassASTWalker()
        {
            this.classDeclaration = new ClassDeclarationTranslationUnit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ITranslationUnit Walk(CSharpSyntaxNode node)
        {
            return null;
        }

        #region CSharpSyntaxWalker overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            base.VisitFieldDeclaration(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);
        }

        #endregion
    }
}
