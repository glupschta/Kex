using System;
using Kex.Model;
using Kex.Views;

namespace Kex.Common
{
    public interface ICommandManager
    {
        ListerView CurrentView { get; set; }
        IItem CurrentItem { get; set; }
        void DoDefaultAction();

        void Select(IItem item);
        void GoUp();
        void GoDown();
        void GoLeft();
        void GoRight();
        void HistoryBack();
        void HistoryForward();
        void DirectoryUp();

        void FitWidthToListers();

        void ShowFavorites();
        void ShowContextMenu();
        void ShowListers();
        void ShowLocationInput();
        void HandleException(Exception ex);

        void SelectAll();
        void ClearSelection();

        void Copy();
        void Cut();
        void Paste();
        void Delete();

        void SetContainer(string container);
        void ShowSortPopup();
        void ShowViewPopup();
        void ShowFilterPopup(bool keepText = false);
        void ShowBrowsingPopup(bool keepText = false);
        void ShowDrivesPopup();
        void ShowSpecialFolderPopup();
        void ShowEnterUrlPopup();
        void ShowNetWorkComputers();

        void FocusView();
        void SetView(string view);
        void SetSorting(string selectedColumn);
        void ClearSorting();
        void GroupByName();
        void ClearGrouping();
        void SetFilter(string filter);
        void ClosePopup();
        void UpdateColumnWidth();
        void ShowShellPropertyPopup();
    }
}
