import React from "react";
import { connect } from "react-redux";
import styled from "styled-components";
import {
  Box,
  Text,
  Link,
  ModalDialog,
  Button,
  Row,
  Icons,
  toastr,
} from "asc-web-components";
import { Loaders, store } from "asc-web-common";
import { withTranslation } from "react-i18next";
import EmptyFolderContainer from "../../../Home/Section/Body/EmptyFolderContainer";
import { createI18N } from "../../../../../helpers/i18n";
import { Trans } from "react-i18next";
import {
  getConnectedCloud,
  deleteThirdParty,
  openConnectWindow,
  setConnectItem,
} from "../../../../../store/files/actions";
import {
  getCapabilities,
  getGoogleConnect,
  getBoxConnect,
  getDropboxConnect,
  getOneDriveConnect,
  getNextCloudConnect,
  getSharePointConnect,
  getkDriveConnect,
  getYandexConnect,
  getOwnCloudConnect,
  getWebDavConnect,
  getConnectItem,
} from "../../../../../store/files/selectors";
import ConnectedDialog from "./ConnectedDialog";

const { isAdmin } = store.auth.selectors;

const i18n = createI18N({
  page: "SectionBodyContent",
  localesPath: "pages/Settings",
});

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
  const capabilityAuthKey = capability[1];
  const capabilityLink = capability[2] ? capability[2] : "";

  const dataProps = {
    "data-link": capabilityLink,
    "data-auth_key": capabilityAuthKey,
    "data-title": capabilityName,
  };

  return <img {...dataProps} {...rest} alt="" />;
};

