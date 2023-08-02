import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";
import QuickButtons from "../components/QuickButtons";

export default function withQuickButtons(WrappedComponent) {
  class WithQuickButtons extends React.Component {
    constructor(props) {
      super(props);

      this.state = {
        isLoading: false,
      };
    }

    onClickLock = () => {
      const { item, lockFileAction, t } = this.props;
      const { locked, id, security } = item;

      if (security?.Lock && !this.state.isLoading) {
        this.setState({ isLoading: true });
        return lockFileAction(id, !locked)
          .then(() =>
            locked
              ? toastr.success(t("Translations:FileUnlocked"))
              : toastr.success(t("Translations:FileLocked"))
          )
          .catch(
            (err) => toastr.error(err),
            this.setState({ isLoading: false })
          );
      }
      return;
    };

    onClickDownload = () => {
      window.open(this.props.item.viewUrl, "_self");
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
      const { isLoading } = this.state;

      const {
        t,
        theme,
        item,
        isAdmin,
        sectionWidth,
        viewAs,
        folderCategory,
        isPublicRoom,
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
          isPublicRoom={isPublicRoom}
          onClickLock={this.onClickLock}
          onClickDownload={this.onClickDownload}
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
      publicRoomStore,
      treeFoldersStore,
    }) => {
      const { lockFileAction, setFavoriteAction, onSelectItem } =
        filesActionsStore;
      const { isPersonalFolderRoot, isArchiveFolderRoot, isTrashFolder } =
        treeFoldersStore;
      const { setSharingPanelVisible } = dialogsStore;

      const folderCategory =
        isTrashFolder || isArchiveFolderRoot || isPersonalFolderRoot;

      const { isPublicRoom } = publicRoomStore;

      return {
        theme: auth.settingsStore.theme,
        isAdmin: auth.isAdmin,
        lockFileAction,
        setFavoriteAction,
        onSelectItem,
        setSharingPanelVisible,
        folderCategory,
        isPublicRoom,
      };
    }
  )(observer(WithQuickButtons));
}
