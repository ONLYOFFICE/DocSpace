import React from "react";
import { inject, observer } from "mobx-react";

import { ShareAccessRights } from "@docspace/common/constants";
import toastr from "client/toastr";
import QuickButtons from "../components/QuickButtons";

export default function withQuickButtons(WrappedComponent) {
  class WithQuickButtons extends React.Component {
    constructor(props) {
      super(props);

      this.state = {
        isLoading: false,
        isCanWebEdit: props.canWebEdit(props.item.fileExst),
      };
    }

    onClickLock = () => {
      const { item, lockFileAction, isAdmin, t } = this.props;
      const { locked, id, access } = item;

      if ((isAdmin || access === 0) && !this.state.isLoading) {
        this.setState({ isLoading: true });
        return lockFileAction(id, !locked)
          .then(() =>
            locked
              ? toastr.success(t("Translations:FileUnlocked"))
              : toastr.success(t("Translations:FileLocked"))
          )
          .catch((err) => toastr.error(err))
          .finally(() => this.setState({ isLoading: false }));
      }
      return;
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

    onClickShare = () => {
      const { item, onSelectItem, setSharingPanelVisible } = this.props;
      const { id, isFolder } = item;

      onSelectItem({ id, isFolder });
      setSharingPanelVisible(true);
    };

    render() {
      const { isLoading, isCanWebEdit } = this.state;

      const {
        t,
        theme,
        item,
        isTrashFolder,
        isAdmin,
        showShare,
        sectionWidth,
        viewAs,
      } = this.props;

      const { access, id, fileExst } = item;

      const accessToEdit =
        access === ShareAccessRights.FullAccess ||
        access === ShareAccessRights.None; // TODO: fix access type for owner (now - None)

      const quickButtonsComponent = (
        <QuickButtons
          t={t}
          theme={theme}
          item={item}
          sectionWidth={sectionWidth}
          isAdmin={isAdmin}
          showShare={showShare}
          isTrashFolder={isTrashFolder}
          accessToEdit={accessToEdit}
          viewAs={viewAs}
          isDisabled={isLoading}
          isCanWebEdit={isCanWebEdit}
          onClickLock={this.onClickLock}
          onClickFavorite={this.onClickFavorite}
          onClickShare={this.onClickShare}
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
      treeFoldersStore,
      filesActionsStore,
      filesStore,
      dialogsStore,
      settingsStore,
    }) => {
      const { isRecycleBinFolder } = treeFoldersStore;
      const {
        lockFileAction,
        setFavoriteAction,
        onSelectItem,
      } = filesActionsStore;

      const { setSharingPanelVisible } = dialogsStore;
      const { canWebEdit } = settingsStore;
      return {
        theme: auth.settingsStore.theme,
        isAdmin: auth.isAdmin,
        isTrashFolder: isRecycleBinFolder,
        lockFileAction,
        setFavoriteAction,
        onSelectItem,
        setSharingPanelVisible,
        canWebEdit,
      };
    }
  )(observer(WithQuickButtons));
}
