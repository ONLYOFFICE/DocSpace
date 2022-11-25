import React from "react";
import { inject, observer } from "mobx-react";
import {
  ShareAccessRights,
  AppServerConfig,
  FileStatus,
} from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";

import Badges from "../components/Badges";
import config from "PACKAGE_FILE";

export default function withBadges(WrappedComponent) {
  class WithBadges extends React.Component {
    constructor(props) {
      super(props);

      this.state = { disableBadgeClick: false, disableUnpinClick: false };
    }

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
      fetchFileVersions(item.id + "", item.access);
      setIsVerHistoryPanel(true);
    };

    onBadgeClick = () => {
      if (this.state.disableBadgeClick) return;

      const { item, markAsRead, setNewFilesPanelVisible } = this.props;
      this.setState(() => ({
        disableBadgeClick: true,
      }));

      const enableBadgeClick = () => {
        this.setState({ disableBadgeClick: false });
      };

      if (item.fileExst) {
        markAsRead([], [item.id], item).then(() => {
          enableBadgeClick();
        });
      } else {
        setNewFilesPanelVisible(true, null, item).then(() => {
          enableBadgeClick();
        });
      }
    };

    onUnpinClick = (e) => {
      if (this.state.disableUnpinClick) return;

      this.setState({ disableUnpinClick: true });

      const { t, setPinAction } = this.props;

      const { action, id } = e.target.closest(".is-pinned").dataset;

      if (!action && !id) return;

      setPinAction(action, id, t).then(() => {
        this.setState({ disableUnpinClick: false });
      });
    };

    setConvertDialogVisible = () => {
      this.props.setConvertItem(this.props.item);
      this.props.setConvertDialogVisible(true);
    };

    render() {
      const {
        t,
        theme,
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
        canViewVersionFileHistory,
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
          theme={theme}
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
          onUnpinClick={this.onUnpinClick}
          setConvertDialogVisible={this.setConvertDialogVisible}
          onFilesClick={onFilesClick}
          viewAs={viewAs}
          canViewVersionFileHistory={canViewVersionFileHistory}
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
        accessRightsStore,
      },
      { item }
    ) => {
      const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;
      const { markAsRead, setPinAction } = filesActionsStore;
      const { isTabletView, isDesktopClient, theme } = auth.settingsStore;
      const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
      const {
        setNewFilesPanelVisible,
        setConvertDialogVisible,
        setConvertItem,
      } = dialogsStore;
      const { setIsLoading } = filesStore;

      const canWebEdit = settingsStore.canWebEdit(item.fileExst);
      const canConvert = settingsStore.canConvert(item.fileExst);
      const canViewVersionFileHistory = accessRightsStore.canViewVersionFileHistory(
        item
      );

      return {
        theme,
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
        setPinAction,
        canViewVersionFileHistory,
      };
    }
  )(observer(WithBadges));
}
