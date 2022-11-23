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

const PrivateFolderContainer = (props) => {
  const {
    t,
    theme,

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
  const privateRoomDescTranslations = [
    t("PrivateRoomDescriptionSafest"),
    t("PrivateRoomDescriptionSecure"),
    t("PrivateRoomDescriptionEncrypted"),
    t("PrivateRoomDescriptionUnbreakable"),
  ];

  const privateRoomDescription = (
    <>
      <div className="empty-folder_container-links list-container">
        {privateRoomDescTranslations.map((el) => (
          <>
            <Box paddingProp="0 7px 0 0">{privacyIcon}</Box>
            <Box>
              <Text fontSize={"12px"}>{el}</Text>
            </Box>
          </>
        ))}
      </div>
      {!isDesktop && (
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
      )}
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
