import React from "react";
import styled from "styled-components";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import Row from "@docspace/components/row";
import RowContent from "@docspace/components/row-content";
import RowContainer from "@docspace/components/row-container";
import { withTranslation } from "react-i18next";
import EmptyFolderContainer from "../../../../components/EmptyContainer/EmptyContainer";
import { inject, observer } from "mobx-react";
import combineUrl from "@docspace/common/utils/combineUrl";
import AppServerConfig from "@docspace/common/constants/AppServerConfig";
import config from "PACKAGE_FILE";
import { withRouter } from "react-router";
import { connectedCloudsTypeTitleTranslation } from "@docspace/client/src/helpers/filesUtils";
import Loaders from "@docspace/common/components/Loaders";
import { tablet } from "@docspace/components/utils/device";
import { ReactSVG } from "react-svg";
import { isMobile } from "react-device-detect";
import { Base } from "@docspace/components/themes";

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  className: "empty-folder_link",
  display: "flex",
};

const StyledHeader = styled.div`
  display: ${isMobile ? "none" : "flex"};
  border-bottom: ${(props) => props.theme.connectedClouds.borderBottom};
  padding-bottom: 12px;

  @media ${tablet} {
    display: none;
  }

  .cloud-settings-clouds {
    width: 30%;
    margin-right: 12px;
  }

  .cloud-settings-name {
    display: flex;
    margin-left: 6px;
    width: 70%;
  }

  .cloud-settings-separator {
    display: block;
    height: 10px;
    margin: 4px 8px 0 0;
    z-index: 1;
    border-right: ${(props) => props.theme.connectedClouds.borderRight};
  }

  .cloud-settings-header_connection {
    display: flex;
    margin-left: -15px;
  }
`;

StyledHeader.defaultProps = { theme: Base };

