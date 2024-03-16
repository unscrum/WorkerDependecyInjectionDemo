using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Worker.Services;

namespace Worker.Test
{ 
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test_Looper_False()
        {
            var picker = new Mock<IPickANumber>(MockBehavior.Strict);
            picker.Setup(p => p.Random()).Returns(0);
            
            var metric = new Mock<IMyMetric>(MockBehavior.Strict);
            metric.Setup(p => p.TrackValue(It.IsAny<double>()));
            
            var telemetry = new Mock<IMyTelemetry>(MockBehavior.Strict);
            telemetry.Setup(p => p.GetMetric(It.IsAny<string>())).Returns(metric.Object);

            var looper = new Looper(NullLoggerFactory.Instance, picker.Object, telemetry.Object);
            telemetry.Verify(p=>p.GetMetric(It.IsAny<string>()), Times.Once);

            looper.DoActualLoopWork();
            
            picker.Verify(p=>p.Random(), Times.Once);
            metric.Verify(p=>p.TrackValue(It.IsAny<double>()), Times.Once);

        }
        
        [Test]
        public void Test_Looper_True()
        {

            var counter = 0;
            var picker = new Mock<IPickANumber>(MockBehavior.Strict);
            picker.Setup(p => p.Random()).Returns(0).Callback(()=>
            {
                counter += 1;
            });
            
            var metric = new Mock<IMyMetric>(MockBehavior.Strict);
            metric.Setup(p => p.TrackValue(It.IsAny<double>()));
            
            var telemetry = new Mock<IMyTelemetry>(MockBehavior.Strict);
            telemetry.Setup(p => p.GetMetric(It.IsAny<string>())).Returns(metric.Object);

            var looper = new Looper(NullLoggerFactory.Instance, picker.Object, telemetry.Object);
            telemetry.Verify(p=>p.GetMetric(It.IsAny<string>()), Times.Once);
            looper.Running = true;
            Task.Delay(2000).ContinueWith(t=>
            {
                looper.Running = false;
            });
            looper.DoActualLoopWork();
            picker.Verify(p=>p.Random(), Times.Exactly(counter));
            metric.Verify(p=>p.TrackValue(It.IsAny<double>()), Times.Once);

        }

        [Test]
        public void Test_MyTelemetry()
        {
            var mtm = new MyTelemetry(new TelemetryClient(TelemetryConfiguration.CreateDefault()));
            var met = mtm.GetMetric("foo");
            Assert.That(met, Is.InstanceOf<MyMetric>());
            var mymet = (MyMetric) met;
            Assert.That(mymet.Metric, Is.Not.Null);
        }
        
        [Test]
        public void Test_PickANumber()
        {
            var metric = new Mock<IMyMetric>(MockBehavior.Strict);
            metric.Setup(p => p.TrackValue(It.IsAny<double>()));
            
            var telemetry = new Mock<IMyTelemetry>(MockBehavior.Strict);
            telemetry.Setup(p => p.GetMetric(It.IsAny<string>())).Returns(metric.Object);

            var pan = new PickANumber(telemetry.Object); 
            var number = pan.Random();
            telemetry.Verify(p=>p.GetMetric(It.IsAny<string>()), Times.Once);
            metric.Verify(p=>p.TrackValue(It.IsAny<double>()), Times.Once);
        }
        
        [Test]
        public async Task Test_Worker()
        {
            var looper = new Mock<ILooper>(MockBehavior.Strict);

            looper.SetupSet(p=>p.Running = true);
            looper.Setup(p => p.DoActualLoopWork());
            var worker = new Worker(NullLoggerFactory.Instance, looper.Object);            
            await worker.StartAsync(CancellationToken.None);

            looper.VerifySet(p => p.Running = true, Times.Once);
            looper.Verify(p=>p.DoActualLoopWork(), Times.Once);
            
            looper.SetupSet(p=>p.Running = false);
            await worker.StopAsync(CancellationToken.None);
            looper.VerifySet(p=>p.Running=false, Times.Once);
        }
    }
}