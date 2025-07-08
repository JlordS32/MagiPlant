public interface IUpgradableCurrency
{
    void ApplyUpgrade(UpgradeEntry upgrade, CurrencyType type);
}