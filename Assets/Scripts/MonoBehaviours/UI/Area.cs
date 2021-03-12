public class Area
{
    public float Start { get; private set; }
    public float End { get; private set; }

    public Area(float start, float end)
    {
        Start = start;
        End = end;
    }

    public bool IsInside(float xPosition) => xPosition >= Start && xPosition <= End;

    public override string ToString() => $"[{Start} - {End}]";
}
