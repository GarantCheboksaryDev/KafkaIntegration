using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Employee;

namespace vf.OverrideBaseDev.Server
{
  partial class EmployeeFunctions
  {
    /// <summary>
    /// Получить синхронизированного сотрудника.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="personnelNumber">Табельный номер.</param>
    /// <returns>Найденный сотрудник. Иначе, новый.</returns>
    [Public]
    public static IEmployee GetEmployeeSynch(string guid1C, string personnelNumber)
    {
      var employee = GetEmployeeByGuid(guid1C);
      if (employee == null)
        employee = OverrideBaseDev.Employees.GetAll(x => !(x.ExternalId != null && x.ExternalId != string.Empty)
                                                    && x.PersonnelNumber != null && x.PersonnelNumber != string.Empty
                                                    && x.PersonnelNumber == personnelNumber).FirstOrDefault();
      
      return employee != null ? employee : OverrideBaseDev.Employees.Create();
    }
    
    /// <summary>
    /// Получить сотрудника по Guid в 1С.
    /// </summary>
    /// <param name="Guid1C">Guid в 1С.</param>
    /// <returns>Найденный сотрудник.</returns>
    [Public]
    public static IEmployee GetEmployeeByGuid(string Guid1C)
    {
      return OverrideBaseDev.Employees.GetAll(x => x.ExternalId == Guid1C).FirstOrDefault();
    }
  }
}