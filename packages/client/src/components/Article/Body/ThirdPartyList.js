import React from "react";
import styled from "styled-components";
import Link from "@docspace/components/link";
import { withTranslation } from "react-i18next";
import { isMobile } from "@docspace/components/utils/device";
import { isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import withLoader from "../../../HOCs/withLoader";
import { useCallback } from "react";
import IconButton from "@docspace/components/icon-button";
import { connectedCloudsTitleTranslation } from "@docspace/client/src/helpers/filesUtils";
import { Base } from "@docspace/components/themes";

const StyledThirdParty = styled.div`
  margin-top: 42px;

  .thirdparty-text {
    color: ${(props) => props.theme.filesArticleBody.thirdPartyList.linkColor};
  }

  .tree-thirdparty-list {
    padding-top: 3px;
    display: flex;
    max-width: inherit;

    .icon {
      padding: 5px;
    }

    div {
      height: 25px;
      width: 25px;

      margin-right: 10px;
      color: ${(props) => props.theme.filesArticleBody.thirdPartyList.color};
      :first-of-type {
        border-radius: 3px 0 0 3px;
      }
      :last-of-type {
        border-radius: 0 3px 3px 0;
      }

      @media (max-width: 1024px) {
        height: 32px;

        :first-of-type {
          border-radius: 3px 0 0 3px;
        }
        :last-of-type {
          border-radius: 0 3px 3px 0;
          padding-right: 5px;

          img {
            margin-top: 7px;
          }
        }

        img {
          padding: 7px 4px 0 4px;
        }
      }

      &:hover {
        cursor: pointer;
      }
    }
  }
`;

StyledThirdParty.defaultProps = { theme: Base };

const iconButtonProps = {
  size: 25,
  className: "icon",
};

const ServiceItem = (props) => {
  const { capability, src, ...rest } = props;

  const capabilityName = capability[0];
  const capabilityLink = capability.length > 1 ? capability[1] : "";

  const dataProps = {
    "data-link": capabilityLink,
    "data-title": capabilityName,
    "data-key": capabilityName,
  };

  return (
    <div {...dataProps} {...rest}>
      <IconButton iconName={src} {...iconButtonProps} />
    </div>
  );
};

const PureThirdPartyListContainer = ({
  t,
  googleConnectItem,
  boxConnectItem,
  dropboxConnectItem,
  oneDriveConnectItem,
  nextCloudConnectItem,
  //webDavConnectItem,
  setConnectItem,
  setConnectDialogVisible,
  setSelectedNode,
  setSelectedFolder,
  getOAuthToken,
  openConnectWindow,
  setThirdPartyDialogVisible,
  history,
  toggleArticleOpen,
}) => {
  const redirectAction = useCallback(() => {
    const thirdPartyUrl = "/settings/connected-clouds";
    if (history.location.pathname.indexOf(thirdPartyUrl) === -1) {
      setSelectedNode(["connected-clouds"]);
      setSelectedFolder(null);
      if (isMobileOnly || isMobile()) {
        toggleArticleOpen();
      }
      return history.push(
        combineUrl(AppServerConfig.proxyURL, config.homepage, thirdPartyUrl)
      );
    }
  }, [setSelectedNode, setSelectedFolder]);

  const onConnect = useCallback(
    (e) => {
      const data = e.currentTarget.dataset;

      if (data.link) {
        let authModal = window.open(
          "",
          "Authorization",
          "height=600, width=1020"
        );
        openConnectWindow(data.title, authModal)
          .then(() => redirectAction())
          .then((modal) =>
            getOAuthToken(modal).then((token) => {
              authModal.close();
              const serviceData = {
                title: connectedCloudsTitleTranslation(data.title, t),
                provider_key: data.title,
                link: data.link,
                token,
              };
              setConnectItem(serviceData);
              setConnectDialogVisible(true);
            })
          )
          .catch((e) => console.error(e));
      } else {
        if (isMobileOnly || isMobile()) {
          toggleArticleOpen();
        }
        data.title = connectedCloudsTitleTranslation(data.title, t);
        setConnectItem(data);
        setConnectDialogVisible(true);
        redirectAction();
      }
    },
    [
      openConnectWindow,
      redirectAction,
      getOAuthToken,
      connectedCloudsTitleTranslation,
      setConnectItem,
      setConnectDialogVisible,
      connectedCloudsTitleTranslation,
    ]
  );

  const onShowConnectPanel = useCallback(() => {
    if (isMobileOnly || isMobile()) {
      toggleArticleOpen();
    }
    setThirdPartyDialogVisible(true);
    redirectAction();
  }, [setThirdPartyDialogVisible, toggleArticleOpen, redirectAction]);

  return (
    <StyledThirdParty>
      <Link
        className="thirdparty-text"
        fontSize="14px"
        fontWeight={600}
        onClick={onShowConnectPanel}
      >
        {t("Translations:AddAccount")}
      </Link>
      <div className="tree-thirdparty-list">
        {googleConnectItem && (
          <ServiceItem
            capability={googleConnectItem}
            src="images/services/google_drive.svg"
            onClick={onConnect}
            title={t("ButtonAddGoogle")}
          />
        )}
        {boxConnectItem && (
          <ServiceItem
            capability={boxConnectItem}
            src="images/services/box.svg"
            onClick={onConnect}
            title={t("ButtonAddBoxNet")}
          />
        )}
        {dropboxConnectItem && (
          <ServiceItem
            capability={dropboxConnectItem}
            src="images/services/dropbox.svg"
            onClick={onConnect}
            title={t("ButtonAddDropBox")}
          />
        )}
        {oneDriveConnectItem && (
          <ServiceItem
            capability={oneDriveConnectItem}
            src="images/services/onedrive.svg"
            onClick={onConnect}
            title={t("ButtonAddSkyDrive")}
          />
        )}
        {nextCloudConnectItem && (
          <ServiceItem
            capability={nextCloudConnectItem}
            src="images/services/nextcloud.svg"
            onClick={onConnect}
            title={t("ButtonAddNextcloud")}
          />
        )}
        {/* {webDavConnectItem && (
          <ServiceItem
            capability={webDavConnectItem}
            src="images/services/more.svg"
            onClick={onConnect}
          />
        )} */}

        <IconButton
          iconName="images/services/more.svg"
          onClick={onShowConnectPanel}
          title={t("Translations:AddAccount")}
          {...iconButtonProps}
        />
      </div>
    </StyledThirdParty>
  );
};

const ThirdPartyList = withTranslation(["Article", "Translations"])(
  withRouter(PureThirdPartyListContainer)
);

export default inject(
  ({
    filesStore,
    auth,
    settingsStore,
    treeFoldersStore,
    selectedFolderStore,
    dialogsStore,
  }) => {
    const { setIsLoading } = filesStore;
    const { setSelectedFolder } = selectedFolderStore;
    const { setSelectedNode } = treeFoldersStore;
    const {
      googleConnectItem,
      boxConnectItem,
      dropboxConnectItem,
      oneDriveConnectItem,
      nextCloudConnectItem,
      webDavConnectItem,
      openConnectWindow,
    } = settingsStore.thirdPartyStore;

    const { getOAuthToken, toggleArticleOpen } = auth.settingsStore;

    const {
      setConnectItem,
      setConnectDialogVisible,
      setThirdPartyDialogVisible,
    } = dialogsStore;
    return {
      googleConnectItem,
      boxConnectItem,
      dropboxConnectItem,
      oneDriveConnectItem,
      nextCloudConnectItem,
      webDavConnectItem,

      setIsLoading,
      setSelectedFolder,
      setSelectedNode,
      setConnectItem,
      setConnectDialogVisible,
      getOAuthToken,
      openConnectWindow,
      setThirdPartyDialogVisible,

      toggleArticleOpen,
    };
  }
)(observer(ThirdPartyList));
