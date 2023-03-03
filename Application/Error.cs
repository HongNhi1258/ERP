/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

using System;

namespace DaiCo.Application
{

  public class Error : ApplicationException
  {

    #region Fields
    #endregion Fields

    #region Contructors
    public Error() : base() { }
    public Error(string message) : base(message) { }
    public Error(string message, Exception inner) : base(message, inner) { }
    #endregion Contructors

    #region Properties
    #endregion Properties

    #region Methods
    #endregion Methods
  }
}
