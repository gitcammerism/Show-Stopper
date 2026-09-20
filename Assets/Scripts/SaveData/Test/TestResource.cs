using UnityEngine;

public class TestResource : MonoBehaviour
{
    public int testResource;

    public void AddTestResource()
    {
        testResource += 1;
    }

    public void SubtractTestResource()
    {
        testResource -= 1;
    }
}
