import React from "react";
import { ReactSVG } from "react-svg";
import styled, { css } from "styled-components";
import { Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import { connectedCloudsTitleTranslation } from "../../../helpers/utils";
import NoUserSelect from "@appserver/components/utils/commonStyles";
import { Base } from "@appserver/components/themes";

const StyledServicesBlock = styled.div`
  display: grid;
  column-gap: 55px;
  row-gap: 20px;
  justify-content: center;
  align-items: center;
  grid-template-columns: repeat(auto-fill, 158px);
  padding-top: 24px;

  .service-item {
    border: ${(props) => props.theme.filesThirdPartyDialog.border};
    width: 158px;
    height: 40px;

    :hover {
      cursor: pointer;
    }
  }

  .service-item__svg {
    ${NoUserSelect}
    border: ${(props) => props.theme.filesThirdPartyDialog.border};
    width: 158px;
    height: 40px;

    display: flex;
    justify-content: center;
    align-item: center;

    :hover {
      cursor: pointer;
    }

    ${(props) =>
      !props.theme.isBase &&
      css`
        svg {
          rect {
            fill: #333333;
          }
          path {
            fill: #ffffff;
            opacity: 0.16;
          }
        }
      `}
  }

  .kDrive {
    svg {
      path:nth-child(7) {
        opacity: 0.5 !important;
      }
      path:nth-child(8) {
        opacity: 0.8 !important;
      }
      path:nth-child(9) {
        opacity: 0.8 !important;
      }
      path:nth-child(10) {
        opacity: 0.16 !important;
      }
      path:nth-child(11) {
        opacity: 0.16 !important;
      }
    }
  }

  .service-text {
    display: flex;
    align-items: center;
    justify-content: center;
  }
`;

StyledServicesBlock.defaultProps = { theme: Base };

const ServiceItem = (props) => {
  const { capability, t, className, ...rest } = props;

  const capabilityName = capability[0];
  const capabilityLink = capability.length > 1 ? capability[1] : "";

  const dataProps = {
    "data-link": capabilityLink,
    "data-title": capabilityName,
    "data-key": capabilityName,
  };

  return (
    <ReactSVG
      {...dataProps}
      {...rest}
      className={`service-item__svg ${className}`}
      alt=""
    />
  );
};

const ThirdPartyDialog = (props) => {
  const {
    t,
    theme,
    i18n,
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
      openConnectWindow(item.title, authModal)
        .then(getOAuthToken)
        .then((token) => {
          authModal.close();
          showOAuthModal(token, item);
        })
        .catch((e) => {
          if (!e) return;
          console.error(e);
        });
    } else {
      item.title = connectedCloudsTitleTranslation(item.title, t);
      setConnectItem(item);
      setConnectDialogVisible(true);
    }

    setThirdPartyDialogVisible(false);
  };

  const yandexLogoUrl =
    i18n && i18n.language === "ru-RU"
      ? "images/services/logo_yandex_ru.svg"
      : "images/services/logo_yandex_en.svg";

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
              src={"images/services/logo_sharepoint.svg"}
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
              src={"images/services/logo_onedrive-for-business.svg"}
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
              className={"kDrive"}
              src="images/services/logo_kdrive.svg"
            />
          )}
          {yandexConnectItem && (
            <ServiceItem
              capability={yandexConnectItem}
              onClick={onShowService}
              src={yandexLogoUrl}
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
  const { getOAuthToken, theme } = auth.settingsStore;

  return {
    theme: theme,
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
