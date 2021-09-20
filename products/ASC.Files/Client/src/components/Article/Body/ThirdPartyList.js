import React from "react";
import styled from "styled-components";
import Link from "@appserver/components/link";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";
import { withRouter } from "react-router";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../package.json";
import withLoader from "../../../HOCs/withLoader";
import { useCallback } from "react";
import IconButton from "@appserver/components/icon-button";
import { connectedCloudsTitleTranslation } from "../../../helpers/utils";

const StyledThirdParty = styled.div`
  margin-top: 42px;

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
      //background: #eceef1;
      //text-align: center;
      margin-right: 10px;
      color: #818b91;
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

const iconButtonProps = {
  color: "#A3A9AE",
  hoverColor: "#818b91",
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
}) => {
  const redirectAction = () => {
    const thirdPartyUrl = "/settings/thirdParty";
    if (history.location.pathname.indexOf(thirdPartyUrl) === -1) {
      setSelectedNode(["thirdParty"]);
      setSelectedFolder(null);
      return history.push(
        combineUrl(AppServerConfig.proxyURL, config.homepage, thirdPartyUrl)
      );
    }
  };

  const onConnect = (e) => {
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
      data.title = connectedCloudsTitleTranslation(data.title, t);
      setConnectItem(data);
      setConnectDialogVisible(true);
      redirectAction();
    }
  };

  const onShowConnectPanel = useCallback(() => {
    setThirdPartyDialogVisible(true);
    redirectAction();
  }, []);

  return (
    <StyledThirdParty>
      <Link
        color="#555F65"
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
          />
        )}
        {boxConnectItem && (
          <ServiceItem
            capability={boxConnectItem}
            src="images/services/box.svg"
            onClick={onConnect}
          />
        )}
        {dropboxConnectItem && (
          <ServiceItem
            capability={dropboxConnectItem}
            src="images/services/dropbox.svg"
            onClick={onConnect}
          />
        )}
        {oneDriveConnectItem && (
          <ServiceItem
            capability={oneDriveConnectItem}
            src="images/services/onedrive.svg"
            onClick={onConnect}
          />
        )}
        {nextCloudConnectItem && (
          <ServiceItem
            capability={nextCloudConnectItem}
            src="images/services/nextcloud.svg"
            onClick={onConnect}
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
          {...iconButtonProps}
        />
      </div>
    </StyledThirdParty>
  );
};

const ThirdPartyList = withTranslation(["Article", "Translations"])(
  withRouter(withLoader(PureThirdPartyListContainer)(<></>))
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

    const { getOAuthToken } = auth.settingsStore;

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
    };
  }
)(observer(ThirdPartyList));
