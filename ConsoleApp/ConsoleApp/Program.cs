using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Weapon weapon = new Weapon(10, 2, 20, 10);
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
            
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue) + " > " + nameof(maxValue));

            if (value < minValue)
                number = minValue;
            else if (value > maxValue)
                number = maxValue;
            else
                number = value;

            return number;
        }
    }
    
    class Weapon 
    {
        private readonly int _damage;
        private readonly int _neededBullets;
        private readonly int _maxBullets;

        private int _currentBullets = 0;

        public Weapon(int damage, int neededBullets, int maxBullets, int currentBullets)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));
            if (neededBullets < 0)
                throw new ArgumentOutOfRangeException(nameof(neededBullets));
            if (maxBullets < 0)
                throw new ArgumentOutOfRangeException(nameof(maxBullets));

            _damage = damage;
            _neededBullets = neededBullets;
            _maxBullets = maxBullets;
            
            AddBullets(currentBullets);
        }

        public Weapon(Weapon weapon) : this(weapon._damage, weapon._neededBullets, weapon._maxBullets, weapon._currentBullets) { }

        public bool CanFire => _currentBullets >= _neededBullets;

        public int ShortageBullets => _maxBullets - _currentBullets;

        public void Fire(Player player)
        {
            RemoveBullets();

            player.TakeDamage(_damage);
        }

        public void AddBullets(int bullets)
        {
            if (bullets < 0)
                throw new ArgumentOutOfRangeException(nameof(bullets));
            if (bullets > ShortageBullets)
                throw new ArgumentOutOfRangeException(nameof(bullets) + " > " + nameof(ShortageBullets));

            _currentBullets += bullets;
        }

        private void RemoveBullets()
        {
            if (_neededBullets > _currentBullets)
                throw new ArgumentOutOfRangeException(nameof(_neededBullets) + " > " + nameof(_currentBullets));

            _currentBullets -= _neededBullets;
        }
    }    
    
    class Player
    {
        private int _health;

        public Player(int health)
        {
            if (health < 0)
                throw new ArgumentOutOfRangeException(nameof(health));

            _health = health;
        }

        public bool IsAlive => _health > 0;

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

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
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));

            _weapon = new Weapon(weapon);
        }

        public void OnSeePlayer(Player player)
        {
            if (player.IsAlive)
            {
                if (_weapon.CanFire)
                    _weapon.Fire(player);

                if (_weapon.CanFire == false)
                    TryReloadWeapon();
            }
        }

        private void TryReloadWeapon()
        {
            int shortageBullets = _weapon.ShortageBullets;

            if (shortageBullets > 0)
                _weapon.AddBullets(shortageBullets);
        }
    }
}