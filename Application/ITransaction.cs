/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

namespace DaiCo.Application
{
  /// <summary>
  /// Summary description for ITransaction.
  /// </summary>
  public interface ITransaction
  {
    void Execute();
    void GetFactory(SqlFactory factory);
    object Result { get; }
  }
}
