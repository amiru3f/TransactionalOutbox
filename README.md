# Simple [Transactional Outbox](https://microservices.io/patterns/data/transactional-outbox.html)

This is a real simple and super fast solution to have a [Transactional Outbox](https://microservices.io/patterns/data/transactional-outbox.html) without any dependencies and with distribution support (SQLSERVER Implementation). I will provide more details about the use cases

## What

As you may know, there are several use cases to use event driven solutions in distributed architectures

In many circumstances delivering the events between bounded contexts or different domains may suck, for example:

* Data is persisted inside one service but the domain event is not published because of network problems or some other errors in the application layer (Retrying is a problem)
* None delivered events cause inconsistency in the system
* In some cases a specific event deliver more than once for the retries, hence handling the idempotency in the consumers (using Inboxes or etc) is somehow a problem
* Loosing the events because of In-Memory retries can also be a problem

To be continued...
