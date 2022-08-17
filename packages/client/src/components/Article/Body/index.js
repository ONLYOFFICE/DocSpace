import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import config from "PACKAGE_FILE";
import { AppServerConfig } from "@docspace/common/constants";
import Items from "./Items";
import { isMobile, tablet } from "@docspace/components/utils/device";
import FilesFilter from "@docspace/common/api/files/filter";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import SettingsItem from "./SettingsItem";
import AccountsItem from "./AccountsItem";
import { combineUrl } from "@docspace/common/utils";
import { isDesktop, isTablet, isMobileOnly } from "react-device-detect";
import ThirdPartyList from "./ThirdPartyList";
import DownloadAppList from "./DownloadAppList";
import Banner from "./Banner";
import { showLoader, hideLoader } from "@docspace/common/utils";
import Loaders from "@docspace/common/components/Loaders";
import withLoader from "../../../HOCs/withLoader";
import { withTranslation } from "react-i18next";
import toastr from "client/toastr";
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
    personal,
    docSpace,
    firstLoad,
    showText,
    isDesktopClient,
    enableThirdParty,
    isVisitor,
    FirebaseHelper,
    theme,
    toggleArticleOpen,
    categoryType,
    isAdmin,
  } = props;

  const campaigns = (localStorage.getItem("campaigns") || "")
    .split(",")
    .filter((campaign) => campaign.length > 0);

  const onClick = React.useCallback(
    (folderId) => {
      const {
        toggleArticleOpen,
        setIsLoading,
        fetchFiles,

        fetchRooms,
        setAlreadyFetchingRooms,

        homepage,
        history,
        roomsFolderId,
        archiveFolderId,
      } = props;

      const filesSection = window.location.pathname.indexOf("/filter") > 0;

      if (filesSection) {
        setIsLoading(true);
      } else {
        showLoader();
      }

      if (folderId === roomsFolderId || folderId === archiveFolderId) {
        setAlreadyFetchingRooms(true);
        fetchRooms(folderId, null)
          .then(() => {
            if (filesSection) {
              const filter = RoomsFilter.getDefault();

              const url = getCategoryUrl(
                folderId === archiveFolderId
                  ? CategoryType.Archive
                  : CategoryType.Shared
              );

              const filterParamsStr = filter.toUrlParams();

              history.push(
                combineUrl(
                  AppServerConfig.proxyURL,
                  homepage,
                  `${url}?${filterParamsStr}`
                )
              );
            }
          })
          .finally(() => {
            if (isMobileOnly || isMobile()) {
              toggleArticleOpen();
            }
            if (filesSection) {
              setIsLoading(false);
            } else {
              hideLoader();
            }
          });
      } else {
        fetchFiles(folderId, null, true, false)
          .then(() => {
            if (!filesSection) {
              const filter = FilesFilter.getDefault();

              filter.folder = folderId;

              const filterParamsStr = filter.toUrlParams();

              const url = getCategoryUrl(categoryType, filter.folder);

              const pathname = `${url}?${filterParamsStr}`;

              history.push(
                combineUrl(AppServerConfig.proxyURL, config.homepage, pathname)
              );
            }
          })
          .catch((err) => toastr.error(err))
          .finally(() => {
            if (isMobileOnly || isMobile()) {
              toggleArticleOpen();
            }
            if (filesSection) {
              setIsLoading(false);
            } else {
              hideLoader();
            }
          });
      }
    },
    [categoryType]
  );

  const onShowNewFilesPanel = React.useCallback((folderId) => {
    props.setNewFilesPanelVisible(true, [`${folderId}`]);
  }, []);

  return (
    <>
      <Items
        onClick={onClick}
        onBadgeClick={onShowNewFilesPanel}
        showText={showText}
        onHide={toggleArticleOpen}
      />
      {!personal && isAdmin && <AccountsItem />}
      {!personal && !firstLoad && <SettingsItem />}
      {!isDesktopClient && showText && !docSpace && (
        <StyledBlock showText={showText}>
          {enableThirdParty && !isVisitor && <ThirdPartyList />}
          <DownloadAppList theme={theme} />
          {(isDesktop || isTablet) &&
            personal &&
            !firstLoad &&
            campaigns.length > 0 && <Banner FirebaseHelper={FirebaseHelper} />}
        </StyledBlock>
      )}
    </>
  );
};

export default inject(
  ({
    auth,
    filesStore,
    treeFoldersStore,
    selectedFolderStore,
    dialogsStore,
    settingsStore,
  }) => {
    const {
      fetchFiles,
      fetchRooms,
      setAlreadyFetchingRooms,
      setIsLoading,
      setFirstLoad,
      firstLoad,
      isLoading,
      isLoaded,
      categoryType,
    } = filesStore;

    const {
      treeFolders,
      setTreeFolders,
      roomsFolderId,
      archiveFolderId,
    } = treeFoldersStore;

    const { setNewFilesPanelVisible } = dialogsStore;
    const isArticleLoading = (!isLoaded || isLoading) && firstLoad;
    const {
      showText,
      articleOpen,

      toggleArticleOpen,

      personal,
      docSpace,

      isDesktopClient,
      FirebaseHelper,
      theme,
    } = auth.settingsStore;

    const selectedFolderTitle = selectedFolderStore.title;

    selectedFolderTitle
      ? setDocumentTitle(selectedFolderTitle)
      : setDocumentTitle();

    return {
      toggleArticleOpen,
      treeFolders,
      showText,
      articleOpen,
      enableThirdParty: settingsStore.enableThirdParty,
      isVisitor: auth.userStore.user.isVisitor,
      isAdmin: auth.userStore.user.isAdmin,
      homepage: config.homepage,

      fetchRooms,
      setAlreadyFetchingRooms,

      personal,
      docSpace,

      isArticleLoading,
      setIsLoading,
      setFirstLoad,
      fetchFiles,

      setTreeFolders,
      setNewFilesPanelVisible,
      firstLoad,
      isDesktopClient,
      FirebaseHelper,
      theme,

      roomsFolderId,
      archiveFolderId,

      categoryType,
    };
  }
)(
  withRouter(
    withTranslation([])(
      withLoader(observer(ArticleBodyContent))(<Loaders.ArticleFolder />)
    )
  )
);
