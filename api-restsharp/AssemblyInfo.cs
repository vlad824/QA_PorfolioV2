// Fixtures run in parallel; tests inside a fixture run sequentially and own their data.
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]
