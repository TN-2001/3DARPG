public interface IBattlerStatusData {
    public int Hp { get; }
    public int Atk { get; }
}

public interface IBattlerStatus {
    public int Hp { get; }
    public int CurrentHp { get; }
    public int Atk { get; }

    public int UpdateHp(int damage);
}
