import React from "react";
import { inject, observer } from "mobx-react";
import {
  ShareAccessRights,
  AppServerConfig,
  FileStatus,
} from "@appserver/common/constants";
import toastr from "@appserver/components/toast/toastr";
import { combineUrl } from "@appserver/common/utils";

import Badges from "../components/Badges";
import config from "../../package.json";

export default function withBadges(WrappedComponent) {
  class WithBadges extends React.Component {
    state = { isLoading: false };

    onClickLock = () => {
      const { item, lockFileAction, isAdmin } = this.props;
      const { locked, id, access } = item;

      if ((isAdmin || access === 0) && !this.state.isLoading) {
        this.setState({ isLoading: true });
        return lockFileAction(id, !locked)
          .catch((err) => toastr.error(err))
          .finally(() => this.setState({ isLoading: false }));
      }
      return;
    };

    onClickFavorite = () => {
      const { t, item, setFavoriteAction } = this.props;

      setFavoriteAction("remove", item.id)
        .then(() => toastr.success(t("RemovedFromFavorites")))
        .catch((err) => toastr.error(err));
    };

    onShowVersionHistory = () => {
      const {
        homepage,
        isTabletView,
        item,
        setIsVerHistoryPanel,
        fetchFileVersions,
        history,
        isTrashFolder,
      } = this.props;
      if (isTrashFolder) return;

      if (!isTabletView) {
        fetchFileVersions(item.id + "");
        setIsVerHistoryPanel(true);
      } else {
        history.push(
          combineUrl(AppServerConfig.proxyURL, homepage, `/${item.id}/history`)
        );
      }
    };
    onBadgeClick = () => {
      const {
        item,
        selectedFolderPathParts,
        markAsRead,
        setNewFilesPanelVisible,
      } = this.props;
      if (item.fileExst) {
        markAsRead([], [item.id], item);
      } else {
        const newFolderIds = selectedFolderPathParts;
        newFolderIds.push(item.id);
        setNewFilesPanelVisible(true, newFolderIds, item);
      }
    };

    setConvertDialogVisible = () => {
      this.props.setConvertItem(this.props.item);
      this.props.setConvertDialogVisible(true);
    };

    render() {
      const {
        t,
        item,
        canWebEdit,
        isTrashFolder,
        isPrivacyFolder,
        canConvert,
        onFilesClick, // from withFileAction HOC
        isAdmin,
        isDesktopClient,
      } = this.props;
      const { fileStatus, access } = item;

      const newItems =
        item.new || (fileStatus & FileStatus.IsNew) === FileStatus.IsNew;
      const showNew = !!newItems;

      const accessToEdit =
        access === ShareAccessRights.FullAccess ||
        access === ShareAccessRights.None; // TODO: fix access type for owner (now - None)

      const badgesComponent = (
        <Badges
          t={t}
          item={item}
          isAdmin={isAdmin}
          showNew={showNew}
          newItems={newItems}
          canWebEdit={canWebEdit}
          canConvert={canConvert}
          isTrashFolder={isTrashFolder}
          isPrivacyFolder={isPrivacyFolder}
          isDesktopClient={isDesktopClient}
          accessToEdit={accessToEdit}
          onClickLock={this.onClickLock}
          onClickFavorite={this.onClickFavorite}
          onShowVersionHistory={this.onShowVersionHistory}
          onBadgeClick={this.onBadgeClick}
          setConvertDialogVisible={this.setConvertDialogVisible}
          onFilesClick={onFilesClick}
        />
      );

      return (
        <WrappedComponent badgesComponent={badgesComponent} {...this.props} />
      );
    }
  }

  return inject(
    (
      {
        auth,
        formatsStore,
        treeFoldersStore,
        filesActionsStore,
        versionHistoryStore,
        selectedFolderStore,
        dialogsStore,
        filesStore,
      },
      { item }
    ) => {
      const { docserviceStore } = formatsStore;
      const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;
      const {
        lockFileAction,
        setFavoriteAction,
        markAsRead,
      } = filesActionsStore;
      const { isTabletView, isDesktopClient } = auth.settingsStore;
      const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
      const {
        setNewFilesPanelVisible,
        setConvertDialogVisible,
        setConvertItem,
      } = dialogsStore;
      const { setIsLoading } = filesStore;

      const canWebEdit = docserviceStore.canWebEdit(item.fileExst);
      const canConvert = docserviceStore.canConvert(item.fileExst);

      return {
        isAdmin: auth.isAdmin,
        canWebEdit,
        canConvert,
        isTrashFolder: isRecycleBinFolder,
        isPrivacyFolder,
        lockFileAction,
        setFavoriteAction,
        homepage: config.homepage,
        isTabletView,
        setIsVerHistoryPanel,
        fetchFileVersions,
        selectedFolderPathParts: selectedFolderStore.pathParts,
        markAsRead,
        setNewFilesPanelVisible,
        setIsLoading,
        setConvertDialogVisible,
        setConvertItem,
        isDesktopClient,
      };
    }
  )(observer(WithBadges));
}
