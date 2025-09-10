namespace QuoteAnalyzer.Statistics;

/// <summary>
///     Streaming median estimator using the P² (Percentile-Percentile) algorithm.
///     This allows approximate median calculation without storing all values in memory.
/// </summary>
public sealed class MedianCalculator
{
    private const decimal TargetPercentile = 0.5m; // Median = 50th percentile

    // Desired positions of markers (where they should ideally be)
    private readonly decimal[] _desiredPositions = new decimal[5];

    // Actual positions of markers in the sorted data stream
    private readonly decimal[] _markerPositions = new decimal[5];

    // Marker heights (the actual values at marker positions)
    private readonly decimal[] _markerValues = new decimal[5];

    // How much each desired position should change with every new observation
    private readonly decimal[] _positionIncrements = new decimal[5];
    private bool _isInitialized;

    private long _sampleCount;

    /// <summary>
    ///     Gets the current estimate of the median.
    ///     Returns 0 if not enough data has been processed.
    /// </summary>
    public decimal Median => !_isInitialized ? 0m : _markerValues[2];

    /// <summary>
    ///     Adds a new sample into the estimator and updates internal markers.
    /// </summary>
    public void Add(decimal value)
    {
        if (_sampleCount < 5)
        {
            // Collect first 5 values before initialization
            _markerValues[_sampleCount++] = value;

            if (_sampleCount == 5)
            {
                Array.Sort(_markerValues);
                InitializeMarkers();
                _isInitialized = true;
            }

            return;
        }

        // Determine which cell the new value belongs to (0–3)
        var cellIndex = FindCellIndex(value);

        // Update marker positions
        for (var i = cellIndex + 1; i < 5; i++)
            _markerPositions[i]++;

        // Update desired marker positions
        for (var i = 0; i < 5; i++)
            _desiredPositions[i] += _positionIncrements[i];

        // Adjust markers if needed
        AdjustMarkers();
    }

    /// <summary>
    ///     Initializes marker positions, desired positions, and increments.
    ///     Called after the first 5 values are collected.
    /// </summary>
    private void InitializeMarkers()
    {
        _markerPositions[0] = 0;
        _markerPositions[1] = 1;
        _markerPositions[2] = 2;
        _markerPositions[3] = 3;
        _markerPositions[4] = 4;

        _desiredPositions[0] = 0;
        _desiredPositions[1] = 2 * TargetPercentile;
        _desiredPositions[2] = 4 * TargetPercentile;
        _desiredPositions[3] = 2 + 2 * TargetPercentile;
        _desiredPositions[4] = 4;

        _positionIncrements[0] = 0;
        _positionIncrements[1] = TargetPercentile / 2;
        _positionIncrements[2] = TargetPercentile;
        _positionIncrements[3] = (1 + TargetPercentile) / 2;
        _positionIncrements[4] = 1;
    }

    /// <summary>
    ///     Determines which interval (cell) the new value belongs to.
    /// </summary>
    private int FindCellIndex(decimal value)
    {
        if (value < _markerValues[0])
        {
            _markerValues[0] = value;
            return 0;
        }

        if (value < _markerValues[1]) return 0;
        if (value < _markerValues[2]) return 1;
        if (value < _markerValues[3]) return 2;

        if (value >= _markerValues[4])
            _markerValues[4] = value;

        return 3;
    }

    /// <summary>
    ///     Adjusts internal markers so that actual positions follow desired positions.
    /// </summary>
    private void AdjustMarkers()
    {
        for (var i = 1; i <= 3; i++)
        {
            var delta = _desiredPositions[i] - _markerPositions[i];

            // Adjust only if the marker is too far from where it should be
            if ((delta >= 1 && _markerPositions[i + 1] - _markerPositions[i] > 1) ||
                (delta <= -1 && _markerPositions[i - 1] - _markerPositions[i] < -1))
            {
                var direction = Math.Sign(delta);

                var candidateValue = ParabolicEstimate(i, direction);

                // Ensure monotonic order, otherwise fall back to linear
                if (_markerValues[i - 1] < candidateValue && candidateValue < _markerValues[i + 1])
                    _markerValues[i] = candidateValue;
                else
                    _markerValues[i] = LinearEstimate(i, direction);

                _markerPositions[i] += direction;
            }
        }
    }

    /// <summary>
    ///     Estimates new marker value using a parabolic formula.
    /// </summary>
    private decimal ParabolicEstimate(int i, int direction)
    {
        var numerator =
            direction * (_markerPositions[i] - _markerPositions[i - 1] + direction) *
            (_markerValues[i + 1] - _markerValues[i]) /
            (_markerPositions[i + 1] - _markerPositions[i]) +
            direction * (_markerPositions[i + 1] - _markerPositions[i] - direction) *
            (_markerValues[i] - _markerValues[i - 1]) /
            (_markerPositions[i] - _markerPositions[i - 1]);

        return _markerValues[i] + numerator / (_markerPositions[i + 1] - _markerPositions[i - 1]);
    }

    /// <summary>
    ///     Estimates new marker value using a linear formula (fallback).
    /// </summary>
    private decimal LinearEstimate(int i, int direction)
    {
        return _markerValues[i] +
               direction * (_markerValues[i + direction] - _markerValues[i]) /
               (_markerPositions[i + direction] - _markerPositions[i]);
    }
}