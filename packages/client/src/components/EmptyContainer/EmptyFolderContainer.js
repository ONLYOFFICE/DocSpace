import PlusSvgUrl from "PUBLIC_DIR/images/plus.svg?url";
import UpSvgUrl from "PUBLIC_DIR/images/up.svg?url";
import EmptyScreenAltSvgUrl from "PUBLIC_DIR/images/empty_screen_alt.svg?url";
import EmptyScreenAltSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_alt_dark.svg?url";
import EmptyScreenCorporateSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate.svg?url";
import EmptyScreenCorporateDarkSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate_dark.svg?url";
import { inject, observer } from "mobx-react";
import React, { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { withTranslation } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import { Text } from "@docspace/components";
import { ReactSVG } from "react-svg";
import FilesFilter from "@docspace/common/api/files/filter";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import Loaders from "@docspace/common/components/Loaders";
import { showLoader, hideLoader } from "./EmptyFolderContainerUtils";
import { FolderType, RoomSearchArea } from "@docspace/common/constants";
import { getCategoryUrl, getCategoryType } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

const EmptyFolderContainer = ({
  t,
  onCreate,

  setIsLoading,
  parentId,
  linkStyles,
  editAccess,
  sectionWidth,
  canCreateFiles,

  onClickInviteUsers,
  folderId,

  theme,

  navigationPath,
  rootFolderType,

  roomType,
  isLoading,
}) => {
  const navigate = useNavigate();
  const location = useLocation();

  const isRoom =
    isLoading && location?.state?.isRoom ? location?.state?.isRoom : !!roomType;

  const canInviteUsers = isRoom && editAccess;

  const onBackToParentFolder = () => {
    setIsLoading(true);

    if (isRoom) {
      const path =
        rootFolderType === FolderType.Archive
          ? getCategoryUrl(CategoryType.Archive)
          : getCategoryUrl(CategoryType.Shared);

      const newFilter = RoomsFilter.getDefault();

      newFilter.searchArea =
        rootFolderType === FolderType.Archive
          ? RoomSearchArea.Archive
          : RoomSearchArea.Active;

      const state = {
        title: navigationPath[0].title,
        isRoot: true,
        rootFolderType,
      };

      navigate(`${path}?${newFilter.toUrlParams()}`, {
        state,
      });
    } else {
      const categoryType = getCategoryType(location);

      const newFilter = FilesFilter.getDefault();
      newFilter.folder = parentId;

      const parentIdx = navigationPath.findIndex((v) => v.id === parentId);

      const parentItem = navigationPath[parentIdx];

      const state = {
        title: parentItem.title,
        isRoot: navigationPath.length === 1,
        rootFolderType,
      };

      const path = getCategoryUrl(categoryType, parentId);

      navigate(`${path}?${newFilter.toUrlParams()}`, {
        state,
      });
    }
  };

  const onInviteUsersClick = () => {
    if (!isRoom) return;

    onClickInviteUsers && onClickInviteUsers(folderId);
  };

  const buttons = canCreateFiles ? (
    <>
      <div className="empty-folder_container-links">
        <ReactSVG
          className="empty-folder_container_plus-image"
          src={PlusSvgUrl}
          data-format="docx"
          onClick={onCreate}
        />
        <Box className="flex-wrapper_container">
          <Link data-format="docx" onClick={onCreate} {...linkStyles}>
            {t("Document")},
          </Link>
          <Link data-format="xlsx" onClick={onCreate} {...linkStyles}>
            {t("Spreadsheet")},
          </Link>
          <Link data-format="pptx" onClick={onCreate} {...linkStyles}>
            {t("Presentation")},
          </Link>
          <Link data-format="docxf" onClick={onCreate} {...linkStyles}>
            {t("Translations:NewForm")}
          </Link>
        </Box>
      </div>

      <div className="empty-folder_container-links">
        <ReactSVG
          className="empty-folder_container_plus-image"
          onClick={onCreate}
          src={PlusSvgUrl}
        />
        <Link {...linkStyles} onClick={onCreate}>
          {t("Folder")}
        </Link>
      </div>

      {isRoom ? (
        canInviteUsers ? (
          <>
            <div className="empty-folder_container-links second-description">
              <Text as="span" color="#6A7378" fontSize="12px" noSelect>
                {t("AddMembersDescription")}
              </Text>
            </div>

            <div className="empty-folder_container-links">
              <ReactSVG
                className="empty-folder_container_plus-image"
                onClick={onInviteUsersClick}
                src={PlusSvgUrl}
              />
              <Link onClick={onInviteUsersClick} {...linkStyles}>
                {t("InviteUsersInRoom")}
              </Link>
            </div>
          </>
        ) : (
          <></>
        )
      ) : (
        <div className="empty-folder_container-links">
          <ReactSVG
            className="empty-folder_container_up-image"
            src={UpSvgUrl}
            onClick={onBackToParentFolder}
          />
          <Link onClick={onBackToParentFolder} {...linkStyles}>
            {t("BackToParentFolderButton")}
          </Link>
        </div>
      )}
    </>
  ) : (
    <></>
  );

  const emptyScreenCorporateSvg = theme.isBase
    ? EmptyScreenCorporateSvgUrl
    : EmptyScreenCorporateDarkSvgUrl;
  const emptyScreenAltSvg = theme.isBase
    ? EmptyScreenAltSvgUrl
    : EmptyScreenAltSvgDarkUrl;

  return (
    <EmptyContainer
      headerText={isRoom ? t("RoomCreated") : t("EmptyScreenFolder")}
      style={{ gridColumnGap: "39px", marginTop: 32 }}
      descriptionText={
        canCreateFiles
          ? t("EmptyFolderDecription")
          : t("EmptyFolderDescriptionUser")
      }
      imageSrc={isRoom ? emptyScreenCorporateSvg : emptyScreenAltSvg}
      buttons={buttons}
      sectionWidth={sectionWidth}
      isEmptyFolderContainer={true}
    />
  );
};

export default inject(
  ({
    auth,
    accessRightsStore,

    selectedFolderStore,
    contextOptionsStore,
    clientLoadingStore,
  }) => {
    const {
      navigationPath,
      parentId,

      id: folderId,

      security,
      rootFolderType,
      roomType,
    } = selectedFolderStore;

    let id;
    if (navigationPath?.length) {
      const elem = navigationPath[0];
      id = elem.id;
    }

    const { canCreateFiles } = accessRightsStore;

    const { onClickInviteUsers } = contextOptionsStore;

    const { setIsSectionFilterLoading, isLoading } = clientLoadingStore;

    const setIsLoading = (param) => {
      setIsSectionFilterLoading(param);
    };

    return {
      setIsLoading,
      isLoading,
      parentId: id ?? parentId,
      roomType,
      canCreateFiles,

      navigationPath,
      rootFolderType,

      editAccess: security?.EditAccess,
      onClickInviteUsers,
      folderId,

      theme: auth.settingsStore.theme,
    };
  }
)(withTranslation(["Files", "Translations"])(observer(EmptyFolderContainer)));
