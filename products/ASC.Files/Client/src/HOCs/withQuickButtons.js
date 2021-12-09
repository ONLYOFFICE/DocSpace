import React from "react";
import { inject, observer } from "mobx-react";

import { ShareAccessRights } from "@appserver/common/constants";
import toastr from "@appserver/components/toast/toastr";
import QuickButtons from "../components/QuickButtons";

export default function withQuickButtons(WrappedComponent) {
  class WithQuickButtons extends React.Component {
    onClickLock = () => {
      const { item, lockFileAction, isAdmin, setIsLoading } = this.props;
      const { locked, id, access } = item;

      if (isAdmin || access === 0) {
        setIsLoading(true);
        return lockFileAction(id, !locked)
          .catch((err) => toastr.error(err))
          .finally(() => setIsLoading(false));
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

    render() {
      const { t, item, isTrashFolder, isAdmin, showShare } = this.props;
      const { access } = item;

      const accessToEdit =
        access === ShareAccessRights.FullAccess ||
        access === ShareAccessRights.None; // TODO: fix access type for owner (now - None)

      const quickButtonsComponent = (
        <QuickButtons
          t={t}
          item={item}
          isAdmin={isAdmin}
          showShare={showShare}
          isTrashFolder={isTrashFolder}
          accessToEdit={accessToEdit}
          onClickLock={this.onClickLock}
          onClickFavorite={this.onClickFavorite}
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

  return inject(({ auth, treeFoldersStore, filesActionsStore, filesStore }) => {
    const { isRecycleBinFolder } = treeFoldersStore;
    const { lockFileAction, setFavoriteAction } = filesActionsStore;
    const { setIsLoading } = filesStore;
    return {
      isAdmin: auth.isAdmin,
      isTrashFolder: isRecycleBinFolder,
      lockFileAction,
      setFavoriteAction,
      setIsLoading,
    };
  })(observer(WithQuickButtons));
}
