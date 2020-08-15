

namespace Game2D
{
    public interface IDamagable
    {
        void Damage(IDieble enemy);

        int Magazine
        {
            get;
        }

        bool IsItemInHand
        {
            set;
        }
    }
}
