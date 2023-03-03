/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */
using System;
namespace DaiCo.Application
{
  /// <summary>
  /// Summary description for SQLException.
  /// </summary>
  public class SQLException : Exception
  {

    #region Fields
    #endregion Fields

    #region Contructors
    public SQLException() : base() { }
    public SQLException(string message) : base(message) { }
    public SQLException(string message, Exception inner) : base(message, inner) { }
    #endregion Contructors

    #region Properties
    #endregion Properties

    #region Methods
    #endregion Methods
  }
}
