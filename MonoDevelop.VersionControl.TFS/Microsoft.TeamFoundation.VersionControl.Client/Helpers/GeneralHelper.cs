//
// Helper.cs
//
// Author:
//       Ventsislav Mladenov <vmladenov.mladenov@gmail.com>
//
// Copyright (c) 2013 Ventsislav Mladenov
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;

namespace Microsoft.TeamFoundation.VersionControl.Client.Helpers
{
    public static class GeneralHelper
    {
        public static byte[] ToByteArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new byte[0];
            return Convert.FromBase64String(value);
        }

        public static bool XmlAttributeToBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        public static int XmlAttributeToInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            return Convert.ToInt32(value);
        }

        public static DateTime XmlAttributeToDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.MinValue;
            return DateTime.Parse(value);
        }
    }
}