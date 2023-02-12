# Simple [Transactional Outbox](https://microservices.io/patterns/data/transactional-outbox.html)

This is a real simple and super fast solution to have a [Transactional Outbox](https://microservices.io/patterns/data/transactional-outbox.html) without any dependencies and with distribution support (SQLSERVER Implementation). I will provide more details about the use cases

## What

As you may know, there are several use cases to use event driven approaches in distributed architectures

In many circumstances delivering the events between bounded contexts or different domains may suck, for example:

* Data is persisted in one service but the domain-event is not published because of network problems or some other issues in the application layer (Retrying would be a problem)
* None delivered events cause inconsistency in the system
* In some cases a specific event delivers more than once for the retries, hence handling the idempotency in the consumers (using Inboxes or etc) is somehow a problem
* Loosing the events because of In-Memory retries can also be a problem

## Why

* Transactional outbox helps consitency of event delivery and data persistence using atomic storage of the pair (data, event)
* An event publisher engine ensures that the persisted events delivers with a minimum QOS
* In some cases redelivery approach can be done using persisted events (In order to fix data loss)

## How

There are several ways to implement outbox pattern (delivery problem)

    1- Seperation of event-delivery concern using a single-instance engine (It is not scalable)
    
    2- Seperation of event-delivery concern using transactional-log-tailing and some tools like Debezium (Debezium itself is a third party tool and needs  to be maintained and it is also a new dependency itself)
    
    3- Implementation of background-jobs inside the application layer and handling concurrency of distributed multi instance pods using REDIS (Now we are dependent to the Redis and it is a big problem)

    4- Handling the concurrency and repeated events in the consumer side using Inbox (It helps but in some cases processing concurrent events in the producer and consumer has performance problem and code repetition)

Consider that we choose ``application level outbox`` in order to solve the problem:

* In a simple form without the concern of newborn services

* Avoid being dependent to REDIS and Debezium (Which does not support RabbitMQ)

* Using SqlServer which is our main DB in some .Net environments

## Implementation Details

Tools and frameworks:

* MSSQL
* .Net 6+ Background (hosted) services
* EFCore / Migrations
* XUnit (for concurrency testing approaches)
* RabbitMQ client (This is ommitted and implemented using mock object)
