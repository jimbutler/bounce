﻿using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class LazyDirectoryUtils : IDirectoryUtils {
        private IDirectoryUtils DirectoryUtils;
        public LazyDirectoryUtils() : this(new DirectoryUtils()) {}

        public LazyDirectoryUtils(IDirectoryUtils directoryUtils) {
            DirectoryUtils = directoryUtils;
        }

        public void CopyDirectory(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes) {
            if (!DirectoryUtils.DirectoryExists(to) || DirectoryUtils.GetLastModTimeForDirectory(from) > DirectoryUtils.GetLastModTimeForDirectory(to)) {
                DirectoryUtils.CopyDirectory(from, to, excludes, includes);
            }
        }

        public DateTime GetLastModTimeForDirectory(string dir) {
            return DirectoryUtils.GetLastModTimeForDirectory(dir);
        }

        public void DeleteDirectory(string dir) {
            DirectoryUtils.DeleteDirectory(dir);
        }

        public bool DirectoryExists(string dir) {
            return DirectoryUtils.DirectoryExists(dir);
        }
    }
}