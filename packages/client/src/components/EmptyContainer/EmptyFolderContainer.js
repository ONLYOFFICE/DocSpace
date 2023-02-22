import PlusSvgUrl from "PUBLIC_DIR/images/plus.svg?url";
import UpSvgUrl from "PUBLIC_DIR/images/up.svg?url";
import EmptyScreenAltSvgUrl from "PUBLIC_DIR/images/empty_screen_alt.svg?url";
import EmptyScreenAltSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_alt_dark.svg?url";
import EmptyScreenCorporateSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate.svg?url";
import EmptyScreenCorporateDarkSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate_dark.svg?url";
import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import { Text } from "@docspace/components";
import { ReactSVG } from "react-svg";
import Loaders from "@docspace/common/components/Loaders";

const EmptyFolderContainer = ({
  t,
  onCreate,
  fetchFiles,
  fetchRooms,
  setIsLoading,
  parentId,
  linkStyles,
  isRooms,
  sectionWidth,
  canCreateFiles,
  canInviteUsers,
  setIsEmptyPage,
  onClickInviteUsers,
  folderId,
  tReady,
  isLoadedFetchFiles,
  viewAs,
  setIsLoadedEmptyPage,
  theme,
}) => {
  const onBackToParentFolder = () => {
    setIsLoading(true);

    isRooms
      ? fetchRooms(parentId).finally(() => setIsLoading(false))
      : fetchFiles(parentId).finally(() => setIsLoading(false));
  };

  React.useEffect(() => {
    if (isLoadedFetchFiles && tReady) {
      setIsLoadedEmptyPage(true);
    } else {
      setIsLoadedEmptyPage(false);
    }
  }, [isLoadedFetchFiles, tReady]);

  React.useEffect(() => {
    setIsEmptyPage(true);

    return () => {
      setIsEmptyPage(false);
      setIsLoadedEmptyPage(false);
    };
  }, []);

  const onInviteUsersClick = () => {
    if (!isRooms) return;

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

      {isRooms ? (
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

  if (!isLoadedFetchFiles || !tReady) {
    return <Loaders.EmptyContainerLoader viewAs={viewAs} />;
  }

  return (
    <EmptyContainer
      headerText={isRooms ? t("RoomCreated") : t("EmptyScreenFolder")}
      style={{ gridColumnGap: "39px" }}
      descriptionText={
        canCreateFiles
          ? t("EmptyFolderDecription")
          : t("EmptyFolderDescriptionUser")
      }
      imageSrc={isRooms ? emptyScreenCorporateSvg : emptyScreenAltSvg}
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
    filesStore,
    selectedFolderStore,
    contextOptionsStore,
  }) => {
    const {
      fetchFiles,
      fetchRooms,
      setIsEmptyPage,
      isLoadedFetchFiles,
      viewAs,
      setIsLoadedEmptyPage,
    } = filesStore;
    const {
      navigationPath,
      parentId,
      access,
      id: folderId,
      roomType,
      security,
    } = selectedFolderStore;

    let id;
    if (navigationPath?.length) {
      const elem = navigationPath[0];
      id = elem.id;
    }

    const isRooms = !!roomType;

    const { canCreateFiles } = accessRightsStore;

    const { onClickInviteUsers } = contextOptionsStore;

    const canInviteUsers = isRooms && security?.EditAccess; // skip sub-folders

    return {
      fetchFiles,
      fetchRooms,
      setIsLoading: filesStore.setIsLoading,
      parentId: id ?? parentId,
      isRooms,
      canCreateFiles,
      canInviteUsers,
      setIsEmptyPage,
      onClickInviteUsers,
      folderId,
      isLoadedFetchFiles,
      viewAs,
      setIsLoadedEmptyPage,
      theme: auth.settingsStore.theme,
    };
  }
)(withTranslation(["Files", "Translations"])(observer(EmptyFolderContainer)));
