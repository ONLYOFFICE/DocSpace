import React from "react";
import styled from "styled-components";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import Box from "@appserver/components/box";
import Row from "@appserver/components/row";
import RowContainer from "@appserver/components/row-container";
import { withTranslation } from "react-i18next";
import EmptyFolderContainer from "../../../../components/EmptyContainer/EmptyContainer";
import BoxIcon from "../../../../../public/images/icon_box.react.svg";
import DropBoxIcon from "../../../../../public/images/icon_dropbox.react.svg";
import GoogleDriveIcon from "../../../../../public/images/icon_google_drive.react.svg";
import KDriveIcon from "../../../../../public/images/icon_kdrive.react.svg";
import NextCloudIcon from "../../../../../public/images/icon_nextcloud.react.svg";
import OneDriveIcon from "../../../../../public/images/icon_onedrive.react.svg";
import OwnCloudIcon from "../../../../../public/images/icon_owncloud.react.svg";
import SharePointIcon from "../../../../../public/images/icon_sharepoint.react.svg";
import WebDavIcon from "../../../../../public/images/icon_webdav.react.svg";
import YandexDiskIcon from "../../../../../public/images/icon_yandex_disk.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { inject, observer } from "mobx-react";
import combineUrl from "@appserver/common/utils/combineUrl";
import AppServerConfig from "@appserver/common/constants/AppServerConfig";
import config from "../../../../../package.json";
import { withRouter } from "react-router";

const StyledBoxIcon = styled(BoxIcon)`
  ${commonIconsStyles}
`;
const StyledDropBoxIcon = styled(DropBoxIcon)`
  ${commonIconsStyles}
`;
const StyledGoogleDriveIcon = styled(GoogleDriveIcon)`
  ${commonIconsStyles}
`;
const StyledKDriveIcon = styled(KDriveIcon)`
  ${commonIconsStyles}
`;
const StyledNextCloudIcon = styled(NextCloudIcon)`
  ${commonIconsStyles}
`;
const StyledOneDriveIcon = styled(OneDriveIcon)`
  ${commonIconsStyles}
`;
const StyledOwnCloudIcon = styled(OwnCloudIcon)`
  ${commonIconsStyles}
`;
const StyledSharePointIcon = styled(SharePointIcon)`
  ${commonIconsStyles}
`;
const StyledWebDavIcon = styled(WebDavIcon)`
  ${commonIconsStyles}
`;
const StyledYandexDiskIcon = styled(YandexDiskIcon)`
  ${commonIconsStyles}
`;

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  color: "#555f65",
  className: "empty-folder_link",
  display: "flex",
};

class ConnectClouds extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      loginValue: "",
      passwordValue: "",
      folderNameValue: "",
    };
  }

  onShowThirdPartyDialog = () => {
    this.props.setThirdPartyDialogVisible(true);
  };

  onDeleteThirdParty = (e) => {
    const { dataset } = (e.originalEvent || e).currentTarget;
    const { id, title } = dataset;
    this.props.setDeleteThirdPartyDialogVisible(true);
    this.props.setRemoveItem({ id, title });
  };

  onChangeThirdPartyInfo = (e) => {
    const { dataset } = (e.originalEvent || e).currentTarget;
    const { provider_id } = dataset;
    const capabilitiesItem = this.props.capabilities.find(
      (x) => x[0] === provider_id
    );
    const providerItem = this.props.providers.find(
      (x) => x.provider_id === provider_id
    );
    const { corporate, customer_title, provider_key } = providerItem;

    const item = {
      title: capabilitiesItem ? capabilitiesItem[0] : customer_title,
      link: capabilitiesItem ? capabilitiesItem[1] : " ",
      corporate,
      provider_id,
      provider_key,
    };

    this.props.setConnectItem(item);
    this.props.setConnectDialogVisible(true);
  };

  getThirdPartyIcon = (iconName) => {
    switch (iconName) {
      case "Box":
        return <StyledBoxIcon size="big" />;
      case "DropboxV2":
        return <StyledDropBoxIcon size="big" />;
      case "GoogleDrive":
        return <StyledGoogleDriveIcon size="big" />;
      case "OneDrive":
        return <StyledOneDriveIcon size="big" />;
      case "SharePoint":
        return <StyledSharePointIcon size="big" />;
      case "kDrive":
        return <StyledKDriveIcon size="big" />;
      case "Yandex":
        return <StyledYandexDiskIcon size="big" />;
      case "OwnCloud":
        return <StyledOwnCloudIcon size="big" />;
      case "NextCloud":
        return <StyledNextCloudIcon size="big" />;
      case "OneDriveForBusiness":
        return <StyledOneDriveIcon size="big" />;
      case "WebDav":
        return <StyledWebDavIcon size="big" />;

      default:
        return;
    }
  };

  openLocation = (e) => {
    const {
      myDirectoryFolders,
      commonDirectoryFolders,
      filter,
      providers,
      homepage,
      history,
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

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.startIndex = 0;
    newFilter.folder = id;

    const urlFilter = newFilter.toUrlParams();
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`)
    );
  };

  getContextOptions = (item, index) => {
    const { t } = this.props;
    return [
      {
        key: `${index}_change`,
        "data-provider_id": item.provider_id,
        label: t("Translations:ThirdPartyInfo"),
        onClick: this.onChangeThirdPartyInfo,
      },
      {
        key: `${index}_delete`,
        "data-id": item.provider_id,
        "data-title": item.customer_title,
        label: t("Translations:DeleteThirdParty"),
        onClick: this.onDeleteThirdParty,
      },
    ];
  };

  render() {
    const { t, providers } = this.props;

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
                    contextOptions={this.getContextOptions(item, index)}
                  >
                    <Box
                      containerMinWidth="200px"
                      containerWidth="100%"
                      displayProp="flex"
                      flexDirection="row"
                      alignItems="baseline"
                      alignSelf="baseline"
                      marginProp="auto 0"
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
            imageSrc="/static/images/empty_screen.png"
            buttons={
              <div className="empty-folder_container-links empty-connect_container-links">
                <img
                  className="empty-folder_container_plus-image"
                  src="images/plus.svg"
                  onClick={this.onShowThirdPartyDialog}
                  alt="plus_icon"
                />
                <Box className="flex-wrapper_container">
                  <Link onClick={this.onShowThirdPartyDialog} {...linkStyles}>
                    {t("Translations:AddAccount")},
                  </Link>
                </Box>
              </div>
            }
          />
        )}
      </>
    );
  }
}

export default inject(
  ({ filesStore, settingsStore, treeFoldersStore, dialogsStore }) => {
    const { providers, capabilities } = settingsStore.thirdPartyStore;
    const { filter } = filesStore;
    const { myFolder, commonFolder } = treeFoldersStore;
    const {
      setConnectItem,
      setThirdPartyDialogVisible,
      setDeleteThirdPartyDialogVisible,
      setRemoveItem,
      setConnectDialogVisible,
    } = dialogsStore;

    return {
      filter,
      providers,
      capabilities,
      myDirectoryFolders: myFolder && myFolder.folders,
      commonDirectoryFolders: commonFolder && commonFolder.folders,

      setThirdPartyDialogVisible,
      setConnectDialogVisible,
      setConnectItem,
      setDeleteThirdPartyDialogVisible,
      setRemoveItem,

      homepage: config.homepage,
    };
  }
)(
  withTranslation(["Settings", "Translations"])(
    observer(withRouter(ConnectClouds))
  )
);
