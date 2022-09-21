import React from "react";
import { FolderType } from "@docspace/common/constants";
import { inject, observer } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import Loaders from "@docspace/common/components/Loaders";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { combineUrl } from "@docspace/common/utils";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import { AppServerConfig } from "@docspace/common/constants";
import history from "@docspace/common/history";
import config from "PACKAGE_FILE";

const RootFolderContainer = (props) => {
  const {
    t,
    theme,
    isPrivacyFolder,
    isRecycleBinFolder,
    isRoomsFolder,
    isArchiveFolder,
    isDesktop,
    isEncryptionSupport,
    organizationName,
    privacyInstructions,
    title,
    onCreate,
    onCreateRoom,
    myFolderId,
    filter,
    fetchFiles,
    setIsLoading,
    rootFolderType,
    linkStyles,
    isLoading,
    viewAs,
    fetchRooms,
    setAlreadyFetchingRooms,
    categoryType,
  } = props;
  const subheadingText = t("SubheadingEmptyText");
  const myDescription = t("MyEmptyContainerDescription");
  const shareDescription = t("SharedEmptyContainerDescription");
  const commonDescription = t("CommonEmptyContainerDescription");
  const trashHeader = t("EmptyScreenFolder");
  const archiveHeader = t("ArchiveEmptyScreenHeader");
  const recentHeader = t("NoFilesHereYet");
  const trashDescription = t("TrashEmptyDescription");
  const favoritesDescription = t("FavoritesEmptyContainerDescription");
  const recentDescription = t("RecentEmptyContainerDescription");
  const roomsDescription = t("RoomEmptyContainerDescription");
  const archiveRoomsDescription = t("ArchiveEmptyScreen");

  const privateRoomHeader = t("PrivateRoomHeader");
  const privacyIcon = <img alt="" src="images/privacy.svg" />;
  const privateRoomDescTranslations = [
    t("PrivateRoomDescriptionSafest"),
    t("PrivateRoomDescriptionSecure"),
    t("PrivateRoomDescriptionEncrypted"),
    t("PrivateRoomDescriptionUnbreakable"),
  ];

  const [showLoader, setShowLoader] = React.useState(false);
  const [isEmptyPage, setIsEmptyPage] = React.useState(false);

  React.useEffect(() => {
    if (
      rootFolderType !== FolderType.USER &&
      rootFolderType !== FolderType.COMMON
    ) {
      setIsEmptyPage(true);
    } else {
      setIsEmptyPage(false);
    }
  }, [isEmptyPage, setIsEmptyPage, rootFolderType]);

  const onGoToMyDocuments = () => {
    const newFilter = filter.clone();
    setIsLoading(true);
    fetchFiles(myFolderId, newFilter).finally(() => setIsLoading(false));
  };

  const onGoToShared = () => {
    setIsLoading(true);

    setAlreadyFetchingRooms(true);
    fetchRooms(null, null)
      .then(() => {
        const filter = RoomsFilter.getDefault();

        const filterParamsStr = filter.toUrlParams();

        const url = getCategoryUrl(categoryType, filter.folder);

        const pathname = `${url}?${filterParamsStr}`;

        history.push(
          combineUrl(AppServerConfig.proxyURL, config.homepage, pathname)
        );
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const getEmptyFolderProps = () => {
    switch (rootFolderType) {
      case FolderType.USER:
        return {
          descriptionText: myDescription,
          imageSrc: "/static/images/empty_screen.png",
          buttons: commonButtons,
        };
      case FolderType.SHARE:
        return {
          descriptionText: shareDescription,
          imageSrc: "images/empty_screen_forme.png",
        };
      case FolderType.COMMON:
        return {
          descriptionText: commonDescription,
          imageSrc: "images/empty_screen_corporate.png",
          buttons: commonButtons,
        };
      case FolderType.Favorites:
        return {
          descriptionText: favoritesDescription,
          imageStyle: { margin: "0px 0 0 auto" },
          imageSrc: "images/empty_screen_favorites.png",
        };
      case FolderType.Recent:
        return {
          headerText: recentHeader,
          descriptionText: recentDescription,
          imageSrc: "images/empty_screen_recent.svg",
          buttons: recentButtons,
        };
      case FolderType.Privacy:
        return {
          descriptionText: privateRoomDescription,
          imageSrc: "images/empty_screen_privacy.png",
          buttons: isDesktop && isEncryptionSupport && commonButtons,
        };
      case FolderType.TRASH:
        return {
          headerText: trashHeader,
          descriptionText: trashDescription,
          style: { gridColumnGap: "39px", gridTemplateColumns: "150px" },
          imageSrc: theme.isBase
            ? "images/empty_screen_trash_alt.png"
            : "images/empty_screen_trash_alt.png",
          buttons: trashButtons,
        };
      case FolderType.Rooms:
        return {
          headerText: "Welcome to DocSpace!",
          descriptionText: roomsDescription,
          imageSrc: "images/empty_screen_corporate.png",
          buttons: roomsButtons,
        };
      case FolderType.Archive:
        return {
          headerText: archiveHeader,
          descriptionText: archiveRoomsDescription,
          imageSrc: "images/empty_screen_archive.svg",
          buttons: archiveButtons,
        };
      default:
        break;
    }
  };

  const privateRoomDescription = (
    <>
      <Text fontSize="15px" as="div">
        {privateRoomDescTranslations.map((el) => (
          <Box
            displayProp="flex"
            alignItems="center"
            paddingProp="0 0 13px 0"
            key={el}
          >
            <Box paddingProp="0 7px 0 0">{privacyIcon}</Box>
            <Box>{el}</Box>
          </Box>
        ))}
      </Text>
      {!isDesktop && (
        <Text fontSize="12px">
          <Trans t={t} i18nKey="PrivateRoomSupport" ns="Files">
            Work in Private Room is available via {{ organizationName }} desktop
            app.
            <Link
              isBold
              isHovered
              color={theme.filesEmptyContainer.privateRoom.linkColor}
              href={privacyInstructions}
            >
              Instructions
            </Link>
          </Trans>
        </Text>
      )}
    </>
  );

  const commonButtons = (
    <span>
      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_plus-image"
          src="images/plus.svg"
          data-format="docx"
          onClick={onCreate}
          alt="plus_icon"
        />
        <Box className="flex-wrapper_container">
          <Link data-format="docx" onClick={onCreate} {...linkStyles}>
            {t("Document")},
          </Link>
          <Link data-format="xlsx" onClick={onCreate} {...linkStyles}>
            {t("Spreadsheet")},
          </Link>
          <Link data-format="pptx" onClick={onCreate} {...linkStyles}>
            {t("Presentation")}
          </Link>
          <Link data-format="docxf" onClick={onCreate} {...linkStyles}>
            {t("Translations:NewForm")}
          </Link>
        </Box>
      </div>

      <div className="empty-folder_container-links">
        <img
          className="empty-folder_container_plus-image"
          src="images/plus.svg"
          onClick={onCreate}
          alt="plus_icon"
        />
        <Link {...linkStyles} onClick={onCreate}>
          {t("Folder")}
        </Link>
      </div>
    </span>
  );

  const trashButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container_up-image"
        src="images/empty_screen_people.svg"
        width="12px"
        alt=""
        onClick={onGoToMyDocuments}
      />
      <Link onClick={onGoToMyDocuments} {...linkStyles}>
        {t("GoToMyButton")}
      </Link>
    </div>
  );

  const roomsButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container_plus-image"
        src="images/plus.svg"
        onClick={onCreateRoom}
        alt="plus_icon"
      />
      <Link onClick={onCreateRoom} {...linkStyles}>
        Create room
      </Link>
    </div>
  );

  const archiveButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container_folder-image"
        src="images/empty-folder-image.svg"
        onClick={onGoToShared}
        alt="folder_icon"
      />
      <Link onClick={onGoToShared} {...linkStyles}>
        {t("GoToShared")}
      </Link>
    </div>
  );

  const recentButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container_person-image"
        src="images/person.svg"
        alt="person_icon"
      />
      <Link {...linkStyles}>{t("GoToPersonal")}</Link>
    </div>
  );

  const headerText = isPrivacyFolder ? privateRoomHeader : title;
  const subheadingTextProp =
    isPrivacyFolder || isRecycleBinFolder || isRoomsFolder || isArchiveFolder
      ? {}
      : { subheadingText };
  const emptyFolderProps = getEmptyFolderProps();

  React.useEffect(() => {
    let timeout;

    if (isLoading) {
      setShowLoader(isLoading);
    } else {
      timeout = setTimeout(() => setShowLoader(isLoading), 300);
    }

    return () => clearTimeout(timeout);
  }, [isLoading]);

  return (
    <>
      {showLoader ? (
        viewAs === "tile" ? (
          <Loaders.Tiles />
        ) : (
          <Loaders.Rows />
        )
      ) : (
        <EmptyContainer
          headerText={headerText}
          isEmptyPage={isEmptyPage}
          {...subheadingTextProp}
          {...emptyFolderProps}
        />
      )}
    </>
  );
};

export default inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const {
      isDesktopClient,
      isEncryptionSupport,
      organizationName,
      theme,
    } = auth.settingsStore;

    const {
      filter,
      fetchFiles,
      privacyInstructions,
      isLoading,
      setIsLoading,
      viewAs,
      fetchRooms,
      categoryType,
      setAlreadyFetchingRooms,
    } = filesStore;
    const { title, rootFolderType } = selectedFolderStore;
    const {
      isPrivacyFolder,
      myFolderId,
      isRecycleBinFolder,
      isRoomsFolder,
      isArchiveFolder,
    } = treeFoldersStore;

    return {
      theme,
      isPrivacyFolder,
      isRecycleBinFolder,
      isRoomsFolder,
      isArchiveFolder,
      isDesktop: isDesktopClient,
      isEncryptionSupport,
      organizationName,
      privacyInstructions,
      title,
      myFolderId,
      filter,
      fetchFiles,
      isLoading,
      setIsLoading,
      rootFolderType,
      viewAs,
      fetchRooms,
      categoryType,
      setAlreadyFetchingRooms,
    };
  }
)(withTranslation("Files")(observer(RootFolderContainer)));
