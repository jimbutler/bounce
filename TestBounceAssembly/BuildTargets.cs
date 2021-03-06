﻿using System.IO;
using System.Linq;
using Bounce.Framework;

namespace TestBounceAssembly {
    public class BuildTargets {
        public static object GetTargets (IParameters parameters) {
            var git = new GitCheckout {
                Repository = @"C:\Users\Public\Documents\Development\BigSolution.git",
                Directory = "one"
            };
            var solution = new VisualStudioSolution {
                SolutionPath = git.Files["BigSolution.sln"]
            };
            var webProject = solution.Projects[parameters.Default("proj", "BigSolution")];
            var serviceName = parameters.Default("svc", "BigWindowsService");
            var service = solution.Projects[serviceName];

            return new {
                WebSite = new Iis7WebSite {
                    Directory = webProject.ProjectDirectory,
                    Name = "BigWebSite",
                    Port = 5001
                },
                Tests = new NUnitTests {
                    DllPaths = solution.Projects.Select(p => p.OutputFile)
                },
                Service = new WindowsService {
                    BinaryPath = service.OutputFile,
                    Name = serviceName,
                    DisplayName = "Big Windows Service",
                    Description = "a big windows service demonstrating the bounce build framework"
                },
                Zip = new ZipFile {
                    Directory = webProject.WhenBuilt(() => Path.GetDirectoryName(webProject.OutputFile.Value)),
                    ZipFileName = "web.zip"
                },
            };
        }

        public static object Targets(IParameters parameters) {
            var solution = new VisualStudioSolution {
                SolutionPath = "WebSolution.sln",
            };
            var webProject = solution.Projects["WebSite"];

            return new {
                WebSite = new Iis7WebSite {
                    Directory = webProject.ProjectDirectory,
                    Name = "My Website",
                    Port = parameters.Default("port", 5001),
                },
                Tests = new NUnitTests {
                    DllPaths = solution.Projects.Select(p => p.OutputFile),
                },
            };
        }

        public static object Iis6Targets(IParameters parameters) {
            var appPool = new Iis6AppPool {Name = "MyNewAppPoolForOutputToFile"};

            return new {
                           WebSite = new Iis6WebSite() {
                                                           Directory =
                                                               @"C:\bzr\current\UI\OutputToFile\src\OutputToFile.UI.WebApplication",
                                                           ScriptMapsToAdd = Iis6WebSite.MvcScriptMaps,
                                                           Name = "OutputToFile (bounce)",
                                                           Port = 7001,
                                                           Authentication = new[] {Iis6Authentication.NTLM},
                                                           AppPool = appPool,
                                                       }
        };
        }

        public static object SomeTargets() {
            return new {
                           Copy = new Copy {FromPath = "TestFrom", ToPath = "TestTo", Excludes = new [] {@"**\_svn\"} },
                       };
        }

        [Targets]
        public static object RealTargets(IParameters buildParameters) {
            var version = buildParameters.Required<string>("version");

            var git = new GitCheckout {
                Repository = "git://github.com/refractalize/bounce.git",
                Directory = "tmp2",
            };
            var solution = new VisualStudioSolution {
                SolutionPath = "Bounce.sln",
            };
            var frameworkProject = solution.Projects["Bounce.Framework"];

            var downloadsDir = new CleanDirectory {
                Path = "Downloads",
            };

            var frameworkZip = new ZipFile {
                Directory = frameworkProject.WhenBuilt(() => Path.GetDirectoryName(frameworkProject.OutputFile.Value)),
                ZipFileName = downloadsDir.Files[version.WhenBuilt(() => string.Format("Bounce.Framework.{0}.zip", version.Value))],
            };

            var downloads = new All(frameworkZip, new GitTag {Directory = ".", Tag = version.WhenBuilt(() => "v" + version.Value)});

            return new {
                Tests = new NUnitTests {
                    DllPaths = solution.Projects.Select(p => p.OutputFile),
                },
                Downloads = downloads,
            };
        }
    }
}
