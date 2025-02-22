// <copyright file="GreeterService.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
using Greet;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace OpenTelemetry.Instrumentation.Grpc.Services.Tests;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger logger;

    public GreeterService(ILoggerFactory loggerFactory)
    {
        this.logger = loggerFactory.CreateLogger<GreeterService>();
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        this.logger.LogInformation("Sending hello to {Name}", request.Name);
        return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
    }

    public override async Task SayHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        var i = 0;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var message = $"How are you {request.Name}? {++i}";
            this.logger.LogInformation("Sending greeting {Message}.", message);

            await responseStream.WriteAsync(new HelloReply { Message = message });

            // Gotta look busy
            await Task.Delay(1000);
        }
    }
}
