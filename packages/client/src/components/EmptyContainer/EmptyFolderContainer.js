import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import { Text } from "@docspace/components";
import { ReactSVG } from "react-svg";
import LoaderEmptyContainer from "./sub-components/loaderEmptyContainer";

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
}) => {
  const onBackToParentFolder = () => {
    setIsLoading(true);

    isRooms
      ? fetchRooms(parentId).finally(() => setIsLoading(false))
      : fetchFiles(parentId).finally(() => setIsLoading(false));
  };

  React.useEffect(() => {
    setIsEmptyPage(true);

    return () => setIsEmptyPage(false);
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
          src="images/plus.svg"
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
          src="images/plus.svg"
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
                src="images/plus.svg"
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
            onClick={onBackToParentFolder}
            src="images/up.svg"
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

  if (!isLoadedFetchFiles || !tReady) {
    return <LoaderEmptyContainer />;
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
      imageSrc={
        isRooms
          ? "images/empty_screen_corporate.png"
          : "/static/images/empty_screen_alt.svg"
      }
      buttons={buttons}
      sectionWidth={sectionWidth}
      isEmptyFolderContainer={true}
    />
  );
};

export default inject(
  ({
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
    };
  }
)(withTranslation(["Files", "Translations"])(observer(EmptyFolderContainer)));
