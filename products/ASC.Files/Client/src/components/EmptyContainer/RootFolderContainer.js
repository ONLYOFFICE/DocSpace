import React from "react";
import { FolderType } from "@appserver/common/constants";
import { inject, observer } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";

const RootFolderContainer = (props) => {
  const {
    t,
    isPrivacyFolder,
    isDesktop,
    isEncryptionSupport,
    organizationName,
    privacyInstructions,
    title,
    onCreate,
    myFolderId,
    filter,
    fetchFiles,
    setIsLoading,
    rootFolderType,
    linkStyles,
  } = props;
  const subheadingText = t("SubheadingEmptyText");
  const myDescription = t("MyEmptyContainerDescription");
  const shareDescription = t("SharedEmptyContainerDescription");
  const commonDescription = t("CommonEmptyContainerDescription");
  const trashDescription = t("TrashEmptyContainerDescription");
  const favoritesDescription = t("FavoritesEmptyContainerDescription");
  const recentDescription = t("RecentEmptyContainerDescription");

  const privateRoomHeader = t("PrivateRoomHeader");
  const privacyIcon = <img alt="" src="images/privacy.svg" />;
  const privateRoomDescTranslations = [
    t("PrivateRoomDescriptionSafest"),
    t("PrivateRoomDescriptionSecure"),
    t("PrivateRoomDescriptionEncrypted"),
    t("PrivateRoomDescriptionUnbreakable"),
  ];

  const onGoToMyDocuments = () => {
    const newFilter = filter.clone();
    setIsLoading(true);
    fetchFiles(myFolderId, newFilter).finally(() => setIsLoading(false));
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
          imageSrc: "images/empty_screen_favorites.png",
        };
      case FolderType.Recent:
        return {
          descriptionText: recentDescription,
          imageSrc: "images/empty_screen_recent.png",
        };
      case FolderType.Privacy:
        return {
          descriptionText: privateRoomDescription,
          imageSrc: "images/empty_screen_privacy.png",
          buttons: isDesktop && isEncryptionSupport && commonButtons,
        };
      case FolderType.TRASH:
        return {
          descriptionText: trashDescription,
          imageSrc: "images/empty_screen_trash.png",
          buttons: trashButtons,
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
          <Trans t={t} i18nKey="PrivateRoomSupport" ns="Home">
            Work in Private Room is available via {{ organizationName }} desktop
            app.
            <Link isBold isHovered color="#116d9d" href={privacyInstructions}>
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

  const headerText = isPrivacyFolder ? privateRoomHeader : title;
  const subheadingTextProp = isPrivacyFolder ? {} : { subheadingText };
  const emptyFolderProps = getEmptyFolderProps();

  return (
    <EmptyContainer
      headerText={headerText}
      {...subheadingTextProp}
      {...emptyFolderProps}
    />
  );
};

export default inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const {
      isDesktopClient,
      isEncryptionSupport,
      organizationName,
    } = auth.settingsStore;

    const {
      filter,
      fetchFiles,
      privacyInstructions,
      setIsLoading,
    } = filesStore;
    const { title, rootFolderType } = selectedFolderStore;
    const { isPrivacyFolder, myFolderId } = treeFoldersStore;

    return {
      isPrivacyFolder,
      isDesktop: isDesktopClient,
      isEncryptionSupport,
      organizationName,
      privacyInstructions,
      title,
      myFolderId,
      filter,
      fetchFiles,
      setIsLoading,
      rootFolderType,
    };
  }
)(withTranslation("Home")(observer(RootFolderContainer)));
