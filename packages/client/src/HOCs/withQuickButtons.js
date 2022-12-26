import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";
import QuickButtons from "../components/QuickButtons";

export default function withQuickButtons(WrappedComponent) {
  class WithQuickButtons extends React.Component {
    constructor(props) {
      super(props);

      this.state = {
        isCanWebEdit: props.item.viewAccessability?.WebEdit,
      };
    }

    onClickLock = async () => {
      let timer = null;
      const { item, setIsLoading, isLoading, lockFileAction, t } = this.props;
      const { locked, id, security } = item;

      try {
        timer = setTimeout(() => {
          setIsLoading(true);
        }, 200);
        if (security?.Lock && !isLoading) {
          await lockFileAction(id, !locked).then(() =>
            locked
              ? toastr.success(t("Translations:FileUnlocked"))
              : toastr.success(t("Translations:FileLocked"))
          );
        }
      } catch (error) {
        toastr.error(err);
      } finally {
        setIsLoading(false), clearTimeout(timer);
      }
    };

    onClickFavorite = (showFavorite) => {
      const { t, item, setFavoriteAction } = this.props;

      if (showFavorite) {
        setFavoriteAction("remove", item.id)
          .then(() => toastr.success(t("RemovedFromFavorites")))
          .catch((err) => toastr.error(err));
        return;
      }

      setFavoriteAction("mark", item.id)
        .then(() => toastr.success(t("MarkedAsFavorite")))
        .catch((err) => toastr.error(err));
    };

    render() {
      const { isCanWebEdit } = this.state;

      const {
        t,
        theme,
        item,
        isAdmin,
        sectionWidth,
        viewAs,
        folderCategory,
        isLoading,
      } = this.props;

      const quickButtonsComponent = (
        <QuickButtons
          t={t}
          theme={theme}
          item={item}
          sectionWidth={sectionWidth}
          isAdmin={isAdmin}
          viewAs={viewAs}
          isDisabled={isLoading}
          isCanWebEdit={isCanWebEdit}
          onClickLock={this.onClickLock}
          onClickFavorite={this.onClickFavorite}
          folderCategory={folderCategory}
        />
      );

      return (
        <WrappedComponent
          quickButtonsComponent={quickButtonsComponent}
          {...this.props}
        />
      );
    }
  }

  return inject(
    ({
      auth,
      filesActionsStore,
      dialogsStore,

      treeFoldersStore,
    }) => {
      const {
        lockFileAction,
        setFavoriteAction,
        onSelectItem,
        setIsLoading,
        isLoading,
      } = filesActionsStore;
      const {
        isPersonalFolderRoot,
        isArchiveFolderRoot,
        isTrashFolder,
      } = treeFoldersStore;
      const { setSharingPanelVisible } = dialogsStore;

      const folderCategory =
        isTrashFolder || isArchiveFolderRoot || isPersonalFolderRoot;

      return {
        theme: auth.settingsStore.theme,
        isAdmin: auth.isAdmin,
        lockFileAction,
        setFavoriteAction,
        onSelectItem,
        setSharingPanelVisible,
        folderCategory,
        setIsLoading,
        isLoading,
      };
    }
  )(observer(WithQuickButtons));
}
