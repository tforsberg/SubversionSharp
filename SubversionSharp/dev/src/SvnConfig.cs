//
// Softec
//
// Contact: Support@softec.st
//
// Designed by Denis Gervalle and Olivier Desaive
// Written by Denis Gervalle
//
// Copyright 2004 by SOFTEC. All rights reserved.
//
using System;
using System.Diagnostics;
using Softec.AprSharp;

namespace Softec.SubversionSharp
{

    public struct SvnConfig
    {
        IntPtr mConfig;

        #region Generic embedding functions of an IntPtr
        private SvnConfig(IntPtr ptr)
        {
            mConfig = ptr;
        }
        
        public bool IsNull()
        {
            return( mConfig == IntPtr.Zero );
        }

        private void CheckPtr()
        {
            if( IsNull() )
                throw new AprNullReferenceException(); 
        }

        public void ClearPtr()
        {
            mConfig = IntPtr.Zero;
        }

        public static implicit operator IntPtr(SvnConfig config)
        {
            return config.mConfig;
        }
        
        public static implicit operator SvnConfig(IntPtr ptr)
        {
            return new SvnConfig(ptr);
        }

        public override string ToString()
        {
            return("[svn_config_t:"+mConfig.ToInt32().ToString("X")+"]");
        }
        #endregion

        #region Wrapper methods
        public static void Ensure()
        {
            AprPool pool =  Svn.PoolCreate();
            try
            {
                Ensure(pool);
            }
            finally
            {
                pool.Destroy();
            }
        }

        public static void Ensure(AprPool pool)
        {
            Debug.WriteLine(String.Format("svn_config_ensure({0},{1})",IntPtr.Zero,pool));
            SvnError err = Svn.svn_config_ensure(IntPtr.Zero, pool);
            if(!err.IsNoError())
                throw new SvnException(err);
        }

        public static void Ensure(string configDir, AprPool pool)
        {
            Debug.WriteLine(String.Format("svn_config_ensure({0},{1})",configDir,pool));
            SvnError err = Svn.svn_config_ensure(configDir, pool);
            if(!err.IsNoError())
                throw new SvnException(err);
        }

        public static void Ensure(AprString configDir, AprPool pool)
        {
            Debug.WriteLine(String.Format("svn_config_ensure({0},{1})",configDir,pool));
            SvnError err = Svn.svn_config_ensure(configDir, pool);
            if(!err.IsNoError())
                throw new SvnException(err);
        }

        public static AprHash GetConfig(AprPool pool)
        {
        	IntPtr h;
            Debug.WriteLine(String.Format("svn_config_get_config({0},{1})",IntPtr.Zero,pool));
            SvnError err = Svn.svn_config_get_config(out h, IntPtr.Zero, pool);
            if(!err.IsNoError())
                throw new SvnException(err);
            return h;
        }

        public static AprHash GetConfig(string configDir, AprPool pool)
        {
        	IntPtr h;
            Debug.WriteLine(String.Format("svn_config_get_config({0},{1})",configDir,pool));
            SvnError err = Svn.svn_config_get_config(out h, configDir, pool);
            if(!err.IsNoError())
                throw new SvnException(err);
            return h;
        }

        public static AprHash GetConfig(AprString configDir, AprPool pool)
        {
        	IntPtr h;
            Debug.WriteLine(String.Format("svn_config_get_config({0},{1})",configDir,pool));
            SvnError err = Svn.svn_config_get_config(out h, configDir, pool);
            if(!err.IsNoError())
                throw new SvnException(err);
            return h;
        }
        #endregion
    }
}