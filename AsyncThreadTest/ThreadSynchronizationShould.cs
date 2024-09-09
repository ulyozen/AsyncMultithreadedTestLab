using FluentAssertions;

namespace AsyncThreadTest;

public class ThreadSynchronizationShould
{
    private int _sharedResource;
    private readonly object _lockObject = new object();

    private void IncrementResource(bool isLocked)
    {
        for (var i = 0; i < 100000; i++)
        {
            if (isLocked)
                lock (_lockObject) _sharedResource++;
            else
                _sharedResource++;
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void VerifySharedResource_WhenSynchronizedOrNot(bool isLocked)
    {
        _sharedResource = 0;
        
        var thread1 = new Thread(() => IncrementResource(isLocked));
        var thread2 = new Thread(() => IncrementResource(isLocked));
        var thread3 = new Thread(() => IncrementResource(isLocked));
        
        thread1.Start();
        thread2.Start();
        thread3.Start();

        thread1.Join();
        thread2.Join();
        thread3.Join();

        if (isLocked) 
            _sharedResource.Should().Be(300000); 
        else 
            _sharedResource.Should().NotBe(300000);
    }
}