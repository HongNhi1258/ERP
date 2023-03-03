/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 14-08-2008
   Company :  Dai Co   
 */
using System.Data;
namespace DaiCo.Application
{
  /// <summary>
  /// Summary description for ITransaction.
  /// </summary>
  public interface IStoreObject
  {
    string GetStoreName();
    void PrepareParameters(IDbCommand cm);
    void GetOutputValue(IDbCommand cm);
  }
}
