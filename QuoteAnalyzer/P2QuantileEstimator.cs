namespace QuoteAnalyzer;

public sealed class P2QuantileEstimator
{
    private readonly decimal _p; // quantile (0..1)
    private readonly decimal[] dn = new decimal[5];
    private readonly decimal[] n = new decimal[5];
    private readonly decimal[] np = new decimal[5];
    private readonly decimal[] q = new decimal[5];
    private long _count;
    private bool _initialized;

    public P2QuantileEstimator(decimal p)
    {
        if (p <= 0 || p >= 1) throw new ArgumentOutOfRangeException(nameof(p));
        _p = p;
    }

    public decimal Estimate => !_initialized ? 0m : q[2];

    public void Add(decimal x)
    {
        if (_count < 5)
        {
            q[_count++] = x;
            if (_count == 5)
            {
                Array.Sort(q);
                n[0] = 0;
                n[1] = 1;
                n[2] = 2;
                n[3] = 3;
                n[4] = 4;
                np[0] = 0;
                np[1] = 2 * _p;
                np[2] = 4 * _p;
                np[3] = 2 + 2 * _p;
                np[4] = 4;
                dn[0] = 0;
                dn[1] = _p / 2;
                dn[2] = _p;
                dn[3] = (1 + _p) / 2;
                dn[4] = 1;
                _initialized = true;
            }

            return;
        }

        // find cell k
        int k;
        if (x < q[0])
        {
            q[0] = x;
            k = 0;
        }
        else if (x < q[1])
        {
            k = 0;
        }
        else if (x < q[2])
        {
            k = 1;
        }
        else if (x < q[3])
        {
            k = 2;
        }
        else if (x < q[4])
        {
            k = 3;
        }
        else
        {
            q[4] = x;
            k = 3;
        }

        for (var i = k + 1; i < 5; i++) n[i]++;
        for (var i = 0; i < 5; i++) np[i] += dn[i];

        // adjust positions of q[i]
        for (var i = 1; i <= 3; i++)
        {
            var d = np[i] - n[i];
            if ((d >= 1 && n[i + 1] - n[i] > 1) || (d <= -1 && n[i - 1] - n[i] < -1))
            {
                var s = Math.Sign(d);
                var qn = Parabolic(i, s);
                if (q[i - 1] < qn && qn < q[i + 1]) q[i] = qn;
                else q[i] = Linear(i, s);
                n[i] += s;
            }
        }
    }

    private decimal Parabolic(int i, int d)
    {
        var a = d * (n[i] - n[i - 1] + d) * (q[i + 1] - q[i]) / (n[i + 1] - n[i])
                + d * (n[i + 1] - n[i] - d) * (q[i] - q[i - 1]) / (n[i] - n[i - 1]);
        return q[i] + a / (n[i + 1] - n[i - 1]);
    }

    private decimal Linear(int i, int d)
    {
        return q[i] + d * (q[i + d] - q[i]) / (n[i + d] - n[i]);
    }
}