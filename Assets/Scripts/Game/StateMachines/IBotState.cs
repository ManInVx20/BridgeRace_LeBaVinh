public interface IBotState
{
    void Enter(Bot bot);
    void Execute(Bot bot);
    void Exit(Bot bot);
}
