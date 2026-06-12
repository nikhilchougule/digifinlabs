using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Enums
{
    public enum AuditAction
    {
        Create, Update, Delete, Access, Export, StageChange
        //Login, Logout, PasswordChange, FailedLogin, DataExport, DataImport, PermissionChange, RoleAssignment, SystemError
    }
}
