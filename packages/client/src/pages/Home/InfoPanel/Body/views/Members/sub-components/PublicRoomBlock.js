import React, { useState } from "react";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";
import Tooltip from "@docspace/components/tooltip";
import toastr from "@docspace/components/toast/toastr";
import LinksToViewingIconUrl from "PUBLIC_DIR/images/links-to-viewing.react.svg?url";
import PublicRoomBar from "./PublicRoomBar";
import { LinksBlock } from "./StyledPublicRoom";
import LinkRow from "./LinkRow";

const LINKS_LIMIT_COUNT = 10;

const PublicRoomBlock = (props) => {
  const {
    t,
    externalLinks,
    isArchiveFolder,
    setLinkParams,
    setEditLinkPanelIsVisible,
  } = props;

  const onAddNewLink = () => {
    setLinkParams({ isEdit: false });
    setEditLinkPanelIsVisible(true);
  };

  return (
    <>
      {externalLinks.length > 0 && !isArchiveFolder && (
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

            <div
              data-for="emailTooltip"
              data-tip={t("Files:MaximumNumberOfExternalLinksCreated")}
            >
              <IconButton
                className="link-to-viewing-icon"
                iconName={LinksToViewingIconUrl}
                onClick={onAddNewLink}
                size={16}
                isDisabled={externalLinks.length >= LINKS_LIMIT_COUNT}
              />

              {externalLinks.length >= LINKS_LIMIT_COUNT && (
                <Tooltip
                  id="emailTooltip"
                  getContent={(dataTip) => (
                    <Text fontSize="12px">{dataTip}</Text>
                  )}
                  effect="float"
                  place="bottom"
                />
              )}
            </div>
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
