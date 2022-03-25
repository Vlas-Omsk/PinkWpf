using System;
using System.Threading.Tasks;

namespace PinkWpf
{
    public interface INavigationListener
    {
        /// <summary>
        /// Вызывается перед навигацией в элемент
        /// </summary>
        /// <returns>Если <see langword="true"/> навигация происходит, если <see langword="false"/> отменяется</returns>
        Task<bool> Navigating();
        /// <summary>
        /// Вызывается после навигации в элемент
        /// </summary>
        Task Navigated();
        /// <summary>
        /// Вызывается перед навигацией из элемента
        /// </summary>
        /// <returns>Если <see langword="true"/> навигация происходит, если <see langword="false"/> отменяется</returns>
        Task<bool> NavigatingTo();
        /// <summary>
        /// Вызывается после навигации из элемента
        /// </summary>
        Task NavigatedTo();
        /// <summary>
        /// Вызывается перед навигацией из элемента и его уничтожением
        /// </summary>
        /// <returns>Если <see langword="true"/> навигация происходит, если <see langword="false"/> отменяется</returns>
        Task<bool> Destroying();
        /// <summary>
        /// Вызывается после навигации из элемента и его уничтожении
        /// </summary>
        Task Destroyed();
    }
}
