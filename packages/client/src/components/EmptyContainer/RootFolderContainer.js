﻿import PrivacySvgUrl from "PUBLIC_DIR/images/privacy.svg?url";
import PersonSvgUrl from "PUBLIC_DIR/images/person.svg?url";
import PlusSvgUrl from "PUBLIC_DIR/images/plus.svg?url";
import RoomsReactSvgUrl from "PUBLIC_DIR/images/rooms.react.svg?url";
import React, { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import styled from "styled-components";
import { FolderType, RoomSearchArea } from "@docspace/common/constants";
import { inject, observer } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import IconButton from "@docspace/components/icon-button";
import Loaders from "@docspace/common/components/Loaders";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import FilesFilter from "@docspace/common/api/files/filter";

import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

import PlusIcon from "PUBLIC_DIR/images/plus.react.svg";
import EmptyScreenPersonalUrl from "PUBLIC_DIR/images/empty_screen_personal.svg?url";
import EmptyScreenPersonalDarkUrl from "PUBLIC_DIR/images/empty_screen_personal_dark.svg?url";
import EmptyScreenCorporateSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate.svg?url";
import EmptyScreenCorporateDarkSvgUrl from "PUBLIC_DIR/images/empty_screen_corporate_dark.svg?url";
import EmptyScreenFavoritesUrl from "PUBLIC_DIR/images/empty_screen_favorites.svg?url";
import EmptyScreenFavoritesDarkUrl from "PUBLIC_DIR/images/empty_screen_favorites_dark.svg?url";
import EmptyScreenRecentUrl from "PUBLIC_DIR/images/empty_screen_recent.svg?url";
import EmptyScreenRecentDarkUrl from "PUBLIC_DIR/images/empty_screen_recent_dark.svg?url";
import EmptyScreenPrivacyUrl from "PUBLIC_DIR/images/empty_screen_privacy.svg?url";
import EmptyScreenPrivacyDarkUrl from "PUBLIC_DIR/images/empty_screen_privacy_dark.svg?url";
import EmptyScreenTrashSvgUrl from "PUBLIC_DIR/images/empty_screen_trash.svg?url";
import EmptyScreenTrashSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_trash_dark.svg?url";
import EmptyScreenArchiveUrl from "PUBLIC_DIR/images/empty_screen_archive.svg?url";
import EmptyScreenArchiveDarkUrl from "PUBLIC_DIR/images/empty_screen_archive_dark.svg?url";

import { showLoader, hideLoader } from "./EmptyFolderContainerUtils";

const StyledPlusIcon = styled(PlusIcon)`
  path {
    fill: #657077;
  }
`;

const RootFolderContainer = (props) => {
  const {
    t,
    theme,
    isPrivacyFolder,
    isDesktop,
    isEncryptionSupport,
    organizationName,
    privacyInstructions,
    title,
    onCreate,
    onCreateRoom,
    myFolderId,

    setIsLoading,
    rootFolderType,
    linkStyles,

    isEmptyPage,

    isVisitor,
    isCollaborator,
    sectionWidth,

    security,

    myFolder,
    roomsFolder,
  } = props;
  const personalDescription = t("EmptyFolderDecription");

  const navigate = useNavigate();
  const location = useLocation();

  const emptyScreenHeader = t("EmptyScreenFolder");
  const archiveHeader = t("ArchiveEmptyScreenHeader");
  const noFilesHeader = t("NoFilesHereYet");
  const trashDescription = t("TrashEmptyDescription");
  const favoritesDescription = t("FavoritesEmptyContainerDescription");
  const recentDescription = t("RecentEmptyContainerDescription");

  const roomsDescription =
    isVisitor || isCollaborator
      ? t("RoomEmptyContainerDescriptionUser")
      : t("RoomEmptyContainerDescription");
  const archiveRoomsDescription =
    isVisitor || isCollaborator
      ? t("ArchiveEmptyScreenUser")
      : t("ArchiveEmptyScreen");

  const privateRoomHeader = t("PrivateRoomHeader");
  const privacyIcon = <img alt="" src={PrivacySvgUrl} />;
  const privateRoomDescTranslations = [
    t("PrivateRoomDescriptionSafest"),
    t("PrivateRoomDescriptionSecure"),
    t("PrivateRoomDescriptionEncrypted"),
    t("PrivateRoomDescriptionUnbreakable"),
  ];

  const roomHeader = t("EmptyRootRoomHeader");

  const onGoToPersonal = () => {
    const newFilter = FilesFilter.getDefault();

    newFilter.folder = myFolderId;

    const state = {
      title: myFolder.title,
      isRoot: true,
      rootFolderType: myFolder.rootFolderType,
    };

    const path = getCategoryUrl(CategoryType.Personal);

    setIsLoading(true);

    navigate(`${path}?${newFilter.toUrlParams()}`, { state });
  };

  const onGoToShared = () => {
    const newFilter = RoomsFilter.getDefault();

    newFilter.searchArea = RoomSearchArea.Active;

    const state = {
      title: roomsFolder.title,
      isRoot: true,
      rootFolderType: roomsFolder.rootFolderType,
    };

    setIsLoading(true);

    const path = getCategoryUrl(CategoryType.Shared);

    navigate(`${path}?${newFilter.toUrlParams()}`, { state });
  };

  const getEmptyFolderProps = () => {
    switch (rootFolderType || location?.state?.rootFolderType) {
      case FolderType.USER:
        return {
          headerText: emptyScreenHeader,
          descriptionText: personalDescription,
          imageSrc: theme.isBase
            ? EmptyScreenPersonalUrl
            : EmptyScreenPersonalDarkUrl,
          buttons: commonButtons,
        };
      case FolderType.Favorites:
        return {
          headerText: noFilesHeader,
          descriptionText: favoritesDescription,
          imageSrc: theme.isBase
            ? EmptyScreenFavoritesUrl
            : EmptyScreenFavoritesDarkUrl,
          buttons: isVisitor ? null : goToPersonalButtons,
        };
      case FolderType.Recent:
        return {
          headerText: noFilesHeader,
          descriptionText: recentDescription,
          imageSrc: theme.isBase
            ? EmptyScreenRecentUrl
            : EmptyScreenRecentDarkUrl,
          buttons: isVisitor ? null : goToPersonalButtons,
        };
      case FolderType.Privacy:
        return {
          descriptionText: privateRoomDescription,
          imageSrc: theme.isBase
            ? EmptyScreenPrivacyUrl
            : EmptyScreenPrivacyDarkUrl,
          buttons: isDesktop && isEncryptionSupport && commonButtons,
        };
      case FolderType.TRASH:
        return {
          headerText: emptyScreenHeader,
          descriptionText: trashDescription,
          style: { gridColumnGap: "39px", gridTemplateColumns: "150px" },
          imageSrc: theme.isBase
            ? EmptyScreenTrashSvgUrl
            : EmptyScreenTrashSvgDarkUrl,
          buttons: trashButtons,
        };
      case FolderType.Rooms:
        return {
          headerText: roomHeader,
          descriptionText: roomsDescription,
          imageSrc: theme.isBase
            ? EmptyScreenCorporateSvgUrl
            : EmptyScreenCorporateDarkSvgUrl,
          buttons: !security?.Create ? null : roomsButtons,
        };
      case FolderType.Archive:
        return {
          headerText: archiveHeader,
          descriptionText: archiveRoomsDescription,
          imageSrc: theme.isBase
            ? EmptyScreenArchiveUrl
            : EmptyScreenArchiveDarkUrl,
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
        <StyledPlusIcon
          className="plus-document empty-folder_container-image"
          data-format="docx"
          onClick={onCreate}
          alt="plus_icon"
        />

        <Box className="flex-wrapper_container">
          <Link
            id="document"
            data-format="docx"
            onClick={onCreate}
            {...linkStyles}
          >
            {t("Document")},
          </Link>
          <Link
            id="spreadsheet"
            data-format="xlsx"
            onClick={onCreate}
            {...linkStyles}
          >
            {t("Spreadsheet")},
          </Link>
          <Link
            id="presentation"
            data-format="pptx"
            onClick={onCreate}
            {...linkStyles}
          >
            {t("Presentation")},
          </Link>
          <Link
            id="form-template"
            data-format="docxf"
            onClick={onCreate}
            {...linkStyles}
          >
            {t("Translations:NewForm")}
          </Link>
        </Box>
      </div>

      <div className="empty-folder_container-links">
        <StyledPlusIcon
          className="plus-folder empty-folder_container-image"
          onClick={onCreate}
          alt="plus_icon"
        />
        <Link id="folder" {...linkStyles} onClick={onCreate}>
          {t("Folder")}
        </Link>
      </div>
    </span>
  );

  const trashButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container-image"
        src={PersonSvgUrl}
        alt="person_icon"
        onClick={onGoToPersonal}
      />
      <Link onClick={onGoToPersonal} {...linkStyles}>
        {t("GoToPersonal")}
      </Link>
    </div>
  );

  const roomsButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container_plus-image"
        src={PlusSvgUrl}
        onClick={onCreateRoom}
        alt="plus_icon"
      />
      <Link onClick={onCreateRoom} {...linkStyles}>
        {t("CreateRoom")}
      </Link>
    </div>
  );

  const archiveButtons = !isVisitor && (
    <div className="empty-folder_container-links">
      <IconButton
        className="empty-folder_container-icon"
        size="12"
        onClick={onGoToShared}
        iconName={RoomsReactSvgUrl}
        isFill
      />
      <Link onClick={onGoToShared} {...linkStyles}>
        {t("GoToMyRooms")}
      </Link>
    </div>
  );

  const goToPersonalButtons = (
    <div className="empty-folder_container-links">
      <img
        className="empty-folder_container-image"
        src={PersonSvgUrl}
        alt="person_icon"
        onClick={onGoToPersonal}
      />
      <Link onClick={onGoToPersonal} {...linkStyles}>
        {t("GoToPersonal")}
      </Link>
    </div>
  );

  const headerText = isPrivacyFolder ? privateRoomHeader : title;
  const emptyFolderProps = getEmptyFolderProps();

  // if (isLoading) {
  //   return (
  //     <Loaders.EmptyContainerLoader
  //       style={{ display: "none", marginTop: 32 }}
  //       id="empty-container-loader"
  //       viewAs={viewAs}
  //     />
  //   );
  // }

  return (
    emptyFolderProps && (
      <EmptyContainer
        headerText={headerText}
        isEmptyPage={isEmptyPage}
        sectionWidth={sectionWidth}
        style={{ marginTop: 32 }}
        {...emptyFolderProps}
      />
    )
  );
};

export default inject(
  ({
    auth,
    filesStore,
    treeFoldersStore,
    selectedFolderStore,
    clientLoadingStore,
  }) => {
    const {
      isDesktopClient,
      isEncryptionSupport,
      organizationName,
      theme,
    } = auth.settingsStore;

    const { setIsSectionFilterLoading } = clientLoadingStore;

    const setIsLoading = (param) => {
      setIsSectionFilterLoading(param);
    };

    const {
      filter,

      privacyInstructions,

      isEmptyPage,
    } = filesStore;
    const { title, rootFolderType, security } = selectedFolderStore;
    const { isPrivacyFolder, myFolderId, myFolder, roomsFolder } =
      treeFoldersStore;

    return {
      theme,
      isPrivacyFolder,
      isDesktop: isDesktopClient,
      isVisitor: auth.userStore.user.isVisitor,
      isCollaborator: auth.userStore.user.isCollaborator,
      isEncryptionSupport,
      organizationName,
      privacyInstructions,
      title,
      myFolderId,
      filter,

      setIsLoading,
      rootFolderType,

      isEmptyPage,

      security,

      myFolder,
      roomsFolder,
    };
  }
)(withTranslation(["Files"])(observer(RootFolderContainer)));
