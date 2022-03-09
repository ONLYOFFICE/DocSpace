import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { setDocumentTitle } from "../../../helpers/utils";
import config from "../../../../package.json";
import { AppServerConfig } from "@appserver/common/constants";
import Items from "./Items";
import { isMobile, tablet } from "@appserver/components/utils/device";
import FilesFilter from "@appserver/common/api/files/filter";
import SettingsItems from "./SettingsItems";
import { combineUrl } from "@appserver/common/utils";
import { isDesktop, isTablet, isMobileOnly } from "react-device-detect";
import ThirdPartyList from "./ThirdPartyList";
import DownloadAppList from "./DownloadAppList";
import Banner from "./Banner";

const StyledBlock = styled.div`
  padding: 0 20px;

  @media ${tablet} {
    padding: ${(props) => (props.showText ? "0 16px" : 0)};
  }
`;

const CatalogBodyContent = (props) => {
  const {
    personal,
    firstLoad,
    showText,
    isDesktopClient,
    enableThirdParty,
    isVisitor,
    campaigns,
    FirebaseHelper,
  } = props;
  const onClick = React.useCallback((data) => {
    const {
      toggleCatalogOpen,
      setIsLoading,
      setSelectedNode,
      fetchFiles,
      homepage,
      history,
      setFirstLoad,
    } = props;

    setSelectedNode(data);
    setIsLoading(true);

    if (window.location.pathname.indexOf("/filter") > 0) {
      fetchFiles(data, null, true, false)
        .then(() => {
          (isMobileOnly || isMobile()) && toggleCatalogOpen();
        })
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoading(false));
    } else {
      setFirstLoad(true);
      const filter = FilesFilter.getDefault();

      filter.folder = data;

      const urlFilter = filter.toUrlParams();
      if (isMobileOnly || isMobile()) toggleCatalogOpen();
      history.push(
        combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`)
      );
    }
  }, []);

  const onShowNewFilesPanel = React.useCallback((folderId) => {
    props.setNewFilesPanelVisible(true, [`${folderId}`]);
  }, []);

  return (
    <>
      <Items onClick={onClick} onBadgeClick={onShowNewFilesPanel} />
      {!personal && !firstLoad && <SettingsItems />}
      {!isDesktopClient && showText && (
        <StyledBlock showText={showText}>
          {enableThirdParty && !isVisitor && <ThirdPartyList />}
          <DownloadAppList />
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
    const { fetchFiles, setIsLoading, setFirstLoad, firstLoad } = filesStore;
    const { treeFolders, setSelectedNode, setTreeFolders } = treeFoldersStore;

    const { setNewFilesPanelVisible } = dialogsStore;

    const {
      showText,
      catalogOpen,

      toggleCatalogOpen,
      personal,
      hideArticle,
      isDesktopClient,
      FirebaseHelper,
    } = auth.settingsStore;

    const selectedFolderTitle = selectedFolderStore.title;

    selectedFolderTitle
      ? setDocumentTitle(selectedFolderTitle)
      : setDocumentTitle();

    return {
      toggleCatalogOpen,
      treeFolders,
      showText,
      catalogOpen,
      enableThirdParty: settingsStore.enableThirdParty,
      isVisitor: auth.userStore.user.isVisitor,
      homepage: config.homepage,
      personal,

      setIsLoading,
      setFirstLoad,
      fetchFiles,
      setSelectedNode,
      setTreeFolders,
      setNewFilesPanelVisible,
      hideArticle,
      firstLoad,
      isDesktopClient,
      FirebaseHelper,
    };
  }
)(observer(withRouter(CatalogBodyContent)));
