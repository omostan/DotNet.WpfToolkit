#region copyright

/*****************************************************************************************
*                                     ______________________________________________     *
*                              o O   |                                              |    *
*                     (((((  o      <               DotNet WPF Tool Kit             |    *
*                    ( o o )         |______________________________________________|    *
* ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
*             Project: DotNetTools.Wpfkit                                                *
*            Filename: CompareCollections.cs                                             *
*              Author: Stanley Omoregie                                                  *
*        Created Date: 27.01.2026                                                        *
*       Modified Date: 27.01.2026                                                        *
*          Created By: Stanley Omoregie                                                  *
*    Last Modified By: Stanley Omoregie                                                  *
*           CopyRight: copyright © 2025 Omotech Digital Solutions                        *
*                  .oooO  Oooo.                                                          *
*                  (   )  (   )                                                          *
* ------------------\ (----) /---------------------------------------------------------- *
*                    \_)  (_/                                                            *
*****************************************************************************************/

#endregion copyright

namespace DotNetTools.Wpfkit.Helpers;

/// <summary>
/// Provides utility methods for comparing collections.
/// </summary>
public static class CompareCollections
{
    #region AreEqual

    /// <summary>
    /// Compares two collections for equality by checking if they contain the same elements in the same order.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="oldItems">The first collection to compare.</param>
    /// <param name="newItems">The second collection to compare.</param>
    /// <returns>True if the collections are equal; otherwise, false.</returns>
    public static bool AreEqual<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems)
    {
        return oldItems.SequenceEqual(newItems);
    }

    #endregion AreEqual

    #region AreEquivalent

    /// <summary>
    /// Determines whether two collections contain the same elements, regardless of order.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="oldItems">The first collection to compare.</param>
    /// <param name="newItems">The second collection to compare.</param>
    /// <returns>True if both collections contain the same elements; otherwise, false.</returns>
    public static bool AreEquivalent<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems)
    {
        HashSet<T> oldSet = [..oldItems];
        HashSet<T> newSet = [..newItems];
        return oldSet.SetEquals(newSet);
    }

    #endregion AreEquivalent

    #region GetAddedItems

    /// <summary>
    /// Returns the items that are present in <paramref name="newItems"/> but not in <paramref name="oldItems"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="oldItems">The original collection.</param>
    /// <param name="newItems">The updated collection.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing items added in <paramref name="newItems"/> compared to <paramref name="oldItems"/>.</returns>
    public static IEnumerable<T> GetAddedItems<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems)
    {
        HashSet<T> oldSet = [..oldItems];
        return newItems.Where(item => !oldSet.Contains(item));
    }

    #endregion GetAddedItems

    #region GetRemovedItems

    /// <summary>
    /// Returns the items that are present in <paramref name="oldItems"/> but not in <paramref name="newItems"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="oldItems">The original collection.</param>
    /// <param name="newItems">The updated collection.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing items removed from <paramref name="oldItems"/> compared to <paramref name="newItems"/>.</returns>
    public static IEnumerable<T> GetRemovedItems<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems)
    {
        HashSet<T> newSet = [..newItems];
        return oldItems.Where(item => !newSet.Contains(item));
    }

    #endregion GetRemovedItems

    #region IsSubset

    /// <summary>
    /// Checks if <paramref name="oldItems"/> is a subset of <paramref name="newItems"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="oldItems">The collection to check as a subset.</param>
    /// <param name="newItems">The collection to check against.</param>
    /// <returns>True if all elements of <paramref name="oldItems"/> are contained in <paramref name="newItems"/>; otherwise, false.</returns>
    public static bool IsSubset<T>(IEnumerable<T> oldItems, IEnumerable<T> newItems)
    {
        HashSet<T> newSet = [..newItems];
        return oldItems.All(item => newSet.Contains(item));
    }

    #endregion IsSubset

    #region GetChangedItems

    /// <summary>
    /// Returns items that exist in both collections (by key) but have changed according to the comparer.
    /// </summary>
    public static IEnumerable<T> GetChangedItems<T, TKey>(
        IEnumerable<T> oldItems,
        IEnumerable<T> newItems,
        Func<T, TKey> keySelector,
        IEqualityComparer<T> comparer) where TKey : notnull
    {
        Dictionary<TKey, T> oldDict = oldItems.ToDictionary(keySelector);
        Dictionary<TKey, T> newDict = newItems.ToDictionary(keySelector);

        foreach (TKey key in oldDict.Keys.Intersect(newDict.Keys))
        {
            if (!comparer.Equals(oldDict[key], newDict[key]))
                yield return newDict[key];
        }
    }

    #endregion GetChangedItems
}

