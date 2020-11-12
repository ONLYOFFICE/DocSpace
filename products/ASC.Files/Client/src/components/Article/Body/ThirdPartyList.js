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
} from "../../../store/files/actions";

const { changeLanguage } = utils;

const i18n = createI18N({
  page: "Article",
  localesPath: "Article",
});

const StyledThirdParty = styled.div`
  margin-top: 60px;

  .tree-thirdparty-list {
    display: flex;

    div {
      height: 26px;
      width: 100%;
      background: #eceef1;
      margin-right: 1px;
      text-align: center;
      color: #818b91;

      &:hover {
        cursor: pointer;
      }

      img {
        padding: 0 5px;
        margin-top: 4px;
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
    //"data-auth_key": capabilityAuthKey,
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
  setSelectedNode,
  setSelectedFolder,
}) => {
  const onConnect = (e) => {
    const data = e.currentTarget.dataset;
    data.link ? openConnectWindow(data.title) : setConnectItem(data);

    const thirdPartyUrl = "/products/files/settings/thirdParty";
    if (history.location.pathname !== thirdPartyUrl) {
      setSelectedNode("thirdParty");
      setSelectedFolder({});
      return history.push(thirdPartyUrl);
    }
  };

  return (
    <StyledThirdParty>
      <Link color="#555F65" fontSize="14px" fontWeight={600}>
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
})(ThirdPartyList);
