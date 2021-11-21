using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Clip clip = new Clip(10, 20);
            Weapon weapon = new Weapon(10, 2, clip);
            Bot bot = new Bot(weapon);
            Player player = new Player(60);
            int shotsCount = 7;

            for (int i = 0; i < shotsCount; i++)
            {
                bot.OnSeePlayer(player);
            }
        }
    }

    class Math
    {
        public static int Clamp(int value, int minValue, int maxValue)
        {
            int number;

            Exception.IsMinGreaterMax(minValue, nameof(minValue), maxValue, nameof(maxValue));

            if (value < minValue)
                number = minValue;
            else if (value > maxValue)
                number = maxValue;
            else
                number = value;

            return number;
        }
    }

    class Exception
    {
        public static void IsNegativeNumber(int number, string name)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(name);
        }

        public static void IsMinGreaterMax(int minValue, string minName, int maxValue, string maxName)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(minName + " > " + maxName);
        }

        public static void IsNull(object receivedObject, string name)
        {
            if (receivedObject == null)
                throw new ArgumentNullException(name);
        }
    }

    class Weapon : ICloneable
    {
        private readonly int _damage;
        private readonly int _neededBulletsCount;
        private readonly Clip _clip;

        public Weapon(int damage, int neededBulletsCount, Clip clip)
        {
            Exception.IsNegativeNumber(damage, nameof(damage));
            Exception.IsNegativeNumber(neededBulletsCount, nameof(neededBulletsCount));
            Exception.IsNull(clip, nameof(clip));

            _damage = damage;
            _neededBulletsCount = neededBulletsCount;
            _clip = (Clip)clip.Clone();
        }

        protected Weapon(Weapon weapon) : this(weapon._damage, weapon._neededBulletsCount, weapon._clip) { }

        public bool CanFire => _neededBulletsCount <= _clip.CurrentBulletsCount;

        public IReadOnlyClip Clip => _clip;

        public virtual object Clone()
        {
            return new Weapon(this);
        }

        public void Fire(Player player)
        {
            _clip.RemoveBullets(_neededBulletsCount);

            player.TakeDamage(_damage);
        }

        public void Reload(int bullets)
        {
            _clip.AddBullets(bullets);
        }
    }

    interface IReadOnlyClip
    {
        int MaxBulletsCount { get; }
        int CurrentBulletsCount { get; }
        int ShortageBulletsCount { get; }
    }

    class Clip : IReadOnlyClip, ICloneable
    {
        public Clip(int currentBulletsCount, int maxBulletsCount)
        {
            Exception.IsNegativeNumber(currentBulletsCount, nameof(currentBulletsCount));
            Exception.IsNegativeNumber(maxBulletsCount, nameof(maxBulletsCount));
            Exception.IsMinGreaterMax(currentBulletsCount, nameof(currentBulletsCount), maxBulletsCount, nameof(maxBulletsCount));

            CurrentBulletsCount = currentBulletsCount;
            MaxBulletsCount = maxBulletsCount;
        }

        public Clip(Clip clip) : this(clip.CurrentBulletsCount, clip.MaxBulletsCount) { }

        public int MaxBulletsCount { get; }

        public int CurrentBulletsCount { get; private set; }

        public int ShortageBulletsCount => MaxBulletsCount - CurrentBulletsCount;

        public virtual object Clone()
        {
            return new Clip(this);
        }

        public void AddBullets(int count)
        {
            Exception.IsNegativeNumber(count, nameof(count));
            Exception.IsMinGreaterMax(count, nameof(count), ShortageBulletsCount, nameof(ShortageBulletsCount));

            CurrentBulletsCount += count;
        }

        public void RemoveBullets(int count)
        {
            Exception.IsNegativeNumber(count, nameof(count));
            Exception.IsMinGreaterMax(count, nameof(count), CurrentBulletsCount, nameof(CurrentBulletsCount));

            CurrentBulletsCount -= count;
        }
    }

    class Player
    {
        private int _health;

        public Player(int health)
        {
            Exception.IsNegativeNumber(health, nameof(health));

            _health = health;
        }

        public bool IsAlive => _health > 0;

        public void TakeDamage(int damage)
        {
            Exception.IsNegativeNumber(damage, nameof(damage));

            int health = _health - damage;

            if (IsAlive)
                _health = Math.Clamp(health, 0, int.MaxValue);
        }
    }

    class Bot
    {
        private readonly Weapon _weapon;

        public Bot(Weapon weapon)
        {
            Exception.IsNull(weapon, nameof(weapon));

            _weapon = (Weapon)weapon.Clone();
        }

        public void OnSeePlayer(Player player)
        {
            if (_weapon.CanFire)
                _weapon.Fire(player);

            if (_weapon.CanFire == false)
                ReloadWeapon();
        }

        private void ReloadWeapon()
        {
            int shortageBulletsCount = _weapon.Clip.ShortageBulletsCount;
            int newBullets = shortageBulletsCount;

            if (shortageBulletsCount > 0)
            {
                _weapon.Reload(newBullets);
            }
        }
    }
}