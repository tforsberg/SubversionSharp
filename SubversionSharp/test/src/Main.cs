// project created on 5/13/2004 at 10:00 PM
using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using Softec.AprSharp;
using Softec.SubversionSharp;

class MainClass
{
	public static void Main(string[] args)
	{
	    Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        SvnConfig.Ensure();
        
        AprPool pool =  Svn.PoolCreate();
        SvnClientContext ctx = SvnClientContext.Create(pool);
        ctx.Config = SvnConfig.GetConfig(pool);
        
        ArrayList authObjs = new ArrayList();
        authObjs.Add(SvnAuthProviderObject.GetSimpleProvider(pool));
        authObjs.Add(SvnAuthProviderObject.GetUsernameProvider(pool));
        authObjs.Add(SvnAuthProviderObject.GetSslServerTrustFileProvider(pool));
        authObjs.Add(SvnAuthProviderObject.GetSslClientCertFileProvider(pool));
        authObjs.Add(SvnAuthProviderObject.GetSslClientCertPwFileProvider(pool));
        authObjs.Add(SvnAuthProviderObject.GetPromptProvider(
        				new SvnAuthProviderObject.SimplePrompt(SimpleAuth),
        				IntPtr.Zero, 2, pool));
        authObjs.Add(SvnAuthProviderObject.GetPromptProvider(
        				new SvnAuthProviderObject.UsernamePrompt(UsernameAuth),
        				IntPtr.Zero, 2, pool));
        authObjs.Add(SvnAuthProviderObject.GetPromptProvider(
        				new SvnAuthProviderObject.SslServerTrustPrompt(SslServerTrustAuth),
        				IntPtr.Zero, pool));
        ctx.AuthBaton = SvnAuthBaton.Open(authObjs,pool);
        ctx.NotifyFunc = new SvnDelegate(new SvnWcNotify.Func(NotifyCallback));
        ctx.LogMsgFunc = new SvnDelegate(new SvnClient.GetCommitLog(GetCommitLogCallback));
        ctx.CancelFunc = new SvnDelegate(new Svn.CancelFunc(CancelCallback));
        
        GCHandle h;
        SvnOptRevision revision = SvnOptRevision.Alloc(out h);
        try
        {
	        revision.Number = 100;
			SvnClient.SvnClientCheckout("https://www.softec.st/svn/test", 
										"/home/denisg/dev/lib/SubversionSharp/test/bin/Debug/test",
										revision, true,	ctx, pool);
										
	        revision.Kind = SvnOptRevision.RevisionKind.Head;
			SvnClient.SvnClientUpdate("/home/denisg/dev/lib/SubversionSharp/test/bin/Debug/test",
									  revision, true,	ctx, pool);
		}
		finally
		{
			h.Free();
		}        
        pool.Destroy();
	}
	
	public static void NotifyCallback(IntPtr baton, AprString Path,  
        			 	 	          SvnWcNotify.Action action, Svn.NodeKind kind,
        			 		          AprString mimeType, SvnWcNotify.State contentState,
        			 		          SvnWcNotify.State propState, int revNum)
    {
    }
	
	public static SvnError GetCommitLogCallback(out AprString logMessage, out AprString tmpFile,
						 		   	  	        AprArray commitItems, IntPtr baton,
								      	        AprPool pool)
	{
		if (!commitItems.IsNull)
		{
			foreach (SvnClientCommitItem item in gclea.CommitItems)
			{
				Console.WriteLine("C{0}: {1} ({2}) r{3}",
					++mCommitCounter,
					item.Path, item.Kind, item.Revision);
				Console.WriteLine("C{0}: {1} -> {2}",
					mCommitCounter,
					item.URL,
					item.CopyFromUrl);
			}
			Console.WriteLine();
		}
		
		Console.Write("Enter log message: ");
		logMessage = new AprString(pool, Console.ReadLine());
		tmpFile = new AprString();
		
		return(SvnError.NoError);
	}

	
	public static SvnError CancelCallback(IntPtr baton)
	{
		return(SvnError.NoError);		
	}
	
