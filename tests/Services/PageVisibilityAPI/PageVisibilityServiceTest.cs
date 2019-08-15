﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;
using Shouldly;
using Xunit;

namespace Egil.RazorComponents.Bootstrap.Services.PageVisibilityAPI
{
    public class PageVisibilityServiceTest
    {
        private const string SubscribeFunction = "window.bootstrapDotNet.services.pageVisibilityApiInterop.subscribe";
        private const string UnsubscribeFunction = "window.bootstrapDotNet.services.pageVisibilityApiInterop.unsubscribe";

        private (PageVisibilityAPIService, Mock<IJSRuntime>, Mock<IComponentContext>) SetupSutAndJsRuntime(MockBehavior mockBehavior = MockBehavior.Strict)
        {
            var jsRuntimeMock = new Mock<IJSRuntime>(mockBehavior);
            var ccMock = new Mock<IComponentContext>(mockBehavior);
            ccMock.SetupGet(x => x.IsConnected).Returns(true);

            var sut = new PageVisibilityAPIService(jsRuntimeMock.Object, ccMock.Object, pvs => default!);

            return (sut, jsRuntimeMock, ccMock);
        }

        [Fact(DisplayName = "When user subscribes the adapter is subscribed to")]
        public void MyTestMethod2()
        {
            var (sut, jsRuntimeMock, _) = SetupSutAndJsRuntime();
            SetupSubscribeCall(jsRuntimeMock);

            sut.OnPageVisibilityChanged += (sender, e) => { };

            VerifySubscribeCall(jsRuntimeMock, Times.Once());
        }

        [Fact(DisplayName = "When another user subscribes and service is already subscribed to api adapter, no new subscription to api adapter is created")]
        public void MyTestMethod3()
        {
            var (sut, jsRuntimeMock, _) = SetupSutAndJsRuntime();
            SetupSubscribeCall(jsRuntimeMock);

            sut.OnPageVisibilityChanged += (sender, e) => { };
            sut.OnPageVisibilityChanged += (sender, e) => { };

            VerifySubscribeCall(jsRuntimeMock, Times.Once());
            jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "When SetVisibilityState is called with a new isVisisble value, subscribers are notified")]
        public void MyTestMethod4()
        {
            var (sut, _, _) = SetupSutAndJsRuntime(MockBehavior.Loose);
            var actualStatusUpdates = new List<bool>();
            sut.OnPageVisibilityChanged += (sender, e) => { actualStatusUpdates.Add(e.IsPageVisible); };

            sut.SetVisibilityState(false);

            actualStatusUpdates.ShouldBe(new List<bool> { false });
        }

        [Fact(DisplayName = "When SetVisibilityState is called with a existing isVisisble value, subscribers are not notified")]
        public void MyTestMethod5()
        {
            var (sut, _, _) = SetupSutAndJsRuntime(MockBehavior.Loose);
            var actualStatusUpdates = new List<bool>();
            sut.OnPageVisibilityChanged += (sender, e) => { actualStatusUpdates.Add(e.IsPageVisible); };

            sut.SetVisibilityState(sut.IsPageVisible);

            actualStatusUpdates.ShouldBeEmpty();
        }

        [Fact(DisplayName = "When there are no more subscribers left, the service unsubscribes from the api adapter")]
        public void MyTestMethod6()
        {
            static void Subscriber(object? sender, PageVisibilityChangedEventArgs args) { };
            var (sut, jsRuntimeMock, _) = SetupSutAndJsRuntime();
            var sequence = new MockSequence();
            SetupSubscribeCall(jsRuntimeMock, sequence);
            SetupUnsubscribeCall(jsRuntimeMock, sequence);

            sut.OnPageVisibilityChanged += Subscriber;
            sut.OnPageVisibilityChanged -= Subscriber;

            VerifySubscribeCall(jsRuntimeMock, Times.Once());
            VerifyUnsubscribeCall(jsRuntimeMock, Times.Once());
        }

