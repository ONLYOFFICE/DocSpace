import React from "react";
import { inject, observer } from "mobx-react";
import {
  ShareAccessRights,
  AppServerConfig,
  FileStatus,
} from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";

import Badges from "../components/Badges";
import config from "../../package.json";

export default function withBadges(WrappedComponent) {
  class WithBadges extends React.Component {
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
      const { item, markAsRead, setNewFilesPanelVisible } = this.props;
      if (item.fileExst) {
        markAsRead([], [item.id], item);
      } else {
        setNewFilesPanelVisible(true, null, item);
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
        onFilesClick,
        isAdmin,
        isDesktopClient,
        sectionWidth,
        viewAs,
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
          sectionWidth={sectionWidth}
          canWebEdit={canWebEdit}
          canConvert={canConvert}
          isTrashFolder={isTrashFolder}
          isPrivacyFolder={isPrivacyFolder}
          isDesktopClient={isDesktopClient}
          accessToEdit={accessToEdit}
          onShowVersionHistory={this.onShowVersionHistory}
          onBadgeClick={this.onBadgeClick}
          setConvertDialogVisible={this.setConvertDialogVisible}
          onFilesClick={onFilesClick}
          viewAs={viewAs}
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
        treeFoldersStore,
        filesActionsStore,
        versionHistoryStore,
        dialogsStore,
        filesStore,
        settingsStore,
      },
      { item }
    ) => {
      const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;
      const { markAsRead } = filesActionsStore;
      const { isTabletView, isDesktopClient } = auth.settingsStore;
      const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
      const {
        setNewFilesPanelVisible,
        setConvertDialogVisible,
        setConvertItem,
      } = dialogsStore;
      const { setIsLoading } = filesStore;

      const canWebEdit = settingsStore.canWebEdit(item.fileExst);
      const canConvert = settingsStore.canConvert(item.fileExst);

      return {
        isAdmin: auth.isAdmin,
        canWebEdit,
        canConvert,
        isTrashFolder: isRecycleBinFolder,
        isPrivacyFolder,
        homepage: config.homepage,
        isTabletView,
        setIsVerHistoryPanel,
        fetchFileVersions,
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
