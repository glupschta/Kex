using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kex.Interfaces
{

    public interface IListerViewHandler
    {
        ILister CurrentLister { get; }
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

        void OpenLister();
        void OpenLister(string directory);
        void CloseNotFocusedListers();
        void CloseCurrentLister();
        void CycleListers(int direction);
        void FitWidthToListers();

        void ShowFavorites();
        void ShowContextMenu();
        void ShowListers();
        void ShowLocationInput();
        void HandleException(Exception ex);

        void SelectAll();
        void ClearSelection();
        void MarkSelected();

        void Copy();
        void Cut();
        void Paste();
        void Delete();

        void SetDirectory(string container);
        void ShowSortPopup();
        void ShowViewPopup();
        void ShowFilterPopup();
        void ShowBrowsingPopup();
        void ShowDrivesPopup();
        void ShowSpecialFolderPopup();
        void ShowEnterUrlPopup();

        void FocusView();
        void SetView(string view);
        void SetSorting(string selectedColumn, bool descending);
        void SetFilter(string filter);
        void ClosePopup();
    }
}
