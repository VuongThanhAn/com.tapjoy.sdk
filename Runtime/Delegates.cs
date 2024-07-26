using System;

namespace TapjoyUnity {

  public delegate void TapjoyAction();
  public delegate void TapjoyAction<T1>(T1 arg1);
  public delegate void TapjoyAction<T1, T2>(T1 arg1, T2 arg2);
  public delegate void TapjoyAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
  public delegate void TapjoyAction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
  public delegate void TapjoyAction<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

  public delegate TResult TapjoyFunc<out TResult>();
  public delegate TResult TapjoyFunc<in T1, out TResult>(T1 arg1);

}