        [Fact(DisplayName = "If two users subscribe at the same time, the api service subscription run only once")]
        public void MyTestMethod8()
        {
            var (sut, jsRuntimeMock, _) = SetupSutAndJsRuntime();
            var injectTask = new Task<object>(() => new object());
            var sequence = new MockSequence();
            SetupSubscribeCall(jsRuntimeMock, sequence, injectTask);

            sut.OnPageVisibilityChanged += (sender, e) => { };
            sut.OnPageVisibilityChanged += (sender, e) => { };
            injectTask.RunSynchronously();

            VerifySubscribeCall(jsRuntimeMock, Times.Once());
            jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "When Dispose is called, the service unsubscribes from api adapter, if subscribed")]
        public void MyTestMethod9()
        {
            var (sut, jsRuntimeMock, _) = SetupSutAndJsRuntime();
            var sequence = new MockSequence();
            SetupSubscribeCall(jsRuntimeMock, sequence);
            SetupUnsubscribeCall(jsRuntimeMock, sequence);
            sut.OnPageVisibilityChanged += (sender, e) => { };

            sut.Dispose();

            VerifySubscribeCall(jsRuntimeMock, Times.Once());
            VerifyUnsubscribeCall(jsRuntimeMock, Times.Once());
            jsRuntimeMock.VerifyNoOtherCalls();
        }


        [Fact(DisplayName = "If the last user unsubscribes at the same time as Dispose is called, unsubscribe to api adapter run only once")]
        public void MyTestMethod10()
        {
            var (sut, jsRuntimeMock, _) = SetupSutAndJsRuntime();
            var unsubscribeTask = new Task<object>(() => new object());
            var sequence = new MockSequence();
            SetupSubscribeCall(jsRuntimeMock, sequence);
            SetupUnsubscribeCall(jsRuntimeMock, sequence, unsubscribeTask);
            sut.OnPageVisibilityChanged += (sender, e) => { };

            sut.OnPageVisibilityChanged -= (sender, e) => { };
            sut.Dispose();
            unsubscribeTask.RunSynchronously();

            VerifySubscribeCall(jsRuntimeMock, Times.Once());
            VerifyUnsubscribeCall(jsRuntimeMock, Times.Once());
            jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "If there is no connection to client context, service waits until connection is up before attempting to subscribe")]
        public async Task MyTestMethod()
        {
            var isConnected = false;
            var (sut, jsRuntimeMock, ccMock) = SetupSutAndJsRuntime(MockBehavior.Loose);
            SetupSubscribeCall(jsRuntimeMock);

            ccMock.SetupGet(x => x.IsConnected).Returns(() => isConnected);

            sut.OnPageVisibilityChanged += (sender, e) => { };
            sut.Status.ShouldBe(PageVisibilityAPIServiceStatus.Subscribing);
            isConnected = true;
            await Task.Delay(100);
            sut.Status.ShouldBe(PageVisibilityAPIServiceStatus.Subscribed);
        }

        private static void SetupSubscribeCall(Mock<IJSRuntime> jsRuntimeMock, MockSequence? sequence = null, Task<object>? injectedTask = null)
        {
            injectedTask ??= Task.FromResult(new object());

            if (sequence is null)
                jsRuntimeMock.Setup(x => x.InvokeAsync<object>(It.IsAny<string>(), It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>())).Returns(injectedTask);
            else
                jsRuntimeMock.InSequence(sequence).Setup(x => x.InvokeAsync<object>(It.IsAny<string>(), It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>())).Returns(injectedTask);
        }

        private static void SetupUnsubscribeCall(Mock<IJSRuntime> jsRuntimeMock, MockSequence? sequence = null, Task<object>? injectedTask = null)
        {
            injectedTask ??= Task.FromResult(new object());

            if (sequence is null)
                jsRuntimeMock.Setup(x => x.InvokeAsync<object>(It.IsAny<string>())).Returns(injectedTask);
            else
                jsRuntimeMock.InSequence(sequence).Setup(x => x.InvokeAsync<object>(It.IsAny<string>())).Returns(injectedTask);
        }

        private static void VerifySubscribeCall(Mock<IJSRuntime> jsRuntimeMock, Times times)
        {
            jsRuntimeMock.Verify(x => x.InvokeAsync<object>(SubscribeFunction, It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>()), times);
        }

        private static void VerifyUnsubscribeCall(Mock<IJSRuntime> jsRuntimeMock, Times times)
        {
            jsRuntimeMock.Verify(x => x.InvokeAsync<object>(UnsubscribeFunction), times);
        }

    }
}
