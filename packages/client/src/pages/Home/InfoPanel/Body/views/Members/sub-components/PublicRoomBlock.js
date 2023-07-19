import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import toastr from "@docspace/components/toast/toastr";
import LinksToViewingIconUrl from "PUBLIC_DIR/images/links-to-viewing.react.svg?url";
import PublicRoomBar from "./PublicRoomBar";
import { LinksBlock } from "./StyledPublicRoom";
import LinkRow from "./LinkRow";

const PublicRoomBlock = (props) => {
  const {
    t,
    externalLinks,
    isArchiveFolder,
    setLinkParams,
    setEditLinkPanelIsVisible,
  } = props;

  const [barIsVisible, setBarVisible] = useState(!isArchiveFolder);

  const onAddNewLink = () => {
    setLinkParams({ isEdit: false });
    setEditLinkPanelIsVisible(true);
  };

  return (
    <>
      {barIsVisible && (
        <PublicRoomBar
          headerText={t("Files:PublicRoom")}
          bodyText={t("CreateEditRoomDialog:PublicRoomBarDescription")}
        />
      )}
      <LinksBlock>
        {isArchiveFolder ? (
          <Text fontSize="14px" fontWeight={600} color="#a3a9ae">
            {t("Files:Links")}: {externalLinks.length}
          </Text>
        ) : (
          <>
            <Text fontSize="14px" fontWeight={600} color="#a3a9ae">
              {externalLinks.length
                ? t("LinksToViewingIcon")
                : t("Files:NoExternalLinks")}
            </Text>
            <IconButton
              className="link-to-viewing-icon"
              iconName={LinksToViewingIconUrl}
              onClick={onAddNewLink}
              size={16}
            />
          </>
        )}
      </LinksBlock>

      {externalLinks.length ? (
        externalLinks.map((link) => (
          <LinkRow link={link} key={link?.sharedTo?.id} />
        ))
      ) : (
        <></>
      )}
    </>
  );
};

export default inject(({ publicRoomStore, treeFoldersStore, dialogsStore }) => {
  const { roomLinks } = publicRoomStore;
  const { isArchiveFolder } = treeFoldersStore;
  const { setLinkParams, setEditLinkPanelIsVisible } = dialogsStore;

  return {
    externalLinks: roomLinks,
    isArchiveFolder,
    setLinkParams,
    setEditLinkPanelIsVisible,
  };
})(observer(PublicRoomBlock));
