using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Peeb.Bot.Timers;

namespace Peeb.Bot.UnitTests.Timers
{
    [TestFixture]
    [Parallelizable]
    public class AsyncTimerTests : TestBase<AsyncTimerTestsContext>
    {
        [Test]
        public void Start_ShouldStartTimer()
        {
            Test(
                c => c.Start(),
                c => c.Timer.Verify(t => t.Start(It.IsAny<Action>(), c.DueTime, c.Period), Times.Once));
        }

        [Test]
        public void Start_TimerNotElapsed_ShouldSetElapsedToFalse()
        {
            Test(
                c => c.Start(),
                c => c.AsyncTimer.Elapsed.Should().BeFalse());
        }

        [Test]
        public void Start_TimerElapsed_ShouldSetElapsedToTrue()
        {
            Test(
                c => c.SetTimerElapsed(),
                c => c.Start(),
                c => c.AsyncTimer.Elapsed.Should().BeTrue());
        }

        [Test]
        public void Start_TimerElapsed_ShouldInvokeCallback()
        {
            Test(
                c => c.SetTimerElapsed(),
                c => c.Start(),
                c => c.Callback.Verify(cb => cb(), Times.Once));
        }

        [Test]
        public void Start_TimerElapsedAndCallbackThrowsException_ShouldNotThrowException()
        {
            TestException(
                c => c.SetTimerElapsed().SetCallbackException(),
                c => c.Start(),
                (c, a) => a.Should().NotThrow());
        }

        [Test]
        public Task Stop_ShouldStopTimer()
        {
            return TestAsync(
                c => c.Stop(),
                c => c.Timer.Verify(t => t.Stop(), Times.Once));
        }

        [Test]
        public Task Stop_TimerElapsedAndCallbackStillRunning_ShouldAwaitCallback()
        {
            return TestAsync(
                c => c.SetTimerElapsed().SetCallbackDelay(),
                c => c.StartStop(),
                c => c.Stopwatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(100));
        }
    }

    public class AsyncTimerTestsContext
    {
        public IAsyncTimer AsyncTimer { get; set; }
        public Mock<Func<Task>> Callback { get; set; }
        public TimeSpan DueTime { get; set; }
        public Mock<ILogger<AsyncTimer>> Logger { get; set; }
        public TimeSpan Period { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public Mock<ITimer> Timer { get; set; }

        public AsyncTimerTestsContext()
        {
            Callback = new Mock<Func<Task>>();
            DueTime = TimeSpan.FromSeconds(1);
            Logger = new Mock<ILogger<AsyncTimer>>();
            Period = TimeSpan.FromSeconds(2);
            Stopwatch = new Stopwatch();
            Timer = new Mock<ITimer>();

            AsyncTimer = new AsyncTimer(Timer.Object, Logger.Object);
        }

        public void Start()
        {
            AsyncTimer.Start(Callback.Object, DueTime, Period);
        }

        public Task Stop()
        {
            return AsyncTimer.Stop();
        }

        public async Task StartStop()
        {
            Start();
            await Stop();
            Stopwatch.Stop();
        }

        public AsyncTimerTestsContext SetTimerElapsed()
        {
            Timer
                .Setup(t => t.Start(It.IsAny<Action>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Callback((Action callback, TimeSpan _, TimeSpan _) => callback());

            return this;
        }

        public AsyncTimerTestsContext SetCallbackException()
        {
            Callback.Setup(c => c()).Throws<Exception>();

            return this;
        }

        public AsyncTimerTestsContext SetCallbackDelay()
        {
            Callback.Setup(c => c()).Callback(() => Stopwatch.Start()).Returns(() => Task.Delay(100));

            return this;
        }
    }
}
