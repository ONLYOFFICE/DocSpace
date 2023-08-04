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

import Banner from "./Banner";

import Loaders from "@docspace/common/components/Loaders";

import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

const StyledBlock = styled.div`
  padding: 0 20px;

  @media ${tablet} {
    padding: ${(props) => (props.showText ? "0 16px" : 0)};
  }
`;

const ArticleBodyContent = (props) => {
  const {
    isDesktopClient,
    firstLoad,
    FirebaseHelper,
    theme,

    showText,
    toggleArticleOpen,

    roomsFolderId,
    archiveFolderId,
    myFolderId,
    recycleBinFolderId,
    rootFolderId,

    isVisitor,
    setIsLoading,

    clearFiles,
    selectedFolderId,
    showArticleLoader,
    setIsBurgerLoading,
  } = props;

  const navigate = useNavigate();
  const location = useLocation();

  const [disableBadgeClick, setDisableBadgeClick] = React.useState(false);
  const [activeItemId, setActiveItemId] = React.useState(null);

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  const isAccounts = location.pathname.includes("accounts/filter");

  const onClick = React.useCallback(
    (folderId, title, rootFolderType) => {
      const { toggleArticleOpen } = props;

      let params = null;
      let path = ``;

      const state = {
        title,
        isRoot: true,
        rootFolderType,
      };

      let withTimer = !!selectedFolderId;

      switch (folderId) {
        case myFolderId:
          const myFilter = FilesFilter.getDefault();
          myFilter.folder = folderId;
          params = myFilter.toUrlParams();

          path = getCategoryUrl(CategoryType.Personal);

          if (activeItemId === myFolderId && folderId === selectedFolderId)
            return;

          break;
        case archiveFolderId:
          const archiveFilter = RoomsFilter.getDefault();
          archiveFilter.searchArea = RoomSearchArea.Archive;
          params = archiveFilter.toUrlParams();
          path = getCategoryUrl(CategoryType.Archive);
          if (activeItemId === archiveFolderId && folderId === selectedFolderId)
            return;
          break;
        case recycleBinFolderId:
          const recycleBinFilter = FilesFilter.getDefault();
          recycleBinFilter.folder = folderId;
          params = recycleBinFilter.toUrlParams();
          path = getCategoryUrl(CategoryType.Trash);
          if (
            activeItemId === recycleBinFolderId &&
            folderId === selectedFolderId
          )
            return;
          break;
        case "accounts":
          clearFiles();
          const accountsFilter = AccountsFilter.getDefault();
          params = accountsFilter.toUrlParams();
          path = getCategoryUrl(CategoryType.Accounts);

          if (activeItemId === "accounts" && isAccounts) return;

          break;
        case "settings":
          clearFiles();

          path = getCategoryUrl(CategoryType.Settings);
          navigate(path);

          if (isMobileOnly || isMobile()) {
            toggleArticleOpen();
          }
          return;
        case roomsFolderId:
        default:
          const roomsFilter = RoomsFilter.getDefault();
          roomsFilter.searchArea = RoomSearchArea.Active;
          params = roomsFilter.toUrlParams();
          path = getCategoryUrl(CategoryType.Shared);
          if (activeItemId === roomsFolderId && folderId === selectedFolderId)
            return;
          break;
      }

      setIsLoading(true, withTimer);
      path += `?${params}`;

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
      activeItemId,
      selectedFolderId,
      isAccounts,
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
      activeItemId !== roomsFolderId
    )
      return setActiveItemId(roomsFolderId);

    if (
      location.pathname.includes("/rooms/archived") &&
      activeItemId !== archiveFolderId
    )
      return setActiveItemId(archiveFolderId);

    if (
      location.pathname.includes("/rooms/personal") &&
      activeItemId !== myFolderId
    )
      return setActiveItemId(myFolderId);

    if (
      location.pathname.includes("/files/trash") &&
      activeItemId !== recycleBinFolderId
    )
      return setActiveItemId(recycleBinFolderId);

    if (
      location.pathname.includes("/accounts/filter") &&
      activeItemId !== "accounts"
    )
      return setActiveItemId("accounts");

    if (location.pathname.includes("/settings") && activeItemId !== "settings")
      return setActiveItemId("settings");

    if (location.pathname.includes("profile")) {
      if (activeItemId) return;
      return setActiveItemId(rootFolderId || roomsFolderId);
    }

    if (location.pathname.includes("/products/files/#preview")) {
      setActiveItemId(rootFolderId);
    }
  }, [
    location.pathname,
    activeItemId,
    roomsFolderId,
    archiveFolderId,
    myFolderId,
    recycleBinFolderId,
    isVisitor,
    rootFolderId,
  ]);

  React.useEffect(() => {
    setIsBurgerLoading(showArticleLoader);
  }, [showArticleLoader]);

  if (showArticleLoader) return <Loaders.ArticleFolder />;

  return (
    <>
      <Items
        onClick={onClick}
        onBadgeClick={onShowNewFilesPanel}
        showText={showText}
        onHide={toggleArticleOpen}
        activeItemId={activeItemId}
      />

      {/* {!isDesktopClient && showText && (
        <StyledBlock showText={showText}>
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
    clientLoadingStore,
  }) => {
    const { clearFiles } = filesStore;
    const {
      showArticleLoader,

      setIsSectionFilterLoading,
      firstLoad,
    } = clientLoadingStore;

    const setIsLoading = (param, withTimer) => {
      setIsSectionFilterLoading(param, withTimer);
    };

    const { roomsFolderId, archiveFolderId, myFolderId, recycleBinFolderId } =
      treeFoldersStore;

    const { setNewFilesPanelVisible } = dialogsStore;

    const selectedFolderId = selectedFolderStore.id;

    const rootFolderId = selectedFolderStore.rootFolderId;

    const {
      showText,

      toggleArticleOpen,

      isDesktopClient,
      FirebaseHelper,
      theme,
      setIsBurgerLoading,
    } = auth.settingsStore;

    return {
      toggleArticleOpen,
      showText,
      showArticleLoader,
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
      rootFolderId,

      setIsLoading,

      clearFiles,
      selectedFolderId,
      setIsBurgerLoading,
    };
  }
)(withTranslation([])(observer(ArticleBodyContent)));
