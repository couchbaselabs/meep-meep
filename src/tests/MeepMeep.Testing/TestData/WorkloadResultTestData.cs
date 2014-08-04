using System;

namespace MeepMeep.Testing.TestData
{
    public static class WorkloadResultTestData
    {
         public static WorkloadResult GetWithFailedAndSucceededOperations()
         {
             var workloadResult = new WorkloadResult("Sample workload result.", "SampleThread", 1000);
             workloadResult.Register(new WorkloadOperationResult(true, "Success operation#1", TimeSpan.FromMilliseconds(10)) { DocSize = 50 });
             workloadResult.Register(new WorkloadOperationResult(true, "Success operation#1", TimeSpan.FromMilliseconds(10)));
             workloadResult.Register(new WorkloadOperationResult(false, "Failed operation#1", TimeSpan.FromMilliseconds(10)) { DocSize = 50 });
             workloadResult.Register(new WorkloadOperationResult(false, "Failed operation#1", TimeSpan.FromMilliseconds(10)));

             return workloadResult;
         }
    }
}