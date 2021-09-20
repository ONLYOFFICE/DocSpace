import React from "react";
import styled from "styled-components";
import { Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import { connectedCloudsTitleTranslation } from "../../../helpers/utils";
import NoUserSelect from "@appserver/components/utils/commonStyles";

const StyledServicesBlock = styled.div`
  display: grid;
  column-gap: 55px;
  row-gap: 20px;
  justify-content: center;
  align-items: center;
  grid-template-columns: repeat(auto-fill, 158px);
  padding-top: 24px;

  .service-item {
    border: 1px solid #d1d1d1;
    width: 158px;
    height: 40px;

    :hover {
      cursor: pointer;
    }
  }

  img {
    ${NoUserSelect}
    border: 1px solid #d1d1d1;
    width: 158px;
    height: 40px;

    :hover {
      cursor: pointer;
    }
  }

  .service-text {
    display: flex;
    align-items: center;
    justify-content: center;
  }
`;

const ServiceItem = (props) => {
  const { capability, t, ...rest } = props;

  const capabilityName = capability[0];
  const capabilityLink = capability.length > 1 ? capability[1] : "";

  const dataProps = {
    "data-link": capabilityLink,
    "data-title": capabilityName,
    "data-key": capabilityName,
  };

  return <img {...dataProps} {...rest} alt="" />;
};

const ThirdPartyDialog = (props) => {
  const {
    t,
    tReady,
    isAdmin,
    googleConnectItem,
    boxConnectItem,
    dropboxConnectItem,
    sharePointConnectItem,
    nextCloudConnectItem,
    ownCloudConnectItem,
    oneDriveConnectItem,
    kDriveConnectItem,
    yandexConnectItem,
    webDavConnectItem,
    visible,
    setThirdPartyDialogVisible,
    openConnectWindow,
    getOAuthToken,
    setConnectDialogVisible,
    setConnectItem,
  } = props;

  const onClose = () => {
    setThirdPartyDialogVisible(false);
  };

  const showOAuthModal = (token, serviceData) => {
    setConnectItem({
      title: connectedCloudsTitleTranslation(serviceData.title, t),
      provider_key: serviceData.title,
      link: serviceData.link,
      token,
    });
    setConnectDialogVisible(true);
  };

  const onShowService = (e) => {
    setThirdPartyDialogVisible(false);
    const item = e.currentTarget.dataset;
    const showAccountSetting = !e.currentTarget.dataset.link;
    if (!showAccountSetting) {
      let authModal = window.open(
        "",
        "Authorization",
        "height=600, width=1020"
      );
      openConnectWindow(item.title, authModal).then((modal) =>
        getOAuthToken(modal).then((token) => {
          authModal.close();
          showOAuthModal(token, item);
        })
      );
    } else {
      item.title = connectedCloudsTitleTranslation(item.title, t);
      setConnectItem(item);
      setConnectDialogVisible(true);
    }

    setThirdPartyDialogVisible(false);
  };

  return (
    <ModalDialog
      isLoading={!tReady}
      visible={visible}
      scale={false}
      displayType="auto"
      zIndex={310}
      onClose={onClose}
    >
      <ModalDialog.Header>
        {t("Translations:ConnectingAccount")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text as="div" noSelect>
          {t("ConnectDescription")}
          {isAdmin && (
            <Trans t={t} i18nKey="ConnectAdminDescription" ns="Settings">
              For successful connection enter the necessary data at
              <Link isHovered href="/settings/integration/third-party-services">
                this page
              </Link>
            </Trans>
          )}
        </Text>
        <StyledServicesBlock>
          {googleConnectItem && (
            <ServiceItem
              capability={googleConnectItem}
              onClick={onShowService}
              src="images/services/logo_google-drive.svg"
            />
          )}

          {boxConnectItem && (
            <ServiceItem
              capability={boxConnectItem}
              onClick={onShowService}
              src="images/services/logo_box.svg"
            />
          )}

          {dropboxConnectItem && (
            <ServiceItem
              capability={dropboxConnectItem}
              onClick={onShowService}
              src="images/services/logo_dropbox.svg"
            />
          )}

          {sharePointConnectItem && (
            <ServiceItem
              capability={sharePointConnectItem}
              onClick={onShowService}
              src="images/services/logo_sharepoint.svg"
            />
          )}

          {oneDriveConnectItem && (
            <ServiceItem
              capability={oneDriveConnectItem}
              onClick={onShowService}
              src="images/services/logo_onedrive.svg"
            />
          )}

          {sharePointConnectItem && (
            <ServiceItem
              capability={sharePointConnectItem}
              onClick={onShowService}
              src="images/services/logo_onedrive-for-business.svg"
            />
          )}

          {nextCloudConnectItem && (
            <ServiceItem
              capability={webDavConnectItem}
              onClick={onShowService}
              src="images/services/logo_nextcloud.svg"
            />
          )}

          {ownCloudConnectItem && (
            <ServiceItem
              capability={webDavConnectItem}
              onClick={onShowService}
              src="images/services/logo_owncloud.svg"
            />
          )}

          {kDriveConnectItem && (
            <ServiceItem
              capability={kDriveConnectItem}
              onClick={onShowService}
              src="images/services/logo_kdrive.svg"
            />
          )}
          {yandexConnectItem && (
            <ServiceItem
              capability={yandexConnectItem}
              onClick={onShowService}
              src="images/services/logo_yandex_ru.svg"
            />
          )}
          {webDavConnectItem && (
            <Text
              onClick={onShowService}
              className="service-item service-text"
              data-title={webDavConnectItem[0]}
              data-key={webDavConnectItem[0]}
              noSelect
            >
              {t("ConnextOtherAccount")}
            </Text>
          )}
        </StyledServicesBlock>
      </ModalDialog.Body>
    </ModalDialog>
  );
};

export default inject(({ auth, settingsStore, dialogsStore }) => {
  const {
    googleConnectItem,
    boxConnectItem,
    dropboxConnectItem,
    oneDriveConnectItem,
    nextCloudConnectItem,
    kDriveConnectItem,
    yandexConnectItem,
    ownCloudConnectItem,
    webDavConnectItem,
    sharePointConnectItem,
    openConnectWindow,
  } = settingsStore.thirdPartyStore;
  const {
    setThirdPartyDialogVisible,
    thirdPartyDialogVisible: visible,
    setConnectDialogVisible,
    setConnectItem,
  } = dialogsStore;
  const { getOAuthToken } = auth.settingsStore;

  return {
    visible,
    isAdmin: auth.isAdmin,
    googleConnectItem,
    boxConnectItem,
    dropboxConnectItem,
    oneDriveConnectItem,
    nextCloudConnectItem,
    kDriveConnectItem,
    yandexConnectItem,
    ownCloudConnectItem,
    webDavConnectItem,
    sharePointConnectItem,

    setConnectDialogVisible,
    setConnectItem,
    setThirdPartyDialogVisible,
    getOAuthToken,
    openConnectWindow,
  };
})(withTranslation(["Settings", "Translations"])(observer(ThirdPartyDialog)));
