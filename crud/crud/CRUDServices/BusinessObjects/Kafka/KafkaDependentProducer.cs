using Confluent.Kafka;

namespace CollectionServices.BusinessObject.Kafka;

public class KafkaDependentProducer<TK, TV> : IKafkaDependentProducer<TK, TV>
{
    private readonly IProducer<TK, TV> _kafkaHandle;

    public KafkaDependentProducer(KafkaClientHandle handle)
    {
        _kafkaHandle = new DependentProducerBuilder<TK, TV>(handle.Handle).Build();
    }

    /// <summary>
    ///     Asynchronously produce a message and expose delivery information
    ///     via the returned Task. Use this method of producing if you would
    ///     like to await the result before flow of execution continues.
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task ProduceAsync(string topic, Message<TK, TV> message)
        => _kafkaHandle.ProduceAsync(topic, message);

    /// <summary>
    ///     Asynchronously produce a message and expose delivery information
    ///     via the provided callback function. Use this method of producing
    ///     if you would like flow of execution to continue immediately, and
    ///     handle delivery information out-of-band.
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <param name="deliveryHandler"></param>
    public void Produce(string topic, Message<TK, TV> message, Action<DeliveryReport<TK, TV>>? deliveryHandler = null)
        => _kafkaHandle.Produce(topic, message, deliveryHandler);

    public void Flush(TimeSpan timeout)
        => _kafkaHandle.Flush(timeout);
}