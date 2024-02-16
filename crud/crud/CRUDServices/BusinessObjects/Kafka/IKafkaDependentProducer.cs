using Confluent.Kafka;

namespace CollectionServices.BusinessObject.Kafka;

public interface IKafkaDependentProducer<TK, TV>
{
    void Flush(TimeSpan timeout);
    void Produce(string topic, Message<TK, TV> message, Action<DeliveryReport<TK, TV>>? deliveryHandler = null);
    Task ProduceAsync(string topic, Message<TK, TV> message);
}