# QuoteAnalyzer

**QuoteAnalyzer** is a .NET solution for real-time processing and analysis of quote data streams. It simulates a stock market feed, computes statistical metrics, and demonstrates high-performance, low-latency data processing.

---

## Features

* UDP Quote Broadcaster and Receiver
* Online statistics: mean, standard deviation, mode, median
* Supports Dictionary (exact) and Space-Saving (approximate) mode algorithms
* Handles very large volumes of data (trillions of quotes) efficiently
* Tracks lost quotes and network errors
* Thread-safe, multi-threaded architecture for receiving and processing data
* Fully unit-tested

---

## Configuration

Both Broadcaster and Analyzer use a shared `config.xml` file with the following structure:

```xml
<Settings>
    <!-- Multicast IP address and port for streaming quotes -->
    <MulticastIP>239.0.0.222</MulticastIP>
    <Port>5000</Port>

    <!-- Range of simulated market quotes -->
    <MinValue>10.00</MinValue>
    <MaxValue>2000.00</MaxValue>

    <!-- Tick size for quotes (step between possible values) -->
    <TickSize>0.10</TickSize>

    <!-- Mode calculation algorithm -->
    <ModeAlgorithm>Dictionary</ModeAlgorithm>
    <!-- Options:
         "Dictionary" – exact counting (may use significant memory for large value ranges),
         "SpaceSaving" – approximate counting for large streams with limited memory (may slightly affect accuracy) -->
</Settings>
```

* `MulticastIP` and `Port` specify where the server broadcasts quotes and where the client listens.
* `MinValue`, `MaxValue`, `TickSize` define the range and steps of simulated quotes.
* `ModeAlgorithm` selects the algorithm for mode calculation.

---

## Running

### 1. Broadcaster

```bash
cd QuoteBroadcaster
dotnet run
```

* Continuously generates quotes and sends them via UDP multicast.

### 2. Analyzer

```bash
cd QuoteAnalyzer
dotnet run
```

* Receives quotes and calculates statistics in real-time.
* Press **Enter** to display current statistics.
* Press **q** or Ctrl+C to quit.

---

## Unit Tests

Run all tests:

```bash
dotnet test
```

* **QuoteAnalyzer.Tests** → Welford, P² quantile estimator, mode counters
* **QuoteBroadcaster.Tests** → QuoteBroadcaster components

---

## Design Highlights

* **High performance:** optimized for minimal latency, suitable for massive data streams.
* **Reliable:** tracks lost quotes, handles network errors, and can run continuously for days or weeks.
* **Thread-safe:** uses `Channel<T>` for safe multi-threaded communication.
* **Configurable:** multicast IP, port, and mode algorithms via a single shared XML file.
