import React from "react";
import styled from "styled-components";
import {
  Box,
  Text,
  Link,
  ModalDialog,
  Button,
  Row,
  Icons,
} from "asc-web-components";
import EmptyFolderContainer from "../../../Home/Section/Body/EmptyFolderContainer";
import { createI18N } from "../../../../../helpers/i18n";
import { Trans } from "react-i18next";
import { getConnectedCloud } from "../../../../../store/files/actions";
import ConnectedDialog from "./ConnectedDialog";

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

  /*
   <img
   src="images/services/logo_nextcloud.svg"
   alt=""
   {...imageProps}
   />
   <img
   src="images/services/logo_owncloud.svg"
   alt=""
   {...imageProps}
   />
*/
  switch (capabilityName) {
    case "Box":
      return (
        <img
          src="images/services/logo_box.svg"
          data-link={`${capabilityLink}`}
          data-title={`${capabilityName}`}
          alt=""
          {...rest}
        />
      );

    case "DropboxV2":
      return (
        <img
          src="images/services/logo_dropbox.svg"
          data-link={`${capabilityLink}`}
          data-title={`${capabilityName}`}
          alt=""
          {...rest}
        />
      );

    case "GoogleDrive":
      return (
        <img
          src="images/services/logo_google-drive.svg"
          data-link={`${capabilityLink}`}
          data-title={`${capabilityName}`}
          alt=""
          {...rest}
        />
      );

    case "OneDrive":
      return (
        <img
          src="images/services/logo_onedrive.svg"
          data-link={`${capabilityLink}`}
          data-title={`${capabilityName}`}
          alt=""
          {...rest}
        />
      );

    case "SharePoint":
      return (
        <>
          <img
            src="images/services/logo_onedrive-for-business.svg"
            data-link={`${capabilityLink}`}
            data-title={`${capabilityName}`}
            alt=""
            {...rest}
          />
          <img
            src="images/services/logo_sharepoint.svg"
            data-link={`${capabilityLink}`}
            data-title={`${capabilityName}`}
            alt=""
            {...rest}
          />
        </>
      );

    case "kDrive":
      return (
        <img
          src="images/services/logo_kdrive.svg"
          data-link={`${capabilityLink}`}
          data-title={`${capabilityName}`}
          alt=""
          {...rest}
        />
      );

    case "Yandex":
      return (
        <img
          src="images/services/logo_yandex_ru.svg"
          data-link={`${capabilityLink}`}
          data-title={`${capabilityName}`}
          alt=""
          {...rest}
        />
      );

    case "WebDav":
      return (
        <Text
          data-link={`${capabilityLink}`}
          data-title={`${capabilityName}`}
          {...rest}
          className="service-item service-text"
        >
          {t("ConnextOtherAccount")}
        </Text>
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
      providers: [],
      capabilities: [],
      loginValue: "",
      passwordValue: "",
      folderNameValue: "",
      selectedService: "",
    };
  }

  componentDidMount() {
    getConnectedCloud().then((res) => {
      this.setState({
        providers: res.providers,
        capabilities: res.capabilities,
      });
    });
  }

  onShowService = (e) => {
    const showAccountSettingDialog = !e.currentTarget.dataset.link;
    this.setState({
      connectDialogVisible: !this.state.connectDialogVisible,
      selectedService: e.currentTarget.dataset.title,
      showAccountSettingDialog,
    });
  };

  onShowConnectDialog = () => {
    this.setState({ connectDialogVisible: !this.state.connectDialogVisible });
  };

  onShowAccountSettingDialog = () => {
    this.setState({
      showAccountSettingDialog: !this.state.showAccountSettingDialog,
    });
  };

  render() {
    const {
      connectDialogVisible,
      providers,
      capabilities,
      showAccountSettingDialog,
      selectedService,
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

    const imageProps = {
      onClick: this.onShowService,
      className: "service-item",
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
                  key={index} //item.provider.id
                  element={<Icons.NavLogoIcon size="big" />}
                  contextOptions={[
                    {
                      label: "Change connection settings",
                      onClick: () => console.log("Change connection settings"),
                    },
                    {
                      label: "Disconnect third party",
                      onClick: () => console.log("Disconnect third party"),
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
                    {...imageProps}
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
            selectedService={selectedService}
          />
        )}
      </>
    );
  }
}

export default ConnectClouds;
