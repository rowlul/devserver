using System.Diagnostics;

using FluentAssertions;

using Xunit;

namespace DevServer.Services.Tests;

public class NativeRunnerTests
{
    private readonly INativeRunner _runner;

    public NativeRunnerTests()
    {
        var processMock = new ProcessMock();
        _runner = new NativeRunner(processMock);
    }

    [Fact]
    public void RunWithArgs_ShouldStartProcessWithArgs()
    {
        var expected = new ProcessStartInfo { FileName = "osu!.exe", Arguments = "-devserver localhost" };
        var actual = _runner.RunWithArgs("osu!.exe", "localhost")?.StartInfo;

        actual.Should().BeEquivalentTo(expected,
                                       x => x
                                            .Including(x => x.FileName)
                                            .Including(x => x.Arguments));
    }
}
