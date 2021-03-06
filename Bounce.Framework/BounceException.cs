﻿using System;
using System.IO;

namespace Bounce.Framework {
    public class BounceException : Exception {
        public BounceException(string message) : base(message) {
        }

        public BounceException() {
        }

        public virtual void Explain(TextWriter stderr) {
            stderr.WriteLine(this);
        }
    }
}