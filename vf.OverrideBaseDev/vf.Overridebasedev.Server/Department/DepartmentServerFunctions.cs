using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Department;

namespace vf.OverrideBaseDev.Server
{
  partial class DepartmentFunctions
  {

    /// <summary>
    /// Получить синхронизированное подразделение.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="HCMCode">Код HCM.</param>
    /// <returns>Найденное подразделение. Если не найдено, то возвращается новое.</returns>
    [Public]
    public static IDepartment GetDepartmentSynch(string guid1C, string HCMCode)
    {
      var department = GetDepartmentByGuid1C(guid1C);
      if (department == null)
        department = OverrideBaseDev.Departments.GetAll(x => !(x.ExternalId != null && x.ExternalId != string.Empty)
                                                        && x.HCMCode != null && x.HCMCode != string.Empty && x.HCMCode == HCMCode).FirstOrDefault();
      
      return department != null ? department : OverrideBaseDev.Departments.Create();
    }
    
    /// <summary>
    /// Получить подразделение по Guid в 1С.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <returns>Найденное подразделение.</returns>
    [Public]
    public static IDepartment GetDepartmentByGuid1C(string guid1C)
    {
      return OverrideBaseDev.Departments.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
    }
    
    /// <summary>
    /// Проверить является ли сотрудник руководителем подразделения.
    /// </summary>
    /// <returns>True - если является, иначе false.</returns>
    public bool CheckEmployeeIsManager(OverrideBaseDev.IEmployee currentEmployee)
    {
      var currentDepartment = _obj;
      
      var employeeIsManager = OverrideBaseDev.Employees.Equals(currentEmployee, currentDepartment.Manager);
      
      // Ввести переменную итерации, чтобы обойти неправильную настройку оргструктуры.
      var iter = 1;
      
      while (currentDepartment != null && !employeeIsManager && iter < 9999)
      {
        currentDepartment = OverrideBaseDev.Departments.As(currentDepartment.HeadOffice);
        
        if (currentDepartment != null)
          employeeIsManager = OverrideBaseDev.Employees.Equals(currentEmployee, currentDepartment.Manager);
        
        iter ++;
      }
      
      return employeeIsManager;
    }
  }
}