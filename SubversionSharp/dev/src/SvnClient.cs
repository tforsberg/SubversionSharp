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
    public class SvnClient
    {
    	public delegate SvnError GetCommitLog(out AprString logMessage, out AprString tmpFile,
         						 		   	  AprArray commitItems, IntPtr baton,
         								      AprPool pool);

 		public delegate SvnError LogMessageReceiver(IntPtr baton, AprHash changed_paths, 
 													int revision, AprString author,
 													AprString date, AprString message,
 													AprPool pool);         								       
         								                								                								       
		public static int Checkout(string url, string path, 
								   SvnOptRevision revision, 
								   bool recurse, SvnClientContext ctx, AprPool pool)
		{
			uint rev;
			Debug.Write(String.Format("svn_client_checkout({0},{1},{2},{3},{4},{5})...",url,path,revision,recurse,ctx,pool));
			SvnError err = Svn.svn_client_checkout(out rev, url, path, 
												   revision, 
												   (recurse ? 1 :0), ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",rev));
			return(unchecked((int)rev));
		}
		
		
		public static int Update(string path, 
								 SvnOptRevision revision, 
								 bool recurse, SvnClientContext ctx, AprPool pool)
		{
			uint rev;
			Debug.Write(String.Format("svn_client_update({0},{1},{2},{3},{4})...",path,revision,recurse,ctx,pool));
			SvnError err = Svn.svn_client_update(out rev, path, 
												 revision,
												 (recurse ? 1 :0), ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",rev));
			return(unchecked((int)rev));
		}
		
		public static int Switch(string path, string url, 
								 SvnOptRevision revision, 
								 bool recurse, SvnClientContext ctx, AprPool pool)
		{
			uint rev;
			Debug.Write(String.Format("svn_client_switch({0},{1},{2},{3},{4},{5})...",path,url,revision,recurse,ctx,pool));
			SvnError err = Svn.svn_client_switch(out rev, path, url, 
												 revision, 
												 (recurse ? 1 :0), ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",rev));
			return(unchecked((int)rev));
		}
		
		public static void Add(string path,
							   bool recurse, 
							   SvnClientContext ctx, AprPool pool)
		{
			Debug.WriteLine(String.Format("svn_client_add({0},{1},{2},{3},{4})",path,recurse,ctx,pool));
			SvnError err = Svn.svn_client_add(path, 
											  (recurse ? 1 :0), ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
		}
		
		public static SvnClientCommitInfo Mkdir(AprArray paths,  
							   					SvnClientContext ctx, AprPool pool)
		{
			IntPtr commitInfo;
			Debug.Write(String.Format("svn_client_mkdir({0},{1},{2})...",paths,ctx,pool));
			SvnError err = Svn.svn_client_mkdir(out commitInfo, paths, ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",commitInfo));
			return(commitInfo);
		}
		
		public static SvnClientCommitInfo Delete(AprArray paths, bool force,
							   					 SvnClientContext ctx, AprPool pool)
		{
			IntPtr commitInfo;
			Debug.Write(String.Format("svn_client_delete({0},{1},{2},{3})...",paths,force,ctx,pool));
			SvnError err = Svn.svn_client_delete(out commitInfo, paths, (force) ? 1 : 0, ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",commitInfo));
			return(commitInfo);
		}
		
		public static SvnClientCommitInfo Import(string path, string url, bool nonrecursive,  
							   					 SvnClientContext ctx, AprPool pool)
		{
			IntPtr commitInfo;
			Debug.Write(String.Format("svn_client_import({0},{1},{2},{3},{4})...",path,url,nonrecursive,ctx,pool));
			SvnError err = Svn.svn_client_import(out commitInfo, path, url, (nonrecursive) ? 1 : 0, 
												 ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",commitInfo));
			return(commitInfo);
		}
		
		public static SvnClientCommitInfo Commit(AprArray targets, bool nonrecursive,
							   					 SvnClientContext ctx, AprPool pool)
		{
			IntPtr commitInfo;
			Debug.Write(String.Format("svn_client_commit({0},{1},{2},{3})...",targets,nonrecursive,ctx,pool));
			SvnError err = Svn.svn_client_commit(out commitInfo, targets, (nonrecursive) ? 1 : 0,
												 ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",commitInfo));
			return(commitInfo);
		}
		
		public static int Status(string path,
								 SvnOptRevision revision,
								 SvnWcStatus.Func statusFunc, IntPtr statusBaton,
								 bool descend, bool getAll, bool update, bool noIgnore,
			   					 SvnClientContext ctx, AprPool pool)
		{
			uint rev;
			SvnDelegate statusDelegate = new SvnDelegate(statusFunc);
			Debug.Write(String.Format("svn_client_status({0},{1},{2},{3},{4:X},{5},{6},{7},{8},{9})...",path,revision,statusFunc.Method.Name,statusBaton.ToInt32(),(descend) ? 1 : 0,(getAll) ? 1 : 0,(update) ? 1 : 0,(noIgnore) ? 1 : 0,ctx,pool));
			SvnError err = Svn.svn_client_status(out rev, path, revision,
												 (Svn.svn_wc_status_func_t) statusDelegate.Wrapper,
												 statusBaton,
												 (descend) ? 1 : 0, (getAll) ? 1 : 0,
												 (update) ? 1 : 0, (noIgnore) ? 1 : 0,
												 ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
			Debug.WriteLine(String.Format("Done({0})",rev));
			if( update )
				return(unchecked((int)rev));
			else
				return(-1);
		}
		
		public static void Log(AprArray targets,
							   SvnOptRevision start, SvnOptRevision end,
							   bool discoverChangedPaths, bool strictNodeHistory,
							   LogMessageReceiver receiver, IntPtr baton,
							   SvnClientContext ctx, AprPool pool)
		{
			SvnDelegate receiverDelegate = new SvnDelegate(receiver);
			Debug.WriteLine(String.Format("svn_client_log({0},{1},{2},{3},{4},{5},{6},{7},{8})",targets,start,end,(discoverChangedPaths ? 1 :0),(strictNodeHistory ? 1 :0),receiver.Method.Name,baton,ctx,pool));
			SvnError err = Svn.svn_client_log(targets, start, end,
											  (discoverChangedPaths ? 1 :0),
											  (strictNodeHistory ? 1 :0),
											  (Svn.svn_log_message_receiver_t)receiverDelegate.Wrapper,
											  baton,
											  ctx, pool);
			if( !err.IsNoError )
				throw new SvnException(err);
		}
	}
}