﻿using System;

namespace Arkivverket.Arkade.Core
{
    public class ArkadeException : Exception
    {
        public ArkadeException(string message) : base(message)
        {
        }

        public ArkadeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}