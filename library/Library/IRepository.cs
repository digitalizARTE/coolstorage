using System;
namespace Vici.CoolStorage
{
    interface IRepository
    {
        TEntityType GetNew<TEntityType>() where TEntityType : CSObject<TEntityType>;
        TEntityType GetNew<TEntityType>(bool addToUnitOfWork) where TEntityType : CSObject<TEntityType>;
        void Save();
    }
}
