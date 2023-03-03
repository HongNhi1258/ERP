/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

using System.Data;

namespace DaiCo.Application
{
  public interface IQuery
  {
    string CommandText { get; }
    void PrepareParameters(IDbCommand cm);
    object ReadResult(object value);
  }
}
