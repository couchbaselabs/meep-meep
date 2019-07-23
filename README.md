# MeepMeep [![Build status](https://ci.appveyor.com/api/projects/status/yxxv2cmrgdocbr9j/branch/master?svg=true)](https://ci.appveyor.com/project/Couchbase/meep-meep/branch/master)

MeepMeep is a simple sample of a workload console app that can be used to simulate workloads against a Couchbase cluster using the .Net client.

## NOTE: Not supported under Couchbase Enterprise Support Subscriptions! ##

# Usage
MeepMeep is a simple console application. Run it with the `--help` switch to show help text that describes each available option.

For example to run with all default values:
`dotnet MeepMeep.ddl`.

There are three `workloads` currently shipped with MeepMeep: `MutationPercentage`, `AddJsonDocumentWorkload` and  `AddAndGetJsonDocumentWorkload`.


The options are:
```
  -n, --nodes                  (Default: couchbase://localhost) Space separated list of nodes to connect to.

  -b, --bucket                 (Default: default) Name of the Bucket

  -p, --bucketpassword         (Default: ) Password for the bucket.

  -c, --num-clients            (Default: 1) Number of client sessions.

  -s, --wl-size                (Default: 20000) The workload size. Depending on Workload, it could e.g. represent the num of documents to work with (per client session).

  --doc-sample-path            (Default: ) Path to a file with an UTF8 JSON document, e.g used for inserts.

  --doc-key-prefix             (Default: mm) Used as a prefix to generated document keys.

  --doc-key-seed               (Default: 0) Used as a starting seed when generating document keys.

  --doc-key-range              (Default: 1000) The maximum range of document IDs use above doc-key-seed.

  --clusteruser                (Default: Administrator) Cluster Administrator username.

  --clusterpassword            (Default: password) Cluster Administraor password.

  -w, --warmup-ms              (Default: 100) Determines how long time (ms) it takes before a work load's operation results is being seen as part of the workload.

  -v, --verbose                (Default: false) Determines detail level of output generated by output writers etc.

  -r, --mutation-percentage    (Default: 0.33) The percentage of operations that should be mutations.

  --workload-type              (Default: MutationPercentage) The type of workload to be used.

  -t, --enable-timings         (Default: false) Time operations and output at end of workload.

  --use-json                   (Default: false) Enable writing JSON values instead of raw bytes.

  --use-sync                   (Default: false) Uses a synchronous workload instead of async.

  --help                       Display this help screen.

  --version                    Display version information.
```

# Authentication with Couchbase Server 5.0+ (RBAC)

From Couchbase Server 5.0, Role based authentication was introduced which replaced the previous bucket password. To test a Cluster that is RBAC enabled, you will need to create a user with the same name as the bucket you want to test with.

For example:
```
dotnet MeepMeep.dll --nodes "couchbase://10.112.180.101" --bucket "test" --bucketpassword "password123"
```

# MyGet feed
We push every incremental change to a public MyGet feed