#region Usage Examples

// bool areEqual = CompareCollections.AreEqual(oldOrderDetails, newOrderDetails);
// bool areEquivalent = CompareCollections.AreEquivalent(oldOrders, newOrders);
// var addedMessages = CompareCollections.GetAddedItems(oldMessages, newMessages);
// var removedMessages = CompareCollections.GetRemovedItems(oldMessages, newMessages);
// bool isSubset = CompareCollections.IsSubset(oldOrders, newOrders);

/*
 *
    using PreCheck.WPF.Utilities;
    using System.Collections.Generic;

    // AreEqual: Check if two playlists are identical (order matters)
    var playlistA = new List<string> { "Song1", "Song2", "Song3" };
    var playlistB = new List<string> { "Song1", "Song2", "Song3" };
    bool arePlaylistsEqual = CompareCollections.AreEqual(playlistA, playlistB);

    // AreEquivalent: Check if two users have the same permissions (order does not matter)
    var user1Permissions = new List<string> { "Read", "Write", "Execute" };
    var user2Permissions = new List<string> { "Execute", "Write", "Read" };
    bool haveSamePermissions = CompareCollections.AreEquivalent(user1Permissions, user2Permissions);

    // GetAddedItems: Find new tasks assigned this week
    var lastWeekTasks = new List<string> { "Design", "Code" };
    var thisWeekTasks = new List<string> { "Design", "Code", "Test" };
    var newTasks = CompareCollections.GetAddedItems(lastWeekTasks, thisWeekTasks); // ["Test"]

    // GetRemovedItems: Find discontinued products
    var lastMonthProducts = new List<string> { "A", "B", "C" };
    var thisMonthProducts = new List<string> { "A", "C" };
    var discontinued = CompareCollections.GetRemovedItems(lastMonthProducts, thisMonthProducts); // ["B"]

    // IsSubset: Validate required documents are received
    var requiredDocs = new List<string> { "ID", "Form" };
    var receivedDocs = new List<string> { "ID", "Form", "Photo" };
    bool allRequiredReceived = CompareCollections.IsSubset(requiredDocs, receivedDocs);

    Usage Example:
    Suppose you have a Product class and want to find products whose price changed:

    using System.Collections.Generic;

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // Custom comparer for Product (compares Price)
    public class ProductPriceComparer : IEqualityComparer<Product>
    {
        public bool Equals(Product x, Product y) => x.Price == y.Price;
        public int GetHashCode(Product obj) => obj.Price.GetHashCode();
    }

    // Example usage
    var oldProducts = new List<Product>
    {
        new Product { Id = 1, Name = "A", Price = 10 },
        new Product { Id = 2, Name = "B", Price = 20 }
    };
    var newProducts = new List<Product>
    {
        new Product { Id = 1, Name = "A", Price = 12 }, // Price changed
        new Product { Id = 2, Name = "B", Price = 20 }
    };

    var changed = CompareCollections.GetChangedItems(
        oldProducts,
        newProducts,
        p => p.Id,
        new ProductPriceComparer()
    );
    // changed will contain Product with Id = 1

    This finds products with the same Id but different Price.
 *
 */

#endregion Usage Examples