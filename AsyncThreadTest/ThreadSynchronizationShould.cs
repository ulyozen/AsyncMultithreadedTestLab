using FluentAssertions;

namespace AsyncThreadTest;

public class ThreadSynchronizationShould
{
    // Общая переменная 
    private int _sharedResource;
    // Объект-заглушка для синхронизации
    private readonly object _lockObject = new object();

    private void IncrementResource(bool isLocked)
    {
        for (int i = 0; i < 100000; i++)
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
        // Инициализация общей переменной перед каждым тестом
        _sharedResource = 0;
        
        // Создаем три потока, которые будут инкрементировать общую переменную
        var thread1 = new Thread(() => IncrementResource(isLocked));
        var thread2 = new Thread(() => IncrementResource(isLocked));
        var thread3 = new Thread(() => IncrementResource(isLocked));
        
        // Запускаем потоки
        thread1.Start();
        thread2.Start();
        thread3.Start();

        // Ожидаем завершения потоков
        thread1.Join();
        thread2.Join();
        thread3.Join();

        if (isLocked) 
            _sharedResource.Should().Be(300000); 
        else 
            _sharedResource.Should().NotBe(300000);
    }
}