const StyledRow = styled(Row)`
  .cloud-settings-row-content {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    width: 100%;
  }
`;

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
    const { id, title, providerKey } = dataset;
    console.log(dataset);
    this.props.setDeleteThirdPartyDialogVisible(true);
    this.props.setRemoveItem({ id, title, providerKey });
  };

  onChangeThirdPartyInfo = (e) => {
    const { capabilities, providers } = this.props;
    const { dataset } = (e.originalEvent || e).currentTarget;
    const { providerId } = dataset;

    const providerItem = providers.find((x) => x.provider_id === providerId);

    const { corporate, customer_title, provider_key } = providerItem;

    const capabilitiesItem = capabilities.find((x) => x[0] === provider_key);

    const item = {
      title: customer_title || (capabilitiesItem && capabilitiesItem[0]),
      link: capabilitiesItem ? capabilitiesItem[1] : " ",
      corporate,
      provider_id: providerId,
      provider_key,
    };

    this.props.setConnectItem(item);
    this.props.setConnectDialogVisible(true);
  };

  openLocation = (e) => {
    const {
      myFolderId,
      commonFolderId,
      filter,
      providers,
      homepage,
      history,
      getSubfolders,
      setFirstLoad,
    } = this.props;
    const { dataset } = (e.originalEvent || e).currentTarget;

    const provider = dataset.providerKey;
    const providerId = dataset.providerId;

    const isCorporate =
      !!providers.length &&
      providers.find((p) => p.provider_key === provider).corporate;
    const dirId = isCorporate ? commonFolderId : myFolderId;

    getSubfolders(dirId).then((subfolders) => {
      const id = subfolders
        .filter((f) => f.providerKey === provider && f.providerId == providerId)
        .map((f) => f.id)
        .join();

      const newFilter = filter.clone();
      newFilter.page = 0;
      newFilter.startIndex = 0;
      newFilter.folder = id;

      const urlFilter = newFilter.toUrlParams();
      setFirstLoad(true);
      history.push(
        combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`) //TODO: Change url by category
      );
    });
  };

  getContextOptions = (item, index) => {
    const { t } = this.props;

    return [
      {
        key: `${index}_open`,
        "data-provider-id": item.provider_id,
        "data-provider-key": item.provider_key,
        icon: "images/folder.react.svg",
        label: t("Files:Open"),
        onClick: this.openLocation,
        disabled: !isMobile,
      },
      {
        key: `${index}_change`,
        "data-provider-id": item.provider_id,
        icon: "/static/images/access.edit.react.svg",
        label: t("Translations:ThirdPartyInfo"),
        onClick: this.onChangeThirdPartyInfo,
      },
      { key: "separator", isSeparator: true },
      {
        key: `${index}_delete`,
        "data-id": item.provider_id,
        "data-title": item.customer_title,
        "data-provider-key": item.provider_key,
        icon: "/static/images/catalog.trash.react.svg",
        label: t("Common:Disconnect"),
        onClick: this.onDeleteThirdParty,
      },
    ];
  };

  render() {
    const { t, providers, tReady, theme, getThirdPartyIcon } = this.props;

    linkStyles.color = theme.filesSettings.color;

    return (
      <>
        {!!providers.length ? (
          <>
            <Button
              size="small"
              style={{ marginBottom: "24px" }}
              onClick={this.onShowThirdPartyDialog}
              label={t("Common:AddButton")}
              primary
            />
            <StyledHeader>
              <Text
                className="cloud-settings-clouds"
                fontSize="12px"
                fontWeight={600}
                color={theme.connectedClouds.color}
              >
                {t("Clouds")}
              </Text>

              <div className="cloud-settings-name">
                <div className="cloud-settings-separator" />
                <Text fontSize="12px" fontWeight={600} color="#A3A9AE">
                  {t("Common:Name")}
                </Text>
              </div>
            </StyledHeader>
            <RowContainer useReactWindow={false}>
              {providers.map((item, index) => {
                const src = getThirdPartyIcon(item.provider_key);
                const element = <ReactSVG src={src} alt="" />;
                const typeTitle = connectedCloudsTypeTitleTranslation(
                  item.provider_key,
                  t
                );

                return (
                  <StyledRow
                    key={index}
                    element={element}
                    contextOptions={this.getContextOptions(item, index)}
                  >
                    <RowContent>
                      <Text
                        as="div"
                        type="page"
                        fontSize="13px"
                        fontWeight={600}
                        title={item.provider_key}
                        //color={theme.filesSettings.linkColor}
                        noSelect
                        containerWidth="30%"
                      >
                        {tReady ? (
                          typeTitle
                        ) : (
                          <Loaders.Rectangle width="90px" height="10px" />
                        )}
                      </Text>
                      <div></div>

                      <Link
                        type="page"
                        title={item.customer_title}
                        //color={theme.filesSettings.linkColor}
                        isHovered={true}
                        color="#A3A9AE"
                        fontSize="11px"
                        fontWeight={400}
                        truncate={true}
                        data-provider-key={item.provider_key}
                        data-provider-id={item.provider_id}
                        onClick={this.openLocation}
                        containerWidth="70%"
                      >
                        {item.customer_title}
                      </Link>
                    </RowContent>
                  </StyledRow>
                );
              })}
            </RowContainer>
          </>
        ) : (
          <EmptyFolderContainer
            headerText={t("ConnectEmpty")}
            descriptionText={t("ConnectDescriptionText")}
            style={{ gridColumnGap: "39px" }}
            buttonStyle={{ marginTop: "16px" }}
            imageSrc="/static/images/empty_screen_alt.svg"
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
                    {t("Common:Connect")}
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
  ({ auth, filesStore, settingsStore, treeFoldersStore, dialogsStore }) => {
    const {
      providers,
      capabilities,
      getThirdPartyIcon,
    } = settingsStore.thirdPartyStore;
    const { filter, setFirstLoad } = filesStore;
    const { myFolder, commonFolder, getSubfolders } = treeFoldersStore;
    const {
      setConnectItem,
      setThirdPartyDialogVisible,
      setDeleteThirdPartyDialogVisible,
      setRemoveItem,
      setConnectDialogVisible,
    } = dialogsStore;

    return {
      theme: auth.settingsStore.theme,
      filter,
      providers,
      capabilities,
      myFolderId: myFolder && myFolder.id,
      commonFolderId: commonFolder && commonFolder.id,
      setFirstLoad,
      setThirdPartyDialogVisible,
      setConnectDialogVisible,
      setConnectItem,
      setDeleteThirdPartyDialogVisible,
      setRemoveItem,
      getSubfolders,
      getThirdPartyIcon,

      homepage: config.homepage,
    };
  }
)(
  withTranslation(["FilesSettings", "Translations", "Files", "Common"])(
    observer(withRouter(ConnectClouds))
  )
);
