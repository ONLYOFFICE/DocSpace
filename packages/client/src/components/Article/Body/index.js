import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isDesktop, isTablet, isMobileOnly } from "react-device-detect";
import { useNavigate, useLocation } from "react-router-dom";

import { RoomSearchArea } from "@docspace/common/constants";
import Items from "./Items";
import { isMobile, tablet } from "@docspace/components/utils/device";

import FilesFilter from "@docspace/common/api/files/filter";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import AccountsFilter from "@docspace/common/api/people/filter";

import DownloadAppList from "./DownloadAppList";
import Banner from "./Banner";

import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";

const StyledBlock = styled.div`
  padding: 0 20px;

  @media ${tablet} {
    padding: ${(props) => (props.showText ? "0 16px" : 0)};
  }
`;

const ArticleBodyContent = (props) => {
  const {
    // isDesktopClient,
    // firstLoad,
    // FirebaseHelper,
    // theme,

    showText,
    toggleArticleOpen,

    roomsFolderId,
    archiveFolderId,
    myFolderId,
    recycleBinFolderId,

    isVisitor,
    setIsLoading,

    clearFiles,
    selectedFolderId,
  } = props;

  const navigate = useNavigate();
  const location = useLocation();

  const [disableBadgeClick, setDisableBadgeClick] = React.useState(false);
  const [activeItem, setActiveItem] = React.useState(null);

  // const campaigns = (localStorage.getItem("campaigns") || "")
  //   .split(",")
  //   .filter((campaign) => campaign.length > 0);

  const onClick = React.useCallback(
    (folderId, title, isEmpty, rootFolderType) => {
      const { toggleArticleOpen } = props;

      let path = `/rooms`;
      let params = null;

      const state = {
        title,
        isRoot: true,
        isEmpty,
        rootFolderType,
      };

      switch (folderId) {
        case myFolderId:
          const myFilter = FilesFilter.getDefault();
          myFilter.folder = folderId;
          params = myFilter.toUrlParams();
          path += "/personal";

          if (activeItem === myFolderId && folderId === selectedFolderId)
            return;

          break;
        case archiveFolderId:
          const archiveFilter = RoomsFilter.getDefault();
          archiveFilter.searchArea = RoomSearchArea.Archive;
          params = archiveFilter.toUrlParams();
          path += "/archived";
          if (activeItem === archiveFolderId && folderId === selectedFolderId)
            return;
          break;
        case recycleBinFolderId:
          const recycleBinFilter = FilesFilter.getDefault();
          recycleBinFilter.folder = folderId;
          params = recycleBinFilter.toUrlParams();
          path = "/files/trash";
          if (
            activeItem === recycleBinFolderId &&
            folderId === selectedFolderId
          )
            return;
          break;
        case "accounts":
          const accountsFilter = AccountsFilter.getDefault();
          params = accountsFilter.toUrlParams();
          path = "/accounts";

          break;
        case "settings":
          clearFiles(isEmpty);
          navigate("/settings/common");

          if (isMobileOnly || isMobile()) {
            toggleArticleOpen();
          }
          return;
        case roomsFolderId:
        default:
          const roomsFilter = RoomsFilter.getDefault();
          roomsFilter.searchArea = RoomSearchArea.Active;
          params = roomsFilter.toUrlParams();
          path += "/shared";
          if (activeItem === roomsFolderId && folderId === selectedFolderId)
            return;
          break;
      }

      clearFiles(isEmpty);
      setIsLoading(true);
      path += `/filter?${params}`;

      navigate(path, { state });

      if (isMobileOnly || isMobile()) {
        toggleArticleOpen();
      }
    },
    [
      roomsFolderId,
      archiveFolderId,
      myFolderId,
      recycleBinFolderId,
      activeItem,
      selectedFolderId,
    ]
  );

  const onShowNewFilesPanel = React.useCallback(
    async (folderId) => {
      if (disableBadgeClick) return;

      setDisableBadgeClick(true);

      await props.setNewFilesPanelVisible(true, [`${folderId}`]);

      setDisableBadgeClick(false);
    },
    [disableBadgeClick]
  );

  React.useEffect(() => {
    if (
      location.pathname.includes("/rooms/shared") &&
      activeItem !== roomsFolderId
    )
      return setActiveItem(roomsFolderId);

    if (
      location.pathname.includes("/rooms/archived") &&
      activeItem !== archiveFolderId
    )
      return setActiveItem(archiveFolderId);

    if (
      location.pathname.includes("/rooms/personal") &&
      activeItem !== myFolderId
    )
      return setActiveItem(myFolderId);

    if (
      location.pathname.includes("/files/trash") &&
      activeItem !== recycleBinFolderId
    )
      return setActiveItem(recycleBinFolderId);

    if (
      location.pathname.includes("/accounts/filter") &&
      activeItem !== "accounts"
    )
      return setActiveItem("accounts");

    if (location.pathname.includes("/settings") && activeItem !== "settings")
      return setActiveItem("settings");

    if (location.pathname.includes("/accounts/view/@self")) {
      if (isVisitor) {
        if (activeItem) return;
        return setActiveItem(myFolderId);
      }

      if (activeItem !== "accounts") return setActiveItem("accounts");
    }
  }, [
    location.pathname,
    activeItem,
    roomsFolderId,
    archiveFolderId,
    myFolderId,
    recycleBinFolderId,
    isVisitor,
  ]);

  return (
    <>
      <Items
        onClick={onClick}
        onBadgeClick={onShowNewFilesPanel}
        showText={showText}
        onHide={toggleArticleOpen}
        activeItem={activeItem}
      />

      {/* {!isDesktopClient && showText && (
        <StyledBlock showText={showText}>
          <DownloadAppList theme={theme} />
          {(isDesktop || isTablet) && !firstLoad && campaigns.length > 0 && (
            <Banner FirebaseHelper={FirebaseHelper} />
          )}
        </StyledBlock>
      )} */}
    </>
  );
};

export default inject(
  ({
    auth,
    filesStore,
    treeFoldersStore,
    dialogsStore,
    selectedFolderStore,
  }) => {
    const { firstLoad, setIsLoading, clearFiles } = filesStore;

    const { roomsFolderId, archiveFolderId, myFolderId, recycleBinFolderId } =
      treeFoldersStore;

    const { setNewFilesPanelVisible } = dialogsStore;

    const selectedFolderId = selectedFolderStore.id;

    const {
      showText,

      toggleArticleOpen,

      isDesktopClient,
      FirebaseHelper,
      theme,
    } = auth.settingsStore;

    return {
      toggleArticleOpen,
      showText,

      isVisitor: auth.userStore.user.isVisitor,

      setNewFilesPanelVisible,

      firstLoad,
      isDesktopClient,
      FirebaseHelper,
      theme,

      roomsFolderId,
      archiveFolderId,
      myFolderId,
      recycleBinFolderId,

      setIsLoading,

      clearFiles,
      selectedFolderId,
    };
  }
)(
  withTranslation([])(
    withLoader(observer(ArticleBodyContent))(<Loaders.ArticleFolder />)
  )
);
