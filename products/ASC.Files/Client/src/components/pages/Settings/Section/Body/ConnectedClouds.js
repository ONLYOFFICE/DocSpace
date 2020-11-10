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
} from "../../../../../store/files/actions";
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

  switch (capabilityName) {
    case "Box":
      return (
        <img
          src="images/services/logo_box.svg"
          alt=""
          {...dataProps}
          {...rest}
        />
      );

    case "DropboxV2":
      return (
        <img
          src="images/services/logo_dropbox.svg"
          alt=""
          {...dataProps}
          {...rest}
        />
      );

    case "GoogleDrive":
      return (
        <img
          src="images/services/logo_google-drive.svg"
          alt=""
          {...dataProps}
          {...rest}
        />
      );

    case "OneDrive":
      return (
        <img
          src="images/services/logo_onedrive.svg"
          alt=""
          {...dataProps}
          {...rest}
        />
      );

    case "SharePoint":
      return (
        <>
          <img
            src="images/services/logo_onedrive-for-business.svg"
            alt=""
            {...dataProps}
            {...rest}
          />
          <img
            src="images/services/logo_sharepoint.svg"
            alt=""
            {...dataProps}
            {...rest}
          />
        </>
      );

    case "kDrive":
      return (
        <img
          src="images/services/logo_kdrive.svg"
          alt=""
          {...dataProps}
          {...rest}
        />
      );

    case "Yandex":
      return (
        <img
          src="images/services/logo_yandex_ru.svg"
          alt=""
          {...dataProps}
          {...rest}
        />
      );

    case "WebDav":
      return (
        <>
          <img
            src="images/services/logo_nextcloud.svg"
            alt=""
            {...dataProps}
            {...rest}
          />
          <img
            src="images/services/logo_owncloud.svg"
            alt=""
            {...dataProps}
            {...rest}
          />
          <Text {...dataProps} {...rest} className="service-item service-text">
            {t("ConnextOtherAccount")}
          </Text>
        </>
      );

    default:
      return;
  }
};

class ConnectClouds extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      connectDialogVisible: false,
      showAccountSettingDialog: false,
      showDeleteDialog: false,
      providers: [],
      capabilities: [],
      loginValue: "",
      passwordValue: "",
      folderNameValue: "",
      selectedServiceData: null,
      removeItemId: null,
      providersLoading: true,
    };
  }

  componentDidMount() {
    getConnectedCloud().then((res) => {
      this.setState({
        providers: res.providers,
        capabilities: res.capabilities,
        providersLoading: false,
      });
    });
  }

  onShowService = (e) => {
    const selectedServiceData = e.currentTarget.dataset;
    const showAccountSettingDialog = !e.currentTarget.dataset.link;
    if (!showAccountSettingDialog) {
      window.open(
        selectedServiceData.link,
        selectedServiceData.title,
        "width=1020,height=600"
      );
    }
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
    this.setState({
      showAccountSettingDialog: !this.state.showAccountSettingDialog,
      selectedServiceData: null,
    });
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
    const capabilitiesItem = this.state.capabilities.find((x) => x[0] === key);
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
    const providers = this.state.providers.filter((x) => x.id === id);
    deleteThirdParty(+id)
      .then(() => this.setState({ showDeleteDialog: false, providers }))
      .catch((err) => {
        toastr(err);
        this.setState({ showDeleteDialog: false });
      });
  };

  render() {
    const {
      connectDialogVisible,
      providers,
      capabilities,
      showAccountSettingDialog,
      showDeleteDialog,
      selectedServiceData,
      providersLoading,
    } = this.state;
    const { t, isAdmin } = this.props;

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
              return (
                <Row
                  key={index}
                  element={<Icons.NavLogoIcon size="big" />}
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
                {capabilities.map((capability, key) => (
                  <ServiceItem
                    capability={capability}
                    onClick={this.onShowService}
                    className="service-item"
                    t={t}
                    key={key}
                  />
                ))}
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
                label={t("SaveButton")}
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
  };
}

export default connect(mapStateToProps, {})(withTranslation()(ConnectClouds));
