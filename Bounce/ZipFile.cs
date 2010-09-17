﻿using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Bounce.Framework {
    public class ZipFile : ITarget {
        private readonly IZipFileCreator ZipFileCreator;
        private readonly IFileSystem FileSystem;
        private readonly IDirectoryUtils DirectoryUtils;
        public IValue<string> Directory;
        public IValue<string> ZipFileName;

        public ZipFile() : this(new ZipFileCreator(), new FileSystem(), new DirectoryUtils()) {
        }

        public ZipFile(IZipFileCreator zipFileCreator, IFileSystem fileSystem, IDirectoryUtils directoryUtils) {
            ZipFileCreator = zipFileCreator;
            FileSystem = fileSystem;
            DirectoryUtils = directoryUtils;
        }

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {Directory, ZipFileName}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
            if (!FileSystem.FileExists(ZipFileName.Value) || (FileSystem.LastWriteTimeForFile(ZipFileName.Value) < DirectoryUtils.GetLastModTimeForDirectory(Directory.Value))) {
                ZipFileCreator.CreateZipFile(ZipFileName.Value, Directory.Value);
            }
        }

        public void Clean() {
            var filename = ZipFileName.Value;
            if (File.Exists(filename)) {
                File.Delete(filename);
            }
        }
    }
}