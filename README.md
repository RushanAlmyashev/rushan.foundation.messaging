# rushan.foundation.messaging

## Overview

An assembly for interact with RabbitMQ broker.

## Features

* Easy way to startup with RabbitMQ broker in project
* Provide publish and consume messages, without trivial settings

## How to use 

Important! Use only "topic" exchange type.

### Config


```
MessagingConfiguration
{
	MessageBrokerUri = "amqp://guest:guest@localhost",
	Exchange = "amq.topic"
}
```


### Connect to rabbitMQ
```
{
    var messaging = new RabbitMqMessaging(_messagingConfiguration);
    messaging.Start();
}
```


### Publish Message

#### Concrete messsage
```
public class MessageOne
{
    public string Value { get; set; }
}
```

#### Publish message instance in message bus
```
{
    //Connect to message bus
    var messaging = new RabbitMqMessaging(_messagingConfiguration);
    //Start iteraction
    messaging.Start();
    //Create message instance
    var message = new MessageOne() { Value = DateTime.UtcNow };
    //Publish message in exchange
    messaging.Publish(message);
}
```


### Consume Message

#### Concrete messsage
```
public class MessageOne
{
    public string Value { get; set; }
}
```

#### Message Reciever
```
public class MessageReciver : IMessageReceiver<MessageOne>
{
    public async Task ReceiveMessageAsync(MessageOne message)
    {
        Console.WriteLine(message.Value);
        await Task.CompletedTask;
    }
}
```

#### Start consume message from message bus
```
{
    //Connect to rabbitMQ
    var messaging = new RabbitMqMessaging(_messagingConfiguration);
    //Initailize reciever instance
    IMessageReceiver<MessageOne> reciever = new MessageReciver();
    //Subscribe messageReceiver to message bus, for recieve concrete message
    messaging.Subscribe(reciever);
    //Starting 
    messaging.Start();
}
```