	public static SvnError SimpleAuth(out SvnAuthCredSimple cred, IntPtr baton, 
        				   AprString realm, AprString username, 
        				   bool maySave, AprPool pool)
	{
		Console.WriteLine("Simple Authentication");
		Console.WriteLine("---------------------");
		Console.WriteLine("Realm: {0}", realm);
		Console.WriteLine("");
		
		bool valid = false;
		string line = "";
		
		while(!valid)
		{
			if (!username.IsNull)
				Console.Write("Enter Username ({0}): ", username);
			else
				Console.Write("Enter Username: ");

			line = Console.ReadLine();

			if (line.Trim().Length == 0 && !username.IsNull)
			{
				line = username.ToString();
				valid = true;
			}
			else if (line.Trim().Length > 0)
			{
				valid = true;
			}
		}
		
		cred = SvnAuthCredSimple.Alloc(pool);
		cred.Username = new AprString(pool, line);
		Console.Write("Enter Password: ");
		cred.Password = new AprString(pool, Console.ReadLine());
		cred.MaySave = maySave;
		return(SvnError.NoError);
	}
	
	public static SvnError UsernameAuth(out SvnAuthCredUsername cred, IntPtr baton, 
										AprString realm, bool maySave, AprPool pool)
	{
		Console.WriteLine("Username Authentication:");
		Console.WriteLine("------------------------");
		Console.WriteLine("Realm: {0}", realm);
		Console.WriteLine("");

		bool valid = false;
		string line = "";
		
		while(!valid)
		{
			Console.Write("Enter Username: ");

			line = Console.ReadLine();

			if (line.Trim().Length > 0)
			{
				valid = true;
			}
		}
		
		cred = SvnAuthCredUsername.Alloc(pool);
		cred.Username = new AprString(pool, line);
		cred.MaySave = maySave;
		return(SvnError.NoError);
	}
	
    public static SvnError SslServerTrustAuth(out SvnAuthCredSslServerTrust cred, 
					       				      IntPtr baton, AprString realm, 
									   		  SvnAuthCredSslServerTrust.CertFailures failures, 
									   		  SvnAuthSslServerCertInfo certInfo, 
									   		  bool maySave, IntPtr pool)
	{
		Console.WriteLine("Ssl Server Trust Prompt:");
		Console.WriteLine("------------------------");
		Console.WriteLine("");
		
		Console.WriteLine("Error validating server certificate for '{0}':", realm);
		if ((failures & SvnAuthCredSslServerTrust.CertFailures.UnknownCA) > 0)
			Console.WriteLine(" - The certificate is not issued by a trusted authority");
		if ((failures & SvnAuthCredSslServerTrust.CertFailures.CNMismatch) > 0)
			Console.WriteLine(" - The certificate hostname does not match");
		if ((failures & SvnAuthCredSslServerTrust.CertFailures.NotYetValid) > 0)
			Console.WriteLine(" - The certificate is not yet valid");
		if ((failures & SvnAuthCredSslServerTrust.CertFailures.Expired) > 0)
			Console.WriteLine(" - The certificate has expired");
		if ((failures & SvnAuthCredSslServerTrust.CertFailures.Other) > 0)
			Console.WriteLine(" - The certificate has an unknown error");
	
		Console.WriteLine("Certificate informations:");
		Console.WriteLine("\tHostName:    " + certInfo.Hostname);
		Console.WriteLine("\tIssuer:      " + certInfo.IssuerDName);
		Console.WriteLine("\tValid From:  " + certInfo.ValidFrom);
		Console.WriteLine("\tValid Until: " + certInfo.ValidUntil);
		Console.WriteLine("\tFingerprint: " + certInfo.Fingerprint);
	
		cred = SvnAuthCredSslServerTrust.Alloc(pool);
		bool valid = false;
		while (!valid)
		{
			if (maySave)
				Console.WriteLine("(R)eject, accept (t)emporarily or accept (p)ermanently? ");
			else
				Console.WriteLine("(R)eject or accept (t)emporarily? ");
				
			string line = Console.ReadLine();
			if (line.Length > 0)
			{
				char choice = line.ToLower()[0];
				if (choice == 'r')
				{
					cred.AcceptedFailures = 0;
					cred.MaySave=false;
					valid = true;
				}
				else if (choice == 't')
				{
					cred.AcceptedFailures = failures;
					cred.MaySave=false;
					valid = true;
				}
				else if (choice == 'p')
				{
					cred.AcceptedFailures = failures;
					cred.MaySave=true;
					valid = true;
				}
			}
		}
		return(SvnError.NoError);
	}
}