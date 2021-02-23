import React from "react";
import styled from "styled-components";
import {
  Box,
  Text,
  Link,
  ModalDialog,
  Button,
  RowContainer,
  Row,
  Icons,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import EmptyFolderContainer from "../../../Home/Section/Body/EmptyFolderContainer";
import { Trans } from "react-i18next";
import { DeleteThirdPartyDialog, ConnectDialog } from "../../../../dialogs";
import { inject, observer } from "mobx-react";

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
  const capabilityLink = capability[1] ? capability[1] : "";

  const dataProps = {
    "data-link": capabilityLink,
    "data-title": capabilityName,
  };

  return <img {...dataProps} {...rest} alt="" />;
};

class ConnectClouds extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showThirdPartyDialog: props.showThirdPartyPanel,
      showAccountSettingDialog: !!props.connectItem,
      showDeleteDialog: false,
      loginValue: "",
      passwordValue: "",
      folderNameValue: "",
      selectedServiceData: props.connectItem,
      removeItem: null,
    };
  }

  componentDidUpdate(prevProps) {
    const visible = !!this.props.connectItem;
    const prevVisible = !!prevProps.connectItem;

    if (this.props.showThirdPartyPanel !== prevProps.showThirdPartyPanel) {
      this.props.showThirdPartyPanel &&
        this.setState({
          showThirdPartyDialog: true,
        });
    }

    if (visible !== prevVisible) {
      visible &&
        this.setState({
          showAccountSettingDialog: true,
          selectedServiceData: this.props.connectItem,
        });
    }
  }

  showOAuthModal = (token, serviceData) => {
    this.setState({
      showAccountSettingDialog: true,
      selectedServiceData: {
        title: serviceData.title,
        provider_key: serviceData.title,
        link: serviceData.link,
        token,
      },
    });
  };

  onShowService = (e) => {
    console.log("onShowService", e.currentTarget.dataset);
    const selectedServiceData = e.currentTarget.dataset;
    const showAccountSettingDialog = !e.currentTarget.dataset.link;
    if (!showAccountSettingDialog) {
      let authModal = window.open(
        "",
        "Authorization",
        "height=600, width=1020"
      );
      this.props
        .openConnectWindow(selectedServiceData.title, authModal)
        .then((modal) =>
          this.props
            .getOAuthToken(modal)
            .then((token) => this.showOAuthModal(token, selectedServiceData))
        );
    }

    this.setState({
      showThirdPartyDialog: !this.state.showThirdPartyDialog,
      showAccountSettingDialog,
      selectedServiceData,
    });
  };

  onShowThirdPartyDialog = () => {
    this.setState(
      { showThirdPartyDialog: !this.state.showThirdPartyDialog },
      () =>
        this.props.showThirdPartyPanel &&
        this.props.setShowThirdPartyPanel(false)
    );
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

  onDeleteThirdParty = (e) => {
    const { id, title } = e.currentTarget.dataset;
    const removeItem = { id, title };

    this.setState({
      showDeleteDialog: !this.state.showDeleteDialog,
      removeItem,
    });
  };

  onShowDeleteDialog = () => {
    this.setState({ showDeleteDialog: !this.state.showDeleteDialog });
  };

  onChangeThirdPartyInfo = (e) => {
    const key = e.currentTarget.dataset.key;
    const capabilitiesItem = this.props.capabilities.find((x) => x[0] === key);
    const providerItem = this.props.providers.find(
      (x) => x.provider_key === key
    );

    const { corporate, provider_id, customer_title } = providerItem;

    const selectedServiceData = {
      title: capabilitiesItem ? capabilitiesItem[0] : customer_title,
      link: capabilitiesItem ? capabilitiesItem[1] : " ",
      corporate: corporate,
      provider_id: provider_id,
      provider_key: key,
    };
    this.setState({ selectedServiceData, showAccountSettingDialog: true });
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

  openLocation = (e) => {
    const {
      myDirectoryFolders,
      commonDirectoryFolders,
      filter,
      fetchFiles,
      setSelectedNode,
      providers,
    } = this.props;
    const provider = e.currentTarget.dataset.providerKey;
    const isCorporate =
      !!providers.length &&
      providers.find((p) => p.provider_key === provider).corporate;
    const dir = isCorporate ? commonDirectoryFolders : myDirectoryFolders;
    const id = dir
      .filter((f) => f.providerKey === provider)
      .map((f) => f.id)
      .join();
    return fetchFiles(id, filter).then(() => setSelectedNode([id]));
  };

  render() {
    const {
      showThirdPartyDialog,
      showAccountSettingDialog,
      showDeleteDialog,
      selectedServiceData,
      removeItem,
    } = this.state;
    const {
      t,
      providers,
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
          onClick={this.onShowThirdPartyDialog}
          alt="plus_icon"
        />
        <Box className="flex-wrapper_container">
          <Link onClick={this.onShowThirdPartyDialog} {...linkStyles}>
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
              onClick={this.onShowThirdPartyDialog}
              label={t("ConnectedCloud")}
              primary
            />
            <RowContainer useReactWindow={false}>
              {providers.map((item, index) => {
                const element = this.getThirdPartyIcon(item.provider_key);
                return (
                  <Row
                    key={index}
                    element={element}
                    contextOptions={[
                      {
                        key: `${index}_change`,
                        "data-key": item.provider_key,
                        label: t("ThirdPartyInfo"),
                        onClick: this.onChangeThirdPartyInfo,
                      },
                      {
                        key: `${index}_delete`,
                        "data-id": item.provider_id,
                        "data-title": item.customer_title,
                        label: t("DeleteThirdParty"),
                        onClick: this.onDeleteThirdParty,
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
                        color="#333"
                        fontSize="13px"
                        fontWeight={400}
                        truncate={true}
                        data-provider-key={item.provider_key}
                        onClick={this.openLocation}
                      >
                        {item.customer_title}
                      </Link>
                    </Box>
                  </Row>
                );
              })}
            </RowContainer>
          </>
        ) : (
          <EmptyFolderContainer
            headerText={t("ConnectAccounts")}
            subheadingText={t("ConnectAccountsSubTitle")}
            imageSrc="images/empty_screen.png"
            buttons={buttons}
          />
        )}

        {showThirdPartyDialog && (
          <ModalDialog
            visible={showThirdPartyDialog}
            scale={false}
            displayType="auto"
            zIndex={310}
            onClose={this.onShowThirdPartyDialog}
          >
            <ModalDialog.Header>{t("ConnectingAccount")}</ModalDialog.Header>
            <ModalDialog.Body>
              <Text as="div">
                {t("ConnectDescription")}
                {isAdmin && (
                  <Trans i18nKey="ConnectAdminDescription" ns="Settings">
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
          <ConnectDialog
            visible={showAccountSettingDialog}
            onClose={this.onShowAccountSettingDialog}
            item={selectedServiceData}
          />
        )}

        {showDeleteDialog && (
          <DeleteThirdPartyDialog
            onClose={this.onShowDeleteDialog}
            visible={showDeleteDialog}
            removeItem={removeItem}
          />
        )}
      </>
    );
  }
}

export default inject(
  ({ auth, filesStore, settingsStore, treeFoldersStore }) => {
    const {
      providers,
      connectItem,
      capabilities,
      setConnectItem,
      showThirdPartyPanel,
      setShowThirdPartyPanel,
      googleConnectItem,
      boxConnectItem,
      dropboxConnectItem,
      oneDriveConnectItem,
      nextCloudConnectItem,
      kDriveConnectItem,
      yandexConnectItem,
      ownCloudConnectItem,
      webDavConnectItem,
      getOAuthToken,
      openConnectWindow,
    } = settingsStore.thirdPartyStore;
    const { fetchFiles, filter } = filesStore;
    const { setSelectedNode, myFolder, commonFolder } = treeFoldersStore;

    return {
      isAdmin: auth.isAdmin,
      filter,
      providers,
      showThirdPartyPanel,
      connectItem,
      capabilities,
      googleConnectItem,
      boxConnectItem,
      dropboxConnectItem,
      oneDriveConnectItem,
      nextCloudConnectItem,
      kDriveConnectItem,
      yandexConnectItem,
      ownCloudConnectItem,
      webDavConnectItem,
      myDirectoryFolders: myFolder && myFolder.folders,
      commonDirectoryFolders: commonFolder && commonFolder.folders,

      fetchFiles,
      setSelectedNode,
      setConnectItem,
      setShowThirdPartyPanel,
      getOAuthToken,
      openConnectWindow,
    };
  }
)(withTranslation("Settings")(observer(ConnectClouds)));
