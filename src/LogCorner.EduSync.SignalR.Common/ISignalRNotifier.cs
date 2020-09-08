﻿using LogCorner.EduSync.Speech.SharedKernel.Events;
using System;
using System.Threading.Tasks;

namespace LogCorner.EduSync.SignalR.Common
{
    public interface ISignalRNotifier
    {
        event Action<string, EventStore> ReceivedOnPublishToTopic;

        Task StartAsync();

        Task OnPublish();

        Task OnPublish(string topic);

        Task StopAsync();
    }
}