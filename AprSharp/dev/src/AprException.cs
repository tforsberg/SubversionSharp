//  AprSharp, a wrapper library around the Apache Portable Runtime Library
#region Copyright (C) 2004 SOFTEC sa.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//  Sources, support options and lastest version of the complete library
//  is available from:
//		http://www.softec.st/AprSharp
//		Support@softec.st
//
//  Initial authors : 
//		Denis Gervalle
//		Olivier Desaive
#endregion
//
using System;
using System.Runtime.Serialization;

namespace Softec.AprSharp
{
    [Serializable]
    public class AprException : Exception
    {
        const int Result = unchecked ((int)0xA0400000);
        
        public AprException() 
               : base ( "An unknown exception from Apr Library has occured." )
        {
            HResult = Result;
        }

        public AprException(string s) 
               : base ( s )
        {
            HResult = Result;
        }

        public AprException(string s, Exception innerException) 
               : base ( s, innerException )
        {
            HResult = Result;
        }

        public AprException(int apr_status) 
               : base ( Apr.StrError(apr_status) )
        {
            HResult = unchecked (Result + apr_status);
        }
        
        public AprException(int apr_status, Exception innerException) 
               : base ( Apr.StrError(apr_status), innerException )
        {
            HResult = unchecked (Result + apr_status);
        }

        public AprException(SerializationInfo info, StreamingContext context)
               : base (info, context)
        {
        }
    }
}