import React from "react";
import { ReactSVG } from "react-svg";
import styled, { css } from "styled-components";
import { Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import {
  connectedCloudsTypeTitleTranslation,
  connectedCloudsTitleTranslation,
} from "@docspace/client/src/helpers/filesUtils";
import { Base } from "@docspace/components/themes";
import Button from "@docspace/components/button";
import SelectorAddButton from "@docspace/components/selector-add-button";
import { isMobile } from "react-device-detect";
import { mobile } from "@docspace/components/utils/device";

const StyledModalDialog = styled(ModalDialog)`
  .modal-dialog-aside-body {
    margin-right: -16px;
  }

  .connect-cloud-message {
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;
    color: #657077;
  }

  .service-block {
    padding-top: 16px;
    display: grid;
    grid-gap: 16px;

    .service-item-container {
      display: flex;

      .service-name-container {
        display: flex;
        align-items: center;

        .service-item__svg {
          width: 24px;
          height: 24px;
          margin-right: 8px;
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
      }

      .service-btn {
        margin-left: auto;

        &-text {
          color: ${(props) => props.theme.connectCloud.connectBtnContent};
          background: ${(props) => props.theme.connectCloud.connectBtnTextBg};
          ${(props) =>
            `border: 1px solid ${props.theme.connectCloud.connectBtnTextBorder}`};
        }

        &-icon {
          width: 36px;
          border-radius: 3px;
          padding: 0;
          display: flex;
          align-items: center;
          justify-content: center;
          background: ${(props) => props.theme.connectCloud.connectBtnIconBg};
          ${(props) =>
            `border: 1px solid ${props.theme.connectCloud.connectBtnIconBorder}`};

          path {
            fill: ${(props) => props.theme.connectCloud.connectBtnContent};
          }
        }
      }
    }

    .modal-dialog-aside {
      padding-bottom: 0;
    }
  }
`;

StyledModalDialog.defaultProps = { theme: Base };

const ServiceItem = (props) => {
  const {
    t,
    capability,
    className,
    getThirdPartyIcon,
    serviceName,
    serviceKey,
    onClick,
  } = props;

  const capabilityKey = capability[0];
  const capabilityLink = capability.length > 1 ? capability[1] : "";

  const dataProps = {
    "data-link": capabilityLink,
    "data-title": capabilityKey,
    "data-key": capabilityKey,
  };

  const src = getThirdPartyIcon(serviceKey || capabilityKey);

  const capabilityName = connectedCloudsTypeTitleTranslation(capabilityKey, t);

  return (
    <div className="service-item-container">
      <div className="service-name-container">
        <ReactSVG
          src={src}
          className={`service-item__svg ${className}`}
          alt=""
        />
        <Text fontWeight={600} fontSize="14px">
          {serviceName ? serviceName : capabilityName}
        </Text>
      </div>
      {isMobile ? (
        <SelectorAddButton
          onClick={onClick}
          iconName="/static/images/actions.plus.icon.react.svg"
          className="service-btn service-btn-icon"
          title={t("Common:Connect")}
          {...dataProps}
        />
      ) : (
        <Button
          size="small"
          className="service-btn service-text"
          label={t("Common:Connect")}
          onClick={onClick}
          {...dataProps}
        />
      )}
    </div>
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
    getThirdPartyIcon,
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
    const item = e.currentTarget.dataset || e.target.dataset;
    const showAccountSetting = !item.link;
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

  return (
    <StyledModalDialog
      isLoading={!tReady}
      visible={visible}
      scale={false}
      displayType="aside"
      zIndex={310}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("ConnectCloud")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text as="div" className="connect-cloud-message" noSelect>
          {t("ConnectMessage")}
          {/* {isAdmin && (
            <Trans t={t} i18nKey="ConnectAdminDescription" ns="FilesSettings">
              For successful connection enter the necessary data at
              <Link isHovered href="/settings/integration/third-party-services">
                this page
              </Link>
            </Trans>
          )} */}
        </Text>
        <div className="service-block">
          {googleConnectItem && (
            <ServiceItem
              t={t}
              capability={googleConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}

          {boxConnectItem && (
            <ServiceItem
              t={t}
              capability={boxConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}

          {dropboxConnectItem && (
            <ServiceItem
              t={t}
              capability={dropboxConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}

          {sharePointConnectItem && (
            <ServiceItem
              t={t}
              capability={sharePointConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}

          {oneDriveConnectItem && (
            <ServiceItem
              t={t}
              capability={oneDriveConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}

          {/* {sharePointConnectItem && (
            <ServiceItem
              t={t}
              capability={sharePointConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )} */}

          {nextCloudConnectItem && (
            <ServiceItem
              t={t}
              serviceName="Nextcloud"
              serviceKey="NextCloud"
              capability={nextCloudConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}

          {ownCloudConnectItem && (
            <ServiceItem
              t={t}
              serviceName="ownCloud"
              serviceKey="OwnCloud"
              capability={ownCloudConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}

          {kDriveConnectItem && (
            <ServiceItem
              t={t}
              capability={kDriveConnectItem}
              onClick={onShowService}
              className={"kDrive"}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}
          {yandexConnectItem && (
            <ServiceItem
              t={t}
              capability={yandexConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}
          {webDavConnectItem && (
            <ServiceItem
              t={t}
              serviceName={t("ConnextOtherAccount")}
              capability={webDavConnectItem}
              onClick={onShowService}
              getThirdPartyIcon={getThirdPartyIcon}
            />
          )}
        </div>
      </ModalDialog.Body>
    </StyledModalDialog>
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
    getThirdPartyIcon,
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
    getThirdPartyIcon,
  };
})(
  withTranslation(["FilesSettings", "Translations, Common"])(
    observer(ThirdPartyDialog)
  )
);
