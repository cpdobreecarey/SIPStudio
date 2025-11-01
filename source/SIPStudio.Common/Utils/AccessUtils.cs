using System.Security.AccessControl;
using System.Security.Principal;

namespace SIPStudio.Common.Utils;

/// <summary>
///     Helper class to check access rights in the system
/// </summary>
public static class AccessUtils
{
    /// <summary>
    ///     Check if the current user has write access to the specified path
    /// </summary>
    public static bool CheckWriteAccess(string path)
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        var accessControl = new DirectoryInfo(path).GetAccessControl();
        var accessRules = accessControl.GetAccessRules(true, true, typeof(NTAccount));
        var writeAccess = false;
        foreach (FileSystemAccessRule rule in accessRules)
        {
            if (principal.IsInRole(rule.IdentityReference.Value) && (rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData)
            {
                writeAccess = true;
                break;
            }
        }

        return writeAccess;
    }
}