import React, { useEffect } from "react";
import { connect } from "react-redux";
import styled from "styled-components";
import { Link } from "asc-web-components";
import { history, utils } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
import {
  getGoogleConnect,
  getBoxConnect,
  getDropboxConnect,
  getOneDriveConnect,
  getNextCloudConnect,
  getWebDavConnect,
} from "../../../store/files/selectors";
import {
  openConnectWindow,
  setConnectItem,
  setSelectedNode,
  setSelectedFolder,
  setShowThirdPartyPanel,
} from "../../../store/files/actions";

const { changeLanguage } = utils;

const i18n = createI18N({
  page: "Article",
  localesPath: "Article",
});

const StyledThirdParty = styled.div`
  margin-top: 42px;

  .tree-thirdparty-list {
    padding-top: 3px;
    display: flex;
    max-width: 200px;

    div {
      height: 26px;
      width: 100%;
      background: #eceef1;
      text-align: center;
      margin-right: 1px;
      color: #818b91;
      :first-of-type {
        border-radius: 3px 0 0 3px;
      }
      :last-of-type {
        border-radius: 0 3px 3px 0;

        img {
          margin-top: 4px;
        }
      }

      img {
        padding: 4px 6px 0 4px;
      }

      @media (max-width: 1024px) {
        height: 32px;
        margin-right: 0px;
        :first-of-type {
          border-radius: 3px 0 0 3px;
          padding-left: 5px;
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

const ServiceItem = (props) => {
  const { capability, src, ...rest } = props;

  const capabilityName = capability[0];
  //const capabilityAuthKey = capability[1];
  const capabilityLink = capability[2] ? capability[2] : "";

  const dataProps = {
    "data-link": capabilityLink,
    //"data-token": capabilityAuthKey,
    "data-title": capabilityName,
  };

  return (
    <div {...dataProps} {...rest}>
      <img src={src} alt="" />
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
  webDavConnectItem,
  setConnectItem,
  setShowThirdPartyPanel,
  setSelectedNode,
  setSelectedFolder,
}) => {
  const redirectAction = () => {
    const thirdPartyUrl = "/products/files/settings/thirdParty";
    if (history.location.pathname !== thirdPartyUrl) {
      setSelectedNode(["thirdParty"]);
      setSelectedFolder({});
      return history.push(thirdPartyUrl);
    }
  };

  const onConnect = (e) => {
    const data = e.currentTarget.dataset;
    data.link ? openConnectWindow(data.title) : setConnectItem(data);

    redirectAction();
  };

  const onShowConnectPanel = () => {
    setShowThirdPartyPanel(true);
    redirectAction();
  };

  return (
    <StyledThirdParty>
      <Link
        color="#555F65"
        fontSize="14px"
        fontWeight={600}
        onClick={onShowConnectPanel}
      >
        {t("AddAccount")}
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
        {webDavConnectItem && (
          <ServiceItem
            capability={webDavConnectItem}
            src="images/services/more.svg"
            onClick={onConnect}
          />
        )}
      </div>
    </StyledThirdParty>
  );
};

const ThirdPartyListContainer = withTranslation()(PureThirdPartyListContainer);

const ThirdPartyList = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <ThirdPartyListContainer {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  return {
    googleConnectItem: getGoogleConnect(state),
    boxConnectItem: getBoxConnect(state),
    dropboxConnectItem: getDropboxConnect(state),
    oneDriveConnectItem: getOneDriveConnect(state),
    nextCloudConnectItem: getNextCloudConnect(state),
    webDavConnectItem: getWebDavConnect(state),
  };
}

export default connect(mapStateToProps, {
  setConnectItem,
  setSelectedNode,
  setSelectedFolder,
  setShowThirdPartyPanel,
})(ThirdPartyList);
