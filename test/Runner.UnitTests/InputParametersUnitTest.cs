﻿/// <summary>
/// InputParametersUnitTest.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Runner.UnitTests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rosetta.Runner.Exceptions;
    using Rosetta.Runner.UnitTests.Mocks;
    using Rosetta.Runner.UnitTests.Utils;

    [TestClass]
    public class InputParametersUnitTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
        }

        [ClassCleanup]
        public static void CleanUp()
        {
        }

        #region General

        /// <summary>
        /// This tests that passing no parameter causes runner not to execute the main routine.
        /// </summary>
        [TestMethod]
        public void WhenCalledWithNoParameterThenMainRoutineIsNotRun()
        {
            var program = new MockedProgram(new string[] { });

            Assert.AreEqual(false, program.NoFeasibleExecutionRoutineRun, "User should be notified about not feasible execution!");
            Assert.AreEqual(true, program.HelpContentDisplayed, "Help content should have been displayed!");
            program.AssertProgramNotRun();
        }

        #endregion

        #region Parameter Help

        [TestMethod]
        public void WhenCalledWithNoParameterThenHelpIsShown()
        {
            var program = new MockedProgram(new string[]{ });

            Assert.AreEqual(true, program.HelpContentDisplayed, "Help content should have been displayed!");
            program.AssertProgramNotRun();
        }

        [TestMethod]
        public void WhenHelpParameterSpecifiedThenHelpIsShown()
        {
            var program = new MockedProgram(new string[] 
            {
                ParameterUtils.HelpArgumentParameter
            });

            Assert.AreEqual(true, program.HelpContentDisplayed, "Help content should have been displayed!");
            program.AssertProgramNotRun();
        }

        [TestMethod]
        public void WhenHelpShortParameterSpecifiedThenHelpIsShown()
        {
            var program = new MockedProgram(new string[]
            {
                ParameterUtils.HelpArgumentShortParameter
            });

            Assert.AreEqual(true, program.HelpContentDisplayed, "Help content should have been displayed!");
            program.AssertProgramNotRun();
        }

        [TestMethod]
        public void WhenHelpParameterSpecifiedWithOtherParametersThenHelpIsShown()
        {
            var parameters = new string[][]
            {
                new string[] 
                {
                    ParameterUtils.HelpArgumentParameter,
                    ParameterUtils.FileArgumentParameter,
                    "file"
                },
                new string[] 
                {
                    ParameterUtils.HelpArgumentParameter,
                    ParameterUtils.ProjectArgumentParameter,
                    "project"
                },
                new string[] 
                {
                    ParameterUtils.HelpArgumentParameter,
                    ParameterUtils.VerboseArgumentParameter
                },
                new string[] 
                {
                    ParameterUtils.HelpArgumentParameter,
                    ParameterUtils.ProjectArgumentParameter,
                    "project",
                    ParameterUtils.FileArgumentParameter,
                    "file"
                }
            };

            foreach (var input in parameters)
            {
                var program = new MockedProgram(input);

                Assert.AreEqual(true, program.HelpContentDisplayed, "Help content should have been displayed!");
                program.AssertProgramNotRun();
            }
        }

        #endregion

        #region Parameter File

        [TestMethod]
        public void WhenFileParameterSpecifiedThenFileRoutineIsExecuted()
        {
            var program = new MockedProgram(new string[]
            {
                ParameterUtils.FileArgumentParameter,
                "file"
            });

            program.AssertMainProgramRoutineRun();
            Assert.AreEqual(true, program.FileConversionRoutineRun, "File conversion routine was expected to be run!");
        }

        [TestMethod]
        public void WhenFileShortParameterSpecifiedThenFileRoutineIsExecuted()
        {
            var program = new MockedProgram(new string[]
            {
                ParameterUtils.ProjectArgumentShortParameter,
                "file"
            });

            program.AssertMainProgramRoutineRun();
            Assert.AreEqual(true, program.FileConversionRoutineRun, "File conversion routine was expected to be run!");
        }

        [TestMethod]
        public void FileParameterValueIsStored()
        {
            var value = "file";

            var program = new MockedProgram(new string[]
            {
                ParameterUtils.FileArgumentParameter,
                value
            });
            
            Assert.AreEqual(PathUtils.GetAbsolutePath(value), program.FilePath, 
                "File conversion routine was expected to be run!");
        }

        [TestMethod]
        public void FileShortParameterValueIsStored()
        {
            var value = "file";

            var program = new MockedProgram(new string[]
            {
                ParameterUtils.FileArgumentShortParameter,
                value
            });

            Assert.AreEqual(PathUtils.GetAbsolutePath(value), program.FilePath, 
                "File conversion routine was expected to be run!");
        }

        [TestMethod]
        public void UnnamedParameterIsStoredAsFileParameter()
        {
            var value = "file";

            var program = new MockedProgram(new string[]
            {
                value
            });

            Assert.AreEqual(PathUtils.GetAbsolutePath(value), program.FilePath, 
                "File conversion routine was expected to be run!");
        }

        [TestMethod]
        public void MoreUnnamedParametersCauseError()
        {
            var value1 = "file1";
            var value2 = "file2";

            var program = new MockedProgram(new string[]
            {
                value1,
                value2
            });

            program.AssertMainProgramRoutineNotRun();

            Assert.IsNotNull(program.ThrownOptionException, "Expecting an option exception");
            Assert.IsNotNull(program.ThrownOptionException.InnerException);
            Assert.IsInstanceOfType(program.ThrownOptionException.InnerException, 
                typeof(DefaultOptionException));
            Assert.AreEqual(2, 
                (program.ThrownOptionException.InnerException as DefaultOptionException).OptionsValues.Count(), 
                "Expecting 2 conflicting options!");
            Assert.AreEqual(value1,
                (program.ThrownOptionException.InnerException as DefaultOptionException).OptionsValues.ElementAt(0),
                "Conflicting option does not match expected!");
            Assert.AreEqual(value2,
                (program.ThrownOptionException.InnerException as DefaultOptionException).OptionsValues.ElementAt(1),
                "Conflicting option does not match expected!");
        }

        [TestMethod]
        public void FileParameterAndUnnamedParameterCauseConflict()
        {
            var program = new MockedProgram(new string[]
            {
                ParameterUtils.FileArgumentParameter,
                "file1",
                "file2"
            });

            program.AssertMainProgramRoutineNotRun();

            Assert.IsNotNull(program.ThrownOptionException, "Expecting an option exception");
            Assert.IsNotNull(program.ThrownOptionException.InnerException);
            Assert.IsInstanceOfType(program.ThrownOptionException.InnerException,
                typeof(ConflictingOptionsException));
            Assert.AreEqual(2,
                (program.ThrownOptionException.InnerException as ConflictingOptionsException).Options.Count(),
                "Expecting 2 conflicting options!");
            Assert.AreEqual(Program.UnnamedArgumentName,
                (program.ThrownOptionException.InnerException as ConflictingOptionsException).Options.ElementAt(0),
                "Conflicting option does not match expected!");
            Assert.AreEqual(Program.FileArgumentName,
                (program.ThrownOptionException.InnerException as ConflictingOptionsException).Options.ElementAt(1),
                "Conflicting option does not match expected!");
        }

        #endregion

        #region Parameter Output

        [TestMethod]
        public void OutputParameterValueIsStored()
        {
            var value = PathUtils.TestFolderAbsolutePath;

            var program = new MockedProgram(new string[]
            {
                ParameterUtils.FileArgumentParameter,
                "file1",
                ParameterUtils.OutputArgumentParameter,
                value
            });

            Assert.IsNotNull(program.OutputFolder, "Expecting output flder path!");
            Assert.AreEqual(value, program.OutputFolder, "Not matching output folder path!");
        }

        [TestMethod]
        public void OutputShortParameterValueIsStored()
        {
            var value = PathUtils.TestFolderAbsolutePath;

            var program = new MockedProgram(new string[]
            {
                ParameterUtils.FileArgumentParameter,
                "file1",
                ParameterUtils.OutputArgumentShortParameter,
                value
            });

            Assert.IsNotNull(program.OutputFolder, "Expecting output flder path!");
            Assert.AreEqual(value, program.OutputFolder, "Not matching output folder path!");
        }

        #endregion
    }
}