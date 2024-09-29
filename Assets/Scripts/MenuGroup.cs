using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MenuGroup : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private List<Menu> MenusInGroup;

    [SerializeField] [FoldoutGroup("Settings")]
	private MenuState InitialState =	MenuState.Closed;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<Menu> CachedOpenMenus;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<Menu> CachedClosedMenus;

    private void Start() {
	    if(InitialState == MenuState.Closed)
		    foreach (var menu in MenusInGroup) {
			    menu.Close();
		    }
	    else foreach (var menu in MenusInGroup) {
		    menu.Open();
	    }
        
    }

    public void CacheMenuStates() {
        CachedOpenMenus.Clear();
        CachedClosedMenus.Clear();
        foreach (var menu in MenusInGroup) {
            if(menu.State == MenuState.Open)
                CachedOpenMenus.Add(menu);
            else CachedClosedMenus.Add(menu);
        }
    }

    public void LoadCachedMenus() {
        foreach (var menu in CachedOpenMenus) {
            menu.Open();
        }

        foreach (var menu in CachedClosedMenus) {
            menu.Close();
        }
    }

    public void OpenExclusive(Menu targetMenu) {
        Open(targetMenu, true);
    }

    public void ToggleOpenExclusive(Menu targetMenu) {
        foreach (var menu in MenusInGroup) {
            if (menu == targetMenu && menu.State == MenuState.Closed)
                menu.Open();
            else menu.Close();
        }
    }

    [Button]
    public void OpenAll() {
        foreach (var menu in MenusInGroup) {
            menu.Open();
        }
    }

    public void Open(Menu targetMenu, bool isExclusive = false) {
        foreach (var menu in MenusInGroup) {
            if (menu == targetMenu && isExclusive)
                menu.Open();
            else menu.Close();
        }
    }

    [Button]
    public void CloseAll() {
        foreach (var menu in MenusInGroup) {
            menu.Close();
        }
    }

    public void Close(Menu targetMenu) {
        foreach (var menu in MenusInGroup) {
            if (menu == targetMenu)
                menu.Close();
        }
    }
}