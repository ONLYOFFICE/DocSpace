import React from "react";
import styled from "styled-components";
import { FolderType } from "@docspace/common/constants";
import { inject, observer } from "mobx-react";
import { withTranslation, Trans } from "react-i18next";
import EmptyContainer from "./EmptyContainer";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import Loaders from "@docspace/common/components/Loaders";
import PlusIcon from "@docspace/client/public/images/plus.react.svg";

const StyledPlusIcon = styled(PlusIcon)`
  path {
    fill: #657077;
  }
`;

const PrivateFolderContainer = (props) => {
  const {
    t,
    theme,

    onCreate,
    linkStyles,

    isDesktop,
    isEncryptionSupport,
    organizationName,
    privacyInstructions,

    isLoading,
    viewAs,

    isEmptyPage,
    setIsEmptyPage,

    sectionWidth,
  } = props;

  const [showLoader, setShowLoader] = React.useState(false);

  React.useEffect(() => {
    setIsEmptyPage(true);

    return () => {
      setIsEmptyPage(false);
    };
  }, [setIsEmptyPage]);

  const privateRoomHeader = t("PrivateRoomHeader");
  const privacyIcon = <img alt="" src="images/privacy.svg" />;

  const privateRoomDescription = (
    <>
      <div className="empty-folder_container-links list-container">
        <Box paddingProp="0 7px 0 0">{privacyIcon}</Box>
        <Box>
          <Text fontSize={"12px"}>{t("PrivateRoomDescriptionSafest")}</Text>
        </Box>

        <Box paddingProp="0 7px 0 0">{privacyIcon}</Box>
        <Box>
          <Text fontSize={"12px"}>{t("PrivateRoomDescriptionSecure")}</Text>
        </Box>

        <Box paddingProp="0 7px 0 0">{privacyIcon}</Box>
        <Box>
          <Text fontSize={"12px"}>{t("PrivateRoomDescriptionEncrypted")}</Text>
        </Box>

        <Box paddingProp="0 7px 0 0">{privacyIcon}</Box>
        <Box>
          <Text fontSize={"12px"}>
            {t("PrivateRoomDescriptionUnbreakable")}
          </Text>
        </Box>
      </div>

      <Text fontSize="13px" lineHeight={"20px"}>
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
    </>
  );

  React.useEffect(() => {
    let timeout;

    if (isLoading) {
      setShowLoader(isLoading);
    } else {
      timeout = setTimeout(() => setShowLoader(isLoading), 300);
    }

    return () => clearTimeout(timeout);
  }, [isLoading]);

  const commonButtons = (
    <span>
      <div className="empty-folder_container-links">
        <StyledPlusIcon
          className="empty-folder_container-image"
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
            {t("Presentation")},
          </Link>
          <Link data-format="docxf" onClick={onCreate} {...linkStyles}>
            {t("Translations:NewForm")}
          </Link>
        </Box>
      </div>

      <div className="empty-folder_container-links">
        <StyledPlusIcon
          className="empty-folder_container-image"
          onClick={onCreate}
          alt="plus_icon"
        />
        <Link {...linkStyles} onClick={onCreate}>
          {t("Folder")}
        </Link>
      </div>
    </span>
  );

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
          headerText={privateRoomHeader}
          descriptionText={privateRoomDescription}
          imageSrc={"images/empty_screen_privacy.svg"}
          buttons={isDesktop && isEncryptionSupport && commonButtons}
          isEmptyPage={isEmptyPage}
          sectionWidth={sectionWidth}
        />
      )}
    </>
  );
};

export default inject(({ auth, filesStore }) => {
  const {
    isDesktopClient,
    isEncryptionSupport,
    organizationName,
    theme,
  } = auth.settingsStore;

  const {
    privacyInstructions,
    isLoading,

    viewAs,

    isEmptyPage,
    setIsEmptyPage,
  } = filesStore;

  return {
    theme,

    isDesktop: isDesktopClient,
    isVisitor: auth.userStore.user.isVisitor,
    isEncryptionSupport,
    organizationName,
    privacyInstructions,

    isLoading,

    viewAs,

    isEmptyPage,
    setIsEmptyPage,
  };
})(withTranslation(["Files"])(observer(PrivateFolderContainer)));
