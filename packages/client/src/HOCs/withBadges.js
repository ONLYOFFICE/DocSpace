import React from "react";
import { inject, observer } from "mobx-react";
import { ShareAccessRights, FileStatus } from "@docspace/common/constants";
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

        isTrashFolder,
      } = this.props;
      if (isTrashFolder) return;
      fetchFileVersions(item.id + "", item.security);
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
        isTrashFolder,
        isPrivacyFolder,
        onFilesClick,
        isAdmin,
        isVisitor,
        isDesktopClient,
        sectionWidth,
        viewAs,
        isMutedBadge,
        isArchiveFolderRoot,
      } = this.props;
      const { fileStatus, access, mute } = item;

      const newItems =
        item.new ||
        (!mute && (fileStatus & FileStatus.IsNew) === FileStatus.IsNew);
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
          isVisitor={isVisitor}
          showNew={showNew}
          newItems={newItems}
          sectionWidth={sectionWidth}
          isTrashFolder={isTrashFolder}
          isPrivacyFolder={isPrivacyFolder}
          isArchiveFolderRoot={isArchiveFolderRoot}
          isDesktopClient={isDesktopClient}
          accessToEdit={accessToEdit}
          onShowVersionHistory={this.onShowVersionHistory}
          onBadgeClick={this.onBadgeClick}
          onUnpinClick={this.onUnpinClick}
          setConvertDialogVisible={this.setConvertDialogVisible}
          onFilesClick={onFilesClick}
          viewAs={viewAs}
          isMutedBadge={isMutedBadge}
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
      },
      { item }
    ) => {
      const { isRecycleBinFolder, isPrivacyFolder, isArchiveFolderRoot } =
        treeFoldersStore;
      const { markAsRead, setPinAction } = filesActionsStore;
      const { isTabletView, isDesktopClient, theme } = auth.settingsStore;
      const { setIsVerHistoryPanel, fetchFileVersions } = versionHistoryStore;
      const {
        setNewFilesPanelVisible,
        setConvertDialogVisible,
        setConvertItem,
      } = dialogsStore;
      const { setIsLoading, isMuteCurrentRoomNotifications } = filesStore;
      const { roomType, mute } = item;

      const isRoom = !!roomType;
      const isMutedBadge = isRoom ? mute : isMuteCurrentRoomNotifications;

      return {
        isArchiveFolderRoot,
        theme,
        isAdmin: auth.isAdmin,
        isVisitor: auth?.userStore?.user?.isVisitor,
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
        isMutedBadge,
      };
    }
  )(observer(WithBadges));
}
