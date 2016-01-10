﻿/// <summary>
/// PropertyDeclarationTranslationUnit.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Translation
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Class describing properties.
    /// </summary>
    /// <remarks>
    /// Internal members protected for testability.
    /// </remarks>
    public class PropertyDeclarationTranslationUnit : MemberTranslationUnit, ITranslationUnit, ICompoundTranslationUnit
    {
        private const string ValueSetParameterName = "value";

        // TODO: Use StatementsGroupTranslationUnit
        protected IEnumerable<ITranslationUnit> getStatements;
        protected IEnumerable<ITranslationUnit> setStatements;

        protected ITranslationUnit returnType;

        protected bool hasGet;
        protected bool hasSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDeclarationTranslationUnit"/> class.
        /// </summary>
        protected PropertyDeclarationTranslationUnit() 
            : this(IdentifierTranslationUnit.Empty, IdentifierTranslationUnit.Empty, VisibilityToken.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDeclarationTranslationUnit"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        /// <param name="visibility"></param>
        protected PropertyDeclarationTranslationUnit(ITranslationUnit name, ITranslationUnit returnType, VisibilityToken visibility) 
            : base(name, visibility)
        {
            this.getStatements = new List<ITranslationUnit>();
            this.setStatements = new List<ITranslationUnit>();

            this.returnType = returnType;

            this.hasGet = true;
            this.hasSet = true;
        }

        /// <summary>
        /// Copy initializes a new instance of the <see cref="MethodDeclarationTranslationUnit"/> class.
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>
        /// For testability.
        /// </remarks>
        public PropertyDeclarationTranslationUnit(PropertyDeclarationTranslationUnit other)
            : base((MemberTranslationUnit)other)
        {
            this.getStatements = other.getStatements;
            this.setStatements = other.setStatements;
            this.returnType = other.returnType;
            this.hasGet = other.hasGet;
            this.hasSet = other.hasSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visibility"></param>
        /// <param name="returnType"></param>
        /// <param name="name"></param>
        /// <param name="hasGet"></param>
        /// <param name="hasSet"></param>
        /// <returns></returns>
        public new static PropertyDeclarationTranslationUnit Create(
            VisibilityToken visibility, ITranslationUnit returnType, ITranslationUnit name, bool hasGet = true, bool hasSet = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (returnType == null)
            {
                throw new ArgumentNullException(nameof(returnType));
            }

            return new PropertyDeclarationTranslationUnit()
            {
                Visibility = visibility,
                Name = name,
                returnType = returnType
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ITranslationUnit> InnerUnits
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Translate the unit into TypeScript.
        /// </summary>
        /// <returns></returns>
        public string Translate()
        {
            FormatWriter writer = new FormatWriter()
            {
                Formatter = this.Formatter
            };

            if (this.hasGet)
            {
                // Opening declaration: [<visibility>] get <name>() : <type> {
                // TODO: Handle case of no visibility specified
                writer.WriteLine("{0}{1} {2}{3} {4} {5} {6}",
                    this.Visibility == VisibilityToken.None ? string.Empty : TokenUtility.ToString(this.Visibility) + " ",
                    Lexems.GetKeyword,
                    this.Name.Translate(),
                    Lexems.OpenRoundBracket + Lexems.CloseRoundBracket,
                    Lexems.Colon,
                    this.returnType.Translate(),
                    Lexems.OpenCurlyBracket);

                TranslateBody(writer, this.getStatements);

                // Closing declaration
                writer.WriteLine("{0}", Lexems.CloseCurlyBracket);
            }

            if (this.hasSet)
            {
                var valueParameter = ArgumentDefinitionTranslationUnit.Create(
                    this.returnType, IdentifierTranslationUnit.Create("value"));

                // Opening declaration: [<visibility>] set <name>(value : <type>) {
                writer.WriteLine("{0}{1} {2}{3}{4}{5} {6}",
                    this.Visibility == VisibilityToken.None ? string.Empty : TokenUtility.ToString(this.Visibility) + " ",
                    Lexems.SetKeyword,
                    this.Name.Translate(),
                    Lexems.OpenRoundBracket,
                    valueParameter.Translate(),
                    Lexems.CloseRoundBracket,
                    Lexems.OpenCurlyBracket);

                TranslateBody(writer, this.setStatements);

                // Closing declaration
                writer.WriteLine("{0}", Lexems.CloseCurlyBracket);
            }

            return writer.ToString();
        }

        private static void TranslateBody(FormatWriter writer, IEnumerable<ITranslationUnit> statements)
        {
            // The body, we render them as a list of semicolon/newline separated elements
            foreach (ITranslationUnit statement in statements)
            {
                writer.WriteLine("{0}{1}",
                    statement.Translate(),
                    ShouldRenderSemicolon(statement) ? Lexems.Semicolon : string.Empty);
            }
        }

        #region Compound translation unit methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationUnit"></param>
        public void AddStatementToGetAccessor(ITranslationUnit translationUnit)
        {
            if (translationUnit == null)
            {
                throw new ArgumentNullException(nameof(translationUnit));
            }

            if (translationUnit as NestedElementTranslationUnit != null)
            {
                ((NestedElementTranslationUnit)translationUnit).NestingLevel = this.NestingLevel + 1;
            }

            ((List<ITranslationUnit>)this.getStatements).Add(translationUnit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationUnit"></param>
        public void AddStatementToSetAccessor(ITranslationUnit translationUnit)
        {
            if (translationUnit == null)
            {
                throw new ArgumentNullException(nameof(translationUnit));
            }

            if (translationUnit as NestedElementTranslationUnit != null)
            {
                ((NestedElementTranslationUnit)translationUnit).NestingLevel = this.NestingLevel + 1;
            }

            ((List<ITranslationUnit>)this.setStatements).Add(translationUnit);
        }

        #endregion

        protected static bool ShouldRenderSemicolon(ITranslationUnit statement)
        {
            var type = statement.GetType();

            var shouldNotRenderSemicolon = type == typeof(ConditionalStatementTranslationUnit);

            return !shouldNotRenderSemicolon;
        }
    }
}