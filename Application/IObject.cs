/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 14-08-2008
   Company :  Dai Co   
 */

namespace DaiCo.Application
{
  /// <summary>
  /// Summary description for ITransaction.
  /// </summary>
  public interface IObject
  {
    object Clone();
    string GetTableName();
    string[] ObjectKey();
  }
}
