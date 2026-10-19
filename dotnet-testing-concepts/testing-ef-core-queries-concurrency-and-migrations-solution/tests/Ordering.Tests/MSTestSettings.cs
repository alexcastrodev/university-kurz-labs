// MSTest runs tests one at a time unless the assembly opts in. Here test classes run in parallel, and the
// methods of one class run one after another. Every class owns its own database, so classes cannot see
// each other's rows; the methods of a class share that database, so they must not run at the same time.
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]