class ConnectClouds extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      connectDialogVisible: false,
      showAccountSettingDialog: !!props.connectItem,
      showDeleteDialog: false,
      providers: [],
      loginValue: "",
      passwordValue: "",
      folderNameValue: "",
      selectedServiceData: props.connectItem, //connectItem
      removeItemId: null,
      providersLoading: true,
    };
  }

  componentDidMount() {
    getConnectedCloud().then((providers) => {
      this.setState({
        providers,
        providersLoading: false,
      });
    });
  }

  componentDidUpdate(prevProps) {
    const visible = !!this.props.connectItem;
    const prevVisible = !!prevProps.connectItem;

    if (visible !== prevVisible) {
      visible &&
        this.setState({
          showAccountSettingDialog: true,
          selectedServiceData: this.props.connectItem,
        });
    }
  }

  onShowService = (e) => {
    const selectedServiceData = e.currentTarget.dataset;
    const showAccountSettingDialog = !e.currentTarget.dataset.link;
    !showAccountSettingDialog && openConnectWindow(selectedServiceData.title);

    this.setState({
      connectDialogVisible: !this.state.connectDialogVisible,
      showAccountSettingDialog,
      selectedServiceData,
    });
  };

  onShowConnectDialog = () => {
    this.setState({ connectDialogVisible: !this.state.connectDialogVisible });
  };

  onShowAccountSettingDialog = () => {
    this.setState(
      {
        showAccountSettingDialog: !this.state.showAccountSettingDialog,
        selectedServiceData: null,
      },
      () => this.props.connectItem && this.props.setConnectItem(null)
    );
  };

  onShowDeleteDialog = (e) => {
    const removeItemId = e.currentTarget.dataset.id;

    this.setState({
      showDeleteDialog: !this.state.showDeleteDialog,
      removeItemId,
    });
  };

  onChangeThirdPartyInfo = (e) => {
    const key = e.currentTarget.dataset.key;
    const capabilitiesItem = this.props.capabilities.find((x) => x[0] === key);
    const providerItem = this.state.providers.find(
      (x) => x.provider_key === key
    );

    const selectedServiceData = {
      title: capabilitiesItem[0],
      auth_key: capabilitiesItem[1],
      link: capabilitiesItem[2],
      corporate: providerItem.corporate,
      provider_id: providerItem.provider_id,
      provider_key: key,
    };
    this.setState({ selectedServiceData, showAccountSettingDialog: true });
  };

  onDeleteThirdParty = () => {
    const id = this.state.removeItemId;
    const providers = this.state.providers.filter((x) => x.provider_id !== id);
    deleteThirdParty(+id)
      .then(() => this.setState({ showDeleteDialog: false, providers }))
      .catch((err) => {
        toastr(err);
        this.setState({ showDeleteDialog: false });
      });
  };

  getThirdPartyIcon = (iconName) => {
    switch (iconName) {
      case "Box":
        return <Icons.BoxIcon size="big" />;
      case "DropboxV2":
        return <Icons.DropBoxIcon size="big" />;
      case "GoogleDrive":
        return <Icons.GoogleDriveIcon size="big" />;
      case "OneDrive":
        return <Icons.OneDriveIcon size="big" />;
      case "SharePoint":
        return <Icons.SharePointIcon size="big" />;
      case "kDrive":
        return <Icons.KDriveIcon size="big" />;
      case "Yandex":
        return <Icons.YandexDiskIcon size="big" />;
      /*--------------------------------------------*/
      case "OwnCloud":
        return <Icons.OwnCloudIcon size="big" />;
      case "NextCloud":
        return <Icons.NextCloudIcon size="big" />;
      case "OneDriveForBusiness":
        return <Icons.OneDriveIcon size="big" />;
      case "WebDav":
        return <Icons.WebDavIcon size="big" />;

      default:
        return;
    }
  };

  render() {
    const {
      connectDialogVisible,
      providers,
      showAccountSettingDialog,
      showDeleteDialog,
      selectedServiceData,
      providersLoading,
    } = this.state;
    const {
      t,
      isAdmin,
      googleConnectItem,
      boxConnectItem,
      dropboxConnectItem,
      sharePointConnectItem,
      oneDriveConnectItem,
      nextCloudConnectItem,
      ownCloudConnectItem,
      kDriveConnectItem,
      yandexConnectItem,
      webDavConnectItem,
    } = this.props;

    const linkStyles = {
      isHovered: true,
      type: "action",
      fontWeight: "600",
      color: "#555f65",
      className: "empty-folder_link",
      display: "flex",
    };

    const buttons = (
      <div className="empty-folder_container-links empty-connect_container-links">
        <img
          className="empty-folder_container_plus-image"
          src="images/plus.svg"
          onClick={this.onShowConnectDialog}
          alt="plus_icon"
        />
        <Box className="flex-wrapper_container">
          <Link onClick={this.onShowConnectDialog} {...linkStyles}>
            {t("AddAccount")},
          </Link>
        </Box>
      </div>
    );

    return (
      <>
        {!!providers.length ? (
          <>
            <Button
              size="base"
              onClick={this.onShowConnectDialog}
              label={t("ConnectedCloud")}
              primary
            />
            {providers.map((item, index) => {
              const element = this.getThirdPartyIcon(item.provider_key);
              return (
                <Row
                  key={index}
                  element={element}
                  contextOptions={[
                    {
                      "data-key": item.provider_key,
                      label: t("ThirdPartyInfo"),
                      onClick: this.onChangeThirdPartyInfo,
                    },
                    {
                      "data-id": item.provider_id,
                      label: t("DeleteThirdParty"),
                      onClick: this.onShowDeleteDialog,
                    },
                  ]}
                >
                  <Box
                    containerMinWidth="200px"
                    containerWidth="100%"
                    displayProp="flex"
                    flexDirection="row"
                    alignItems="baseline"
                    alignSelf="baseline"
                  >
                    <Text
                      style={{ width: 100 }}
                      as="div"
                      type="page"
                      title={item.provider_key}
                      fontWeight="600"
                      fontSize="15px"
                      color="#333"
                    >
                      {item.provider_key}
                    </Text>
                    <Link
                      type="page"
                      title={item.customer_title}
                      as="div"
                      color="#333"
                      fontSize="12px"
                      fontWeight={400}
                      truncate={true}
                    >
                      {item.customer_title}
                    </Link>
                  </Box>
                </Row>
              );
            })}
          </>
        ) : providersLoading ? (
          <Loaders.Rows />
        ) : (
          <EmptyFolderContainer
            headerText={t("ConnectAccounts")}
            subheadingText={t("ConnectAccountsSubTitle")}
            imageSrc="images/empty_screen.png"
            buttons={buttons}
          />
        )}

        {connectDialogVisible && (
          <ModalDialog
            visible={connectDialogVisible}
            scale={false}
            displayType="auto"
            zIndex={310}
            onClose={this.onShowConnectDialog}
          >
            <ModalDialog.Header>{t("ConnectingAccount")}</ModalDialog.Header>
            <ModalDialog.Body>
              <Text as="div">
                {t("ConnectDescription")}
                {isAdmin && (
                  <Trans i18nKey="ConnectAdminDescription" i18n={i18n}>
                    For successful connection enter the necessary data at
                    <Link
                      isHovered
                      href="/settings/integration/third-party-services"
                    >
                      this page
                    </Link>
                  </Trans>
                )}
              </Text>
              <StyledServicesBlock>
                {googleConnectItem && (
                  <ServiceItem
                    capability={googleConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_google-drive.svg"
                  />
                )}

                {boxConnectItem && (
                  <ServiceItem
                    capability={boxConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_box.svg"
                  />
                )}

                {dropboxConnectItem && (
                  <ServiceItem
                    capability={dropboxConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_dropbox.svg"
                  />
                )}

                {sharePointConnectItem && (
                  <ServiceItem
                    capability={sharePointConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_sharepoint.svg"
                  />
                )}

                {oneDriveConnectItem && (
                  <ServiceItem
                    capability={oneDriveConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_onedrive.svg"
                  />
                )}

                {sharePointConnectItem && (
                  <ServiceItem
                    capability={sharePointConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_onedrive-for-business.svg"
                  />
                )}

                {nextCloudConnectItem && (
                  <ServiceItem
                    capability={webDavConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_nextcloud.svg"
                  />
                )}

                {ownCloudConnectItem && (
                  <ServiceItem
                    capability={webDavConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_owncloud.svg"
                  />
                )}

                {kDriveConnectItem && (
                  <ServiceItem
                    capability={kDriveConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_kdrive.svg"
                  />
                )}
                {yandexConnectItem && (
                  <ServiceItem
                    capability={yandexConnectItem}
                    onClick={this.onShowService}
                    src="images/services/logo_yandex_ru.svg"
                  />
                )}
                {webDavConnectItem && (
                  <Text
                    onClick={this.onShowService}
                    className="service-item service-text"
                    data-title={webDavConnectItem[0]}
                  >
                    {t("ConnextOtherAccount")}
                  </Text>
                )}
              </StyledServicesBlock>
            </ModalDialog.Body>
          </ModalDialog>
        )}
        {showAccountSettingDialog && (
          <ConnectedDialog
            visible={showAccountSettingDialog}
            onClose={this.onShowAccountSettingDialog}
            t={t}
            item={selectedServiceData}
          />
        )}

        {showDeleteDialog && (
          <ModalDialog
            visible={showDeleteDialog}
            zIndex={310}
            onClose={this.onShowDeleteDialog}
          >
            <ModalDialog.Header>{t("DeleteThirdParty")}</ModalDialog.Header>
            <ModalDialog.Body>{t("DeleteThirdPartyAlert")}</ModalDialog.Body>
            <ModalDialog.Footer>
              <Button
                label={t("OKButton")}
                size="big"
                primary
                onClick={this.onDeleteThirdParty}
              />
            </ModalDialog.Footer>
          </ModalDialog>
        )}
      </>
    );
  }
}

function mapStateToProps(state) {
  return {
    isAdmin: isAdmin(state),
    capabilities: getCapabilities(state),
    googleConnectItem: getGoogleConnect(state),
    boxConnectItem: getBoxConnect(state),
    dropboxConnectItem: getDropboxConnect(state),
    oneDriveConnectItem: getOneDriveConnect(state),
    nextCloudConnectItem: getNextCloudConnect(state),
    sharePointConnectItem: getSharePointConnect(state),
    kDriveConnectItem: getkDriveConnect(state),
    yandexConnectItem: getYandexConnect(state),
    ownCloudConnectItem: getOwnCloudConnect(state),
    webDavConnectItem: getWebDavConnect(state),
    connectItem: getConnectItem(state),
  };
}

export default connect(mapStateToProps, { setConnectItem })(
  withTranslation()(ConnectClouds)
);
