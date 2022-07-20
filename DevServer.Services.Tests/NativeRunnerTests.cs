using System.Diagnostics;

using FluentAssertions;

using Xunit;

namespace DevServer.Services.Tests;

public class NativeRunnerTests
{
    [Fact]
    public void RunWithArgs_ShouldStartProcessWithArgs()
    {
        var processMock = new ProcessMock();
        var runner = new NativeRunner(processMock);

        var expected = new ProcessStartInfo { FileName = "osu!.exe", Arguments = "-devserver localhost" };
        var actual = runner.RunWithArgs("osu!.exe", "localhost")?.StartInfo;

        actual.Should().BeEquivalentTo(expected,
                                       x => x
                                            .Including(x => x.FileName)
                                            .Including(x => x.Arguments));
    }
}
