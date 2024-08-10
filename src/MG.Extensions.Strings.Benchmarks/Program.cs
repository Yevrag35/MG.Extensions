using BenchmarkDotNet.Running;
using MG.Extensions.Strings.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<SpanStringBuilderBench>();
    }
}